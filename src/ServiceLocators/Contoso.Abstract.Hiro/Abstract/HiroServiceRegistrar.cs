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
using System.Linq;
using System.Abstract;
using System.Collections.Generic;
using Hiro;
using Hiro.Containers;
namespace Contoso.Abstract
{
    /// <summary>
    /// IHiroServiceRegistrar
    /// </summary>
    public interface IHiroServiceRegistrar : IServiceRegistrar { }

    /// <summary>
    /// HiroServiceRegistrar
    /// </summary>
    public class HiroServiceRegistrar : IHiroServiceRegistrar, IDisposable, ICloneable, IServiceRegistrarBehaviorAccessor
    {
        private HiroServiceLocator _parent;
        private DependencyMap _builder;
        private IMicroContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="HiroServiceRegistrar"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="containerBuilder">The container builder.</param>
        public HiroServiceRegistrar(HiroServiceLocator parent, DependencyMap builder, out Func<IMicroContainer> containerBuilder)
        {
            _parent = parent;
            _builder = builder;
            containerBuilder = (() => _container = _builder.CreateContainer());
            LifetimeForRegisters = ServiceRegistrarLifetime.Transient;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() { }
        object ICloneable.Clone() { return MemberwiseClone(); }

        // locator
        /// <summary>
        /// Gets the locator.
        /// </summary>
        public IServiceLocator Locator
        {
            get { return _parent; }
        }

        // enumerate
        /// <summary>
        /// Determines whether this instance has registered.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>
        ///   <c>true</c> if this instance has registered; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRegistered<TService>() { return _builder.Contains(typeof(TService)); }
        /// <summary>
        /// Determines whether the specified service type has registered.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        ///   <c>true</c> if the specified service type has registered; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRegistered(Type serviceType) { return _builder.Contains(serviceType); }
        /// <summary>
        /// Gets the registrations for.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            return _builder.Dependencies
                .Where(x => serviceType.IsAssignableFrom(x.ServiceType))
                .Select(x => new ServiceRegistration { ServiceType = x.ServiceType, Name = x.ServiceName });
        }
        /// <summary>
        /// Gets the registrations.
        /// </summary>
        public IEnumerable<ServiceRegistration> Registrations
        {
            get
            {
                return _builder.Dependencies
                    .Select(x => new ServiceRegistration { ServiceType = x.ServiceType, Name = x.ServiceName });
            }
        }

        // register type
        /// <summary>
        /// Gets the lifetime for registers.
        /// </summary>
        public ServiceRegistrarLifetime LifetimeForRegisters { get; private set; }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        public void Register(Type serviceType)
        {
            if (IsDefaultLifetime()) _builder.AddService(serviceType, serviceType);
            else _builder.AddSingletonService(serviceType, serviceType);
        }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, string name)
        {
            if (IsDefaultLifetime()) _builder.AddService(name, serviceType, serviceType);
            else _builder.AddSingletonService(name, serviceType, serviceType);
        }

        // register implementation
        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            if (IsDefaultLifetime()) _builder.AddService<TService, TImplementation>();
            else _builder.AddSingletonService<TService, TImplementation>();
        }
        /// <summary>
        /// Registers the specified name.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="name">The name.</param>
        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService
        {
            if (IsDefaultLifetime()) _builder.AddService<TService, TImplementation>(name);
            else _builder.AddSingletonService<TService, TImplementation>(name);
        }
        /// <summary>
        /// Registers the specified implementation type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationType">Type of the implementation.</param>
        public void Register<TService>(Type implementationType)
             where TService : class
        {
            if (IsDefaultLifetime()) _builder.AddService(typeof(TService), implementationType);
            else _builder.AddSingletonService(typeof(TService), implementationType);
        }
        /// <summary>
        /// Registers the specified implementation type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="name">The name.</param>
        public void Register<TService>(Type implementationType, string name)
             where TService : class
        {
            if (IsDefaultLifetime()) _builder.AddService(name, typeof(TService), implementationType);
            else _builder.AddSingletonService(name, typeof(TService), implementationType);
        }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        public void Register(Type serviceType, Type implementationType)
        {
            if (IsDefaultLifetime()) _builder.AddService(serviceType, implementationType);
            else _builder.AddSingletonService(serviceType, implementationType);
        }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, Type implementationType, string name)
        {
            if (IsDefaultLifetime()) _builder.AddService(name, serviceType, implementationType);
            else _builder.AddSingletonService(name, serviceType, implementationType);
        }

        // register instance
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        public void RegisterInstance<TService>(TService instance)
            where TService : class
        {
            EnsureTransientLifestyle();
            Func<IMicroContainer, TService> f = (x => instance);
            _builder.AddService<TService>(f);
        }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class
        {
            EnsureTransientLifestyle();
            Func<IMicroContainer, TService> f = (x => instance);
            _builder.AddService<TService>(name, f);
        }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        public void RegisterInstance(Type serviceType, object instance)
        {
            EnsureTransientLifestyle();
            Func<IMicroContainer, object> f = (x => instance);
            _builder.AddService(serviceType, f);
        }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public void RegisterInstance(Type serviceType, object instance, string name)
        {
            EnsureTransientLifestyle();
            Func<IMicroContainer, object> f = (x => instance);
            _builder.AddService(name, serviceType, f);
        }

        // register method
        /// <summary>
        /// Registers the specified factory method.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="factoryMethod">The factory method.</param>
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class
        {
            EnsureTransientLifestyle();
            Func<IMicroContainer, TService> f = (x => factoryMethod(_parent));
            _builder.AddService<TService>(f);
        }
        /// <summary>
        /// Registers the specified factory method.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="factoryMethod">The factory method.</param>
        /// <param name="name">The name.</param>
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class
        {
            EnsureTransientLifestyle();
            Func<IMicroContainer, TService> f = (x => factoryMethod(_parent));
            _builder.AddService<TService>(name, f);
        }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="factoryMethod">The factory method.</param>
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod)
        {
            EnsureTransientLifestyle();
            Func<IMicroContainer, object> f = (x => factoryMethod(_parent));
            _builder.AddService(serviceType, f);
        }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="factoryMethod">The factory method.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name)
        {
            EnsureTransientLifestyle();
            Func<IMicroContainer, object> f = (x => factoryMethod(_parent));
            _builder.AddService(name, serviceType, f);
        }

        // interceptor
        /// <summary>
        /// Registers the interceptor.
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor) { throw new NotSupportedException(); }

        #region Behavior

        bool IServiceRegistrarBehaviorAccessor.RegisterInLocator
        {
            get { return true; }
        }

        ServiceRegistrarLifetime IServiceRegistrarBehaviorAccessor.Lifetime
        {
            get { return LifetimeForRegisters; }
            set { LifetimeForRegisters = value; }
        }

        #endregion

        private void EnsureTransientLifestyle()
        {
            if (LifetimeForRegisters != ServiceRegistrarLifetime.Transient)
                throw new NotSupportedException();
        }

        private bool IsDefaultLifetime()
        {
            switch (LifetimeForRegisters)
            {
                case ServiceRegistrarLifetime.Transient: return true;
                case ServiceRegistrarLifetime.Singleton: return false;
                default: throw new NotSupportedException();
            }
        }
    }
}
