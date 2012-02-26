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
using Microsoft.ApplicationServer.Caching;
using System.Collections.Generic;
// [Overview] http://blogs.msdn.com/b/endpoint/archive/2010/03/24/windows-server-appfabric-architecture.aspx
// [Architecture] http://msdn.microsoft.com/en-us/library/ee677374.aspx
// http://msdn.microsoft.com/en-us/windowsserver/ee695849
namespace Contoso.Abstract
{
    /// <summary>
    /// IServerAppFabricServiceCache
    /// </summary>
    public interface IServerAppFabricServiceCache : IDistributedServiceCache
    {
        DataCache Cache { get; }
        //
        object GetAndLock(string name, TimeSpan timeout, out DataCacheLockHandle lockHandle);
        object GetAndLock(string name, TimeSpan timeout, out DataCacheLockHandle lockHandle, bool forceLock);
        void Unlock(string name, DataCacheLockHandle lockHandle);
        void Unlock(string name, DataCacheLockHandle lockHandle, TimeSpan timeout);
    }

    /// <summary>
    /// ServerAppFabricServiceCache
    /// </summary>
    public class ServerAppFabricServiceCache : IServerAppFabricServiceCache, ServiceCacheManager.ISetupRegistration
    {
        static ServerAppFabricServiceCache() { ServiceCacheManager.EnsureRegistration(); }
        public ServerAppFabricServiceCache()
            : this(new DataCacheFactory()) { }
        public ServerAppFabricServiceCache(DataCacheFactoryConfiguration configuration)
            : this(new DataCacheFactory(configuration)) { }
        public ServerAppFabricServiceCache(DataCacheFactory cacheFactory)
            : this(cacheFactory.GetDefaultCache()) { CacheFactory = cacheFactory; }
        public ServerAppFabricServiceCache(DataCacheFactory cacheFactory, string cacheName)
            : this(cacheFactory.GetCache(cacheName)) { CacheFactory = cacheFactory; }
        public ServerAppFabricServiceCache(DataCache cache)
        {
            Cache = cache;
            Settings = new ServiceCacheSettings(new DefaultTouchableCacheItem(this, null));
        }

