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
        /// <summary>
        /// Gets the cache.
        /// </summary>
        DataCache Cache { get; }
        //
        /// <summary>
        /// Gets the and lock.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="lockHandle">The lock handle.</param>
        /// <returns></returns>
        object GetAndLock(string name, TimeSpan timeout, out DataCacheLockHandle lockHandle);
        /// <summary>
        /// Gets the and lock.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="lockHandle">The lock handle.</param>
        /// <param name="forceLock">if set to <c>true</c> [force lock].</param>
        /// <returns></returns>
        object GetAndLock(string name, TimeSpan timeout, out DataCacheLockHandle lockHandle, bool forceLock);
        /// <summary>
        /// Unlocks the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="lockHandle">The lock handle.</param>
        void Unlock(string name, DataCacheLockHandle lockHandle);
        /// <summary>
        /// Unlocks the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="lockHandle">The lock handle.</param>
        /// <param name="timeout">The timeout.</param>
        void Unlock(string name, DataCacheLockHandle lockHandle, TimeSpan timeout);
    }

    /// <summary>
    /// ServerAppFabricServiceCache
    /// </summary>
    public class ServerAppFabricServiceCache : IServerAppFabricServiceCache, ServiceCacheManager.ISetupRegistration
    {
        /// <summary>
        /// DefaultDataCacheFactory
        /// </summary>
        public static readonly DataCacheFactory DefaultCacheFactory = new DataCacheFactory();

        static ServerAppFabricServiceCache() { ServiceCacheManager.EnsureRegistration(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerAppFabricServiceCache"/> class.
        /// </summary>
        public ServerAppFabricServiceCache()
            : this(DefaultCacheFactory) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerAppFabricServiceCache"/> class.
        /// </summary>
        /// <param name="cacheName">Name of the cache.</param>
        public ServerAppFabricServiceCache(string cacheName)
            : this(DefaultCacheFactory, cacheName) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerAppFabricServiceCache"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public ServerAppFabricServiceCache(DataCacheFactoryConfiguration configuration)
            : this(new DataCacheFactory(configuration)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerAppFabricServiceCache"/> class.
        /// </summary>
        /// <param name="cacheFactory">The cache factory.</param>
        public ServerAppFabricServiceCache(DataCacheFactory cacheFactory)
            : this(cacheFactory.GetDefaultCache()) { CacheFactory = cacheFactory; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerAppFabricServiceCache"/> class.
        /// </summary>
        /// <param name="cacheFactory">The cache factory.</param>
        /// <param name="cacheName">Name of the cache.</param>
        public ServerAppFabricServiceCache(DataCacheFactory cacheFactory, string cacheName)
            : this(cacheFactory.GetCache(cacheName)) { CacheFactory = cacheFactory; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerAppFabricServiceCache"/> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        public ServerAppFabricServiceCache(DataCache cache)
        {
            Cache = cache;
            Settings = new ServiceCacheSettings(new DefaultTouchableCacheItem(this, null));
        }

        Action<IServiceLocator, string> ServiceCacheManager.ISetupRegistration.DefaultServiceRegistrar
        {
            get { return (locator, name) => ServiceCacheManager.RegisterInstance<IServerAppFabricServiceCache>(this, locator, name); }
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
        public object Get(object tag, string name)
        {
            var version = (tag as DataCacheItemVersion);
            string regionName;
            if (version == null)
                return (!Settings.TryGetRegion(ref name, out regionName) ? Cache.Get(name) : Cache.Get(name, regionName));
            return (!Settings.TryGetRegion(ref name, out regionName) ? Cache.GetIfNewer(name, ref version) : Cache.GetIfNewer(name, ref version, regionName));
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
            var version = (tag as DataCacheItemVersion);
            string regionName;
            if (!registration.UseHeaders)
                header = null;
            else if (version == null)
                header = (CacheItemHeader)(!Settings.TryGetRegion(ref name, out regionName) ? Cache.Get(name + "#") : Cache.Get(name + "#", regionName));
            else
                header = (CacheItemHeader)(!Settings.TryGetRegion(ref name, out regionName) ? Cache.GetIfNewer(name + "#", ref version) : Cache.GetIfNewer(name + "#", ref version, regionName));
            if (version == null)
                return (!Settings.TryGetRegion(ref name, out regionName) ? Cache.Get(name) : Cache.Get(name, regionName));
            return (!Settings.TryGetRegion(ref name, out regionName) ? Cache.GetIfNewer(name, ref version) : Cache.GetIfNewer(name, ref version, regionName));
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
        /// Removes from cache the item associated with the key provided.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <param name="registration">The registration.</param>
        /// <returns>
        /// The item removed from the Cache. If the value in the key parameter is not found, returns null.
        /// </returns>
        public object Remove(object tag, string name, IServiceCacheRegistration registration)
        {
            if (registration != null && registration.UseHeaders)
                Remove(tag, name + "#", null);
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
            private ServerAppFabricServiceCache _parent;
            private ITouchableCacheItem _base;
            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultTouchableCacheItem"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="base">The @base.</param>
            public DefaultTouchableCacheItem(ServerAppFabricServiceCache parent, ITouchableCacheItem @base) { _parent = parent; _base = @base; }

            /// <summary>
            /// Touches the specified tag.
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="names">The names.</param>
            public void Touch(object tag, string[] names)
            {
                if (names == null || names.Length == 0)
                    return;
                throw new NotSupportedException();
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
                var dataCacheTags = names.Select(x => new DataCacheTag(x));
                return (_base == null ? dataCacheTags : dataCacheTags.Union(_base.MakeDependency(tag, names) as IEnumerable<DataCacheTag>));
            }
        }

        #endregion

        #region Domain-specific

        /// <summary>
        /// Gets the cache factory.
        /// </summary>
        public DataCacheFactory CacheFactory { get; private set; }
        /// <summary>
        /// Gets the cache.
        /// </summary>
        public DataCache Cache { get; private set; }

        /// <summary>
        /// Gets the and lock.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="lockHandle">The lock handle.</param>
        /// <returns></returns>
        public object GetAndLock(string name, TimeSpan timeout, out DataCacheLockHandle lockHandle)
        {
            string region;
            return (!Settings.TryGetRegion(ref name, out region) ? Cache.GetAndLock(name, timeout, out lockHandle) : Cache.GetAndLock(name, timeout, out lockHandle, region));
        }
        /// <summary>
        /// Gets the and lock.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="lockHandle">The lock handle.</param>
        /// <param name="forceLock">if set to <c>true</c> [force lock].</param>
        /// <returns></returns>
        public object GetAndLock(string name, TimeSpan timeout, out DataCacheLockHandle lockHandle, bool forceLock)
        {
            string region;
            return (!Settings.TryGetRegion(ref name, out region) ? Cache.GetAndLock(name, timeout, out lockHandle, forceLock) : Cache.GetAndLock(name, timeout, out lockHandle, region, forceLock));
        }

        /// <summary>
        /// Unlocks the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="lockHandle">The lock handle.</param>
        public void Unlock(string name, DataCacheLockHandle lockHandle)
        {
            string region;
            if (!Settings.TryGetRegion(ref name, out region)) Cache.Unlock(name, lockHandle);
            else Cache.Unlock(name, lockHandle);
        }
        /// <summary>
        /// Unlocks the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="lockHandle">The lock handle.</param>
        /// <param name="timeout">The timeout.</param>
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

        private static TimeSpan GetTimeout(DateTime absoluteExpiration) { return (absoluteExpiration == ServiceCache.InfiniteAbsoluteExpiration ? TimeSpan.Zero : absoluteExpiration - DateTime.Now); }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        public IEnumerable<object> GetHeaders(ServiceCacheRegistration registration)
        {
            return null;
        }
    }
}
