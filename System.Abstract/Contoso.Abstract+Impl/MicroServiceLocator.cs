using System;
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
        IDictionary<Type, Type> Container { get; }
    }

    /// <summary>
    /// MicroServiceLocator
    /// </summary>
    public class MicroServiceLocator : IMicroServiceLocator
    {
        private IDictionary<Type, Type> _container;
        private MicroServiceRegistrar _registrar;

        public MicroServiceLocator()
            : this(new Dictionary<Type, Type>()) { }
        public MicroServiceLocator(IDictionary<Type, Type> container)
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
            where TService : class { return (TService)Resolve(typeof(TService)); }
        public TService Resolve<TService>(string id)
            where TService : class { throw new NotSupportedException(); }
        public object Resolve(Type serviceType)
        {
            try
            {
                Type concreteType;
                if (!_container.TryGetValue(serviceType, out concreteType))
                    throw new ArgumentOutOfRangeException(string.Format("Could not resolve implementation for [{0}]", serviceType.ToString()));
                //
                var constructors = concreteType.GetConstructors();
                ConstructorInfo constructorInformation = null;
                int maxParameters = 0;
                foreach (var constructor in constructors)
                    if (constructor.GetParameters().Length > maxParameters)
                        constructorInformation = constructor;
                if (constructorInformation == null)
                    return Activator.CreateInstance(concreteType);
                var parameters = new List<object>();
                foreach (var parameter in constructorInformation.GetParameters())
                    parameters.Add(Resolve(parameter.ParameterType));
                return Activator.CreateInstance(concreteType, parameters.ToArray());
            }
            catch (Exception ex) { throw new ServiceResolutionException(serviceType, ex); }
        }
        public IEnumerable<object> ResolveAll(Type serviceType) { throw new NotSupportedException(); }
        public IEnumerable<TService> ResolveAll<TService>()
            where TService : class { throw new NotSupportedException(); }

        // inject
        public TService Inject<TService>(TService instance)
            where TService : class { throw new NotSupportedException(); }

        // release and teardown
        public void Release(object instance) { }
        public void TearDown<TService>(TService instance)
            where TService : class { }
        public void Reset() { }

        #region Domain specific

        public IDictionary<Type, Type> Container
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

