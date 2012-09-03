using System;
using System.Abstract;
using System.Collections.Generic;
using System.Linq;
namespace Contoso.Abstract
{
    /// <summary>
    /// IMicroServiceLocator
    /// </summary>
    public interface IMicroServiceLocator : IServiceLocator
    {
        /// <summary>
        /// Gets the container.
        /// </summary>
        IDictionary<string, IDictionary<Type, object>> Container { get; }
    }

    /// <summary>
    /// MicroServiceLocator
    /// </summary>
    public class MicroServiceLocator : IMicroServiceLocator, ServiceLocatorManager.ISetupRegistration
    {
        private IDictionary<string, IDictionary<Type, object>> _containers;
        private MicroServiceRegistrar _registrar;

        // used so Type is not a reserved value
        internal class Trampoline { public bool AsSingleton; public Type Type; public Func<IServiceLocator, object> Factory; }

        static MicroServiceLocator() { ServiceLocatorManager.EnsureRegistration(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceLocator"/> class.
        /// </summary>
        public MicroServiceLocator()
            : this(new Dictionary<Type, object>()) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceLocator"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public MicroServiceLocator(IDictionary<Type, object> container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            Container = new Dictionary<string, IDictionary<Type, object>> { 
                {string.Empty, container}
            };
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceLocator"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public MicroServiceLocator(IDictionary<string, IDictionary<Type, object>> container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            Container = container;
        }

        Action<IServiceLocator, string> ServiceLocatorManager.ISetupRegistration.DefaultServiceRegistrar
        {
            get { return (locator, name) => ServiceLocatorManager.RegisterInstance<IMicroServiceLocator>(this, locator, name); }
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.
        /// -or-
        /// null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public object GetService(Type serviceType) { return Resolve(serviceType); }

        /// <summary>
        /// Creates the child.
        /// </summary>
        /// <returns></returns>
        public IServiceLocator CreateChild(object tag) { throw new NotSupportedException(); }

        /// <summary>
        /// Gets the underlying container.
        /// </summary>
        /// <typeparam name="TContainer">The type of the container.</typeparam>
        /// <returns></returns>
        public TContainer GetUnderlyingContainer<TContainer>()
            where TContainer : class { return (_containers as TContainer); }

        // registrar
        /// <summary>
        /// Gets the registrar.
        /// </summary>
        public IServiceRegistrar Registrar
        {
            get { return _registrar; }
        }

        // resolve
        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns></returns>
        public TService Resolve<TService>()
            where TService : class { return (TService)Resolve(typeof(TService), string.Empty); }
        /// <summary>
        /// Resolves the specified name.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public TService Resolve<TService>(string name)
            where TService : class { return (TService)Resolve(typeof(TService), name); }
        /// <summary>
        /// Resolves the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public object Resolve(Type serviceType) { return Resolve(serviceType, string.Empty); }
        /// <summary>
        /// Resolves the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public object Resolve(Type serviceType, string name)
        {
            try
            {
                IDictionary<Type, object> container;
                if (!_containers.TryGetValue(name ?? string.Empty, out container))
                    throw new ArgumentOutOfRangeException(string.Format("Could not resolve implementation for [{0}-{1}]", name ?? "+", serviceType.ToString()));
                // if not registered, then use requested type
                object concrete;
                if (!container.TryGetValue(serviceType, out concrete))
                {
                    if (serviceType.IsInterface)
                        throw new ArgumentOutOfRangeException(string.Format("Anonymous registrations for [{0}-{1}] can not be an interface.", name ?? "+", serviceType.ToString()));
                    concrete = new Trampoline { Type = serviceType };
                }
                // register as null for default constructor
                if (concrete == null)
                    return Activator.CreateInstance(serviceType);
                // try factory
                var factory = (concrete as Func<IServiceLocator, object>);
                if (factory != null)
                    return factory(this);
                // try trampoline, then register as factory|singleton
                var trampoline = (concrete as Trampoline);
                if (trampoline != null)
                {
                    var trampolineAsType = trampoline.Type;
                    if (trampolineAsType != null)
                    {
                        if (!trampolineAsType.IsInterface)
                        {
                            var constructorInfo = trampolineAsType.GetConstructors()
                                .FirstOrDefault(constructor => constructor.GetParameters().Length > 0);
                            if (constructorInfo == null)
                                factory = l => Activator.CreateInstance(trampolineAsType);
                            else
                            {
                                var args = constructorInfo.GetParameters()
                                    .Select(arg => Resolve(arg.ParameterType))
                                    .ToArray();
                                factory = l => Activator.CreateInstance(trampolineAsType, args);
                            }
                        }
                        else
                            factory = l => Resolve(trampolineAsType, name);
                    }
                    else if ((factory = trampoline.Factory) == null)
                        throw new InvalidOperationException();
                    concrete = factory(this);
                    container[serviceType] = (!trampoline.AsSingleton ? factory : concrete);
                }
                return concrete;
            }
            catch (Exception ex) { throw new ServiceLocatorResolutionException(serviceType, ex); }
        }
        //
        /// <summary>
        /// Resolves all.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns></returns>
        public IEnumerable<TService> ResolveAll<TService>()
            where TService : class { return ResolveAll(typeof(TService)).Cast<TService>(); }
        /// <summary>
        /// Resolves all.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            var items = _containers.SelectMany(x => x.Value, (a, b) => new { Name = a.Key, Services = b })
                .Where(x => x.Services.Key == serviceType)
                .Select(x => new { ServiceType = x.Services.Key, x.Name })
                .ToList();
            return items
                .Select(x => Resolve(x.ServiceType, x.Name))
                .ToList();
        }

        // inject
        /// <summary>
        /// Injects the specified instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public TService Inject<TService>(TService instance)
            where TService : class { throw new NotSupportedException(); }

        // release and teardown
        /// <summary>
        /// Releases the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void Release(object instance) { }
        /// <summary>
        /// Tears down.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        public void TearDown<TService>(TService instance)
            where TService : class { }
        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset() { }

        #region Domain specific

        /// <summary>
        /// Gets the container.
        /// </summary>
        public IDictionary<string, IDictionary<Type, object>> Container
        {
            get { return _containers; }
            private set
            {
                _containers = value;
                _registrar = new MicroServiceRegistrar(this, value);
            }
        }

        #endregion
    }
}

