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
        IDictionary<string, IDictionary<Type, object>> Container { get; }
    }

    /// <summary>
    /// MicroServiceLocator
    /// </summary>
    public class MicroServiceLocator : IMicroServiceLocator, ServiceLocatorManager.ISetupRegistration
    {
        private IDictionary<string, IDictionary<Type, object>> _container;
        private MicroServiceRegistrar _registrar;

        // used so Type is not a reserved value
        internal class Trampoline { public Type Type; }

        static MicroServiceLocator() { ServiceLocatorManager.EnsureRegistration(); }
        public MicroServiceLocator()
            : this(new Dictionary<Type, object>()) { }
        public MicroServiceLocator(IDictionary<Type, object> container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            Container = new Dictionary<string, IDictionary<Type, object>> { 
                {string.Empty, container}
            };
        }
        public MicroServiceLocator(IDictionary<string, IDictionary<Type, object>> container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            Container = container;
        }

        Action<IServiceLocator, string> ServiceLocatorManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (locator, name) => ServiceLocatorManager.RegisterInstance<IMicroServiceLocator>(this, locator, name); }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        // registrar
        public IServiceRegistrar Registrar
        {
            get { return _registrar; }
        }
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
                IDictionary<Type, object> container;
                if (!_container.TryGetValue(name ?? string.Empty, out container))
                    throw new ArgumentOutOfRangeException(string.Format("Could not resolve implementation for [{0}-{1}]", name ?? "+", serviceType.ToString()));
                // if not registered, then use requested type
                object concrete;
                if (!container.TryGetValue(serviceType, out concrete))
                    concrete = new Trampoline { Type = serviceType };
                // register as null for default constructor
                if (concrete == null)
                    return Activator.CreateInstance(serviceType);
                // try factory
                var factory = (concrete as Func<IServiceLocator, object>);
                if (factory != null)
                    return factory(this);
                // try type, then register as factory
                var concreteAsTrampoline = (concrete as Trampoline);
                if (concreteAsTrampoline != null)
                {
                    var concreteAsType = concreteAsTrampoline.Type;
                    var constructorInfo = concreteAsType.GetConstructors()
                        .FirstOrDefault(constructor => constructor.GetParameters().Length > 0);
                    if (constructorInfo == null)
                        container[serviceType] = factory = (l => Activator.CreateInstance(concreteAsType));
                    else
                    {
                        var args = constructorInfo.GetParameters()
                            .Select(arg => Resolve(arg.ParameterType))
                            .ToArray();
                        container[serviceType] = factory = (l => Activator.CreateInstance(concreteAsType, args));
                    }
                    return factory(this);
                }
                // other wise was registered as an instance
                return concrete;
            }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }
        //
        public IEnumerable<TService> ResolveAll<TService>()
            where TService : class { return ResolveAll(typeof(TService)).Cast<TService>(); }
        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            var items = _container.SelectMany(x => x.Value, (a, b) => new { Name = a.Key, Services = b })
                .Where(x => x.Services.Key == serviceType)
                .Select(x => new { ServiceType = x.Services.Key, x.Name })
                .ToList();
            return items
                .Select(x => Resolve(x.ServiceType, x.Name))
                .ToList();
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

        public IDictionary<string, IDictionary<Type, object>> Container
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

