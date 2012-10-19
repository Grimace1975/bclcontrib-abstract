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
namespace System.Web.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    public static class DependencyResolver
    {
        private static IDependencyResolver _current = new DefaultDependencyResolver();

        private class DefaultDependencyResolver : IDependencyResolver
        {
            public object GetService(Type serviceType)
            {
                try { return Activator.CreateInstance(serviceType); }
                catch { return null; }
            }

            public IEnumerable<object> GetServices(Type serviceType) { return Enumerable.Empty<object>(); }
        }

        /// <summary>
        /// Sets the resolver.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        public static void SetResolver(IDependencyResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException("resolver");
            _current = resolver;
        }

        /// <summary>
        /// Gets the current.
        /// </summary>
        public static IDependencyResolver Current
        {
            get { return _current; }
        }
    }
}
