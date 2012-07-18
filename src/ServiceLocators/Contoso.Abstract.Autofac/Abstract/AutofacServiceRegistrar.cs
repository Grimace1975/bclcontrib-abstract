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
using Autofac;
using Autofac.Builder;
using Autofac.Core;
namespace Contoso.Abstract
{
    /// <summary>
    /// IAutofacServiceRegistrar
    /// </summary>
    public interface IAutofacServiceRegistrar : IServiceRegistrar { }

    /// <summary>
    /// AutofacServiceRegistrar
    /// </summary>
    public class AutofacServiceRegistrar : IAutofacServiceRegistrar, IDisposable, ICloneable, IServiceRegistrarBehaviorAccessor
    {
        private AutofacServiceLocator _parent;
        private ContainerBuilder _builder;
        private IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacServiceRegistrar"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="container">The container.</param>
        public AutofacServiceRegistrar(AutofacServiceLocator parent, IContainer container)
        {
            _parent = parent;
            _container = container;
            LifetimeForRegisters = ServiceRegistrarLifetime.Transient;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacServiceRegistrar"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="containerBuilder">The container builder.</param>
        public AutofacServiceRegistrar(AutofacServiceLocator parent, ContainerBuilder builder, out Func<IContainer> containerBuilder)
            : this(parent, null)
        {
            _builder = builder;
            containerBuilder = (() => _container = _builder.Build());
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
        public bool HasRegistered<TService>() { return _parent.Container.IsRegistered<TService>(); }
        /// <summary>
        /// Determines whether the specified service type has registered.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        ///   <c>true</c> if the specified service type has registered; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRegistered(Type serviceType) { return _parent.Container.IsRegistered(serviceType); }
        /// <summary>
        /// Gets the registrations for.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            return _parent.Container.ComponentRegistry.Registrations
                .SelectMany(x => x.Services.OfType<TypedService>())
                .Where(x => serviceType.IsAssignableFrom(x.ServiceType))
                .Select(x => new ServiceRegistration { ServiceType = serviceType, ImplementationType = x.ServiceType, Name = x.Description });
        }
        /// <summary>
        /// Gets the registrations.
        /// </summary>
        public IEnumerable<ServiceRegistration> Registrations
        {
            get
            {
                return _parent.Container.ComponentRegistry.Registrations
                    .SelectMany(x => x.Services.OfType<TypedService>())
                    .Select(x => new ServiceRegistration { ImplementationType = x.ServiceType, Name = x.Description });
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
            SetLifestyle(_builder.RegisterType(serviceType));
            if (_container != null)
                UpdateAndClearBuilder();
        }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, string name)
        {
            SetLifestyle(_builder.RegisterType(serviceType).Named(name, serviceType));
            if (_container != null)
                UpdateAndClearBuilder();
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
            if (_container == null)
                SetLifestyle(_builder.RegisterType<TImplementation>().As<TService>());
            else
                _container.ComponentRegistry.Register(SetLifestyle(RegistrationBuilder.ForType<TImplementation>().As<TService>()).CreateRegistration());
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
            Register<TService, TImplementation>();
            if (_container == null)
                SetLifestyle(_builder.RegisterType<TImplementation>().Named<TService>(name));
            else
                _container.ComponentRegistry.Register(SetLifestyle(RegistrationBuilder.ForType<TImplementation>().Named<TService>(name)).CreateRegistration());
        }
        /// <summary>
        /// Registers the specified implementation type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationType">Type of the implementation.</param>
        public void Register<TService>(Type implementationType)
             where TService : class
        {
            if (_container == null)
                SetLifestyle(_builder.RegisterType(implementationType).As<TService>());
            else
                _container.ComponentRegistry.Register(SetLifestyle(RegistrationBuilder.ForType(implementationType).As<TService>()).CreateRegistration());
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
            Register<TService>(implementationType);
            if (_container == null)
                SetLifestyle(_builder.RegisterType(implementationType).Named<TService>(name));
            else
                _container.ComponentRegistry.Register(SetLifestyle(RegistrationBuilder.ForType(implementationType).Named<TService>(name)).CreateRegistration());
        }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        public void Register(Type serviceType, Type implementationType)
        {
            if (_container == null)
                SetLifestyle(_builder.RegisterType(implementationType).As(serviceType));
            else
                _container.ComponentRegistry.Register(SetLifestyle(RegistrationBuilder.ForType(implementationType).As(serviceType)).CreateRegistration());
        }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, Type implementationType, string name)
        {
            Register(serviceType, implementationType);
            if (_container == null)
                SetLifestyle(_builder.RegisterType(implementationType).Named(name, serviceType));
            else
                _container.ComponentRegistry.Register(SetLifestyle(RegistrationBuilder.ForType(implementationType).Named(name, serviceType)).CreateRegistration());
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
            _builder.RegisterInstance(instance).As<TService>().ExternallyOwned();
            if (_container != null)
                UpdateAndClearBuilder();
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
            _builder.RegisterInstance(instance).Named(name, typeof(TService)).ExternallyOwned();
            if (_container != null)
                UpdateAndClearBuilder();
        }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        public void RegisterInstance(Type serviceType, object instance)
        {
            EnsureTransientLifestyle();
            _builder.RegisterInstance(instance).As(serviceType).ExternallyOwned();
            if (_container != null)
                UpdateAndClearBuilder();
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
            _builder.RegisterInstance(instance).Named(name, serviceType).ExternallyOwned();
            if (_container != null)
                UpdateAndClearBuilder();
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
            SetLifestyle(_builder.Register(x => factoryMethod(_parent)).As<TService>());
            if (_container != null)
                UpdateAndClearBuilder();
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
            SetLifestyle(_builder.Register(x => factoryMethod(_parent)).Named<TService>(name));
            if (_container != null)
                UpdateAndClearBuilder();

        }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="factoryMethod">The factory method.</param>
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod)
        {
            SetLifestyle(_builder.Register(x => factoryMethod(_parent)).As(serviceType));
            if (_container != null)
                UpdateAndClearBuilder();
        }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="factoryMethod">The factory method.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name)
        {
            SetLifestyle(_builder.Register(x => factoryMethod(_parent)).Named(name, serviceType));
            if (_container != null)
                UpdateAndClearBuilder();
        }

        // interceptor
        /// <summary>
        /// Registers the interceptor.
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor)
        {
            EnsureTransientLifestyle();
            _builder.RegisterModule(new Interceptor(interceptor));
            if (_container != null)
                UpdateAndClearBuilder();
        }

        #region Domain specific

        private void UpdateAndClearBuilder()
        {
            _builder.Update(_container);
            _builder = new ContainerBuilder();
        }

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

        private IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> SetLifestyle<TLimit, TActivatorData, TRegistrationStyle>(IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> b)
        {
            // must cast to IServiceRegistrar for behavior wrappers
            switch (LifetimeForRegisters)
            {
                case ServiceRegistrarLifetime.Transient: break; // b.InstancePerDependency();
                case ServiceRegistrarLifetime.Singleton: b.SingleInstance(); break;
                default: throw new NotSupportedException();
            }
            return b;
        }
    }
}
