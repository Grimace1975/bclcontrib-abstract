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
namespace System.Abstract
{
	/// <summary>
	/// ServiceCache
	/// </summary>
	public static class ServiceCache
	{
		/// <summary>
		/// Provides <see cref="System.DateTime"/> instance to be used when no absolute expiration value to be set.
		/// </summary>
		public static readonly DateTime NoAbsoluteExpiration = DateTime.MaxValue;
		/// <summary>
		/// Provides <see cref="System.TimeSpan"/> instance to be used when no sliding expiration value to be set.
		/// </summary>
		public static readonly TimeSpan NoSlidingExpiration = TimeSpan.Zero;

		public static IServiceCache GetNamespace(object[] values, Func<string, IServiceCache> cacheBuilder)
		{
			if (values == null)
				throw new ArgumentNullException("values");
			if (cacheBuilder == null)
				throw new ArgumentNullException("cacheBuilder");
			// add one additional item, so join ends with scope character.
			string[] valuesAsText = new string[values.Length + 1];
			for (int valueIndex = 0; valueIndex < values.Length; valueIndex++)
			{
				object value = values[valueIndex];
				valuesAsText[valueIndex] = (value != null ? "." + value.ToString() : string.Empty);
			}
			// set additional item to null incase declaration doesnt clear value
			valuesAsText[valuesAsText.Length - 1] = null;
			return cacheBuilder(string.Join("::", valuesAsText));
		}
	}
}
