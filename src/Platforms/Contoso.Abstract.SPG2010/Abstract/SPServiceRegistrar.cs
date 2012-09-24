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
using Contoso.Abstract.SPG2010;
using SPIServiceLocator = Microsoft.Practices.ServiceLocation.IServiceLocator;
using InstantiationType = Microsoft.Practices.SharePoint.Common.ServiceLocation.InstantiationType;
using System.Reflection;
using Microsoft.SharePoint;
namespace Contoso.Abstract
{
    /// <summary>
    /// ISPServiceRegistrar
    /// </summary>
    public interface ISPServiceRegistrar : IServiceRegistrar
    {
        /// <summary>
        /// Gets the config.
        /// </summary>
        IServiceLocatorConfig Config { get; }
        /// <summary>
        /// Removes this instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        void Remove<TService>();
        /// <summary>
        /// Removes the specified name.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="name">The name.</param>
        void Remove<TService>(string name);
    }

    /// <summary>
    /// SPServiceRegistrar
    /// </summary>
    public class SPServiceRegistrar : ISPServiceRegistrar, IDisposable, ICloneable, IServiceRegistrarBehaviorAccessor, ISPServiceBehaviorAccessor
    {
        private SPServiceLocator _parent;
        private SPIServiceLocator _container;
        private ServiceLocatorConfig _registrar;

        /// <summary>
        /// Initializes a new instance of the <see cref="SPServiceRegistrar"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="container">The container.</param>
        public SPServiceRegistrar(SPServiceLocator parent, SPIServiceLocator container)
        {
            _parent = parent;
            _container = container;
            _registrar = (_container.GetInstance<IServiceLocatorConfig>() as ServiceLocatorConfig);
            if (_registrar == null)
                throw new NullReferenceException("registrar");
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
        public bool HasRegistered<TService>() { throw new NotSupportedException(); }
        /// <summary>
        /// Determines whether the specified service type has registered.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        ///   <c>true</c> if the specified service type has registered; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRegistered(Type serviceType) { throw new NotSupportedException(); }
        /// <summary>
        /// Gets the registrations for.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            return Registrations
                .Where(x => serviceType.IsAssignableFrom(x.ServiceType));
        }
        /// <summary>
        /// Gets the registrations.
        /// </summary>
        public IEnumerable<ServiceRegistration> Registrations
        {
            get
            {
                return _registrar.GetTypeMappings()
                    .Select(x => new ServiceRegistration
                    {
                        ServiceType = Assembly.Load(x.FromAssembly).GetType(x.FromType),
                        ImplementationType = Assembly.Load(x.ToAssembly).GetType(x.ToAssembly),
                        Name = x.Key
                    });
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
        public void Register(Type serviceType) { throw new NotSupportedException(); }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, string name) { throw new NotSupportedException(); }

        // register implementation
        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService { _registrar.RegisterTypeMapping<TService, TImplementation>(null, GetLifestyle()); }
        /// <summary>
        /// Registers the specified name.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="name">The name.</param>
        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService { _registrar.RegisterTypeMapping<TService, TImplementation>(name, GetLifestyle()); }
        /// <summary>
        /// Registers the specified implementation type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationType">Type of the implementation.</param>
        public void Register<TService>(Type implementationType)
             where TService : class { throw new NotSupportedException(); }
        /// <summary>
        /// Registers the specified implementation type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="name">The name.</param>
        public void Register<TService>(Type implementationType, string name)
             where TService : class { throw new NotSupportedException(); }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        public void Register(Type serviceType, Type implementationType) { throw new NotSupportedException(); }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, Type implementationType, string name) { throw new NotSupportedException(); }

        // register instance
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        public void RegisterInstance<TService>(TService instance)
            where TService : class { throw new NotSupportedException(); }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { throw new NotSupportedException(); }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        public void RegisterInstance(Type serviceType, object instance) { throw new NotSupportedException(); }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public void RegisterInstance(Type serviceType, object instance, string name) { throw new NotSupportedException(); }

        // register method
        /// <summary>
        /// Registers the specified factory method.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="factoryMethod">The factory method.</param>
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { throw new NotSupportedException(); }
        /// <summary>
        /// Registers the specified factory method.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="factoryMethod">The factory method.</param>
        /// <param name="name">The name.</param>
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class { throw new NotSupportedException(); }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="factoryMethod">The factory method.</param>
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod) { throw new NotSupportedException(); }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="factoryMethod">The factory method.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name) { throw new NotSupportedException(); }

        // interceptor
        /// <summary>
        /// Registers the interceptor.
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor) { throw new NotSupportedException(); }

        #region Behavior

        bool IServiceRegistrarBehaviorAccessor.RegisterInLocator
        {
            get { return false; }
        }

        ServiceRegistrarLifetime IServiceRegistrarBehaviorAccessor.Lifetime
        {
            get { return LifetimeForRegisters; }
            set { LifetimeForRegisters = value; }
        }

        #endregion

        #region SPBehavior

        void ISPServiceBehaviorAccessor.SetContainer(SPSite site)
        {
            _container = (site != null ? SharePointServiceLocator.GetCurrent(site) : SharePointServiceLocator.GetCurrent());
            _registrar = (_container.GetInstance<IServiceLocatorConfig>() as ServiceLocatorConfig);
            if (_registrar == null)
                throw new NullReferenceException("registrar");
        }

        void ISPServiceBehaviorAccessor.SetContainerAsFarm()
        {
            _container = SharePointServiceLocator.GetCurrentFarm();
            _registrar = (_container.GetInstance<IServiceLocatorConfig>() as ServiceLocatorConfig);
            if (_registrar == null)
                throw new NullReferenceException("registrar");
        }

        #endregion

        #region Domain specific

        /// <summary>
        /// Gets the config.
        /// </summary>
        public IServiceLocatorConfig Config
        {
            get { return _registrar; }
        }

        /// <summary>
        /// Removes this instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        public void Remove<TService>() { _registrar.RemoveTypeMappings<TService>(); }
        /// <summary>
        /// Removes the specified name.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="name">The name.</param>
        public void Remove<TService>(string name) { _registrar.RemoveTypeMapping<TService>(name); }

        #endregion

        private InstantiationType GetLifestyle()
        {
            switch (LifetimeForRegisters)
            {
                case ServiceRegistrarLifetime.Transient: return InstantiationType.NewInstanceForEachRequest;
                case ServiceRegistrarLifetime.Singleton: return InstantiationType.AsSingleton;
                default: throw new NotSupportedException();
            }
        }
    }
}
