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
    public interface IServerAppFabricServiceCache : IServiceCache
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
    public class ServerAppFabricServiceCache : IServerAppFabricServiceCache
    {
        public ServerAppFabricServiceCache(DataCache cache)
        {
            Cache = cache;
            Settings = new ServerAppFabricServiceCacheSettings();
        }

        public DataCache Cache { get; private set; }

        public ServerAppFabricServiceCacheSettings Settings { get; private set; }

        public object this[string name]
        {
            get { return Get(name, null); }
            set { Insert(name, value, null, DateTime.Now.AddMinutes(60), ServiceCache.NoSlidingExpiration, CacheItemPriority.Normal, null, null); }
        }

        public object Add(string name, object value, ServiceCacheDependency dependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback, object tag)
        {
            if (slidingExpiration != ServiceCache.NoSlidingExpiration)
                throw new ArgumentOutOfRangeException("slidingExpiration", "not supported.");
            if (onRemoveCallback != null)
                throw new ArgumentOutOfRangeException("onRemoveCallback", "not supported.");
            //
            var tags = (dependency == null ? null : dependency.CacheTags.Select(x => new DataCacheTag(x)));
            var timeout = (absoluteExpiration == ServiceCache.NoAbsoluteExpiration ? TimeSpan.Zero : DateTime.Now - absoluteExpiration);
            string region;
            if ((timeout == TimeSpan.Zero) && (tags == null))
            {
                if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.Add(name, value);
                else Cache.Add(name, value, region);
            }
            else if ((timeout != TimeSpan.Zero) && (tags == null))
            {
                if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.Add(name, value, timeout);
                else Cache.Add(name, value, timeout, region);
            }
            else if ((timeout == TimeSpan.Zero) && (tags != null))
            {
                if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.Add(name, value, tags);
                else Cache.Add(name, value, tags, region);
            }
            else
            {
                if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.Add(name, value, timeout, tags);
                else Cache.Add(name, value, timeout, tags, region);
            }
            return value;
        }

        public object Get(string name, object tag)
        {
            var version = (tag as DataCacheItemVersion);
            string region;
            if (version == null)
                return (!TryGetRegion(Settings.RegionMarker, ref name, out region) ? Cache.Get(name) : Cache.Get(name, region));
            return (!TryGetRegion(Settings.RegionMarker, ref name, out region) ? Cache.GetIfNewer(name, ref version) : Cache.GetIfNewer(name, ref version, region));
        }

        public object Remove(string name, object tag)
        {
            string region;
            string regionMarker = Settings.RegionMarker;
            var value = (!Settings.ReturnsCachedValueOnRemove ? null : (!TryGetRegion(regionMarker, ref name, out region) ? Cache.Get(name) : Cache.Get(name, region)));
            //
            var version = (tag as DataCacheItemVersion);
            var lockHandle = (tag as DataCacheLockHandle);
            if ((version == null) && (lockHandle == null))
            {
                if (!TryGetRegion(regionMarker, ref name, out region)) Cache.Remove(name);
                else Cache.Remove(name, region);
            }
            else if (version != null)
            {
                if (!TryGetRegion(regionMarker, ref name, out region)) Cache.Remove(name, version);
                else Cache.Remove(name, version, region);
            }
            else
            {
                if (!TryGetRegion(regionMarker, ref name, out region)) Cache.Remove(name, lockHandle);
                else Cache.Remove(name, lockHandle, region);
            }
            return value;
        }

        public object Insert(string name, object value, ServiceCacheDependency dependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback, object tag)
        {
            if (slidingExpiration != ServiceCache.NoSlidingExpiration)
                throw new ArgumentOutOfRangeException("slidingExpiration", "not supported.");
            if (onRemoveCallback != null)
                throw new ArgumentOutOfRangeException("onRemoveCallback", "not supported.");
            //
            var oldVersion = (tag as DataCacheItemVersion);
            var lockHandle = (tag as DataCacheLockHandle);
            var tags = ((dependency == null) && (dependency.CacheTags != null) ? null : dependency.CacheTags.Select(x => new DataCacheTag(x)));
            var timeout = (absoluteExpiration == ServiceCache.NoAbsoluteExpiration ? TimeSpan.Zero : DateTime.Now - absoluteExpiration);
            string region;
            if ((timeout == TimeSpan.Zero) && (tags == null))
                if ((oldVersion == null) && (lockHandle == null))
                {
                    if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.Put(name, value);
                    else Cache.Put(name, value, region);
                }
                else if (oldVersion != null)
                {
                    if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.Put(name, value, oldVersion);
                    else Cache.Put(name, value, oldVersion, region);
                }
                else
                {
                    if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.PutAndUnlock(name, value, lockHandle);
                    else Cache.PutAndUnlock(name, value, lockHandle, region);
                }
            else if ((timeout != TimeSpan.Zero) && (tags == null))
                if ((oldVersion == null) && (lockHandle == null))
                {
                    if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.Put(name, value, timeout);
                    else Cache.Put(name, value, timeout, region);
                }
                else if (oldVersion != null)
                {
                    if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.Put(name, value, oldVersion, timeout);
                    else Cache.Put(name, value, oldVersion, timeout, region);
                }
                else
                {
                    if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.PutAndUnlock(name, value, lockHandle, timeout);
                    else Cache.PutAndUnlock(name, value, lockHandle, timeout, region);
                }
            else if ((timeout == TimeSpan.Zero) && (tags != null))
                if ((oldVersion == null) && (lockHandle == null))
                {
                    if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.Put(name, value, tags);
                    else Cache.Put(name, value, tags, region);
                }
                else if (oldVersion != null)
                {
                    if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.Put(name, value, oldVersion, tags);
                    else Cache.Put(name, value, oldVersion, tags, region);
                }
                else
                {
                    if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.PutAndUnlock(name, value, lockHandle, tags);
                    else Cache.PutAndUnlock(name, value, lockHandle, tags, region);
                }
            else
                if ((oldVersion == null) && (lockHandle == null))
                {
                    if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.Put(name, value, timeout, tags);
                    else Cache.Put(name, value, timeout, tags, region);
                }
                else if (oldVersion != null)
                {
                    if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.Put(name, value, oldVersion, timeout, tags);
                    else Cache.Put(name, value, oldVersion, timeout, tags, region);
                }
                else
                {
                    if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.PutAndUnlock(name, value, lockHandle, timeout, tags);
                    else Cache.PutAndUnlock(name, value, lockHandle, timeout, tags, region);
                }
            return value;
        }

        public void Touch(string name, object tag)
        {
            throw new NotSupportedException();
        }

        #region Domain-specific

        public object GetAndLock(string name, TimeSpan timeout, out DataCacheLockHandle lockHandle)
        {
            string region;
            return (!TryGetRegion(Settings.RegionMarker, ref name, out region) ? Cache.GetAndLock(name, timeout, out lockHandle) : Cache.GetAndLock(name, timeout, out lockHandle, region));
        }
        public object GetAndLock(string name, TimeSpan timeout, out DataCacheLockHandle lockHandle, bool forceLock)
        {
            string region;
            return (!TryGetRegion(Settings.RegionMarker, ref name, out region) ? Cache.GetAndLock(name, timeout, out lockHandle, forceLock) : Cache.GetAndLock(name, timeout, out lockHandle, region, forceLock));
        }

        public void Unlock(string name, DataCacheLockHandle lockHandle)
        {
            string region;
            if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.Unlock(name, lockHandle);
            else Cache.Unlock(name, lockHandle);
        }
        public void Unlock(string name, DataCacheLockHandle lockHandle, TimeSpan timeout)
        {
            string region;
            if (!TryGetRegion(Settings.RegionMarker, ref name, out region)) Cache.Unlock(name, lockHandle, timeout);
            else Cache.Unlock(name, lockHandle, timeout);
        }

        #endregion

        private static bool TryGetRegion(string regionMarker, ref string name, out string region)
        {
            var index = name.IndexOf(regionMarker);
            if (index != -1)
            {
                region = null;
                return false;
            }
            string originalName = name;
            region = originalName.Substring(0, index);
            name = originalName.Substring(index + regionMarker.Length);
            return true;
        }
    }
}
