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
namespace Contoso.Abstract
{
    /// <remark>
    /// An static dictionary specific service cache interface
    /// </remark>
    public interface IStaticServiceCache : IServiceCache
    {
        /// <summary>
        /// Gets the cache.
        /// </summary>
        Dictionary<string, object> Cache { get; }
    }

    //: might need to make thread safe
    /// <summary>
    /// 
    /// </summary>
    /// <remark>
    /// Provides a static dictionary adapter for the service cache sub-system.
    ///   </remark>
    ///   <example>
    /// ServiceCacheManager.SetProvider(() =&gt; new StaticServiceCache())
    ///   </example>
    public class StaticServiceCache : IStaticServiceCache, ServiceCacheManager.ISetupRegistration
    {
        private static readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        static StaticServiceCache() { ServiceCacheManager.EnsureRegistration(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticServiceCache"/> class.
        /// </summary>
        public StaticServiceCache()
        {
            Settings = new ServiceCacheSettings(new DefaultFileTouchableCacheItem(this, new DefaultTouchableCacheItem(this, null)));
        }

        Action<IServiceLocator, string> ServiceCacheManager.ISetupRegistration.DefaultServiceRegistrar
        {
            get { return (locator, name) => ServiceCacheManager.RegisterInstance<IStaticServiceCache>(this, locator, name); }
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
        /// Adds an object into cache based on the parameters provided.
        /// </summary>
        /// <param name="tag">Not used</param>
        /// <param name="name">The key used to identify the item in cache.</param>
        /// <param name="itemPolicy">Not used</param>
        /// <param name="value">The value to store in cache.</param>
        /// <param name="dispatch">Not used</param>
        /// <returns>Last value that what in cache.</returns>
        public object Add(object tag, string name, CacheItemPolicy itemPolicy, object value, ServiceCacheByDispatcher dispatch)
        {
            // TODO: Throw on dependency or other stuff not supported by this simple system
            object lastValue;
            if (!_cache.TryGetValue(name, out lastValue))
            {
                _cache[name] = value;
                var registration = dispatch.Registration;
                if (registration != null && registration.UseHeaders)
                {
                    var header = dispatch.Header;
                    header.Item = name;
                    _cache[name + "#"] = header;
                }
                return null;
            }
            return lastValue;
        }

        /// <summary>
        /// Gets the item from cache associated with the key provided.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The key.</param>
        /// <returns>
        /// The cached item.
        /// </returns>
        public object Get(object tag, string name)
        {
            object value;
            return (_cache.TryGetValue(name, out value) ? value : null);
        }
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
            object value;
            header = (registration.UseHeaders && _cache.TryGetValue(name + "#", out value) ? (CacheItemHeader)value : null);
            return (_cache.TryGetValue(name, out value) ? value : null);
        }
        /// <summary>
        /// Gets the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="names">The names.</param>
        /// <returns></returns>
        public object Get(object tag, IEnumerable<string> names)
        {
            if (names == null)
                throw new ArgumentNullException("names");
            return names.Select(name => new { name, value = Get(null, name) }).ToDictionary(x => x.name, x => x.value);
        }
        /// <summary>
        /// Gets the specified registration.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<CacheItemHeader> Get(object tag, IServiceCacheRegistration registration)
        {
            if (registration == null)
                throw new ArgumentNullException("registration");
            var registrationName = registration.AbsoluteName + "#";
            CacheItemHeader value;
            var e = _cache.GetEnumerator();
            while (e.MoveNext())
            {
                var current = e.Current;
                var key = current.Key;
                if (key == null || !key.EndsWith(registrationName) || (value = (current.Value as CacheItemHeader)) == null)
                    continue;
                yield return value;
            }
        }

        /// <summary>
        /// Tries the get.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGet(object tag, string name, out object value) { return _cache.TryGetValue(name, out value); }

        /// <summary>
        /// Adds an object into cache based on the parameters provided.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name used to identify the item in cache.</param>
        /// <param name="itemPolicy">The itemPolicy defining caching policies.</param>
        /// <param name="value">The value to store in cache.</param>
        /// <param name="dispatch"></param>
        /// <returns></returns>
        public object Set(object tag, string name, CacheItemPolicy itemPolicy, object value, ServiceCacheByDispatcher dispatch)
        {
            _cache[name] = value;
            var registration = dispatch.Registration;
            if (registration != null && registration.UseHeaders)
            {
                var header = dispatch.Header;
                header.Item = name;
                _cache[name + "#"] = header;
            }
            return value;
        }

        /// <summary>
        /// Removes from cache the item associated with the key provided.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The key.</param>
        /// <param name="registration">The registration.</param>
        /// <returns>
        /// The item removed from the Cache. If the value in the key parameter is not found, returns null.
        /// </returns>
        public object Remove(object tag, string name, IServiceCacheRegistration registration)
        {
            object value;
            if (_cache.TryGetValue(name, out value))
            {
                if (registration != null && registration.UseHeaders)
                    _cache.Remove(name + "#");
                _cache.Remove(name);
                return value;
            }
            return null;
        }

        /// <summary>
        /// Settings
        /// </summary>
        public ServiceCacheSettings Settings { get; private set; }

        #region TouchableCacheItem

        /// <summary>
        /// DefaultTouchableCacheItem
        /// </summary>
        public class DefaultTouchableCacheItem : ITouchableCacheItem
        {
            private StaticServiceCache _parent;
            private ITouchableCacheItem _base;
            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultTouchableCacheItem"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="base">The @base.</param>
            public DefaultTouchableCacheItem(StaticServiceCache parent, ITouchableCacheItem @base) { _parent = parent; _base = @base; }

            /// <summary>
            /// Touches the specified tag.
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="names">The names.</param>
            public void Touch(object tag, string[] names)
            {
                if (names == null || names.Length == 0)
                    return;
                _cache.Clear();
                if (_base != null)
                    _base.Touch(tag, names);
            }

            /// <summary>
            /// Makes the dependency.
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="names">The names.</param>
            /// <returns></returns>
            public object MakeDependency(object tag, string[] names)
            {
                if (names == null || names.Length == 0)
                    return null;
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// DefaultFileTouchableCacheItem
        /// </summary>
        public class DefaultFileTouchableCacheItem : ServiceCache.FileTouchableCacheItemBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultFileTouchableCacheItem"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="base">The @base.</param>
            public DefaultFileTouchableCacheItem(StaticServiceCache parent, ITouchableCacheItem @base)
                : base(parent, @base) { }

            /// <summary>
            /// Makes the dependency internal.
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="names">The names.</param>
            /// <returns></returns>
            protected override object MakeDependencyInternal(object tag, string[] names) { return null; }
        }

        #endregion

        #region Domain-specific

        /// <summary>
        /// Gets the cache.
        /// </summary>
        public Dictionary<string, object> Cache
        {
            get { return _cache; }
        }

        #endregion
    }
}
