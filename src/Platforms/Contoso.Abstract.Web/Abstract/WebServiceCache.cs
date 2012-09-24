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
using System.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using WebCache = System.Web.Caching.Cache;
using WebCacheDependency = System.Web.Caching.CacheDependency;
using WebCacheItemPriority = System.Web.Caching.CacheItemPriority;
using WebCacheItemRemovedCallback = System.Web.Caching.CacheItemRemovedCallback;
namespace Contoso.Abstract
{
    /// <summary>
    /// IWebServiceCache
    /// </summary>
    public interface IWebServiceCache : IServiceCache
    {
        /// <summary>
        /// Gets the cache.
        /// </summary>
        WebCache Cache { get; }
    }

    /// <summary>
    /// WebServiceCache
    /// </summary>
    public class WebServiceCache : IWebServiceCache, ServiceCacheManager.ISetupRegistration
    {
        static WebServiceCache() { ServiceCacheManager.EnsureRegistration(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="WebServiceCache"/> class.
        /// </summary>
        public WebServiceCache()
        {
            Cache = (HttpRuntime.Cache ?? HostingEnvironment.Cache);
            Settings = new ServiceCacheSettings(new DefaultFileTouchableCacheItem(this, new DefaultTouchableCacheItem(this, null)));
        }

        Action<IServiceLocator, string> ServiceCacheManager.ISetupRegistration.DefaultServiceRegistrar
        {
            get { return (locator, name) => ServiceCacheManager.RegisterInstance<IWebServiceCache>(this, locator, name); }
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
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
            // item priority
            WebCacheItemPriority cacheItemPriority;
            switch (itemPolicy.Priority)
            {
                case CacheItemPriority.AboveNormal: cacheItemPriority = WebCacheItemPriority.AboveNormal; break;
                case CacheItemPriority.BelowNormal: cacheItemPriority = WebCacheItemPriority.BelowNormal; break;
                case CacheItemPriority.High: cacheItemPriority = WebCacheItemPriority.High; break;
                case CacheItemPriority.Low: cacheItemPriority = WebCacheItemPriority.Low; break;
                case CacheItemPriority.Normal: cacheItemPriority = WebCacheItemPriority.Normal; break;
                case CacheItemPriority.NotRemovable: cacheItemPriority = WebCacheItemPriority.NotRemovable; break;
                default: cacheItemPriority = WebCacheItemPriority.Default; break;
            }
            //
            var removedCallback = (itemPolicy.RemovedCallback == null ? null : new WebCacheItemRemovedCallback((n, v, c) => { itemPolicy.RemovedCallback(n, v); }));
            value = Cache.Add(name, value, GetCacheDependency(tag, itemPolicy.Dependency, dispatch), itemPolicy.AbsoluteExpiration, itemPolicy.SlidingExpiration, cacheItemPriority, removedCallback);
            var registration = dispatch.Registration;
            if (registration != null && registration.UseHeaders)
            {
                var headerPolicy = new WebCacheDependency(null, new[] { name });
                var header = dispatch.Header;
                header.Item = name;
                Cache.Add(name + "#", header, headerPolicy, itemPolicy.AbsoluteExpiration, itemPolicy.SlidingExpiration, cacheItemPriority, null);
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
            if (names == null)
                throw new ArgumentNullException("names");
            return names.Select(name => new { name, value = Cache.Get(name) }).ToDictionary(x => x.name, x => x.value);
        }
        /// <summary>
        /// Gets the specified registration.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public IEnumerable<CacheItemHeader> Get(object tag, IServiceCacheRegistration registration)
        {
            if (registration == null)
                throw new ArgumentNullException("registration");
            var registrationName = registration.AbsoluteName + "#";
            CacheItemHeader value;
            var e = Cache.GetEnumerator();
            while (e.MoveNext())
            {
                var key = (e.Key as string);
                if (key == null || !key.EndsWith(registrationName) || (value = (e.Value as CacheItemHeader)) == null)
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
        public bool TryGet(object tag, string name, out object value)
        {
            throw new NotSupportedException();
        }

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
            // item priority
            WebCacheItemPriority cacheItemPriority;
            switch (itemPolicy.Priority)
            {
                case CacheItemPriority.AboveNormal: cacheItemPriority = WebCacheItemPriority.AboveNormal; break;
                case CacheItemPriority.BelowNormal: cacheItemPriority = WebCacheItemPriority.BelowNormal; break;
                case CacheItemPriority.High: cacheItemPriority = WebCacheItemPriority.High; break;
                case CacheItemPriority.Low: cacheItemPriority = WebCacheItemPriority.Low; break;
                case CacheItemPriority.Normal: cacheItemPriority = WebCacheItemPriority.Normal; break;
                case CacheItemPriority.NotRemovable: cacheItemPriority = WebCacheItemPriority.NotRemovable; break;
                default: cacheItemPriority = WebCacheItemPriority.Default; break;
            }
            //
            var removedCallback = (itemPolicy.RemovedCallback == null ? null : new WebCacheItemRemovedCallback((n, v, c) => { itemPolicy.RemovedCallback(n, v); }));
            Cache.Insert(name, value, GetCacheDependency(tag, itemPolicy.Dependency, dispatch), itemPolicy.AbsoluteExpiration, itemPolicy.SlidingExpiration, cacheItemPriority, removedCallback);
            var registration = dispatch.Registration;
            if (registration != null && registration.UseHeaders)
            {
                var headerPolicy = new WebCacheDependency(null, new[] { name });
                var header = dispatch.Header;
                header.Item = name;
                Cache.Insert(name + "#", header, headerPolicy, itemPolicy.AbsoluteExpiration, itemPolicy.SlidingExpiration, cacheItemPriority, null);
            }
            return value;
        }

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
        /// Settings
        /// </summary>
        public ServiceCacheSettings Settings { get; private set; }

        #region TouchableCacheItem

        /// <summary>
        /// DefaultTouchableCacheItem
        /// </summary>
        public class DefaultTouchableCacheItem : ITouchableCacheItem
        {
            private WebServiceCache _parent;
            private ITouchableCacheItem _base;
            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultTouchableCacheItem"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="base">The @base.</param>
            public DefaultTouchableCacheItem(WebServiceCache parent, ITouchableCacheItem @base) { _parent = parent; _base = @base; }

            /// <summary>
            /// Touches the specified tag.
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="names">The names.</param>
            public void Touch(object tag, string[] names)
            {
                if (names == null || names.Length == 0)
                    return;
                var cache = _parent.Cache;
                foreach (var name in names)
                    cache.Insert(name, string.Empty, null, ServiceCache.InfiniteAbsoluteExpiration, ServiceCache.NoSlidingExpiration, WebCacheItemPriority.Normal, null);
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
                var cache = _parent.Cache;
                // ensure
                foreach (var name in names)
                    cache.Insert(name, string.Empty, null, ServiceCache.InfiniteAbsoluteExpiration, ServiceCache.NoSlidingExpiration, WebCacheItemPriority.Normal, null);
                return (_base == null ? new WebCacheDependency(null, names) : new WebCacheDependency(null, names, _base.MakeDependency(tag, names) as WebCacheDependency));
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
            public DefaultFileTouchableCacheItem(WebServiceCache parent, ITouchableCacheItem @base)
                : base(parent, @base) { }

            /// <summary>
            /// Makes the dependency internal.
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="names">The names.</param>
            /// <returns></returns>
            protected override object MakeDependencyInternal(object tag, string[] names) { return new WebCacheDependency(names.Select(x => GetFilePathForName(x)).ToArray()); }
            //    var touchablesDependency = (touchables.Count > 0 ? (WebCacheDependency)touchable.MakeDependency(tag, touchables.ToArray())(this, tag) : null);
            //    return (touchablesDependency == null ? new WebCacheDependency(null, cacheKeys.ToArray()) : new WebCacheDependency(null, cacheKeys.ToArray(), touchablesDependency));
        }

        #endregion

        #region Domain-specific

        /// <summary>
        /// Gets the cache.
        /// </summary>
        public WebCache Cache { get; private set; }

        #endregion

        private WebCacheDependency GetCacheDependency(object tag, CacheItemDependency dependency, ServiceCacheByDispatcher dispatch)
        {
            object value;
            if (dependency == null || (value = dependency(this, dispatch.Registration, tag, dispatch.Values)) == null)
                return null;
            //
            var names = (value as string[]);
            var touchable = Settings.Touchable;
            return ((touchable != null && names != null ? touchable.MakeDependency(tag, names) : value) as WebCacheDependency);
        }
    }
}
