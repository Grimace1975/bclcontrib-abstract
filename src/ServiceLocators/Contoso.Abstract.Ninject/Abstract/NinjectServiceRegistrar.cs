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
using Ninject;
using Ninject.Modules;
using System.Collections.Generic;
using Ninject.Syntax;
using Ninject.Infrastructure;
namespace Contoso.Abstract
{
    /// <summary>
    /// INinjectServiceRegistrar
    /// </summary>
    public interface INinjectServiceRegistrar : IServiceRegistrar { }

    /// <summary>
    /// NinjectServiceRegistrar
    /// </summary>
    public class NinjectServiceRegistrar : NinjectModule, INinjectServiceRegistrar, IDisposable, ICloneable, IServiceRegistrarBehaviorAccessor
    {
        private NinjectServiceLocator _parent;
        private IKernel _container;
        private Guid _moduleId;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectServiceRegistrar"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="kernel">The kernel.</param>
        public NinjectServiceRegistrar(NinjectServiceLocator parent, IKernel kernel)
        {
            _parent = parent;
            _container = kernel;
            _container.Load(new INinjectModule[] { this });
            LifetimeForRegisters = ServiceRegistrarLifetime.Transient;
        }

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
        public bool HasRegistered<TService>() { return Kernel.GetBindings(typeof(TService)).Any(); }
        /// <summary>
        /// Determines whether the specified service type has registered.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        ///   <c>true</c> if the specified service type has registered; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRegistered(Type serviceType) { return Kernel.GetBindings(serviceType).Any(); }
        /// <summary>
        /// Gets the registrations for.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            return Kernel.GetBindings(serviceType)
                .Select(x => new ServiceRegistration { ServiceType = x.Service });
        }
        /// <summary>
        /// Gets the registrations.
        /// </summary>
        public IEnumerable<ServiceRegistration> Registrations
        {
            get
            {
                return Bindings
                    .Select(x => new ServiceRegistration { ServiceType = x.Service });
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
        public void Register(Type serviceType) { Bind(serviceType).ToSelf(); }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, string name) { Bind(serviceType).ToSelf().Named(name); }

        // register implementation
        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService { SetLifetime(Bind<TService>().To<TImplementation>()); }
        /// <summary>
        /// Registers the specified name.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="name">The name.</param>
        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService { SetLifetime(Bind<TService>().To(typeof(TImplementation))).Named(name); }

        /// <summary>
        /// Registers the specified implementation type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationType">Type of the implementation.</param>
        public void Register<TService>(Type implementationType)
            where TService : class { SetLifetime(Bind<TService>().To(implementationType)); }
        /// <summary>
        /// Registers the specified implementation type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="name">The name.</param>
        public void Register<TService>(Type implementationType, string name)
            where TService : class { SetLifetime(Bind<TService>().To(implementationType)).Named(name); }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        public void Register(Type serviceType, Type implementationType) { SetLifetime(Bind(serviceType).To(implementationType)); }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, Type implementationType, string name) { SetLifetime(Bind(serviceType).To(implementationType)).Named(name); }

        // register instance
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        public void RegisterInstance<TService>(TService instance)
            where TService : class { EnsureTransientLifestyle(); Bind<TService>().ToConstant(instance); }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { EnsureTransientLifestyle(); Bind<TService>().ToConstant(instance).Named(name); }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        public void RegisterInstance(Type serviceType, object instance) { EnsureTransientLifestyle(); Bind(serviceType).ToConstant(instance); }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public void RegisterInstance(Type serviceType, object instance, string name) { EnsureTransientLifestyle(); Bind(serviceType).ToConstant(instance).Named(name); }

        // register method
        /// <summary>
        /// Registers the specified factory method.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="factoryMethod">The factory method.</param>
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { SetLifetime(Bind<TService>().ToMethod(x => factoryMethod(_parent))); }
        /// <summary>
        /// Registers the specified factory method.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="factoryMethod">The factory method.</param>
        /// <param name="name">The name.</param>
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class { SetLifetime(Bind<TService>().ToMethod(x => factoryMethod(_parent))).Named(name); }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="factoryMethod">The factory method.</param>
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod) { SetLifetime(Bind(serviceType).ToMethod(x => factoryMethod(_parent))); }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="factoryMethod">The factory method.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name) { SetLifetime(Bind(serviceType).ToMethod(x => factoryMethod(_parent))).Named(name); }

        // interceptor
        /// <summary>
        /// Registers the interceptor.
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor)
        {
            throw new NotSupportedException();
        }

        #region Domain specific

        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            _moduleId = Guid.NewGuid();
        }

        /// <summary>
        /// Gets the module's name. Only a single module with a given name can be loaded at one time.
        /// </summary>
        public override string Name
        {
            get { return _moduleId.ToString(); }
        }

        //private string MakeId(Type serviceType, Type implementationType) { return serviceType.Name + "->" + implementationType.FullName; }

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

        private IBindingNamedWithOrOnSyntax<TService> SetLifetime<TService>(IBindingWhenInNamedWithOrOnSyntax<TService> bindingWhenInNamedWithOrOnSyntax)
        {
            switch (LifetimeForRegisters)
            {
                case ServiceRegistrarLifetime.Transient: return bindingWhenInNamedWithOrOnSyntax.InScope(StandardScopeCallbacks.Transient);
                case ServiceRegistrarLifetime.Singleton: return bindingWhenInNamedWithOrOnSyntax.InScope(StandardScopeCallbacks.Singleton);
                case ServiceRegistrarLifetime.Thread: return bindingWhenInNamedWithOrOnSyntax.InScope(StandardScopeCallbacks.Thread);
                default: throw new NotSupportedException();
            }
        }
    }
}
