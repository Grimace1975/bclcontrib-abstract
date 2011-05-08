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
namespace System.Abstract
{
    /// <summary>
    /// IServiceLocator
    /// </summary>
    public interface IServiceLocator // : IServiceProvider
    {
        // registrar
        IServiceRegistrar GetRegistrar();
        TServiceRegistrar GetRegistrar<TServiceRegistrar>()
            where TServiceRegistrar : class, IServiceRegistrar;

        // resolve
        TService Resolve<TService>()
            where TService : class;
        TService Resolve<TService>(string name)
            where TService : class;
        object Resolve(Type serviceType);
        object Resolve(Type serviceType, string name);
        //
        IEnumerable<TService> ResolveAll<TService>()
            where TService : class;
        IEnumerable<object> ResolveAll(Type serviceType);

        // inject
        TService Inject<TService>(TService instance)
            where TService : class;

        // release and teardown
        void Release(object instance);
        void TearDown<TService>(TService instance)
            where TService : class;
        void Reset();
    }

    /// <summary>
    /// IServiceLocatorExtensions
    /// </summary>
    public static class IServiceLocatorExtensions
    {
        public static TServiceLocator GetServiceLocator<TServiceLocator>(this IServiceLocator locator)
            where TServiceLocator : class, IServiceLocator
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            return (locator as TServiceLocator);
        }

        public static TService Resolve<TService>(this IServiceLocator locator, Type serviceType)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            return (TService)locator.Resolve(serviceType);
        }

        public static IEnumerable<TService> ResolveAll<TService>(this IServiceLocator locator, Type serviceType)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            return locator.ResolveAll(serviceType).Cast<TService>();
        }
    }
}