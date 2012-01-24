using System;
using System.Linq;
using System.Abstract;
using System.Collections.Generic;
using System.Reflection;
namespace Contoso.Abstract
{
    /// <summary>
    /// IMicroServiceRegistrar
    /// </summary>
    public interface IMicroServiceRegistrar : IServiceRegistrar { }

    /// <summary>
    /// MicroServiceRegistrar
    /// </summary>
    internal class MicroServiceRegistrar : IMicroServiceRegistrar
    {
        private MicroServiceLocator _parent;
        private IDictionary<string, IDictionary<Type, object>> _container;

        public MicroServiceRegistrar(MicroServiceLocator parent, IDictionary<string, IDictionary<Type, object>> container)
        {
            _parent = parent;
            _container = container;
            LifetimeForRegisters = ServiceRegistrarLifetime.Transient;
        }

        // locator
        public IServiceLocator Locator
        {
            get { return _parent; }
        }

        // enumerate
        public bool HasRegistered<TService>() { return HasRegistered(typeof(TService)); }
        public bool HasRegistered(Type serviceType) { return _container.Any(x => x.Value.Any(y => y.Key == serviceType)); }
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            return _container.SelectMany(x => x.Value, (a, b) => new { Name = a.Key, Services = b })
                .Where(x => serviceType.IsAssignableFrom(x.Services.Key))
                .Select(x => new ServiceRegistration { ServiceType = x.Services.Key, ServiceName = x.Name });
        }
        public IEnumerable<ServiceRegistration> Registrations
        {
            get
            {
                return _container.SelectMany(x => x.Value, (a, b) => new { Name = a.Key, Services = b })
                    .Select(x => new ServiceRegistration { ServiceType = x.Services.Key, ServiceName = x.Name });
            }
        }

        // register type
        public ServiceRegistrarLifetime LifetimeForRegisters { get; set; }
        public void Register(Type serviceType) { RegisterInternal(serviceType, new MicroServiceLocator.Trampoline { Type = serviceType }, string.Empty); }
        public void Register(Type serviceType, string name) { RegisterInternal(serviceType, new MicroServiceLocator.Trampoline { Type = serviceType }, name); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService { RegisterInternal(typeof(TService), new MicroServiceLocator.Trampoline { Type = typeof(TImplementation) }, string.Empty); }
        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService { RegisterInternal(typeof(TService), new MicroServiceLocator.Trampoline { Type = typeof(TImplementation) }, name); }
        public void Register<TService>(Type implementationType)
           where TService : class { RegisterInternal(typeof(TService), new MicroServiceLocator.Trampoline { Type = implementationType }, string.Empty); }
        public void Register<TService>(Type implementationType, string name)
           where TService : class { RegisterInternal(typeof(TService), new MicroServiceLocator.Trampoline { Type = implementationType }, name); }
        public void Register(Type serviceType, Type implementationType) { RegisterInternal(serviceType, new MicroServiceLocator.Trampoline { Type = implementationType }, string.Empty); }
        public void Register(Type serviceType, Type implementationType, string name) { RegisterInternal(serviceType, new MicroServiceLocator.Trampoline { Type = implementationType }, name); }
        private void RegisterInternal(Type serviceType, object concrete, string name)
        {
            IDictionary<Type, object> singleContainer;
            if (!_container.TryGetValue(name, out singleContainer))
                _container[name] = singleContainer = new Dictionary<Type, object>();
            singleContainer[serviceType] = concrete;
        }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class { RegisterInternal(typeof(TService), instance, string.Empty); }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { RegisterInternal(typeof(TService), instance, name); }
        public void RegisterInstance(Type serviceType, object instance) { RegisterInternal(serviceType, instance, string.Empty); }
        public void RegisterInstance(Type serviceType, object instance, string name) { RegisterInternal(serviceType, instance, name); }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { RegisterInternal(typeof(TService), (Func<IServiceLocator, object>)(l => factoryMethod(l)), string.Empty); }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class { RegisterInternal(typeof(TService), (Func<IServiceLocator, object>)(l => factoryMethod(l)), name); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod) { RegisterInternal(serviceType, (Func<IServiceLocator, object>)(l => factoryMethod(l)), string.Empty); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name) { RegisterInternal(serviceType, (Func<IServiceLocator, object>)(l => factoryMethod(l)), name); }

        // interceptor
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor)
        {
            throw new NotSupportedException();
        }
    }
}

