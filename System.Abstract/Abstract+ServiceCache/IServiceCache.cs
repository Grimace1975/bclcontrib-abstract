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
namespace System.Abstract
{
    /// <summary>
    /// IServiceCache
    /// </summary>
    public interface IServiceCache
    {
        object this[string name] { get; set; }

        object Add(string name, object value, ServiceCacheDependency dependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback, object tag);

        /// <summary>
        /// Gets the item from cache associated with the key provided.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The cached item.</returns>
        object Get(string name, object tag);

        /// <summary>
        /// Removes from cache the item associated with the key provided.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The item removed from the Cache. If the value in the key parameter is not found, returns null.</returns>
        object Remove(string name, object tag);

        /// <summary>
        /// Adds an object into cache based on the parameters provided.
        /// </summary>
        /// <param name="key">The key used to identify the item in cache.</param>
        /// <param name="value">The value to store in cache.</param>
        /// <param name="dependency">The dependency object defining caching validity dependencies.</param>
        /// <param name="absoluteExpiration">The absolute expiration value used to determine when a cache item must be considerd invalid.</param>
        /// <param name="slidingExpiration">The sliding expiration value used to determine when a cache item is considered invalid due to lack of use.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="onRemoveCallback">The delegate to invoke when the item is removed from cache.</param>
        object Insert(string name, object value, ServiceCacheDependency dependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback, object tag);

        /// <summary>
        /// Touches the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        void Touch(string name, object tag);
    }

    /// <summary>
    /// IServiceCacheExtensions
    /// </summary>
    public static class IServiceCacheExtensions
    {
        public static object Add(this IServiceCache cache, ServiceCacheCommand cacheCommand, object value) { return Add(cache, null, cacheCommand, cacheCommand.Name, value); }
        public static object Add(this IServiceCache cache, ServiceCacheCommand cacheCommand, string name, object value) { return Add(cache, null, cacheCommand, name, value); }
        public static object Add(this IServiceCache cache, object tag, ServiceCacheCommand cacheCommand, object value) { return Add(cache, tag, cacheCommand, cacheCommand.Name, value); }
        public static object Add(this IServiceCache cache, object tag, ServiceCacheCommand cacheCommand, string name, object value)
        {
            if (cacheCommand == null)
                throw new ArgumentNullException("cacheCommand");
            var itemAddedCallback = cacheCommand.ItemAddedCallback;
            if (itemAddedCallback != null)
                itemAddedCallback(name, value);
            return cache.Add(name, value, cacheCommand.Dependency, cacheCommand.AbsoluteExpiration, cacheCommand.SlidingExpiration, cacheCommand.Priority, cacheCommand.ItemRemovedCallback, tag);
        }

        public static object Get(this IServiceCache cache, string name) { return cache.Get(name, null); }

        public static object Remove(this IServiceCache cache, string name) { return cache.Remove(name, null); }
        public static object Remove(this IServiceCache cache, ServiceCacheCommand cacheCommand) { return Remove(cache, null, cacheCommand); }
        public static object Remove(this IServiceCache cache, object tag, ServiceCacheCommand cacheCommand)
        {
            if (cacheCommand == null)
                throw new ArgumentNullException("cacheCommand");
            return cache.Remove(cacheCommand.Name, tag);
        }

        public static object Insert(this IServiceCache cache, ServiceCacheCommand cacheCommand, object value) { return Insert(cache, null, cacheCommand, cacheCommand.Name, value); }
        public static object Insert(this IServiceCache cache, ServiceCacheCommand cacheCommand, string name, object value) { return Insert(cache, null, cacheCommand, cacheCommand.Name, value); }
        public static object Insert(this IServiceCache cache, object tag, ServiceCacheCommand cacheCommand, object value) { return Insert(cache, tag, cacheCommand, cacheCommand.Name, value); }
        public static object Insert(this IServiceCache cache, object tag, ServiceCacheCommand cacheCommand, string name, object value)
        {
            if (cacheCommand == null)
                throw new ArgumentNullException("cacheCommand");
            var itemAddedCallback = cacheCommand.ItemAddedCallback;
            if (itemAddedCallback != null)
                itemAddedCallback(name, value);
            return cache.Insert(name, value, cacheCommand.Dependency, cacheCommand.AbsoluteExpiration, cacheCommand.SlidingExpiration, cacheCommand.Priority, cacheCommand.ItemRemovedCallback, tag);
        }

        public static void InternalEnsureDependency(IServiceCache cache, ServiceCacheDependency dependency)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");
            if (dependency != null)
                return;
            var names = dependency.CacheTags;
            if (names != null)
                foreach (string name in names)
                    cache.Add(name, string.Empty, null, ServiceCache.NoAbsoluteExpiration, ServiceCache.NoSlidingExpiration, CacheItemPriority.Normal, null, null);
        }

        public static void Touch(this IServiceCache cache, string name) { cache.Touch(name, null); }
        public static void Touch(this IServiceCache cache, params string[] names) { Touch(cache, null, names); }
        public static void Touch(this IServiceCache cache, object tag, params string[] names)
        {
            if (names == null)
                throw new ArgumentNullException("names");
            foreach (string name in names)
                cache.Touch(name, tag);
        }
    }
}
