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
        WebCache Cache { get; }
    }

    /// <summary>
    /// WebServiceCache
    /// </summary>
    public class WebServiceCache : IWebServiceCache, ServiceCacheManager.ISetupRegistration
    {
        static WebServiceCache() { ServiceCacheManager.EnsureRegistration(); }
        public WebServiceCache()
        {
            Cache = (HttpRuntime.Cache ?? HostingEnvironment.Cache);
            Settings = new ServiceCacheSettings(new DefaultFileTouchableCacheItem(this, new DefaultTouchableCacheItem(this, null)));
        }

        Action<IServiceLocator, string> ServiceCacheManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (locator, name) => ServiceCacheManager.RegisterInstance<IWebServiceCache>(this, locator, name); }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        public object this[string name]
        {
            get { return Get(null, name); }
            set { Set(null, name, CacheItemPolicy.Default, value, ServiceCacheByDispatcher.Empty); }
        }

        public object Add(object tag, string name, CacheItemPolicy itemPolicy, object value, ServiceCacheByDispatcher dispatch)
        {
            if (itemPolicy == null)
                throw new ArgumentNullException("itemPolicy");
            var updateCallback = itemPolicy.UpdateCallback;
            if (updateCallback != null)
                updateCallback(name, value);
            // item priority
            WebCacheItemPriority itemPriority;
            switch (itemPolicy.Priority)
            {
                case CacheItemPriority.AboveNormal:
                    itemPriority = WebCacheItemPriority.AboveNormal;
                    break;
                case CacheItemPriority.BelowNormal:
                    itemPriority = WebCacheItemPriority.BelowNormal;
                    break;
                case CacheItemPriority.High:
                    itemPriority = WebCacheItemPriority.High;
                    break;
                case CacheItemPriority.Low:
                    itemPriority = WebCacheItemPriority.Low;
                    break;
                case CacheItemPriority.Normal:
                    itemPriority = WebCacheItemPriority.Normal;
                    break;
                case CacheItemPriority.NotRemovable:
                    itemPriority = WebCacheItemPriority.NotRemovable;
                    break;
                default:
                    itemPriority = WebCacheItemPriority.Default;
                    break;
            }
            // item removed callback
            var removedCallback = (itemPolicy.RemovedCallback == null ? null : new WebCacheItemRemovedCallback((n, v, c) => { itemPolicy.RemovedCallback(n, v); }));
            return Cache.Add(name, value, GetCacheDependency(tag, itemPolicy.Dependency, dispatch), itemPolicy.AbsoluteExpiration, itemPolicy.SlidingExpiration, itemPriority, removedCallback);
        }

        public object Get(object tag, string name) { return Cache.Get(name); }
        public object Get(object tag, IEnumerable<string> names)
        {
            if (names == null)
                throw new ArgumentNullException("names");
            return names.Select(name => new { name, value = Cache.Get(name) }).ToDictionary(x => x.name, x => x.value);
        }
        public bool TryGet(object tag, string name, out object value)
        {
            throw new NotSupportedException();
        }

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
                case CacheItemPriority.AboveNormal:
                    cacheItemPriority = WebCacheItemPriority.AboveNormal;
                    break;
                case CacheItemPriority.BelowNormal:
                    cacheItemPriority = WebCacheItemPriority.BelowNormal;
                    break;
                case CacheItemPriority.High:
                    cacheItemPriority = WebCacheItemPriority.High;
                    break;
                case CacheItemPriority.Low:
                    cacheItemPriority = WebCacheItemPriority.Low;
                    break;
                case CacheItemPriority.Normal:
                    cacheItemPriority = WebCacheItemPriority.Normal;
                    break;
                case CacheItemPriority.NotRemovable:
                    cacheItemPriority = WebCacheItemPriority.NotRemovable;
                    break;
                default:
                    cacheItemPriority = WebCacheItemPriority.Default;
                    break;
            }
            // item removed callback
            var removedCallback = (itemPolicy.RemovedCallback == null ? null : new WebCacheItemRemovedCallback((n, v, c) => { itemPolicy.RemovedCallback(n, v); }));
            Cache.Insert(name, value, GetCacheDependency(tag, itemPolicy.Dependency, dispatch), itemPolicy.AbsoluteExpiration, itemPolicy.SlidingExpiration, cacheItemPriority, removedCallback);
            return value;
        }

        public object Remove(object tag, string name) { return Cache.Remove(name); }

        public ServiceCacheSettings Settings { get; private set; }

        #region TouchableCacheItem

        /// <summary>
        /// DefaultTouchableCacheItem
        /// </summary>
        public class DefaultTouchableCacheItem : ITouchableCacheItem
        {
            private WebServiceCache _parent;
            private ITouchableCacheItem _base;
            public DefaultTouchableCacheItem(WebServiceCache parent, ITouchableCacheItem @base) { _parent = parent; _base = @base; }

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
            public DefaultFileTouchableCacheItem(WebServiceCache parent, ITouchableCacheItem @base)
                : base(parent, @base) { }

            protected override object MakeDependencyInternal(object tag, string[] names) { return new WebCacheDependency(names.Select(x => GetFilePathForName(x)).ToArray()); }
            //    var touchablesDependency = (touchables.Count > 0 ? (WebCacheDependency)touchable.MakeDependency(tag, touchables.ToArray())(this, tag) : null);
            //    return (touchablesDependency == null ? new WebCacheDependency(null, cacheKeys.ToArray()) : new WebCacheDependency(null, cacheKeys.ToArray(), touchablesDependency));
        }

        #endregion

        #region Domain-specific

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
