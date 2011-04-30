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
#if EXPERIMENTAL
using System;
using System.Abstract;
using System.Collections.Generic;
namespace Contoso.Abstract
{
	//: might need to make thread safe
	/// <summary>
	/// Provides the core factory method mechanism for generating or accessing a singleton-based Cache Provider.
	/// </summary>
	public class MemoryServiceCache : IServiceCache
	{
		public static readonly Dictionary<string, object> Dictionary = new Dictionary<string, object>();

		public object this[string name]
		{
			get { return Get(name, null); }
			set { Insert(name, value, null, DateTime.Now.AddMinutes(60), ServiceCache.NoSlidingExpiration, CacheItemPriority.Normal, null, null); }
		}

		/// <summary>
		/// Adds an object into cache based on the parameters provided.
		/// </summary>
		/// <param name="name">The key used to identify the item in cache.</param>
		/// <param name="value">The value to store in cache.</param>
		/// <param name="dependency">The dependency object defining caching validity dependencies.</param>
		/// <param name="absoluteExpiration">The absolute expiration value used to determine when a cache item must be considerd invalid.</param>
		/// <param name="slidingExpiration">The sliding expiration value used to determine when a cache item is considered invalid due to lack of use.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="onRemoveCallback">The delegate to invoke when the item is removed from cache.</param>
		/// <returns></returns>
        public object Add(string name, object value, ServiceCacheDependency dependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback, object tag)
		{
			// TODO: Throw on dependency or other stuff not supported by this simple system
			object lastValue;
			if (!Dictionary.TryGetValue(name, out lastValue))
			{
				Dictionary[name] = value;
				return null;
			}
			return lastValue;
		}

		/// <summary>
		/// Gets the item from cache associated with the key provided.
		/// </summary>
		/// <param name="name">The key.</param>
		/// <returns>The cached item.</returns>
        public object Get(string name, object tag)
		{
			object value;
			return (Dictionary.TryGetValue(name, out value) ? value : null);
		}

		/// <summary>
		/// Adds an object into cache based on the parameters provided.
		/// </summary>
		/// <param name="name">The key used to identify the item in cache.</param>
		/// <param name="value">The value to store in cache.</param>
		/// <param name="dependency">The dependency object defining caching validity dependencies.</param>
		/// <param name="absoluteExpiration">The absolute expiration value used to determine when a cache item must be considerd invalid.</param>
		/// <param name="slidingExpiration">The sliding expiration value used to determine when a cache item is considered invalid due to lack of use.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="onRemoveCallback">The delegate to invoke when the item is removed from cache.</param>
        public object Insert(string name, object value, ServiceCacheDependency dependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback, object tag)
		{
			return (Dictionary[name] = value);
		}

		/// <summary>
		/// Removes from cache the item associated with the key provided.
		/// </summary>
		/// <param name="name">The key.</param>
		/// <returns>
		/// The item removed from the Cache. If the value in the key parameter is not found, returns null.
		/// </returns>
        public object Remove(string name, object tag)
		{
			object value;
			if (Dictionary.TryGetValue(name, out value))
			{
				Dictionary.Remove(name);
				return value;
			}
			return null;
		}

		/// <summary>
		/// Touches the specified key.
		/// </summary>
		/// <param name="name">The key.</param>
		public void Touch(string name, object tag)
		{
			Dictionary.Clear();
		}
    }
}
#endif