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

    public class SpringServiceRegistrar : ISpringServiceRegistrar, ICloneable, IServiceRegistrarBehaviorAccessor
    {
        private SpringServiceLocator _parent;
        private GenericApplicationContext _container; // IConfigurableApplicationContext
        private IObjectDefinitionFactory _factory = new DefaultObjectDefinitionFactory();

        public SpringServiceRegistrar(SpringServiceLocator parent, GenericApplicationContext container)
        {
            _parent = parent;
            _container = container;
            LifetimeForRegisters = ServiceRegistrarLifetime.Transient;
        }

        object ICloneable.Clone() { return MemberwiseClone(); }

        // locator
        public IServiceLocator Locator
        {
            get { return _parent; }
        }

        // enumerate
        public bool HasRegistered<TService>() { return (_container.GetObjectsOfType(typeof(TService)).Count > 0); }
        public bool HasRegistered(Type serviceType) { return (_container.GetObjectsOfType(serviceType).Count > 0); }
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
        public ServiceRegistrarLifetime LifetimeForRegisters { get; private set; }
        public void Register(Type serviceType)
        {
            var b = SetLifetime(ObjectDefinitionBuilder.RootObjectDefinition(_factory, serviceType));
            _container.RegisterObjectDefinition(SpringServiceLocator.GetName(serviceType), b.ObjectDefinition);
        }

        public void Register(Type serviceType, string name)
        {
            var b = SetLifetime(ObjectDefinitionBuilder.RootObjectDefinition(_factory, serviceType));
            _container.RegisterObjectDefinition(name, b.ObjectDefinition);
        }

        // register implementation
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            var b = SetLifetime(ObjectDefinitionBuilder.RootObjectDefinition(_factory, typeof(TImplementation)));
            _container.RegisterObjectDefinition(SpringServiceLocator.GetName(typeof(TImplementation)), b.ObjectDefinition);
        }
        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService
        {
            var b = SetLifetime(ObjectDefinitionBuilder.RootObjectDefinition(_factory, typeof(TImplementation)));
            _container.RegisterObjectDefinition(name, b.ObjectDefinition);
        }
        public void Register<TService>(Type implementationType)
            where TService : class
        {
            var b = SetLifetime(ObjectDefinitionBuilder.RootObjectDefinition(_factory, implementationType));
            _container.RegisterObjectDefinition(SpringServiceLocator.GetName(implementationType), b.ObjectDefinition);
        }
        public void Register<TService>(Type implementationType, string name)
            where TService : class
        {
            var b = ObjectDefinitionBuilder.RootObjectDefinition(_factory, implementationType);
            _container.RegisterObjectDefinition(name, b.ObjectDefinition);
        }
        public void Register(Type serviceType, Type implementationType)
        {
            var b = SetLifetime(ObjectDefinitionBuilder.RootObjectDefinition(_factory, implementationType));
            _container.RegisterObjectDefinition(SpringServiceLocator.GetName(implementationType), b.ObjectDefinition);
        }
        public void Register(Type serviceType, Type implementationType, string name)
        {
            var b = SetLifetime(ObjectDefinitionBuilder.RootObjectDefinition(_factory, implementationType));
            _container.RegisterObjectDefinition(name, b.ObjectDefinition);
        }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class { EnsureTransientLifestyle(); _container.ObjectFactory.RegisterSingleton(SpringServiceLocator.GetName(typeof(TService)), instance); }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { EnsureTransientLifestyle(); _container.ObjectFactory.RegisterSingleton(name, instance); }
        public void RegisterInstance(Type serviceType, object instance) { EnsureTransientLifestyle(); _container.ObjectFactory.RegisterSingleton(SpringServiceLocator.GetName(serviceType), instance); }
        public void RegisterInstance(Type serviceType, object instance, string name) { EnsureTransientLifestyle(); _container.ObjectFactory.RegisterSingleton(name, instance); }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class
        {
            EnsureTransientLifestyle();
            Func<TService> func = () => factoryMethod(_parent);
            _container.ObjectFactory.RegisterSingleton(SpringServiceLocator.GetName(typeof(TService)), func);
        }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class
        {
            EnsureTransientLifestyle();
            Func<TService> func = () => factoryMethod(_parent);
            _container.ObjectFactory.RegisterSingleton(name, func);
        }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod)
        {
            EnsureTransientLifestyle();
            Func<object> func = () => factoryMethod(_parent);
            _container.ObjectFactory.RegisterSingleton(SpringServiceLocator.GetName(serviceType), func);
        }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name)
        {
            EnsureTransientLifestyle();
            Func<object> func = () => factoryMethod(_parent);
            _container.ObjectFactory.RegisterSingleton(name, func);
        }

        // interceptor
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