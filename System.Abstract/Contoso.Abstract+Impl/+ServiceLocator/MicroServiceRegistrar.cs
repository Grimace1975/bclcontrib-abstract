using System;
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
        }

        // locator
        public IServiceLocator GetLocator() { return _parent; }
        public TServiceLocator GetLocator<TServiceLocator>()
            where TServiceLocator : class, IServiceLocator { return (_parent as TServiceLocator); }

        // register type
        public void Register(Type serviceType) { RegisterInternal(serviceType, new MicroServiceLocator.Trampoline { Type = serviceType }, string.Empty); }
        public void Register(Type serviceType, string name) { RegisterInternal(serviceType, new MicroServiceLocator.Trampoline { Type = serviceType }, name); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TImplementation : class, TService { RegisterInternal(typeof(TService), new MicroServiceLocator.Trampoline { Type = typeof(TImplementation) }, string.Empty); }
        public void Register<TService, TImplementation>(string name)
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

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { RegisterInternal(typeof(TService), (Func<IServiceLocator, object>)(l => factoryMethod(l)), string.Empty); }
    }
}

