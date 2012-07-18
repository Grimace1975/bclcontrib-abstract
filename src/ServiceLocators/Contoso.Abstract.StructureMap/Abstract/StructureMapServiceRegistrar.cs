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
using StructureMap.Configuration.DSL;
using StructureMap;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Pipeline;
using System.Collections.Generic;
namespace Contoso.Abstract
{
    /// <summary>
    /// IStructureMapServiceRegistrar
    /// </summary>
    public interface IStructureMapServiceRegistrar : IServiceRegistrar
    {
        /// <summary>
        /// Registers all.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        void RegisterAll<TService>();
    }

    /// <summary>
    /// StructureMapServiceRegistrar
    /// </summary>
    public class StructureMapServiceRegistrar : Registry, IStructureMapServiceRegistrar, IDisposable, ICloneable, IServiceRegistrarBehaviorAccessor
    {
        private StructureMapServiceLocator _parent;
        private IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructureMapServiceRegistrar"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="container">The container.</param>
        public StructureMapServiceRegistrar(StructureMapServiceLocator parent, IContainer container)
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

        /// <summary>
        /// Gets a value indicating whether this instance has pending registrations.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has pending registrations; otherwise, <c>false</c>.
        /// </value>
        public bool HasPendingRegistrations { get; private set; }

        /// <summary>
        /// Loads the pending registrations.
        /// </summary>
        public void LoadPendingRegistrations()
        {
            _container.Configure(x => x.AddRegistry(this));
            HasPendingRegistrations = false;
        }

        // enumerate
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
        public bool HasRegistered<TService>() { return _container.Model.HasDefaultImplementationFor(typeof(TService)); }
        /// <summary>
        /// Determines whether the specified service type has registered.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        ///   <c>true</c> if the specified service type has registered; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRegistered(Type serviceType) { return _container.Model.HasDefaultImplementationFor(serviceType); }
        /// <summary>
        /// Gets the registrations for.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            return _container.Model.AllInstances
                .Where(x => serviceType.IsAssignableFrom(x.ConcreteType))
                .Select(x => new ServiceRegistration { ServiceType = x.PluginType, ImplementationType = x.ConcreteType, Name = x.Description });
        }
        /// <summary>
        /// Gets the registrations.
        /// </summary>
        public IEnumerable<ServiceRegistration> Registrations
        {
            get
            {
                return _container.Model.AllInstances
                    .Select(x => new ServiceRegistration { ServiceType = x.PluginType, ImplementationType = x.ConcreteType, Name = x.Description });
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
        public void Register(Type serviceType) { SetLifetime(new GenericFamilyExpression(serviceType, this)).Use((Instance)new ConfiguredInstance(serviceType)); HasPendingRegistrations = true; }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, string name) { SetLifetime(new GenericFamilyExpression(serviceType, this)).Use((Instance)new ConfiguredInstance(serviceType) { Name = name }); HasPendingRegistrations = true; }

        // register implementation
        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService { SetLifetime(new GenericFamilyExpression(typeof(TService), this)).Use((Instance)new ConfiguredInstance(typeof(TImplementation))); HasPendingRegistrations = true; }
        /// <summary>
        /// Registers the specified name.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="name">The name.</param>
        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService { SetLifetime(new GenericFamilyExpression(typeof(TService), this)).Use((Instance)new ConfiguredInstance(typeof(TImplementation)) { Name = name }); HasPendingRegistrations = true; }
        /// <summary>
        /// Registers the specified implementation type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationType">Type of the implementation.</param>
        public void Register<TService>(Type implementationType)
            where TService : class { SetLifetime(new GenericFamilyExpression(typeof(TService), this)).Use((Instance)new ConfiguredInstance(implementationType)); HasPendingRegistrations = true; }
        /// <summary>
        /// Registers the specified implementation type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="name">The name.</param>
        public void Register<TService>(Type implementationType, string name)
            where TService : class { SetLifetime(new GenericFamilyExpression(typeof(TService), this)).Use((Instance)new ConfiguredInstance(implementationType) { Name = name }); HasPendingRegistrations = true; }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        public void Register(Type serviceType, Type implementationType) { SetLifetime(new GenericFamilyExpression(serviceType, this)).Use((Instance)new ConfiguredInstance(implementationType)); HasPendingRegistrations = true; }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, Type implementationType, string name) { SetLifetime(new GenericFamilyExpression(serviceType, this)).Use((Instance)new ConfiguredInstance(implementationType) { Name = name }); HasPendingRegistrations = true; }

        // register instance
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        public void RegisterInstance<TService>(TService instance)
            where TService : class { EnsureTransientLifestyle(); new GenericFamilyExpression(typeof(TService), this).Use((Instance)new ObjectInstance(instance)); HasPendingRegistrations = true; }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { EnsureTransientLifestyle(); new GenericFamilyExpression(typeof(TService), this).Use((Instance)new ObjectInstance(instance) { Name = name }); HasPendingRegistrations = true; }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        public void RegisterInstance(Type serviceType, object instance) { EnsureTransientLifestyle(); new GenericFamilyExpression(serviceType, this).Use((Instance)new ObjectInstance(instance)); HasPendingRegistrations = true; }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public void RegisterInstance(Type serviceType, object instance, string name) { EnsureTransientLifestyle(); new GenericFamilyExpression(serviceType, this).Use((Instance)new ObjectInstance(instance) { Name = name }); HasPendingRegistrations = true; }

        // register method
        /// <summary>
        /// Registers the specified factory method.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="factoryMethod">The factory method.</param>
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { SetLifetime(new GenericFamilyExpression(typeof(TService), this)).Use((Instance)new LambdaInstance<object>(x => factoryMethod(_parent))); HasPendingRegistrations = true; }
        /// <summary>
        /// Registers the specified factory method.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="factoryMethod">The factory method.</param>
        /// <param name="name">The name.</param>
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class { SetLifetime(new GenericFamilyExpression(typeof(TService), this)).Use((Instance)new LambdaInstance<object>(x => factoryMethod(_parent)) { Name = name }); HasPendingRegistrations = true; }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="factoryMethod">The factory method.</param>
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod) { SetLifetime(new GenericFamilyExpression(serviceType, this)).Use((Instance)new LambdaInstance<object>(x => factoryMethod(_parent))); HasPendingRegistrations = true; }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="factoryMethod">The factory method.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name) { SetLifetime(new GenericFamilyExpression(serviceType, this)).Use((Instance)new LambdaInstance<object>(x => factoryMethod(_parent)) { Name = name }); HasPendingRegistrations = true; }

        // interceptor
        /// <summary>
        /// Registers the interceptor.
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor)
        {
            _container.Configure(x => x.RegisterInterceptor(new Interceptor(interceptor, _container)));
        }

        #region Domain extents

        /// <summary>
        /// Registers all.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        public void RegisterAll<TService>() { Scan(scanner => scanner.AddAllTypesOf<TService>()); }

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

        private GenericFamilyExpression SetLifetime(GenericFamilyExpression e)
        {
            switch (LifetimeForRegisters)
            {
                case ServiceRegistrarLifetime.Transient: return e; // e.LifecycleIs(InstanceScope.Transient);
                case ServiceRegistrarLifetime.Singleton: return e.LifecycleIs(InstanceScope.Singleton);
                case ServiceRegistrarLifetime.Thread: return e.LifecycleIs(InstanceScope.ThreadLocal);
                case ServiceRegistrarLifetime.Request: return e.LifecycleIs(InstanceScope.PerRequest);
                default: throw new NotSupportedException();
            }
        }
    }
}
