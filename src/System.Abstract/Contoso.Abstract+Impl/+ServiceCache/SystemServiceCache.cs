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
        SystemCaching.ObjectCache Cache { get; }
        object Get(object tag, string regionName, IEnumerable<string> names);
    }

    /// <summary>
    /// SystemServiceCache
    /// </summary>
    public class SystemServiceCache : ISystemServiceCache, ServiceCacheManager.ISetupRegistration
    {
        static SystemServiceCache() { ServiceCacheManager.EnsureRegistration(); }
        public SystemServiceCache()
            : this(SystemCaching.MemoryCache.Default) { }
        public SystemServiceCache(string name)
            : this(new SystemCaching.MemoryCache(name)) { }
        public SystemServiceCache(SystemCaching.ObjectCache cache)
        {
            Cache = cache;
            Settings = new ServiceCacheSettings(new DefaultFileTouchableCacheItem(this, new DefaultTouchableCacheItem(this, null)));
        }

        Action<IServiceLocator, string> ServiceCacheManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (locator, name) => ServiceCacheManager.RegisterInstance<ISystemServiceCache>(this, locator, name); }
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
            // item removed callback
            string regionName;
            Settings.TryGetRegion(ref name, out regionName);
            return Cache.Add(name, value, GetCacheDependency(tag, itemPolicy, dispatch), regionName);
        }

        public object Get(object tag, string name)
        {
            string regionName;
            Settings.TryGetRegion(ref name, out regionName);
            return Cache.Get(name, regionName);
        }
        public object Get(object tag, IEnumerable<string> names) { return Cache.GetValues(null, names.ToArray()); }
        public object Get(object tag, string regionName, IEnumerable<string> names) { return Cache.GetValues(regionName, names.ToArray()); }
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

        public object Remove(object tag, string name)
        {
            string regionName;
            Settings.TryGetRegion(ref name, out regionName);
            return Cache.Remove(name, regionName);
        }

        public ServiceCacheSettings Settings { get; private set; }

        #region TouchableCacheItem

        /// <summary>
        /// DefaultTouchableCacheItem
        /// </summary>
        public class DefaultTouchableCacheItem : ITouchableCacheItem
        {
            private SystemServiceCache _parent;
            private ITouchableCacheItem _base;
            public DefaultTouchableCacheItem(SystemServiceCache parent, ITouchableCacheItem @base) { _parent = parent; _base = @base; }

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
            public DefaultFileTouchableCacheItem(SystemServiceCache parent, ITouchableCacheItem @base)
                : base(parent, @base) { }

            protected override object MakeDependencyInternal(object tag, string[] names) { return new SystemCaching.HostFileChangeMonitor(names.Select(x => GetFilePathForName(x)).ToArray()); }
        }

        #endregion

        #region Domain-specific

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