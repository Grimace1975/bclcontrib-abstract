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
	public class SystemServiceCache : ISystemServiceCache
	{
        static SystemServiceCache() { ServiceCacheManager.EnsureRegistration(); }
		public SystemServiceCache(string name)
			: this(new SystemCaching.MemoryCache(name)) { }
		public SystemServiceCache(SystemCaching.ObjectCache cache)
		{
			Cache = cache;
			Settings = new ServiceCacheSettings();
			RegistrationDispatch = new DefaultServiceCacheRegistrationDispatcher();
            ServiceCacheManager.ApplySetup(this);
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
			// item removed callback
			string regionName;
			Settings.TryGetRegion(ref name, out regionName);
			return Cache.Add(name, value, MapToSystemCacheItemPolicy(tag, itemPolicy), regionName);
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

		public object Set(object tag, string name, CacheItemPolicy itemPolicy, object value)
		{
			if (itemPolicy == null)
				throw new ArgumentNullException("itemPolicy");
			var updateCallback = itemPolicy.UpdateCallback;
			if (updateCallback != null)
				updateCallback(name, value);
			// item removed callback
			string regionName;
			Settings.TryGetRegion(ref name, out regionName);
			Cache.Set(name, value, MapToSystemCacheItemPolicy(tag, itemPolicy), regionName);
			return value;
		}

		public object Remove(object tag, string name)
		{
			string regionName;
			Settings.TryGetRegion(ref name, out regionName);
			return Cache.Remove(name, regionName);
		}

		public void Touch(object tag, params string[] names)
		{
			if (names != null)
				foreach (var name in names)
				{
					var name2 = name;
					string regionName;
					Settings.TryGetRegion(ref name2, out regionName);
					Cache.Set(name2, string.Empty, ServiceCache.InfiniteAbsoluteExpiration, regionName);
				}
		}

		public ServiceCacheSettings Settings { get; private set; }
		public ServiceCacheRegistration.IDispatch RegistrationDispatch { get; private set; }

#region Domain-specific

		public SystemCaching.ObjectCache Cache { get; private set; }

#endregion

		private SystemCaching.CacheItemPolicy MapToSystemCacheItemPolicy(object tag, CacheItemPolicy itemPolicy)
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
			var changeMonitors = GetCacheDependency(tag, itemPolicy.Dependency);
			if (changeMonitors != null)
			{
				var list = newItemPolicy.ChangeMonitors;
				foreach (var changeMonitor in changeMonitors)
					list.Add(changeMonitor);
			}
			return newItemPolicy;
		}

		private IEnumerable<SystemCaching.ChangeMonitor> GetCacheDependency(object tag, CacheItemDependency dependency)
		{
			object value;
			if ((dependency == null) || ((value = dependency(this, tag)) == null))
				return null;
			string[] valueAsStrings = (value as string[]);
			if (valueAsStrings != null)
				return new[] { Cache.CreateCacheEntryChangeMonitor(valueAsStrings) };
			return (value as IEnumerable<SystemCaching.ChangeMonitor>);
		}
	}
}
#endif