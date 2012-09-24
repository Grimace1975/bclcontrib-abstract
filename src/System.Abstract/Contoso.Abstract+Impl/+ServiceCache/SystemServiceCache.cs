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
#if CLR4
using System;
using System.Linq;
using System.Abstract;
using System.Collections.Generic;
using SystemCaching = System.Runtime.Caching;
using System.Collections.ObjectModel;
namespace Contoso.Abstract
{
    /// <summary>
    /// IWebServiceCache
    /// </summary>
    public interface ISystemServiceCache : IServiceCache
    {
        /// <summary>
        /// Gets the cache.
        /// </summary>
        SystemCaching.ObjectCache Cache { get; }
        /// <summary>
        /// Gets the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="names">The names.</param>
        /// <returns></returns>
        object Get(object tag, string regionName, IEnumerable<string> names);
    }

    /// <summary>
    /// SystemServiceCache
    /// </summary>
    public class SystemServiceCache : ISystemServiceCache, ServiceCacheManager.ISetupRegistration
    {
        static SystemServiceCache() { ServiceCacheManager.EnsureRegistration(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemServiceCache"/> class.
        /// </summary>
        public SystemServiceCache()
            : this(SystemCaching.MemoryCache.Default) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemServiceCache"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public SystemServiceCache(string name)
            : this(new SystemCaching.MemoryCache(name)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemServiceCache"/> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        public SystemServiceCache(SystemCaching.ObjectCache cache)
        {
            Cache = cache;
            Settings = new ServiceCacheSettings(new DefaultFileTouchableCacheItem(this, new DefaultTouchableCacheItem(this, null)));
        }

        Action<IServiceLocator, string> ServiceCacheManager.ISetupRegistration.DefaultServiceRegistrar
        {
            get { return (locator, name) => ServiceCacheManager.RegisterInstance<ISystemServiceCache>(this, locator, name); }
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
            // item removed callback
            string regionName;
            Settings.TryGetRegion(ref name, out regionName);
            return Cache.Add(name, value, GetCacheDependency(tag, itemPolicy, dispatch), regionName);
        }

        /// <summary>
        /// Gets the item from cache associated with the key provided.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The cached item.
        /// </returns>
        public object Get(object tag, string name)
        {
            string regionName;
            Settings.TryGetRegion(ref name, out regionName);
            return Cache.Get(name, regionName);
        }
        /// <summary>
        /// Gets the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="names">The names.</param>
        /// <returns></returns>
        public object Get(object tag, IEnumerable<string> names) { return Cache.GetValues(null, names.ToArray()); }
        /// <summary>
        /// Gets the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="names">The names.</param>
        /// <returns></returns>
        public object Get(object tag, string regionName, IEnumerable<string> names) { return Cache.GetValues(regionName, names.ToArray()); }
        /// <summary>
        /// Tries the get.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGet(object tag, string name, out object value)
        {
            string regionName;
            Settings.TryGetRegion(ref name, out regionName);
            var cacheItem = Cache.GetCacheItem(name, regionName);
            if (cacheItem != null)
            {
                value = cacheItem.Value;
                return true;
            }
            value = null;
            return false;
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
            // item removed callback
            string regionName;
            Settings.TryGetRegion(ref name, out regionName);
            Cache.Set(name, value, GetCacheDependency(tag, itemPolicy, dispatch), regionName);
            return value;
        }

        /// <summary>
        /// Removes from cache the item associated with the key provided.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The item removed from the Cache. If the value in the key parameter is not found, returns null.
        /// </returns>
        public object Remove(object tag, string name)
        {
            string regionName;
            Settings.TryGetRegion(ref name, out regionName);
            return Cache.Remove(name, regionName);
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
            private SystemServiceCache _parent;
            private ITouchableCacheItem _base;
            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultTouchableCacheItem"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="base">The @base.</param>
            public DefaultTouchableCacheItem(SystemServiceCache parent, ITouchableCacheItem @base) { _parent = parent; _base = @base; }

            /// <summary>
            /// Touches the specified tag.
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="names">The names.</param>
            public void Touch(object tag, string[] names)
            {
                if (names == null || names.Length == 0)
                    return;
                var settings = _parent.Settings;
                var cache = _parent.Cache;
                foreach (var name in names)
                {
                    var touchName = name;
                    string regionName;
                    settings.TryGetRegion(ref touchName, out regionName);
                    cache.Set(touchName, string.Empty, ServiceCache.InfiniteAbsoluteExpiration, regionName);
                }
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
                var changeMonitors = new[] { _parent.Cache.CreateCacheEntryChangeMonitor(names) };
                return (_base == null ? changeMonitors : changeMonitors.Union(_base.MakeDependency(tag, names) as IEnumerable<SystemCaching.ChangeMonitor>));
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
            public DefaultFileTouchableCacheItem(SystemServiceCache parent, ITouchableCacheItem @base)
                : base(parent, @base) { }

            /// <summary>
            /// Makes the dependency internal.
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="names">The names.</param>
            /// <returns></returns>
            protected override object MakeDependencyInternal(object tag, string[] names) { return new SystemCaching.HostFileChangeMonitor(names.Select(x => GetFilePathForName(x)).ToArray()); }
        }

        #endregion

        #region Domain-specific

        /// <summary>
        /// Gets the cache.
        /// </summary>
        public SystemCaching.ObjectCache Cache { get; private set; }

        #endregion

        private SystemCaching.CacheItemPolicy GetCacheDependency(object tag, CacheItemPolicy itemPolicy, ServiceCacheByDispatcher dispatch)
        {
            // item priority
            SystemCaching.CacheItemPriority itemPriority;
            switch (itemPolicy.Priority)
            {
                case CacheItemPriority.NotRemovable:
                    itemPriority = SystemCaching.CacheItemPriority.NotRemovable;
                    break;
                default:
                    itemPriority = SystemCaching.CacheItemPriority.Default;
                    break;
            }
            var removedCallback = (itemPolicy.RemovedCallback != null ? new SystemCaching.CacheEntryRemovedCallback(x => { itemPolicy.RemovedCallback(x.CacheItem.Key, x.CacheItem.Value); }) : null);
            var updateCallback = (itemPolicy.UpdateCallback != null ? new SystemCaching.CacheEntryUpdateCallback(x => { itemPolicy.UpdateCallback(x.UpdatedCacheItem.Key, x.UpdatedCacheItem.Value); }) : null);
            var newItemPolicy = new SystemCaching.CacheItemPolicy
            {
                AbsoluteExpiration = itemPolicy.AbsoluteExpiration,
                Priority = itemPriority,
                RemovedCallback = removedCallback,
                SlidingExpiration = itemPolicy.SlidingExpiration,
                UpdateCallback = updateCallback,
            };
            var changeMonitors = GetCacheDependency(tag, itemPolicy.Dependency, dispatch);
            if (changeMonitors != null)
            {
                var list = newItemPolicy.ChangeMonitors;
                foreach (var changeMonitor in changeMonitors)
                    list.Add(changeMonitor);
            }
            return newItemPolicy;
        }

        private IEnumerable<SystemCaching.ChangeMonitor> GetCacheDependency(object tag, CacheItemDependency dependency, ServiceCacheByDispatcher dispatch)
        {
            object value;
            if (dependency == null || (value = dependency(this, dispatch.Registration, tag, dispatch.Values)) == null)
                return null;
            //
            var names = (value as string[]);
            var touchable = Settings.Touchable;
            return ((touchable != null && names != null ? touchable.MakeDependency(tag, names) : value) as IEnumerable<SystemCaching.ChangeMonitor>);
        }
    }
}
#endif