using System;
using System.Abstract;
using System.Collections.Generic;
using System.Linq;
namespace Example
{
    public class ServiceCacheExamples
    {
        public void WorkingWithCache()
        {
            object tag = null;
            IServiceCache cache = null;

            // setting an item in cache
            cache["name"] = "value";
            // getting an item from cache
            var value = cache["name"];

            // adding an item to cache
            cache.Add("name", "value");
            // adding an item to cache with a CacheItemPolicy
            cache.Add("name", new CacheItemPolicy { SlidingExpiration = new TimeSpan(1, 0, 0) }, "value");
            // adding an item to cache with a tag
            cache.Add(tag, "name", "value");
            // adding an item to cache with a tag, and a CacheItemPolicy
            cache.Add(tag, "name", new CacheItemPolicy { SlidingExpiration = new TimeSpan(1, 0, 0) }, "value");

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


            // removing an item from cache
            var removedValue = cache.Remove("name");
            // removing an item from cache with a tag
            var removedValue2 = cache.Remove(tag, "name");

            // removing a registered item from cache 
            cache.Remove(MyCache.MyItem);
            // removing a registered item by name from cache 
            cache.Remove(typeof(MyCache), "MyItem");

            // removing all registered items by anchorType from cache 
            cache.RemoveAll(typeof(MyCache));

            // inserting an item into cache
            cache.Set("name", "value");
            // inserting an item into cache with a CacheItemPolicy
            cache.Set("name", new CacheItemPolicy { SlidingExpiration = new TimeSpan(1, 0, 0) }, "value");
            // inserting an item into cache with a tag
            cache.Set(tag, "name", "value");
            // inserting an item into cache with a tag, and a CacheItemPolicy and a name
            cache.Set(tag, "name", new CacheItemPolicy { SlidingExpiration = new TimeSpan(1, 0, 0) }, "value");

            // ensuring a cache dependency is ready from a builder
            ServiceCacheExtensions.EnsureCacheDependency(cache, tag, (c, r, t, v) => new[] { "tag", "tag2" });
            // ensuring a cache dependency is ready from values
            ServiceCacheExtensions.EnsureCacheDependency(cache, new[] { "tag", "tag2" });

            // touch cache tags
            cache.Touch(new[] { "tag", "tag2" });
            // touch cache tags with tag
            cache.Touch(tag, new[] { "tag", "tag2" });

            // wrapping a servicecache with a namespace
            var newCache2 = cache.BehaveAs("namespace");
            // wrapping a servicecache using a generated namespace from an object[] of values
            string @namespace;
            var newCache = cache.BehaveAs(new object[] { "value", 5 }, out @namespace);
        }

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

        public void WorkingWithRegistrations()
        {
            IServiceCache cache = null;
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
        }
    }
}
