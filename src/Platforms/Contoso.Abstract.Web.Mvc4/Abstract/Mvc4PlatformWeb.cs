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
using System.Abstract;
namespace Contoso.Abstract
{
    /// <summary>
    /// IMvc4PlatformWeb
    /// </summary>
    public interface IMvc4PlatformWeb : IPlatformWeb
    {
    }

    /// <summary>
    /// Mvc4PlatformWeb
    /// </summary>
    public class Mvc4PlatformWeb : IMvc4PlatformWeb, PlatformWebManager.ISetupRegistration
    {
        static Mvc4PlatformWeb() { PlatformWebManager.EnsureRegistration(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="Mvc4PlatformWeb"/> class.
        /// </summary>
        public Mvc4PlatformWeb()
        {
        }

        Action<IServiceLocator, string> PlatformWebManager.ISetupRegistration.DefaultServiceRegistrar
        {
            get { return (locator, name) => PlatformWebManager.RegisterInstance<IMvc4PlatformWeb>(this, locator, name); }
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public object GetService(Type serviceType) { throw new NotImplementedException(); }
    }
}
