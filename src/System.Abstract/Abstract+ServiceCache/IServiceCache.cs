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
using System.Collections.Generic;
using System.Linq;
namespace System.Abstract
{
	/// <summary>
	/// IServiceCache
	/// </summary>
	public interface IServiceCache : IServiceProvider
	{
		object this[string name] { get; set; }

        object Add(object tag, string name, CacheItemPolicy itemPolicy, object value, ServiceCacheByDispatcher dispatch);

		/// <summary>
		/// Gets the item from cache associated with the key provided.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <param name="name">The name.</param>
		/// <returns>
		/// The cached item.
		/// </returns>
		object Get(object tag, string name);
		object Get(object tag, IEnumerable<string> names);
		bool TryGet(object tag, string name, out object value);

		/// <summary>
		/// Removes from cache the item associated with the key provided.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <param name="name">The name.</param>
		/// <returns>The item removed from the Cache. If the value in the key parameter is not found, returns null.</returns>
		object Remove(object tag, string name);

		/// <summary>
		/// Adds an object into cache based on the parameters provided.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <param name="name">The name.</param>
		/// <param name="itemPolicy">The itemPolicy object.</param>
		/// <param name="value">The value to store in cache.</param>
		/// <returns></returns>
        object Set(object tag, string name, CacheItemPolicy itemPolicy, object value, ServiceCacheByDispatcher dispatch);

		/// <summary>
		/// Settings
		/// </summary>
		ServiceCacheSettings Settings { get; }
	}
}
