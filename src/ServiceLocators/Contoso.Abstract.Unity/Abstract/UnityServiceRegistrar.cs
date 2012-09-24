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
using Microsoft.Practices.Unity;
using System.Collections.Generic;
namespace Contoso.Abstract
{
    /// <summary>
    /// IUnityServiceRegistrar
    /// </summary>
    public interface IUnityServiceRegistrar : IServiceRegistrar
    {
        /// <summary>
        /// Gets the container.
        /// </summary>
        Func<Type, LifetimeManager> RequestLifetimeBuilder { get; set; }
    }

    /// <summary>
    /// UnityServiceRegistrar
    /// </summary>
    internal sealed class UnityServiceRegistrar : IUnityServiceRegistrar, IDisposable, ICloneable, IServiceRegistrarBehaviorAccessor
    {
        private UnityServiceLocator _parent;
        private IUnityContainer _container;

        public UnityServiceRegistrar(UnityServiceLocator parent, IUnityContainer container)
        {
            _parent = parent;
            _container = container;
        }

        public void Dispose() { }
        object ICloneable.Clone() { return MemberwiseClone(); }

        // locator
        public IServiceLocator Locator
        {
            get { return _parent; }
        }

        // enumerate
        public bool HasRegistered<TService>() { return _container.IsRegistered<TService>(); }
        public bool HasRegistered(Type serviceType) { return _container.IsRegistered(serviceType); }
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            return _container.Registrations
                .Where(x => serviceType.IsAssignableFrom(x.MappedToType))
                .Select(x => new ServiceRegistration { ServiceType = x.RegisteredType, ImplementationType = x.MappedToType, Name = x.Name });
        }
        public IEnumerable<ServiceRegistration> Registrations
        {
            get
            {
                return _container.Registrations
                    .Select(x => new ServiceRegistration { ServiceType = x.RegisteredType, ImplementationType = x.MappedToType, Name = x.Name });
            }
        }

        // register type
        public ServiceRegistrarLifetime LifetimeForRegisters { get; private set; }
        public void Register(Type serviceType) { _container.RegisterType(serviceType, GetLifetime(serviceType), new InjectionMember[0]); }
        public void Register(Type serviceType, string name) { _container.RegisterType(serviceType, name, GetLifetime(serviceType), new InjectionMember[0]); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService { _container.RegisterType<TService, TImplementation>(GetLifetime(typeof(TService)), new InjectionMember[0]); }
        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService { _container.RegisterType<TService, TImplementation>(name, GetLifetime(typeof(TService)), new InjectionMember[0]); }
        public void Register<TService>(Type implementationType)
           where TService : class { _container.RegisterType(typeof(TService), implementationType, GetLifetime(typeof(TService)), new InjectionMember[0]); }
        public void Register<TService>(Type implementationType, string name)
           where TService : class { _container.RegisterType(typeof(TService), implementationType, name, GetLifetime(typeof(TService)), new InjectionMember[0]); }
        public void Register(Type serviceType, Type implementationType) { _container.RegisterType(serviceType, implementationType, GetLifetime(serviceType), new InjectionMember[0]); }
        public void Register(Type serviceType, Type implementationType, string name) { _container.RegisterType(serviceType, implementationType, name, GetLifetime(serviceType), new InjectionMember[0]); }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class { EnsureTransientLifestyle(); _container.RegisterInstance<TService>(instance); }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { EnsureTransientLifestyle(); _container.RegisterInstance<TService>(name, instance); }
        public void RegisterInstance(Type serviceType, object instance) { EnsureTransientLifestyle(); _container.RegisterInstance(serviceType, instance); }
        public void RegisterInstance(Type serviceType, object instance, string name) { EnsureTransientLifestyle(); _container.RegisterInstance(serviceType, name, instance); }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { _container.RegisterType<TService>(GetLifetime(typeof(TService)), new InjectionFactory(c => factoryMethod(_parent))); }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class { _container.RegisterType<TService>(name, GetLifetime(typeof(TService)), new InjectionFactory(c => factoryMethod(_parent))); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod) { _container.RegisterType(serviceType, GetLifetime(serviceType), new InjectionFactory(c => factoryMethod(_parent))); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name) { _container.RegisterType(serviceType, name, GetLifetime(serviceType), new InjectionFactory(c => factoryMethod(_parent))); }

        // interceptor
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor)
        {
            _container.AddExtension(new Interceptor(interceptor));
        }

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

        #region Domain specific

        public Func<Type, LifetimeManager> RequestLifetimeBuilder { get; set; }

        #endregion

        private void EnsureTransientLifestyle()
        {
            if (LifetimeForRegisters != ServiceRegistrarLifetime.Transient)
                throw new NotSupportedException();
        }

        private LifetimeManager GetLifetime(Type serviceType)
        {
            // must cast to IServiceRegistrar for behavior wrappers
            switch (LifetimeForRegisters)
            {
                case ServiceRegistrarLifetime.Transient: return null;
                case ServiceRegistrarLifetime.Singleton: return new ContainerControlledLifetimeManager();
                case ServiceRegistrarLifetime.Thread: return new PerThreadLifetimeManager();
                case ServiceRegistrarLifetime.Request:
                    if (RequestLifetimeBuilder == null)
                        throw new ArgumentOutOfRangeException("RequestLifetimeManagerBuilder", "Please define a request lifetime builder");
                    return RequestLifetimeBuilder(serviceType);
                default: throw new NotSupportedException();
            }
        }
    }
}