        Action<IServiceLocator, string> ServiceCacheManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (locator, name) => ServiceCacheManager.RegisterInstance<IServerAppFabricServiceCache>(this, locator, name); }
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
            //
            if (itemPolicy.SlidingExpiration != ServiceCache.NoSlidingExpiration)
                throw new ArgumentOutOfRangeException("itemPolicy.SlidingExpiration", "not supported.");
            if (itemPolicy.RemovedCallback != null)
                throw new ArgumentOutOfRangeException("itemPolicy.RemovedCallback", "not supported.");
            //
            var dataCacheTags = GetCacheDependency(tag, itemPolicy.Dependency, dispatch);
            var timeout = GetTimeout(itemPolicy.AbsoluteExpiration);
            string regionName;
            if (timeout == TimeSpan.Zero && dataCacheTags == null)
            {
                if (!Settings.TryGetRegion(ref name, out regionName)) Cache.Add(name, value);
                else Cache.Add(name, value, regionName);
            }
            else if (timeout != TimeSpan.Zero && dataCacheTags == null)
            {
                if (!Settings.TryGetRegion(ref name, out regionName)) Cache.Add(name, value, timeout);
                else Cache.Add(name, value, timeout, regionName);
            }
            else if (timeout == TimeSpan.Zero && dataCacheTags != null)
            {
                if (!Settings.TryGetRegion(ref name, out regionName)) Cache.Add(name, value, dataCacheTags);
                else Cache.Add(name, value, dataCacheTags, regionName);
            }
            else
            {
                if (!Settings.TryGetRegion(ref name, out regionName)) Cache.Add(name, value, timeout, dataCacheTags);
                else Cache.Add(name, value, timeout, dataCacheTags, regionName);
            }
            return value;
        }

        public object Get(object tag, string name)
        {
            var version = (tag as DataCacheItemVersion);
            string regionName;
            if (version == null)
                return (!Settings.TryGetRegion(ref name, out regionName) ? Cache.Get(name) : Cache.Get(name, regionName));
            return (!Settings.TryGetRegion(ref name, out regionName) ? Cache.GetIfNewer(name, ref version) : Cache.GetIfNewer(name, ref version, regionName));
        }

        public object Get(object tag, IEnumerable<string> names)
        {
            if (names == null)
                throw new ArgumentNullException("names");
            return names.Select(name => new { name, value = Get(null, name) }).ToDictionary(x => x.name, x => x.value);
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
            //
            if (itemPolicy.SlidingExpiration != ServiceCache.NoSlidingExpiration)
                throw new ArgumentOutOfRangeException("itemPolicy.SlidingExpiration", "not supported.");
            if (itemPolicy.RemovedCallback != null)
                throw new ArgumentOutOfRangeException("itemPolicy.RemovedCallback", "not supported.");
            //
            var oldVersion = (tag as DataCacheItemVersion);
            var lockHandle = (tag as DataCacheLockHandle);
            var dataCacheTags = GetCacheDependency(tag, itemPolicy.Dependency, dispatch);
            var timeout = GetTimeout(itemPolicy.AbsoluteExpiration);
            string regionName;
            if (timeout == TimeSpan.Zero && dataCacheTags == null)
                if (oldVersion == null && lockHandle == null)
                {
                    if (!Settings.TryGetRegion(ref name, out regionName)) Cache.Put(name, value);
                    else Cache.Put(name, value, regionName);
                }
                else if (oldVersion != null)
                {
                    if (!Settings.TryGetRegion(ref name, out regionName)) Cache.Put(name, value, oldVersion);
                    else Cache.Put(name, value, oldVersion, regionName);
                }
                else
                {
                    if (!Settings.TryGetRegion(ref name, out regionName)) Cache.PutAndUnlock(name, value, lockHandle);
                    else Cache.PutAndUnlock(name, value, lockHandle, regionName);
                }
            else if (timeout != TimeSpan.Zero && dataCacheTags == null)
                if ((oldVersion == null) && (lockHandle == null))
                {
                    if (!Settings.TryGetRegion(ref name, out regionName)) Cache.Put(name, value, timeout);
                    else Cache.Put(name, value, timeout, regionName);
                }
                else if (oldVersion != null)
                {
                    if (!Settings.TryGetRegion(ref name, out regionName)) Cache.Put(name, value, oldVersion, timeout);
                    else Cache.Put(name, value, oldVersion, timeout, regionName);
                }
                else
                {
                    if (!Settings.TryGetRegion(ref name, out regionName)) Cache.PutAndUnlock(name, value, lockHandle, timeout);
                    else Cache.PutAndUnlock(name, value, lockHandle, timeout, regionName);
                }
            else if (timeout == TimeSpan.Zero && dataCacheTags != null)
                if ((oldVersion == null) && (lockHandle == null))
                {
                    if (!Settings.TryGetRegion(ref name, out regionName)) Cache.Put(name, value, dataCacheTags);
                    else Cache.Put(name, value, dataCacheTags, regionName);
                }
                else if (oldVersion != null)
                {
                    if (!Settings.TryGetRegion(ref name, out regionName)) Cache.Put(name, value, oldVersion, dataCacheTags);
                    else Cache.Put(name, value, oldVersion, dataCacheTags, regionName);
                }
                else
                {
                    if (!Settings.TryGetRegion(ref name, out regionName)) Cache.PutAndUnlock(name, value, lockHandle, dataCacheTags);
                    else Cache.PutAndUnlock(name, value, lockHandle, dataCacheTags, regionName);
                }
            else
                if (oldVersion == null && lockHandle == null)
                {
                    if (!Settings.TryGetRegion(ref name, out regionName)) Cache.Put(name, value, timeout, dataCacheTags);
                    else Cache.Put(name, value, timeout, dataCacheTags, regionName);
                }
                else if (oldVersion != null)
                {
                    if (!Settings.TryGetRegion(ref name, out regionName)) Cache.Put(name, value, oldVersion, timeout, dataCacheTags);
                    else Cache.Put(name, value, oldVersion, timeout, dataCacheTags, regionName);
                }
                else
                {
                    if (!Settings.TryGetRegion(ref name, out regionName)) Cache.PutAndUnlock(name, value, lockHandle, timeout, dataCacheTags);
                    else Cache.PutAndUnlock(name, value, lockHandle, timeout, dataCacheTags, regionName);
                }
            return value;
        }

        public object Remove(object tag, string name)
        {
            string regionName;
            var value = ((Settings.Options & ServiceCacheOptions.ReturnsCachedValueOnRemove) == 0 ? null : (!Settings.TryGetRegion(ref name, out regionName) ? Cache.Get(name) : Cache.Get(name, regionName)));
            //
            var version = (tag as DataCacheItemVersion);
            var lockHandle = (tag as DataCacheLockHandle);
            if (version == null && lockHandle == null)
            {
                if (!Settings.TryGetRegion(ref name, out regionName)) Cache.Remove(name);
                else Cache.Remove(name, regionName);
            }
            else if (version != null)
            {
                if (!Settings.TryGetRegion(ref name, out regionName)) Cache.Remove(name, version);
                else Cache.Remove(name, version, regionName);
            }
            else
            {
                if (!Settings.TryGetRegion(ref name, out regionName)) Cache.Remove(name, lockHandle);
                else Cache.Remove(name, lockHandle, regionName);
            }
            return value;
        }

        public ServiceCacheSettings Settings { get; private set; }

        #region TouchableCacheItem

        /// <summary>
        /// DefaultTouchableCacheItem
        /// </summary>
        public class DefaultTouchableCacheItem : ITouchableCacheItem
        {
            private ServerAppFabricServiceCache _parent;
            private ITouchableCacheItem _base;
            public DefaultTouchableCacheItem(ServerAppFabricServiceCache parent, ITouchableCacheItem @base) { _parent = parent; _base = @base; }

            public void Touch(object tag, string[] names)
            {
                if (names == null || names.Length == 0)
                    return;
                throw new NotSupportedException();
            }

            public object MakeDependency(object tag, string[] names)
            {
                if (names == null || names.Length == 0)
                    return null;
                var dataCacheTags = names.Select(x => new DataCacheTag(x));
                return (_base == null ? dataCacheTags : dataCacheTags.Union(_base.MakeDependency(tag, names) as IEnumerable<DataCacheTag>));
            }
        }

        #endregion

        #region Domain-specific

        public static DataCacheFactory CacheFactory { get; private set; }
        public DataCache Cache { get; private set; }

        public object GetAndLock(string name, TimeSpan timeout, out DataCacheLockHandle lockHandle)
        {
            string region;
            return (!Settings.TryGetRegion(ref name, out region) ? Cache.GetAndLock(name, timeout, out lockHandle) : Cache.GetAndLock(name, timeout, out lockHandle, region));
        }
        public object GetAndLock(string name, TimeSpan timeout, out DataCacheLockHandle lockHandle, bool forceLock)
        {
            string region;
            return (!Settings.TryGetRegion(ref name, out region) ? Cache.GetAndLock(name, timeout, out lockHandle, forceLock) : Cache.GetAndLock(name, timeout, out lockHandle, region, forceLock));
        }

        public void Unlock(string name, DataCacheLockHandle lockHandle)
        {
            string region;
            if (!Settings.TryGetRegion(ref name, out region)) Cache.Unlock(name, lockHandle);
            else Cache.Unlock(name, lockHandle);
        }
        public void Unlock(string name, DataCacheLockHandle lockHandle, TimeSpan timeout)
        {
            string region;
            if (!Settings.TryGetRegion(ref name, out region)) Cache.Unlock(name, lockHandle, timeout);
            else Cache.Unlock(name, lockHandle, timeout);
        }

        #endregion

        private IEnumerable<DataCacheTag> GetCacheDependency(object tag, CacheItemDependency dependency, ServiceCacheByDispatcher dispatch)
        {
            object value;
            if (dependency == null || (value = dependency(this, dispatch.Registration, tag, dispatch.Values)) == null)
                return null;
            //
            var names = (value as string[]);
            var touchable = Settings.Touchable;
            return ((touchable != null && names != null ? touchable.MakeDependency(tag, names) : value) as IEnumerable<DataCacheTag>);
        }

        private static TimeSpan GetTimeout(DateTime absoluteExpiration) { return (absoluteExpiration == ServiceCache.InfiniteAbsoluteExpiration ? TimeSpan.Zero : DateTime.Now - absoluteExpiration); }
    }
}
