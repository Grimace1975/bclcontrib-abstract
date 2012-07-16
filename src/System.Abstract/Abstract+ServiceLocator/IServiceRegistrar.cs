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
        /// <summary>
        /// Gets the locator.
        /// </summary>
        IServiceLocator Locator { get; }

        // enumerate
        /// <summary>
        /// Determines whether this instance has registered.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>
        ///   <c>true</c> if this instance has registered; otherwise, <c>false</c>.
        /// </returns>
        bool HasRegistered<TService>();
        /// <summary>
        /// Determines whether the specified service type has registered.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        ///   <c>true</c> if the specified service type has registered; otherwise, <c>false</c>.
        /// </returns>
        bool HasRegistered(Type serviceType);
        /// <summary>
        /// Gets the registrations for.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType);
        /// <summary>
        /// Gets the registrations.
        /// </summary>
        IEnumerable<ServiceRegistration> Registrations { get; }

        // register type
        /// <summary>
        /// Gets the lifetime for registers.
        /// </summary>
        ServiceRegistrarLifetime LifetimeForRegisters { get; }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        void Register(Type serviceType);
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="name">The name.</param>
        void Register(Type serviceType, string name);

        // register implementation
        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        void Register<TService, TImplementation>()
            where TImplementation : class, TService
            where TService : class;
        /// <summary>
        /// Registers the specified name.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="name">The name.</param>
        void Register<TService, TImplementation>(string name)
            where TImplementation : class, TService
            where TService : class;
        /// <summary>
        /// Registers the specified implementation type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationType">Type of the implementation.</param>
        void Register<TService>(Type implementationType)
            where TService : class;
        /// <summary>
        /// Registers the specified implementation type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="name">The name.</param>
        void Register<TService>(Type implementationType, string name)
            where TService : class;
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        void Register(Type serviceType, Type implementationType);
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="name">The name.</param>
        void Register(Type serviceType, Type implementationType, string name);

        // register instance
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        void RegisterInstance<TService>(TService instance)
            where TService : class;
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        void RegisterInstance<TService>(TService instance, string name)
            where TService : class;
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        void RegisterInstance(Type serviceType, object instance);
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        void RegisterInstance(Type serviceType, object instance, string name);

        // register method
        /// <summary>
        /// Registers the specified factory method.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="factoryMethod">The factory method.</param>
        void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class;
        /// <summary>
        /// Registers the specified factory method.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="factoryMethod">The factory method.</param>
        /// <param name="name">The name.</param>
        void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class;
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="factoryMethod">The factory method.</param>
        void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod);
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="factoryMethod">The factory method.</param>
        /// <param name="name">The name.</param>
        void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name);

        // interceptor
        /// <summary>
        /// Registers the interceptor.
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        void RegisterInterceptor(IServiceLocatorInterceptor interceptor);
    }
}
