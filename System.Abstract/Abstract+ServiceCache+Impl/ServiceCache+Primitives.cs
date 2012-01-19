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
	public static partial class ServiceCache
	{
		/// <summary>
		/// Primitives
		/// </summary>
		public static class Primitives
		{
			/// <summary>
			/// YesNo
			/// </summary>
			public static readonly ServiceCacheRegistration YesNo = new ServiceCacheRegistration("YesNo", (tag, values) =>
			{
				var values2 = new Dictionary<string, string>(3);
				switch (values != null && values.Length == 0 ? null : values[0] as string)
				{
					case null:
					case "": break;
					case "--": values2.Add(string.Empty, "--"); break;
					default: throw new InvalidOperationException();
				}
				values2.Add(bool.TrueString, "Yes");
				values2.Add(bool.FalseString, "No");
				return values2;
			});
			/// <summary>
			/// Gender
			/// </summary>
			public static readonly ServiceCacheRegistration Gender = new ServiceCacheRegistration("Gender", (tag, values) =>
			{
				var values2 = new Dictionary<string, string>(3);
				switch (values != null && values.Length == 0 ? null : values[0] as string)
				{
					case null:
					case "": break;
					case "--": values2.Add(string.Empty, "--"); break;
					default: throw new InvalidOperationException();
				}
				values2.Add("Male", "Male");
				values2.Add("Female", "Female");
				return values2;
			});
			/// <summary>
			/// Integer
			/// </summary>
			public static readonly ServiceCacheRegistration Integer = new ServiceCacheRegistration("Integer", (tag, values) =>
			{
				var values2 = new Dictionary<string, string>(3);
				switch (values != null && values.Length == 0 ? null : values[0] as string)
				{
					case null:
					case "": break;
					case "--": values2.Add(string.Empty, "--"); break;
					default: throw new InvalidOperationException();
				}
                var startIndex = (int)values[0];
                var endIndex = (int)values[1];
                var indexStep = (int)values[2];
				for (var index = startIndex; index < endIndex; index += indexStep)
					values2.Add(index.ToString(), index.ToString());
				return values2;
			});
		}
	}
}
