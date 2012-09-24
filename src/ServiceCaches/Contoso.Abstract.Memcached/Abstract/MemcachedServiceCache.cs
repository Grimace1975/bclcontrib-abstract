#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System;
using System.Linq;
using System.Abstract;
using System.Collections.Generic;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
// https://github.com/enyim/EnyimMemcached/wiki/MemcachedClient-Usage
namespace Contoso.Abstract
{
    /// <summary>
    /// IMemcachedServiceCache
    /// </summary>
    public interface IMemcachedServiceCache : IDistributedServiceCache
    {
        /// <summary>
        /// Gets the cache.
        /// </summary>
        IMemcachedClient Cache { get; }
        /// <summary>
        /// Flushes all.
        /// </summary>
        void FlushAll();
        /// <summary>
        /// Gets the with cas.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        IDictionary<string, CasResult<object>> GetWithCas(IEnumerable<string> keys);
        /// <summary>
        /// Gets the with cas.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        CasResult<object> GetWithCas(string key);
        /// <summary>
        /// Gets the with cas.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        CasResult<T> GetWithCas<T>(string key);
        /// <summary>
        /// Tries the get with cas.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        bool TryGetWithCas(string key, out CasResult<object> value);
    }

    /// <summary>
    /// MemcachedServiceCache
    /// </summary>
    public partial class MemcachedServiceCache : IMemcachedServiceCache, IDisposable, ServiceCacheManager.ISetupRegistration
    {
        private ITagMapper _tagMapper;

        #region Tags

        /// <summary>
        /// IncrementTag
        /// </summary>
        public struct IncrementTag
        {
            /// <summary>
            /// DefaultValue
            /// </summary>
            public ulong DefaultValue;
            /// <summary>
            /// Delta
            /// </summary>
            public ulong Delta;
        }

        /// <summary>
        /// DecrementTag
        /// </summary>
        public struct DecrementTag
        {
            /// <summary>
            /// DefaultValue
            /// </summary>
            public ulong DefaultValue;
            /// <summary>
            /// Delta
            /// </summary>
            public ulong Delta;
        }

        #endregion

        static MemcachedServiceCache() { ServiceCacheManager.EnsureRegistration(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedServiceCache"/> class.
        /// </summary>
        public MemcachedServiceCache()
            : this(new MemcachedClient(), new TagMapper()) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedServiceCache"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public MemcachedServiceCache(IMemcachedClientConfiguration configuration)
            : this(configuration == null ? new MemcachedClient() : new MemcachedClient(configuration), new TagMapper()) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedServiceCache"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        public MemcachedServiceCache(IMemcachedClient client)
            : this(client, new TagMapper()) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedServiceCache"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="tagMapper">The tag mapper.</param>
        public MemcachedServiceCache(IMemcachedClient client, ITagMapper tagMapper)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            Cache = client;
            _tagMapper = tagMapper;
            Settings = new ServiceCacheSettings();
        }
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="MemcachedServiceCache"/> is reclaimed by garbage collection.
        /// </summary>
        ~MemcachedServiceCache()
        {
            try { ((IDisposable)this).Dispose(); }
            catch { }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Cache != null)
            {
                GC.SuppressFinalize(this);
                try { Cache.Dispose(); }
                finally { Cache = null; }
            }
        }

        private static ArraySegment<T> OpValueToArraySegment<T>(object opvalue) { return (opvalue is ArraySegment<T> ? (ArraySegment<T>)opvalue : new ArraySegment<T>()); }
        private static DecrementTag OpValueToDecrementTag(object opvalue)
        {
            if (opvalue is DecrementTag)
                return (DecrementTag)opvalue;
            if (opvalue is ulong)
                return new DecrementTag { Delta = (ulong)opvalue, DefaultValue = 0 };
            if (opvalue is long)
                return new DecrementTag { Delta = (ulong)opvalue, DefaultValue = 0 };
            if (opvalue is int)
                return new DecrementTag { Delta = (ulong)opvalue, DefaultValue = 0 };
            throw new ArgumentOutOfRangeException("opvalue");
        }
        private static IncrementTag OpValueToIncrementTag(object opvalue)
        {
            if (opvalue is IncrementTag)
                return (IncrementTag)opvalue;
            if (opvalue is ulong)
                return new IncrementTag { Delta = (ulong)opvalue, DefaultValue = 0 };
            if (opvalue is long)
                return new IncrementTag { Delta = (ulong)opvalue, DefaultValue = 0 };
            if (opvalue is int)
                return new IncrementTag { Delta = (ulong)opvalue, DefaultValue = 0 };
            throw new ArgumentOutOfRangeException("opvalue");
        }

