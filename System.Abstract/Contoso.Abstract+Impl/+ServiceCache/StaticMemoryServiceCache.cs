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
using System.Collections.Generic;
namespace Contoso.Abstract
{
	//: might need to make thread safe
	/// <summary>
	/// Provides the core factory method mechanism for generating or accessing a singleton-based Cache Provider.
	/// </summary>
	public class StaticMemoryServiceCache : IServiceCache
	{
		public static readonly Dictionary<string, object> Dictionary = new Dictionary<string, object>();

		public StaticMemoryServiceCache()
		{
			RegistrationDispatch = new DefaultServiceCacheRegistrationDispatcher();
		}

		public object this[string name]
		{
			get { return Get(null, name); }
			set { Set(null, name, CacheItemPolicy.Default, value); }
		}

		/// <summary>
		/// Adds an object into cache based on the parameters provided.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <param name="name">The key used to identify the item in cache.</param>
		/// <param name="dependency">The dependency object defining caching validity dependencies.</param>
		/// <param name="absoluteExpiration">The absolute expiration value used to determine when a cache item must be considerd invalid.</param>
		/// <param name="slidingExpiration">The sliding expiration value used to determine when a cache item is considered invalid due to lack of use.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="onRemoveCallback">The delegate to invoke when the item is removed from cache.</param>
		/// <param name="value">The value to store in cache.</param>
		/// <returns></returns>
		public object Add(object tag, string name, CacheItemPolicy itemPolicy, object value)
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
		public object Get(object tag, string name)
		{
			object value;
			return (Dictionary.TryGetValue(name, out value) ? value : null);
		}

		public object Get(object tag, IEnumerable<string> names)
		{
			if (names == null)
				throw new ArgumentNullException("names");
			return names.Select(name => new { name, value = Get(null, name) }).ToDictionary(x => x.name, x => x.value);
		}

		public bool TryGet(object tag, string name, out object value) { return Dictionary.TryGetValue(name, out value); }

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
		public object Set(object tag, string name, CacheItemPolicy itemPolicy, object value)
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
		public object Remove(object tag, string name)
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
		/// Touches the specified names.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <param name="names">The names.</param>
		public void Touch(object tag, params string[] names)
		{
			Dictionary.Clear();
		}

		public ServiceCacheSettings Settings
		{
			get { return null; }
		}
		public ServiceCacheRegistration.IDispatch RegistrationDispatch { get; private set; }
	}
}