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
    public interface IMemcachedServiceCache : IServiceCache
    {
        IMemcachedClient Cache { get; }
        void FlushAll();
        IDictionary<string, CasResult<object>> GetWithCas(IEnumerable<string> keys);
        CasResult<object> GetWithCas(string key);
        CasResult<T> GetWithCas<T>(string key);
        bool TryGetWithCas(string key, out CasResult<object> value);
    }

    /// <summary>
    /// MemcachedServiceCache
    /// </summary>
    public partial class MemcachedServiceCache : IMemcachedServiceCache, IDisposable
    {
        private ITagMapper _tagMapper;

        #region Tags

        public struct IncrementTag
        {
            public ulong DefaultValue;
            public ulong Delta;
        }

        public struct DecrementTag
        {
            public ulong DefaultValue;
            public ulong Delta;
        }

        #endregion

        public MemcachedServiceCache()
            : this(new MemcachedClient(), new TagMapper()) { }
        public MemcachedServiceCache(IMemcachedClientConfiguration configuration)
            : this(configuration == null ? new MemcachedClient() : new MemcachedClient(configuration), new TagMapper()) { }
        public MemcachedServiceCache(IMemcachedClient client)
            : this(client, new TagMapper()) { }
        public MemcachedServiceCache(IMemcachedClient client, ITagMapper tagMapper)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            Cache = client;
            RegistrationDispatch = new DefaultServiceCacheRegistrationDispatcher();
            _tagMapper = tagMapper;
            ServiceCacheManager.Setup(this);
        }
        ~MemcachedServiceCache()
        {
            try { ((IDisposable)this).Dispose(); }
            catch { }
        }

        public void Dispose()
        {
            if (Cache != null)
            {
                GC.SuppressFinalize(this);
                try { Cache.Dispose(); }
                finally { Cache = null; }
            }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        public object this[string name]
        {
            get { return Get(null, name); }
            set { Set(null, name, CacheItemPolicy.Default, value); }
        }

        public object Add(object tag, string name, CacheItemPolicy itemPolicy, object value)
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
            if ((absoluteExpiration == ServiceCache.InfiniteAbsoluteExpiration) && (slidingExpiration == ServiceCache.NoSlidingExpiration))
                switch (_tagMapper.ToAddOpcode(tag, ref name, out cas, out opvalue))
                {
                    case TagMapper.AddOpcode.Append: return Cache.Append(name, (ArraySegment<byte>)opvalue);
                    case TagMapper.AddOpcode.AppendCas: return Cache.Append(name, cas, (ArraySegment<byte>)opvalue);
                    case TagMapper.AddOpcode.Store: return Cache.Store(StoreMode.Add, name, value);
                    case TagMapper.AddOpcode.Cas: return Cache.Cas(StoreMode.Add, name, value, cas);
                    default: throw new InvalidOperationException();
                }
            if ((absoluteExpiration != ServiceCache.InfiniteAbsoluteExpiration) && (slidingExpiration == ServiceCache.NoSlidingExpiration))
                switch (_tagMapper.ToAddOpcode(tag, ref name, out cas, out opvalue))
                {
                    case TagMapper.AddOpcode.Append:
                    case TagMapper.AddOpcode.AppendCas:
                        throw new NotSupportedException("Operation not supported with absoluteExpiration && slidingExpiration");
                    case TagMapper.AddOpcode.Store: return Cache.Store(StoreMode.Add, name, value, absoluteExpiration);
                    case TagMapper.AddOpcode.Cas: return Cache.Cas(StoreMode.Add, name, value, absoluteExpiration, cas);
                    default: throw new InvalidOperationException();
                }
            if ((absoluteExpiration == ServiceCache.InfiniteAbsoluteExpiration) && (slidingExpiration != ServiceCache.NoSlidingExpiration))
                switch (_tagMapper.ToAddOpcode(tag, ref name, out cas, out opvalue))
                {
                    case TagMapper.AddOpcode.Append:
                    case TagMapper.AddOpcode.AppendCas:
                        throw new NotSupportedException("Operation not supported with absoluteExpiration && slidingExpiration");
                    case TagMapper.AddOpcode.Store: return Cache.Store(StoreMode.Add, name, value, slidingExpiration);
                    case TagMapper.AddOpcode.Cas: return Cache.Cas(StoreMode.Add, name, value, slidingExpiration, cas);
                    default: throw new InvalidOperationException();
                }
            throw new InvalidOperationException("absoluteExpiration && slidingExpiration");
        }

        public object Get(object tag, string name) { return Cache.Get(name); }
        public object Get(object tag, IEnumerable<string> names)
        {
            object opvalue;
            switch (_tagMapper.ToGetManyOpcode(tag, out opvalue))
            {
                case TagMapper.GetManyOpcode.Get: return Cache.Get(names);
                case TagMapper.GetManyOpcode.PerformMultiGet:
                    {
                        var cache2 = (MemcachedClient)Cache;
                        if (cache2 == null)
                            throw new InvalidOperationException("Must be a MemcachedClient for PerformMultiGet");
                        return cache2.PerformMultiGet<object>(names, (Func<IMultiGetOperation, KeyValuePair<string, CacheItem>, object>)opvalue);
                    }
                default: throw new InvalidOperationException();
            }
        }
        public bool TryGet(object tag, string name, out object value) { return Cache.TryGet(name, out value); }

        public object Remove(object tag, string name) { return Cache.Remove(name); }

        public object Set(object tag, string name, CacheItemPolicy itemPolicy, object value)
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
            if ((absoluteExpiration == ServiceCache.InfiniteAbsoluteExpiration) && (slidingExpiration == ServiceCache.NoSlidingExpiration))
                switch (_tagMapper.ToSetOpcode(tag, ref name, out cas, out opvalue, out storeMode))
                {
                    case TagMapper.SetOpcode.Prepend: return Cache.Prepend(name, (ArraySegment<byte>)opvalue);
                    case TagMapper.SetOpcode.PrependCas: return Cache.Prepend(name, cas, (ArraySegment<byte>)opvalue);
                    //
                    case TagMapper.SetOpcode.Store: return Cache.Store(storeMode, name, value);
                    case TagMapper.SetOpcode.Cas: return Cache.Cas(storeMode, name, value, cas);
                    case TagMapper.SetOpcode.Decrement: decrement = (DecrementTag)opvalue; return Cache.Decrement(name, decrement.DefaultValue, decrement.Delta);
                    case TagMapper.SetOpcode.DecrementCas: decrement = (DecrementTag)opvalue; return Cache.Decrement(name, decrement.DefaultValue, decrement.Delta, cas);
                    case TagMapper.SetOpcode.Increment: increment = (IncrementTag)opvalue; return Cache.Increment(name, increment.DefaultValue, increment.Delta);
                    case TagMapper.SetOpcode.IncrementCas: increment = (IncrementTag)opvalue; return Cache.Increment(name, increment.DefaultValue, increment.Delta, cas);
                    default: throw new InvalidOperationException();
                }
            if ((absoluteExpiration != ServiceCache.InfiniteAbsoluteExpiration) && (slidingExpiration == ServiceCache.NoSlidingExpiration))
                switch (_tagMapper.ToSetOpcode(tag, ref name, out cas, out opvalue, out storeMode))
                {
                    case TagMapper.SetOpcode.Prepend:
                    case TagMapper.SetOpcode.PrependCas:
                        throw new NotSupportedException("Operation not supported with absoluteExpiration && slidingExpiration");
                    case TagMapper.SetOpcode.Store: return Cache.Store(storeMode, name, value, absoluteExpiration);
                    case TagMapper.SetOpcode.Cas: return Cache.Cas(storeMode, name, value, absoluteExpiration, cas);
                    case TagMapper.SetOpcode.Decrement: decrement = (DecrementTag)opvalue; return Cache.Decrement(name, decrement.DefaultValue, decrement.Delta, absoluteExpiration);
                    case TagMapper.SetOpcode.DecrementCas: decrement = (DecrementTag)opvalue; return Cache.Decrement(name, decrement.DefaultValue, decrement.Delta, absoluteExpiration, cas);
                    case TagMapper.SetOpcode.Increment: increment = (IncrementTag)opvalue; return Cache.Increment(name, increment.DefaultValue, increment.Delta, absoluteExpiration);
                    case TagMapper.SetOpcode.IncrementCas: increment = (IncrementTag)opvalue; return Cache.Increment(name, increment.DefaultValue, increment.Delta, absoluteExpiration, cas);
                    default: throw new InvalidOperationException();
                }
            if ((absoluteExpiration == ServiceCache.InfiniteAbsoluteExpiration) && (slidingExpiration != ServiceCache.NoSlidingExpiration))
                switch (_tagMapper.ToSetOpcode(tag, ref name, out cas, out opvalue, out storeMode))
                {
                    case TagMapper.SetOpcode.Prepend:
                    case TagMapper.SetOpcode.PrependCas:
                        throw new NotSupportedException("Operation not supported with absoluteExpiration && slidingExpiration");
                    case TagMapper.SetOpcode.Store: return Cache.Store(storeMode, name, value, slidingExpiration);
                    case TagMapper.SetOpcode.Cas: return Cache.Cas(storeMode, name, value, slidingExpiration, cas);
                    case TagMapper.SetOpcode.Decrement: decrement = (DecrementTag)opvalue; return Cache.Decrement(name, decrement.DefaultValue, decrement.Delta, slidingExpiration);
                    case TagMapper.SetOpcode.DecrementCas: decrement = (DecrementTag)opvalue; return Cache.Decrement(name, decrement.DefaultValue, decrement.Delta, slidingExpiration, cas);
                    case TagMapper.SetOpcode.Increment: increment = (IncrementTag)opvalue; return Cache.Increment(name, increment.DefaultValue, increment.Delta, slidingExpiration);
                    case TagMapper.SetOpcode.IncrementCas: increment = (IncrementTag)opvalue; return Cache.Increment(name, increment.DefaultValue, increment.Delta, slidingExpiration, cas);
                    default: throw new InvalidOperationException();
                }
            throw new InvalidOperationException("absoluteExpiration && slidingExpiration");
        }

        public void Touch(object tag, params string[] names)
        {
            throw new NotSupportedException();
        }

        public ServiceCacheSettings Settings
        {
            get { return null; }
        }
        public ServiceCacheRegistration.IDispatch RegistrationDispatch { get; private set; }

        #region Domain-specific

        public IMemcachedClient Cache { get; private set; }

        public void FlushAll() { Cache.FlushAll(); }
        public IDictionary<string, CasResult<object>> GetWithCas(IEnumerable<string> keys) { return Cache.GetWithCas(keys); }
        public CasResult<object> GetWithCas(string key) { return Cache.GetWithCas(key); }
        public CasResult<T> GetWithCas<T>(string key) { return Cache.GetWithCas<T>(key); }
        public bool TryGetWithCas(string key, out CasResult<object> value) { return TryGetWithCas(key, out value); }

        #endregion
    }
}
