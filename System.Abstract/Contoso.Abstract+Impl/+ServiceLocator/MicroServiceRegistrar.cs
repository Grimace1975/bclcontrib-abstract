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
        private IDictionary<string, IDictionary<Type, Type>> _container;

        public MicroServiceRegistrar(MicroServiceLocator parent, IDictionary<string, IDictionary<Type, Type>> container)
        {
            _parent = parent;
            _container = container;
        }

        // locator
        public IServiceLocator GetLocator() { return _parent; }
        public TServiceLocator GetLocator<TServiceLocator>()
            where TServiceLocator : class, IServiceLocator { return (_parent as TServiceLocator); }

        // register type
        public void Register(Type serviceType) { Register(serviceType, serviceType, string.Empty); }
        public void Register(Type serviceType, string name) { Register(serviceType, serviceType, name); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TImplementation : class, TService { Register(typeof(TService), typeof(TImplementation), string.Empty); }
        public void Register<TService, TImplementation>(string name)
            where TImplementation : class, TService { Register(typeof(TService), typeof(TImplementation), name); }
        public void Register<TService>(Type implementationType)
           where TService : class { Register(typeof(TService), implementationType, string.Empty); }
        public void Register<TService>(Type implementationType, string name)
           where TService : class { Register(typeof(TService), implementationType, name); }
        public void Register(Type serviceType, Type implementationType) { Register(serviceType, implementationType, string.Empty); }
        public void Register(Type serviceType, Type implementationType, string name)
        {
            IDictionary<Type, Type> singleContainer;
            if (!_container.TryGetValue(name, out singleContainer))
                _container[name] = singleContainer = new Dictionary<Type, Type>();
            singleContainer[serviceType] = implementationType;
        }

        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class { throw new NotSupportedException(); }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { throw new NotSupportedException(); }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { throw new NotSupportedException(); }
    }
}

