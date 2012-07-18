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
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using System.Reflection;
using Castle.Core;
namespace Contoso.Abstract
{
    /// <summary>
    /// ICastleWindsorServiceRegistrar
    /// </summary>
    public interface ICastleWindsorServiceRegistrar : IServiceRegistrar
    {
        //void RegisterAll<Source>();
    }

    /// <summary>
    /// CastleWindsorServiceRegistrar
    /// </summary>
    public class CastleWindsorServiceRegistrar : ICastleWindsorServiceRegistrar, IDisposable, ICloneable, IServiceRegistrarBehaviorAccessor
    {
        private CastleWindsorServiceLocator _parent;
        private IWindsorContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="CastleWindsorServiceRegistrar"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="container">The container.</param>
        public CastleWindsorServiceRegistrar(CastleWindsorServiceLocator parent, IWindsorContainer container)
        {
            _parent = parent;
            _container = container;
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
        public bool HasRegistered<TService>() { return HasRegistered(typeof(TService)); }
        /// <summary>
        /// Determines whether the specified service type has registered.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        ///   <c>true</c> if the specified service type has registered; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRegistered(Type serviceType) { return _container.Kernel.HasComponent(serviceType); }
        /// <summary>
        /// Gets the registrations for.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            return _container.Kernel.GetAssignableHandlers(serviceType)
                .Select(x => new ServiceRegistration { ImplementationType = x.ComponentModel.Implementation });
        }
        /// <summary>
        /// Gets the registrations.
        /// </summary>
        public IEnumerable<ServiceRegistration> Registrations
        {
            get
            {
                return _container.Kernel.GetAssignableHandlers(typeof(object))
                    .Select(x => new ServiceRegistration { ImplementationType = x.ComponentModel.Implementation });
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
        public void Register(Type serviceType) { _container.Register(Component.For(serviceType).LifeStyle.Is(GetLifetime())); }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, string name) { _container.Register(Component.For(serviceType).Named(name).LifeStyle.Is(GetLifetime())); }

        // register implementation
        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService { EnsureTransientLifestyle(); _container.Register(Component.For<TService>().ImplementedBy<TImplementation>().LifeStyle.Is(GetLifetime())); }
        /// <summary>
        /// Registers the specified name.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="name">The name.</param>
        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService { EnsureTransientLifestyle(); _container.Register(Component.For<TService>().Named(name).ImplementedBy<TImplementation>().LifeStyle.Is(GetLifetime())); }
        /// <summary>
        /// Registers the specified implementation type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationType">Type of the implementation.</param>
        public void Register<TService>(Type implementationType)
             where TService : class { EnsureTransientLifestyle(); _container.Register(Component.For<TService>().ImplementedBy(implementationType).LifeStyle.Is(GetLifetime())); }
        /// <summary>
        /// Registers the specified implementation type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="name">The name.</param>
        public void Register<TService>(Type implementationType, string name)
             where TService : class { EnsureTransientLifestyle(); _container.Register(Component.For<TService>().Named(name).ImplementedBy(implementationType).LifeStyle.Is(GetLifetime())); }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        public void Register(Type serviceType, Type implementationType) { EnsureTransientLifestyle(); _container.Register(Component.For(serviceType).ImplementedBy(implementationType).LifeStyle.Is(GetLifetime())); }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, Type implementationType, string name) { EnsureTransientLifestyle(); _container.Register(Component.For(serviceType).Named(name).ImplementedBy(implementationType).LifeStyle.Is(GetLifetime())); }

        // register instance
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        public void RegisterInstance<TService>(TService instance)
            where TService : class { _container.Register(Component.For<TService>().Instance(instance)); }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { _container.Register(Component.For<TService>().Named(name).Instance(instance)); }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        public void RegisterInstance(Type serviceType, object instance) { _container.Register(Component.For(serviceType).Instance(instance)); }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public void RegisterInstance(Type serviceType, object instance, string name) { _container.Register(Component.For(serviceType).Named(name).Instance(instance)); }

        // register method
        /// <summary>
        /// Registers the specified factory method.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="factoryMethod">The factory method.</param>
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { _container.Register(Component.For<TService>().UsingFactoryMethod<TService>(x => factoryMethod(_parent)).LifeStyle.Is(GetLifetime())); }
        /// <summary>
        /// Registers the specified factory method.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="factoryMethod">The factory method.</param>
        /// <param name="name">The name.</param>
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class { _container.Register(Component.For<TService>().UsingFactoryMethod<TService>(x => factoryMethod(_parent)).Named(name).LifeStyle.Is(GetLifetime())); }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="factoryMethod">The factory method.</param>
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod) { _container.Register(Component.For(serviceType).UsingFactoryMethod(x => factoryMethod(_parent)).LifeStyle.Is(GetLifetime())); }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="factoryMethod">The factory method.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name) { _container.Register(Component.For(serviceType).UsingFactoryMethod(x => factoryMethod(_parent)).Named(name).LifeStyle.Is(GetLifetime())); }

        // interceptor
        /// <summary>
        /// Registers the interceptor.
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor)
        {
            _container.Kernel.ComponentModelCreated +=
                model =>
                {
                    if (interceptor.Match(model.Implementation))
                        interceptor.ItemCreated(model.Implementation, model.LifestyleType == LifestyleType.Transient);
                };
        }

        #region Domain specific

        //public void RegisterAll<TService>() { AllTypes.Of<TService>(); }
        //private static string MakeId(Type serviceType, Type implementationType) { return serviceType.Name + "->" + implementationType.FullName; }

        #endregion

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

        private LifestyleType GetLifetime()
        {
            switch (LifetimeForRegisters)
            {
                case ServiceRegistrarLifetime.Transient: return LifestyleType.Transient;
                case ServiceRegistrarLifetime.Singleton: return LifestyleType.Singleton;
                case ServiceRegistrarLifetime.Thread: return LifestyleType.Thread;
                case ServiceRegistrarLifetime.Pooled: return LifestyleType.Pooled;
                case ServiceRegistrarLifetime.Request: return LifestyleType.PerWebRequest;
                default: throw new NotSupportedException();
            }
        }
    }
}
