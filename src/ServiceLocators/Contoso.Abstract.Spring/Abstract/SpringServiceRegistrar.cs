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
using Spring.Objects.Factory;
using Spring.Context;
using Spring.Objects.Factory.Support;
using Spring.Context.Support;
using System.Collections.Generic;
using System.Collections;
namespace Contoso.Abstract
{
    /// <summary>
    /// ISpringServiceRegistrar
    /// </summary>
    public interface ISpringServiceRegistrar : IServiceRegistrar { }

    /// <summary>
    /// SpringServiceRegistrar
    /// </summary>
    public class SpringServiceRegistrar : ISpringServiceRegistrar, ICloneable, IServiceRegistrarBehaviorAccessor
    {
        private SpringServiceLocator _parent;
        private GenericApplicationContext _container; // IConfigurableApplicationContext
        private IObjectDefinitionFactory _factory = new DefaultObjectDefinitionFactory();

        /// <summary>
        /// Initializes a new instance of the <see cref="SpringServiceRegistrar"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="container">The container.</param>
        public SpringServiceRegistrar(SpringServiceLocator parent, GenericApplicationContext container)
        {
            _parent = parent;
            _container = container;
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
        public bool HasRegistered<TService>() { return (_container.GetObjectsOfType(typeof(TService)).Count > 0); }
        /// <summary>
        /// Determines whether the specified service type has registered.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        ///   <c>true</c> if the specified service type has registered; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRegistered(Type serviceType) { return (_container.GetObjectsOfType(serviceType).Count > 0); }
        /// <summary>
        /// Gets the registrations for.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            return _container.GetObjectsOfType(serviceType).Cast<DictionaryEntry>()
                .Select(x =>
                {
                    var objectName = (string)x.Key;
                    var objectDefinition = _container.GetObjectDefinition(objectName);
                    return new ServiceRegistration { ServiceType = objectDefinition.ObjectType, ImplementationType = objectDefinition.ObjectType, Name = objectName };
                });
        }
        /// <summary>
        /// Gets the registrations.
        /// </summary>
        public IEnumerable<ServiceRegistration> Registrations
        {
            get
            {
                return _container.GetObjectsOfType(typeof(object)).Cast<DictionaryEntry>()
                    .Select(x =>
                    {
                        var objectName = (string)x.Key;
                        var objectDefinition = _container.GetObjectDefinition(objectName);
                        return new ServiceRegistration { ServiceType = objectDefinition.ObjectType, ImplementationType = objectDefinition.ObjectType, Name = objectName };
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
        public void Register(Type serviceType)
        {
            var b = SetLifetime(ObjectDefinitionBuilder.RootObjectDefinition(_factory, serviceType));
            _container.RegisterObjectDefinition(SpringServiceLocator.GetName(serviceType), b.ObjectDefinition);
        }

        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, string name)
        {
            var b = SetLifetime(ObjectDefinitionBuilder.RootObjectDefinition(_factory, serviceType));
            _container.RegisterObjectDefinition(name, b.ObjectDefinition);
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
            var b = SetLifetime(ObjectDefinitionBuilder.RootObjectDefinition(_factory, typeof(TImplementation)));
            _container.RegisterObjectDefinition(SpringServiceLocator.GetName(typeof(TImplementation)), b.ObjectDefinition);
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
            var b = SetLifetime(ObjectDefinitionBuilder.RootObjectDefinition(_factory, typeof(TImplementation)));
            _container.RegisterObjectDefinition(name, b.ObjectDefinition);
        }
        /// <summary>
        /// Registers the specified implementation type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="implementationType">Type of the implementation.</param>
        public void Register<TService>(Type implementationType)
            where TService : class
        {
            var b = SetLifetime(ObjectDefinitionBuilder.RootObjectDefinition(_factory, implementationType));
            _container.RegisterObjectDefinition(SpringServiceLocator.GetName(implementationType), b.ObjectDefinition);
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
            var b = ObjectDefinitionBuilder.RootObjectDefinition(_factory, implementationType);
            _container.RegisterObjectDefinition(name, b.ObjectDefinition);
        }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        public void Register(Type serviceType, Type implementationType)
        {
            var b = SetLifetime(ObjectDefinitionBuilder.RootObjectDefinition(_factory, implementationType));
            _container.RegisterObjectDefinition(SpringServiceLocator.GetName(implementationType), b.ObjectDefinition);
        }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="name">The name.</param>
        public void Register(Type serviceType, Type implementationType, string name)
        {
            var b = SetLifetime(ObjectDefinitionBuilder.RootObjectDefinition(_factory, implementationType));
            _container.RegisterObjectDefinition(name, b.ObjectDefinition);
        }

        // register instance
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        public void RegisterInstance<TService>(TService instance)
            where TService : class { EnsureTransientLifestyle(); _container.ObjectFactory.RegisterSingleton(SpringServiceLocator.GetName(typeof(TService)), instance); }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { EnsureTransientLifestyle(); _container.ObjectFactory.RegisterSingleton(name, instance); }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        public void RegisterInstance(Type serviceType, object instance) { EnsureTransientLifestyle(); _container.ObjectFactory.RegisterSingleton(SpringServiceLocator.GetName(serviceType), instance); }
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public void RegisterInstance(Type serviceType, object instance, string name) { EnsureTransientLifestyle(); _container.ObjectFactory.RegisterSingleton(name, instance); }

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
            Func<TService> func = () => factoryMethod(_parent);
            _container.ObjectFactory.RegisterSingleton(SpringServiceLocator.GetName(typeof(TService)), func);
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
            Func<TService> func = () => factoryMethod(_parent);
            _container.ObjectFactory.RegisterSingleton(name, func);
        }
        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="factoryMethod">The factory method.</param>
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod)
        {
            EnsureTransientLifestyle();
            Func<object> func = () => factoryMethod(_parent);
            _container.ObjectFactory.RegisterSingleton(SpringServiceLocator.GetName(serviceType), func);
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
            Func<object> func = () => factoryMethod(_parent);
            _container.ObjectFactory.RegisterSingleton(name, func);
        }

        // interceptor
        /// <summary>
        /// Registers the interceptor.
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor) { _container.ObjectFactory.AddObjectPostProcessor(new Interceptor(interceptor, _container)); }

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

        private ObjectDefinitionBuilder SetLifetime(ObjectDefinitionBuilder b)
        {
            switch (LifetimeForRegisters)
            {
                case ServiceRegistrarLifetime.Transient: break;
                case ServiceRegistrarLifetime.Singleton: b.SetSingleton(true); break;
                default: throw new NotSupportedException();
            }
            return b;
        }
    }
}
