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
        private IDictionary<Type, Type> _container;

        public MicroServiceRegistrar(MicroServiceLocator parent, IDictionary<Type, Type> container)
        {
            _parent = parent;
            _container = container;
        }

        // locator
        public IServiceLocator GetLocator() { return _parent; }
        public TServiceLocator GetLocator<TServiceLocator>()
            where TServiceLocator : class, IServiceLocator { return (_parent as TServiceLocator); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TImplementation : class, TService { Register(typeof(TService), typeof(TImplementation)); }
        public void Register<TService, TImplementation>(string id)
            where TImplementation : class, TService { throw new NotSupportedException(); }
        public void Register<TService>(Type implementationType)
           where TService : class { Register(typeof(TService), implementationType); }
        public void Register<TService>(Type implementationType, string id)
           where TService : class { throw new NotSupportedException(); }
        public void Register(Type serviceType, Type implementationType)
        {
            if (_container.ContainsKey(serviceType))
                _container[serviceType] = implementationType;
            else
                _container.Add(serviceType, implementationType);
        }
        public void Register(Type serviceType, Type implementationType, string id) { throw new NotSupportedException(); }

        // register id
        public void Register(Type serviceType, string id) { throw new NotSupportedException(); }

        // register instance
        public void Register<TService>(TService instance)
            where TService : class { throw new NotSupportedException(); }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { throw new NotSupportedException(); }
    }
}

