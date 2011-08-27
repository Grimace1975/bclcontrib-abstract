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
using System.Linq;
using System.Collections.Generic;
using System.Text;
namespace System.Abstract
{
    /// <summary>
    /// ServiceCache
    /// </summary>
    public static partial class ServiceCache
    {
        /// <summary>
        /// Provides <see cref="System.DateTime"/> instance to be used when no absolute expiration value to be set.
        /// </summary>
        public static readonly DateTime InfiniteAbsoluteExpiration = DateTime.MaxValue;
        /// <summary>
        /// Provides <see cref="System.TimeSpan"/> instance to be used when no sliding expiration value to be set.
        /// </summary>
        public static readonly TimeSpan NoSlidingExpiration = TimeSpan.Zero;

        static ServiceCache()
        {
            var registrar = ServiceCacheRegistrar.Get(typeof(ServiceCache));
            registrar.Register(Primitives.YesNo);
            registrar.Register(Primitives.Gender);
            registrar.Register(Primitives.Integer);
        }

        public static string GetNamespace(IEnumerable<object> values)
        {
            if ((values == null) || !values.Any())
                return null;
            var b = new StringBuilder();
            foreach (var x in values)
            {
                if (x != null)
                    b.Append(x.ToString());
                b.Append("\\");
            }
            return b.ToString();
        }
    }
}
