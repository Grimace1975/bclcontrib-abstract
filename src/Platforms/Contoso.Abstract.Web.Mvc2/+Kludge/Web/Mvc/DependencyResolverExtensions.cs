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
namespace System.Web.Mvc
{
    /// <summary>
    /// DependencyResolverExtensions
    /// </summary>
    public static class DependencyResolverExtensions
    {
        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        public static TService GetService<TService>(this IDependencyResolver resolver) { return (TService)resolver.GetService(typeof(TService)); }
        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        public static IEnumerable<TService> GetServices<TService>(this IDependencyResolver resolver) { return resolver.GetServices(typeof(TService)).Cast<TService>(); }
    }
}
