using System;
using System.Linq;
using System.Abstract;
using System.Collections.Generic;
using System.Reflection;
namespace Contoso.Abstract
{
    /// <summary>
    /// IMicroServiceLocator
    /// </summary>
    public interface IMicroServiceLocator : IServiceLocator
    {
        IDictionary<string, IDictionary<Type, Type>> Container { get; }
    }

    /// <summary>
    /// MicroServiceLocator
    /// </summary>
    public class MicroServiceLocator : IMicroServiceLocator
    {
        private IDictionary<string, IDictionary<Type, Type>> _container;
        private MicroServiceRegistrar _registrar;

        public MicroServiceLocator()
            : this(new Dictionary<Type, Type>()) { }
        public MicroServiceLocator(IDictionary<Type, Type> container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            Container = new Dictionary<string, IDictionary<Type, Type>> { 
                {string.Empty, container}
            };
        }
        public MicroServiceLocator(IDictionary<string, IDictionary<Type, Type>> container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            Container = container;
        }

        // registrar
        public IServiceRegistrar GetRegistrar() { return _registrar; }
        public TServiceRegistrar GetRegistrar<TServiceRegistrar>()
            where TServiceRegistrar : class, IServiceRegistrar { return (_registrar as TServiceRegistrar); }

        // resolve
        public TService Resolve<TService>()
            where TService : class { return (TService)Resolve(typeof(TService), string.Empty); }
        public TService Resolve<TService>(string name)
            where TService : class { return (TService)Resolve(typeof(TService), name); }
        public object Resolve(Type serviceType) { return Resolve(serviceType, string.Empty); }
        public object Resolve(Type serviceType, string name)
        {
            try
            {
                IDictionary<Type, Type> container;
                if (!_container.TryGetValue(name, out container))
                    throw new ArgumentOutOfRangeException(string.Format("Could not resolve implementation for [{0}-{1}]", name, serviceType.ToString()));
                Type concreteType;
                if (!container.TryGetValue(serviceType, out concreteType))
                    return Activator.CreateInstance(serviceType);
                var constructorInfo = concreteType.GetConstructors()
                    .FirstOrDefault(constructor => constructor.GetParameters().Length > 0);
                if (constructorInfo == null)
                    return Activator.CreateInstance(concreteType);
                var args = constructorInfo.GetParameters()
                    .Select(arg => Resolve(arg.ParameterType))
                    .ToArray();
                return Activator.CreateInstance(concreteType, args);
            }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }
        //
        public IEnumerable<TService> ResolveAll<TService>()
            where TService : class { return ResolveAll(typeof(TService)).Cast<TService>(); }
        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            return _container.SelectMany(x => x.Value, (a, b) => new { Name = a.Key, Services = b })
                .Where(x => x.Services.Key == serviceType)
                .Select(x => Resolve(x.Services.Value, x.Name));
        }

        // inject
        public TService Inject<TService>(TService instance)
            where TService : class { throw new NotSupportedException(); }

        // release and teardown
        public void Release(object instance) { }
        public void TearDown<TService>(TService instance)
            where TService : class { }
        public void Reset() { }

        #region Domain specific

        public IDictionary<string, IDictionary<Type, Type>> Container
        {
            get { return _container; }
            private set
            {
                _container = value;
                _registrar = new MicroServiceRegistrar(this, value);
            }
        }

        #endregion
    }
}

