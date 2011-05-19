using System;
using System.Abstract;
using Contoso.Abstract;
using Enyim.Caching.Configuration;
using System.Collections.Generic;
using System.Net;
using Enyim.Caching.Memcached;
using Enyim.Caching;
namespace Example.Memcached
{
	public class ConstructorExamples
	{
		public void Constructor()
		{
			// set service cache provider from app.config
			ServiceCacheManager.SetProvider(() => new MemcachedServiceCache());

			// set service cache provider from a IMemcachedClientConfiguration
			var configuration = new MemcachedClientConfiguration();
			configuration.AddServer("serveraddress", 11211);
			ServiceCacheManager.SetProvider(() => new MemcachedServiceCache(configuration));

			// set service cache provider from an existing MemcachedClient
			var memcachedClient = new MemcachedClient();
			ServiceCacheManager.SetProvider(() => new MemcachedServiceCache(memcachedClient));
		}
	}
}
