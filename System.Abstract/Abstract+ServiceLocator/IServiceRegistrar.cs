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
    /// IServiceRegistrar
    /// </summary>
    public interface IServiceRegistrar
    {
        // locator
        IServiceLocator Locator { get; }

        // enumerate
        bool HasRegistered<TService>();
        bool HasRegistered(Type serviceType);
        IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType);
        IEnumerable<ServiceRegistration> Registrations { get; }

        // register type
        void Register(Type serviceType);
        void Register(Type serviceType, string name);

        // register implementation
        void Register<TService, TImplementation>()
            where TImplementation : class, TService
            where TService : class;
        void Register<TService, TImplementation>(string name)
            where TImplementation : class, TService
            where TService : class;
        void Register<TService>(Type implementationType)
            where TService : class;
        void Register<TService>(Type implementationType, string name)
            where TService : class;
        void Register(Type serviceType, Type implementationType);
        void Register(Type serviceType, Type implementationType, string name);

        // register instance
        void RegisterInstance<TService>(TService instance)
            where TService : class;
        void RegisterInstance<TService>(TService instance, string name)
            where TService : class;
        void RegisterInstance(Type serviceType, object instance);
        void RegisterInstance(Type serviceType, object instance, string name);

        // register method
        void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class;
        void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class;
        void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod);
        void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name);

        // interceptor
        void RegisterInterceptor(IServiceLocatorInterceptor interceptor);
    }
}
