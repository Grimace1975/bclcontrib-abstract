# Service Caches
> abstracts [cache algorithms](http://en.wikipedia.org/wiki/Cache_algorithms) implementations. this object caching abstraction is implemented by various providers.




# Defining the Provider

## Registering a Service Cache

as a singleton:

	ServiceCacheManager.SetProvider(() => new WebServiceCache());

## Consuming a Service Cache

from a singleton:

	var myService = ServiceCache.Get("MyService");

from an injected dependency:

	public MyClass(IServiceCache cache)
	{
	   var myService = cache.Get("MyService");
	}




# Working with IServiceCache
adding and removing objects from cache.

## Item
gets or sets values in abstracted cache.

	object this[string name] { get; set; }
### Examples
	// setting an item in cache
	cache["name"] = "value";
	// getting an item from cache
	var value = cache["name"];

## Add
adds an object to abstracted cache.

	object Add(string name, object value)
	object Add(string name, CacheItemPolicy itemPolicy, object value)
	object Add(object tag, string name, object value)
	object Add(object tag, string name, CacheItemPolicy itemPolicy, object value)
### Examples
	// adding an item to cache
	cache.Add("name", "value");
	// adding an item to cache with a CacheItemPolicy
	cache.Add("name", new CacheItemPolicy { SlidingExpiration = new TimeSpan(1, 0, 0) }, "value");
	// adding an item to cache with a tag
	cache.Add(tag, "name", "value");
	// adding an item to cache with a tag, and a CacheItemPolicy
	cache.Add(tag, "name", new CacheItemPolicy { SlidingExpiration = new TimeSpan(1, 0, 0) }, "value");

## Get/TryGet
gets an object from abstracted cache.

	object Get(string name)
	object Get(object tag, string name)
	object Get(IEnumerable<string> names)
	object Get(object tag, IEnumerable<string> names)
	bool TryGet(string name, out object value)
	bool TryGet(object tag, string name, out object value)
### Examples
	// getting an item from cache
	var getValue = cache.Get("name");
	// getting an item from cache with a tag
	var getValue2 = cache.Get(tag, "name");
	// getting an item from cache
	var getValues = cache.Get(new[] { "name", "name2" });
	// getting an item from cache with a tag
	var getValues2 = cache.Get(tag, new[] { "name", "name2" });
	// trying to get an item from cache
	var hasValue = cache.TryGet("name", out value);
	// trying to get an item from cache with a tag
	var hasValue2 = cache.TryGet(tag, "name", out value);

## Remove
removes an object from abstracted cache.

	object Remove(string name)
	object Remove2(object tag, string name)
### Examples
	// removing an item from cache
	var removedValue = cache.Remove("name");
	// removing an item from cache with a tag
	var removedValue2 = cache.Remove(tag, "name");

## Set
inserts an object into abstracted cache.

	object Set(string name, object value)
	object Set(string name, CacheItemPolicy itemPolicy, object value)
	object Set(object tag, string name, object value)
	object Set(object tag, string name, CacheItemPolicy itemPolicy, object value)
### Examples
	// inserting an item into cache
	cache.Set("name", "value");
	// inserting an item into cache with a CacheItemPolicy
	cache.Set("name", new CacheItemPolicy { SlidingExpiration = new TimeSpan(1, 0, 0) }, "value");
	// inserting an item into cache with a tag
	cache.Set(tag, "name", "value");
	// inserting an item into cache with a tag, and a CacheItemPolicy and a name
	cache.Set(tag, "name", new CacheItemPolicy { SlidingExpiration = new TimeSpan(1, 0, 0) }, "value");

## EnsureCacheDependency
adds cachetags of dependency into abstracted cache.

	static void EnsureCacheDependency(IServiceCache cache, object tag, ServiceCacheDependency dependency)
	static void EnsureCacheDependency(IServiceCache cache, IEnumerable<string> cacheTags)
### Examples
	// ensuring a cache dependency is ready from a builder
	IServiceCacheExtensions.EnsureCacheDependency(cache, tag, (a, b) => new[] { "tag", "tag2" });
	// ensuring a cache dependency is ready from values
	IServiceCacheExtensions.EnsureCacheDependency(cache, new[] { "tag", "tag2" });

## Touch
touched cache keys.

	void Touch(params string[] names)
	void Touch(object tag, params string[] names)
### Examples
	// touch cache tags
	cache.Touch("tag", "tag2");
	// touch cache tags with tag
	cache.Touch(tag, "tag", "tag2");

## BehaveAs
wrappes a servicecache namespacing all keys.

	IServiceCache BehaveAs(string @namespace)
	IServiceCache BehaveAs(IEnumerable<object> values, out string @namespace)
### Examples
	// wrapping a servicecache with a namespace
	var newCache2 = cache.Wrap("namespace");
	// wrapping a servicecache using a generated namespace from an object[] of values
	string @namespace;
	var newCache = cache.Wrap(new object[] { "value", 5 }, out @namespace);




# Defining Caching Policy
information for an item being caching is stored in a cache item policy:

	class CacheItemPolicy
	{
	   CacheItemPolicy();
	   CacheItemPolicy(int floatingAbsoluteMinuteTimeout);

	   CacheItemDependency Dependency { get; set; }
	   DateTime AbsoluteExpiration { get; set; }
	   TimeSpan FloatingAbsoluteExpiration { get; set; }
	   TimeSpan SlidingExpiration { get; set; }
	   CacheItemPriority Priority { get; set; }
	   CacheEntryUpdateCallback UpdateCallback { get; set; }
	   CacheEntryRemovedCallback RemovedCallback { get; set; }
	}

## Setting the priority of cached items
<table>
<tr>
    <td>CacheItemPriority</td>
    <td>Description</td>
</tr>
<tr>
    <td>AboveNormal</td>
    <td>Cached item has an above normal relative priority value.</td>
</tr>
<tr>
    <td>BelowNormal</td>
    <td>Cached item has an below normal relative priority value.</td>
</tr>
<tr>
    <td>Default</td>
    <td>Cached item has a default priority value.</td>
</tr>
<tr>
    <td>High</td>
    <td>Cached item has a high priority value.</td>
</tr>
<tr>
    <td>Low</td>
    <td>Cached item has a low priority value.</td>
</tr>
<tr>
    <td>Normal</td>
    <td>Cached item has a normal priority value.</td>
</tr>
<tr>
    <td>NotRemovable</td>
    <td>Cached item has a priority value indicating it can not be removed from cache.</td>
</tr>
</table>
<br />
callback for cacheitem entry removed:

	delegate void CacheEntryRemovedCallback(string name, object value)

callback for cacheitem entry updated:

	delegate void CacheEntryUpdateCallback(string name, object value)

builder for new cacheitem, used with registrations:

	delegate object CacheItemBuilder(object tag, object[] values)

provider specific cache item dependencies can generated with the cache dependecies builder:

	delegate object CacheItemDependency(IServiceCache cache, object tag)




# Working with Registrations
Registrations delegate cache item creation on cache miss.

Builder for registered item

	delegate object ServiceCacheBuilder(object tag, object[] values)

registration object

	class ServiceCacheRegistration
	{
	   ServiceCacheRegistration(string name, CacheItemBuilder builder, params string[] cacheTags);
	   ServiceCacheRegistration(string name, int minuteTimeout, CacheItemBuilder builder, params string[] cacheTags);
	   ServiceCacheRegistration(string name, CacheItemPolicy itemPolicy, CacheItemBuilder builder,
		  params string[] cacheTags);
	   string Name { get; set; }
	   CacheItemPolicy ItemPolicy { get; set; }
	   CacheItemBuilder Builder { get; set; }
	}

foreign registration object

	class ServiceCacheForeignRegistration : ServiceCacheRegistration
	{
	   ServiceCacheForeignRegistration(string name);
	   ServiceCacheForeignRegistration(string name, Type foreignType, string foreignName);

	   Type ForeignType { get; set; }
	   string ForeignName { get; set; }
	}

Setting up registrations

	class MyCache
	{
		// static constructor auto-registeres
		static MyCache() { ServiceCacheRegistrar.RegisterAllBelow<MyCache>(); }

		// simple registration for single item
		public static readonly ServiceCacheRegistration MyItem = new ServiceCacheRegistration("MyItem",
			(tag, values) => "MyItem");
		// registration for single item, with a 2 hour timeout, and dependencies
		public static readonly ServiceCacheRegistration MyItemWithDep = new ServiceCacheRegistration("MyItemWithDep",
			60 * 2, (tag, values) => "MyItem", "tag", "tag2");
		// registration for single item, with a cachecommmand, and dependencies
		public static readonly ServiceCacheRegistration MyItemWithCmd = new ServiceCacheRegistration("MyItemWithCmd",
			new CacheItemPolicy
			{
				Priority = CacheItemPriority.High,
				// floating absolute expiration, will expire ever hour
				FloatingAbsoluteExpiration = new TimeSpan(1, 0, 0),
			}, (tag, values) => "MyItem", "tag", "tag2");
		// registration for many items
		public static readonly ServiceCacheRegistration MyItems = new ServiceCacheRegistration("MyItems",
			(tag, values) => new[] { "MyItem0", "MyItem1" });
		// registration for many items based on integer passed into values
		public static readonly ServiceCacheRegistration MyItems2 = new ServiceCacheRegistration("MyItems2",
			(tag, values) =>
			{
				if ((values == null) || (values.Length != 1))
					throw new ArgumentOutOfRangeException("values");
				var items = (int)values[0];
				return Enumerable.Range(0, items)
					.Select(x => "MyItem" + x.ToString())
					.ToList();
			});
		// registration for many items as queryable
		public static readonly ServiceCacheRegistration MyItemsQuery = new ServiceCacheRegistration("MyItemsQuery",
			(tag, values) => new[] { "MyItem0", "MyItem1" }.AsQueryable());
	}

and getting their values

	// getting registered MyItem
	var myItem = (string)cache.Get(MyCache.MyItem);
	// getting registered MyItemWithDep
	var myItemWithDependencies = cache.Get<string>(MyCache.MyItemWithDep);
	// getting registered MyItemWithCmd
	var myItemWithCacheCommand = cache.Get<string>(MyCache.MyItemWithCmd);
	// getting an IEnumerable<string> from registered MyItems
	var myItems = cache.GetMany<string>(MyCache.MyItems);
	// getting an IEnumerable<string> from registered MyItems2 based on value
	var myItems2 = cache.GetMany<string>(MyCache.MyItems2, new object[] { 10 });
	// getting an IQueryable<string> from registered MyItemsQuery
	var myItems3 = cache.GetQuery<string>(MyCache.MyItemsQuery);

## Get
returns a registered object from cache, building if necessary.

	object Get(ServiceCacheRegistration registration)
	object Get(ServiceCacheRegistration registration, object[] values)
	object Get(ServiceCacheRegistration registration, object tag)
	object Get(ServiceCacheRegistration registration, object tag, object[] values)
	object Get(Type anchorType, string registrationName)
	object Get(Type anchorType, string registrationName, object[] values)
	object Get(Type anchorType, string registrationName, object tag)
	object Get(Type anchorType, string registrationName, object tag, object[] values)
### Examples:
	// getting a registered item from cache
	var registeredGetValue = (string)cache.Get(MyCache.MyItem);
	// getting a registered item from cache with values
	var registeredGetValue2 = (IEnumerable<string>)cache.Get(MyCache.MyItems2, new object[] { 10 });
	// getting a registered item from cache with a tag
	var registeredGetValue3 = (string)cache.Get(MyCache.MyItem, tag);
	// getting a registered item from cache with a tag, and values
	var registeredGetValue4 = (IEnumerable<string>)cache.Get(MyCache.MyItems2, tag, new object[] { 10 });
	// getting a registered item by name from cache
	var registeredGetValue5 = (string)cache.Get(typeof(MyCache), "MyItem");
	// getting a registered item by name from cache with values
	var registeredGetValue6 = (IEnumerable<string>)cache.Get(typeof(MyCache), "MyItems2", new object[] { 10 });
	// getting a registered item by name from cache with a tag
	var registeredGetValue7 = (string)cache.Get(typeof(MyCache), "MyItem", tag);
	// getting a registered item by name from cache with a tag, and values
	var registeredGetValue8 = (IEnumerable<string>)cache.Get(typeof(MyCache), "MyItems2", tag, new object[] { 10 });

## Get&lt;T&gt;
returns a registered object from cache cast as `T`, building if necessary.

	T Get<T>(ServiceCacheRegistration registration)
	T Get<T>(ServiceCacheRegistration registration, object[] values)
	T Get<T>(ServiceCacheRegistration registration, object tag)
	T Get<T>(ServiceCacheRegistration registration, object tag, object[] values)
	T Get<T>(Type anchorType, string registrationName)
	T Get<T>(Type anchorType, string registrationName, object[] values)
	T Get<T>(Type anchorType, string registrationName, object tag)
	T Get<T>(Type anchorType, string registrationName, object tag, object[] values)
### Examples
	// getting a registered item from cache
	var registered2GetValue = cache.Get<string>(MyCache.MyItem);
	// getting a registered item from cache with values
	var registered2GetValue2 = cache.Get<IEnumerable<string>>(MyCache.MyItems2, new object[] { 10 });
	// getting a registered item from cache with a tag
	var registered2GetValue3 = cache.Get<string>(MyCache.MyItem, tag);
	// getting a registered item from cache with a tag, and values
	var registered2GetValue4 = cache.Get<IEnumerable<string>>(MyCache.MyItems2, tag, new object[] { 10 });
	// getting a registered item by name from cache
	var registered2GetValue5 = cache.Get<string>(typeof(MyCache), "MyItem");
	// getting a registered item by name from cache with values
	var registered2GetValue6 = cache.Get<IEnumerable<string>>(typeof(MyCache), "MyItems2", new object[] { 10 });
	// getting a registered item by name from cache with a tag
	var registered2GetValue7 = cache.Get<string>(typeof(MyCache), "MyItem", tag);
	// getting a registered item by name from cache with a tag, and values
	var registered2GetValue8 = cache.Get<IEnumerable<string>>(typeof(MyCache), "MyItems2", tag, new object[] { 10 });

## GetMany&lt;T&gt;
returns enumerable registered objects from cache cast as `IEnumerable<T>`, building if necessary.

	IEnumerable<T> GetMany<T>(ServiceCacheRegistration registration)
	IEnumerable<T> GetMany<T>(ServiceCacheRegistration registration, object[] values)
	IEnumerable<T> GetMany<T>(ServiceCacheRegistration registration, object tag)
	IEnumerable<T> GetMany<T>(ServiceCacheRegistration registration, object tag, object[] values)
	IEnumerable<T> GetMany<T>(Type anchorType, string registrationName)
	IEnumerable<T> GetMany<T>(Type anchorType, string registrationName, object[] values)
	IEnumerable<T> GetMany<T>(Type anchorType, string registrationName, object tag)
	IEnumerable<T> GetMany<T>(Type anchorType, string registrationName, object tag, object[] values)
### Examples
	// getting a registered item from cache
	var registered3GetValue = cache.GetMany<string>(MyCache.MyItems);
	// getting a registered item from cache with values
	var registered3GetValue2 = cache.GetMany<string>(MyCache.MyItems2, new object[] { 10 });
	// getting a registered item from cache with a tag
	var registered3GetValue3 = cache.GetMany<string>(MyCache.MyItems, tag);
	// getting a registered item from cache with a tag, and values
	var registered3GetValue4 = cache.GetMany<string>(MyCache.MyItems2, tag, new object[] { 10 });
	// getting a registered item by name from cache
	var registered3GetValue5 = cache.GetMany<string>(typeof(MyCache), "MyItems");
	// getting a registered item by name from cache with values
	var registered3GetValue6 = cache.GetMany<string>(typeof(MyCache), "MyItems2", new object[] { 10 });
	// getting a registered item by name from cache with a tag
	var registered3GetValue7 = cache.GetMany<string>(typeof(MyCache), "MyItems", tag);
	// getting a registered item by name from cache with a tag, and values
	var registered3GetValue8 = cache.GetMany<string>(typeof(MyCache), "MyItems2", tag, new object[] { 10 });

## GetQuery&lt;T&gt;
returns queryable registered objects from cache cast as `IQueryable<T>`, building if necessary.

	IQueryable<T> GetQuery<T>(ServiceCacheRegistration registration)
	IQueryable<T> GetQuery<T>(ServiceCacheRegistration registration, object[] values)
	IQueryable<T> GetQuery<T>(ServiceCacheRegistration registration, object tag)
	IQueryable<T> GetQuery<T>(ServiceCacheRegistration registration, object tag, object[] values)
	IQueryable<T> GetQuery<T>(Type anchorType, string registrationName)
	IQueryable<T> GetQuery<T>(Type anchorType, string registrationName, object[] values)
	IQueryable<T> GetQuery<T>(Type anchorType, string registrationName, object tag)
	IQueryable<T> GetQuery<T>(Type anchorType, string registrationName, object tag, object[] values)
### Examples
	// getting a registered item from cache
	var registered4GetValue = cache.GetQuery<string>(MyCache.MyItemsQuery);
	// getting a registered item from cache with values
	var registered4GetValue2 = cache.GetQuery<string>(MyCache.MyItemsQuery, new object[] { 10 });
	// getting a registered item from cache with a tag
	var registered4GetValue3 = cache.GetQuery<string>(MyCache.MyItemsQuery, tag);
	// getting a registered item from cache with a tag, and values
	var registered4GetValue4 = cache.GetQuery<string>(MyCache.MyItemsQuery, tag, new object[] { 10 });
	// getting a registered item by name from cache
	var registered4GetValue5 = cache.GetQuery<string>(typeof(MyCache), "MyItemsQuery");
	// getting a registered item by name from cache with values
	var registered4GetValue6 = cache.GetQuery<string>(typeof(MyCache), "MyItemsQuery", new object[] { 10 });
	// getting a registered item by name from cache with a tag
	var registered4GetValue7 = cache.GetQuery<string>(typeof(MyCache), "MyItemsQuery", tag);
	// getting a registered item by name from cache with a tag, and values
	var registered4GetValue8 = cache.GetQuery<string>(typeof(MyCache), "MyItemsQuery", tag, new object[] { 10 });

## Remove
removes an registered object from abstracted cache.

	void Remove(ServiceCacheRegistration registration)
	void Remove(Type anchorType, string registrationName)
### Examples
	// removing a registered item from cache 
	cache.Remove(MyCache.MyItem);
	// removing a registered item by name from cache 
	cache.Remove(typeof(MyCache), "MyItem");

## RemoveAll
removes all registered objects, in registrar at anchor, from abstracted cache.

	void RemoveAll(Type anchorType)
### Examples
	// removing all registered items by anchorType from cache 
	cache.RemoveAll(typeof(MyCache));