        Action<IServiceLocator, string> ServiceCacheManager.ISetupRegistration.DefaultServiceRegistrar
        {
            get { return (locator, name) => ServiceCacheManager.RegisterInstance<IMemcachedServiceCache>(this, locator, name); }
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.
        /// -or-
        /// null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified name.
        /// </summary>
        public object this[string name]
        {
            get { return Get(null, name); }
            set { Set(null, name, CacheItemPolicy.Default, value, ServiceCacheByDispatcher.Empty); }
        }

        /// <summary>
        /// Adds the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <param name="itemPolicy">The item policy.</param>
        /// <param name="value">The value.</param>
        /// <param name="dispatch">The dispatch.</param>
        /// <returns></returns>
        public object Add(object tag, string name, CacheItemPolicy itemPolicy, object value, ServiceCacheByDispatcher dispatch)
        {
            if (itemPolicy == null)
                throw new ArgumentNullException("itemPolicy");
            var updateCallback = itemPolicy.UpdateCallback;
            if (updateCallback != null)
                updateCallback(name, value);
            //
            var absoluteExpiration = itemPolicy.AbsoluteExpiration;
            var slidingExpiration = itemPolicy.SlidingExpiration;
            ulong cas;
            object opvalue;
            if (absoluteExpiration == DateTime.MaxValue && slidingExpiration == TimeSpan.Zero)
                switch (_tagMapper.ToAddOpcode(tag, ref name, out cas, out opvalue))
                {
                    case TagMapper.AddOpcode.Append: value = Cache.Append(name, OpValueToArraySegment<byte>(opvalue)); break;
                    case TagMapper.AddOpcode.AppendCas: value = Cache.Append(name, cas, OpValueToArraySegment<byte>(opvalue)); break;
                    case TagMapper.AddOpcode.Store: value = Cache.Store(StoreMode.Add, name, value); break;
                    case TagMapper.AddOpcode.Cas: value = Cache.Cas(StoreMode.Add, name, value, cas); break;
                    default: throw new InvalidOperationException();
                }
            else if (absoluteExpiration != DateTime.MinValue && slidingExpiration == TimeSpan.Zero)
                switch (_tagMapper.ToAddOpcode(tag, ref name, out cas, out opvalue))
                {
                    case TagMapper.AddOpcode.Append:
                    case TagMapper.AddOpcode.AppendCas:
                        throw new NotSupportedException("Operation not supported with absoluteExpiration && slidingExpiration");
                    case TagMapper.AddOpcode.Store: value = Cache.Store(StoreMode.Add, name, value, absoluteExpiration); break;
                    case TagMapper.AddOpcode.Cas: value = Cache.Cas(StoreMode.Add, name, value, absoluteExpiration, cas); break;
                    default: throw new InvalidOperationException();
                }
            else if (absoluteExpiration == DateTime.MinValue && slidingExpiration != TimeSpan.Zero)
                switch (_tagMapper.ToAddOpcode(tag, ref name, out cas, out opvalue))
                {
                    case TagMapper.AddOpcode.Append:
                    case TagMapper.AddOpcode.AppendCas:
                        throw new NotSupportedException("Operation not supported with absoluteExpiration && slidingExpiration");
                    case TagMapper.AddOpcode.Store: value = Cache.Store(StoreMode.Add, name, value, slidingExpiration); break;
                    case TagMapper.AddOpcode.Cas: value = Cache.Cas(StoreMode.Add, name, value, slidingExpiration, cas); break;
                    default: throw new InvalidOperationException();
                }
            else throw new InvalidOperationException("absoluteExpiration && slidingExpiration");
            //
            var registration = dispatch.Registration;
            if (registration != null && registration.UseHeaders)
            {
                var header = dispatch.Header;
                header.Item = name;
                Add(tag, name + "#", CacheItemPolicy.Default, header, ServiceCacheByDispatcher.Empty);
            }
            return value;
        }

        /// <summary>
        /// Gets the item from cache associated with the key provided.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The cached item.
        /// </returns>
        public object Get(object tag, string name) { return Cache.Get(name); }
        /// <summary>
        /// Gets the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="header">The header.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public object Get(object tag, string name, IServiceCacheRegistration registration, out CacheItemHeader header)
        {
            if (registration == null)
                throw new ArgumentNullException("registration");
            header = (registration.UseHeaders ? (CacheItemHeader)Cache.Get(name + "#") : null);
            return Cache.Get(name);
        }
        /// <summary>
        /// Gets the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="names">The names.</param>
        /// <returns></returns>
        public object Get(object tag, IEnumerable<string> names)
        {
            object opvalue;
            switch (_tagMapper.ToGetOpcode(tag, out opvalue))
            {
                case TagMapper.GetOpcode.Get: return Cache.Get(names);
                //case TagMapper.GetManyOpcode.PerformMultiGet:
                //    {
                //        var cache2 = (MemcachedClient)Cache;
                //        if (cache2 == null)
                //            throw new InvalidOperationException("Must be a MemcachedClient for PerformMultiGet");
                //        return cache2.PerformMultiGet<object>(names, (Func<IMultiGetOperation, KeyValuePair<string, CacheItem>, object>)opvalue);
                //    }
                default: throw new InvalidOperationException();
            }
        }
        /// <summary>
        /// Gets the specified registration.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<CacheItemHeader> Get(object tag, IServiceCacheRegistration registration)
        {
            if (registration == null)
                throw new ArgumentNullException("registration");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tries the get.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGet(object tag, string name, out object value) { return Cache.TryGet(name, out value); }

        /// <summary>
        /// Removes from cache the item associated with the key provided.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <param name="registration">The registration.</param>
        /// <returns>
        /// The item removed from the Cache. If the value in the key parameter is not found, returns null.
        /// </returns>
        public object Remove(object tag, string name, IServiceCacheRegistration registration) { if (registration != null && registration.UseHeaders) Cache.Remove(name + "#"); return Cache.Remove(name); }

        /// <summary>
        /// Adds an object into cache based on the parameters provided.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <param name="itemPolicy">The itemPolicy object.</param>
        /// <param name="value">The value to store in cache.</param>
        /// <param name="dispatch">The dispatch.</param>
        /// <returns></returns>
        public object Set(object tag, string name, CacheItemPolicy itemPolicy, object value, ServiceCacheByDispatcher dispatch)
        {
            if (itemPolicy == null)
                throw new ArgumentNullException("itemPolicy");
            var updateCallback = itemPolicy.UpdateCallback;
            if (updateCallback != null)
                updateCallback(name, value);
            //
            var absoluteExpiration = itemPolicy.AbsoluteExpiration;
            var slidingExpiration = itemPolicy.SlidingExpiration;
            ulong cas;
            object opvalue;
            StoreMode storeMode;
            IncrementTag increment;
            DecrementTag decrement;
            if (absoluteExpiration == DateTime.MaxValue && slidingExpiration == TimeSpan.Zero)
                switch (_tagMapper.ToSetOpcode(tag, ref name, out cas, out opvalue, out storeMode))
                {
                    case TagMapper.SetOpcode.Prepend: value = Cache.Prepend(name, OpValueToArraySegment<byte>(opvalue)); break;
                    case TagMapper.SetOpcode.PrependCas: value = Cache.Prepend(name, cas, OpValueToArraySegment<byte>(opvalue)); break;
                    //
                    case TagMapper.SetOpcode.Store: value = Cache.Store(storeMode, name, value); break;
                    case TagMapper.SetOpcode.Cas: value = Cache.Cas(storeMode, name, value, cas); break;
                    case TagMapper.SetOpcode.Decrement: decrement = OpValueToDecrementTag(opvalue); value = Cache.Decrement(name, decrement.DefaultValue, decrement.Delta); break;
                    case TagMapper.SetOpcode.DecrementCas: decrement = OpValueToDecrementTag(opvalue); value = Cache.Decrement(name, decrement.DefaultValue, decrement.Delta, cas); break;
                    case TagMapper.SetOpcode.Increment: increment = OpValueToIncrementTag(opvalue); value = Cache.Increment(name, increment.DefaultValue, increment.Delta); break;
                    case TagMapper.SetOpcode.IncrementCas: increment = OpValueToIncrementTag(opvalue); value = Cache.Increment(name, increment.DefaultValue, increment.Delta, cas); break;
                    default: throw new InvalidOperationException();
                }
            else if (absoluteExpiration != DateTime.MinValue && slidingExpiration == TimeSpan.Zero)
                switch (_tagMapper.ToSetOpcode(tag, ref name, out cas, out opvalue, out storeMode))
                {
                    case TagMapper.SetOpcode.Prepend:
                    case TagMapper.SetOpcode.PrependCas:
                        throw new NotSupportedException("Operation not supported with absoluteExpiration && slidingExpiration");
                    case TagMapper.SetOpcode.Store: value = Cache.Store(storeMode, name, value, absoluteExpiration); break;
                    case TagMapper.SetOpcode.Cas: value = Cache.Cas(storeMode, name, value, absoluteExpiration, cas); break;
                    case TagMapper.SetOpcode.Decrement: decrement = OpValueToDecrementTag(opvalue); value = Cache.Decrement(name, decrement.DefaultValue, decrement.Delta, absoluteExpiration); break;
                    case TagMapper.SetOpcode.DecrementCas: decrement = OpValueToDecrementTag(opvalue); value = Cache.Decrement(name, decrement.DefaultValue, decrement.Delta, absoluteExpiration, cas); break;
                    case TagMapper.SetOpcode.Increment: increment = OpValueToIncrementTag(opvalue); value = Cache.Increment(name, increment.DefaultValue, increment.Delta, absoluteExpiration); break;
                    case TagMapper.SetOpcode.IncrementCas: increment = OpValueToIncrementTag(opvalue); value = Cache.Increment(name, increment.DefaultValue, increment.Delta, absoluteExpiration, cas); break;
                    default: throw new InvalidOperationException();
                }
            else if (absoluteExpiration == DateTime.MinValue && slidingExpiration != TimeSpan.Zero)
                switch (_tagMapper.ToSetOpcode(tag, ref name, out cas, out opvalue, out storeMode))
                {
                    case TagMapper.SetOpcode.Prepend:
                    case TagMapper.SetOpcode.PrependCas:
                        throw new NotSupportedException("Operation not supported with absoluteExpiration && slidingExpiration");
                    case TagMapper.SetOpcode.Store: value = Cache.Store(storeMode, name, value, slidingExpiration); break;
                    case TagMapper.SetOpcode.Cas: value = Cache.Cas(storeMode, name, value, slidingExpiration, cas); break;
                    case TagMapper.SetOpcode.Decrement: decrement = OpValueToDecrementTag(opvalue); value = Cache.Decrement(name, decrement.DefaultValue, decrement.Delta, slidingExpiration); break;
                    case TagMapper.SetOpcode.DecrementCas: decrement = OpValueToDecrementTag(opvalue); value = Cache.Decrement(name, decrement.DefaultValue, decrement.Delta, slidingExpiration, cas); break;
                    case TagMapper.SetOpcode.Increment: increment = OpValueToIncrementTag(opvalue); value = Cache.Increment(name, increment.DefaultValue, increment.Delta, slidingExpiration); break;
                    case TagMapper.SetOpcode.IncrementCas: increment = OpValueToIncrementTag(opvalue); value = Cache.Increment(name, increment.DefaultValue, increment.Delta, slidingExpiration, cas); break;
                    default: throw new InvalidOperationException();
                }
            else throw new InvalidOperationException("absoluteExpiration && slidingExpiration");
            //
            var registration = dispatch.Registration;
            if (registration != null && registration.UseHeaders)
            {
                var header = dispatch.Header;
                header.Item = name;
                Add(tag, name + "#", CacheItemPolicy.Default, header, ServiceCacheByDispatcher.Empty);
            }
            return value;
        }

        /// <summary>
        /// Settings
        /// </summary>
        public ServiceCacheSettings Settings { get; private set; }

        #region TouchableCacheItem

        ///// <summary>
        ///// DefaultTouchableCacheItem
        ///// </summary>
        //public class DefaultTouchableCacheItem : ITouchableCacheItem
        //{
        //    private MemcachedServiceCache _parent;
        //    private ITouchableCacheItem _base;
        //    public DefaultTouchableCacheItem(MemcachedServiceCache parent, ITouchableCacheItem @base) { _parent = parent; _base = @base; }

        //    public void Touch(object tag, string[] names)
        //    {
        //        if (names == null || names.Length == 0)
        //            return;
        //        throw new NotSupportedException();
        //    }

        //    public object MakeDependency(object tag, string[] names)
        //    {
        //        if (names == null || names.Length == 0)
        //            return null;
        //        throw new NotSupportedException();
        //    }
        //}

        #endregion

        #region Domain-specific

        /// <summary>
        /// Gets the cache.
        /// </summary>
        public IMemcachedClient Cache { get; private set; }

        /// <summary>
        /// Flushes all.
        /// </summary>
        public void FlushAll() { Cache.FlushAll(); }
        /// <summary>
        /// Gets the with cas.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        public IDictionary<string, CasResult<object>> GetWithCas(IEnumerable<string> keys) { return Cache.GetWithCas(keys); }
        /// <summary>
        /// Gets the with cas.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public CasResult<object> GetWithCas(string key) { return Cache.GetWithCas(key); }
        /// <summary>
        /// Gets the with cas.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public CasResult<T> GetWithCas<T>(string key) { return Cache.GetWithCas<T>(key); }
        /// <summary>
        /// Tries the get with cas.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGetWithCas(string key, out CasResult<object> value) { return Cache.TryGetWithCas(key, out value); }

        #endregion
    }
}
