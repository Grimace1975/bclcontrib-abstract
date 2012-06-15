[ServiceCache]: /xyz

# BclEx-Abstract.Memcached
> implementation of ServiceCache for Memcached.

Memcached is an implementation of the abstract [ServiceCache][ServiceCache]. It is based on, and documented, at the [EnyimMemcached](https://github.com/enyim/EnyimMemcached/wiki "EnyimMemcached") repository. It is expected that a Memcached server(s) is configured, and a MemcachedClient or its default configuration can be used to connect to these server(s).

# Defining the Provider

The [MemcachedClient Configuration](https://github.com/enyim/EnyimMemcached/wiki/MemcachedClient-Configuration "MemcachedClient Configuration") states:

> You have two options to configure the MemcachedClient:
>  * using the app.config,
>  * providing an object which implements IMemcachedClientConfiguration
>
> App.config (or web.config) is recommended when your pool is static or you rarely change it. Use the programmatic configuration (either implementing the interface or just using the MemcachedClientConfiguration) when your application has a custom configuration mechanism and you need to integrate memcached there.
>
> _Important! MemcachedClient is a "heavy object", as in creating and initializing the client is quite expensive. (It has a socket pool inside, needs to calculate the tables for the consistent hashing, etc.) So, DO NOT create a client every time you want to perform an operation (well, you can, but 1) you need to Dispose() it after you finished, and 2) this will slow down your application considerably)._

So use the following examples to create a service cache:

If the default configuration section (enyim.com/memcached) exists in the app.config, we can just do

	// set service cache provider from app.config
	ServiceCacheManager.SetProvider(() => new MemcachedServiceCache());


or can be a custom IMemcachedClientConfiguration implementation

	// set service cache provider from a IMemcachedClientConfiguration
	var configuration = new MemcachedClientConfiguration();
	configuration.AddServer("serveraddress", 11211);
	ServiceCacheManager.SetProvider(() => new MemcachedServiceCache(configuration));


alternatively an existing MemcachedClient can be used

	// set service cache provider from an existing MemcachedClient
	var memcachedClient = new MemcachedClient();
	ServiceCacheManager.SetProvider(() => new MemcachedServiceCache(memcachedClient));

## Operation Mapping
<table>
<tr>
	<td>tag</td>
	<td>add method</td>
	<td>insert method</td>
</tr>
<tr>
	<td>null</td>
	<td>Store</td>
	<td>Store</td>
</tr>
<tr>
	<td>CasResult&lt;object&gt;</td>
	<td>Store</td>
	<td>Store</td>
</tr>
<tr>
	<td>ArraySegment&lt;byte&gt;</td>
	<td>Store</td>
	<td>Store</td>
</tr>
<tr>
	<td>CasResult&lt;ArraySegment&lt;byte&gt;&gt;</td>
	<td>Store</td>
	<td>Store</td>
</tr>
<tr>
	<td>DecrementTag</td>
	<td>Store</td>
	<td>Store</td>
</tr>
<tr>
	<td>CasResult&lt;DecrementTag&gt;</td>
	<td>Store</td>
	<td>Store</td>
</tr>
<tr>
	<td>IncrementTag</td>
	<td>Store</td>
	<td>Store</td>
</tr>
<tr>
	<td>CasResult&lt;IncrementTag&gt;</td>
	<td>Store</td>
	<td>Store</td>
</tr>
</table>

## Extended Methods
IMemcachedServiceCache extended methods:
<table>
<tr>
	<td>Method</td>
	<td></td>
</tr>
<tr>
	<td>MemcachedClient Cache { get; }</td>
	<td>get the underlining MemcachedClient.</td>
</tr>
<tr>
	<td>void FlushAll()</td>
	<td>flush all values in client to cache servers.</td>
</tr>
<tr>
	<td>IDictionary<string, CasResult<object>> GetWithCas(IEnumerable<string> keys)</td>
	<td>get multiple values with their Cas.</td>
</tr>
<tr>
	<td>CasResult<object> GetWithCas(string key)</td>
	<td>get value with its Cas.</td>
</tr>
<tr>
	<td>CasResult<T> GetWithCas<T>(string key)</td>
	<td>get value, cast to T, with its Cas.</td>
</tr>
<tr>
	<td>bool TryGetWithCas(string key, out CasResult<object> value)</td>
	<td>try get value with its Cas.</td>
</tr>
</table>