using System;
using System.Abstract;
using System.Collections.Generic;
using System.Linq;
using Trampoline = Contoso.Abstract.MicroServiceLocator.Trampoline;
namespace Contoso.Abstract
{
    /// <summary>
    /// IMicroServiceRegistrar
    /// </summary>
    public interface IMicroServiceRegistrar : IServiceRegistrar { }

    /// <summary>
    /// MicroServiceRegistrar
    /// </summary>
    internal class MicroServiceRegistrar : IMicroServiceRegistrar, ICloneable, IServiceRegistrarBehaviorAccessor
    {
        private MicroServiceLocator _parent;
        private IDictionary<string, IDictionary<Type, object>> _containers;
        //private IList<IServiceLocatorInterceptor> _interceptors;

        public MicroServiceRegistrar(MicroServiceLocator parent, IDictionary<string, IDictionary<Type, object>> container)
        {
            _parent = parent;
            _containers = container;
            LifetimeForRegisters = ServiceRegistrarLifetime.Transient;
        }

        object ICloneable.Clone() { return MemberwiseClone(); }

        // locator
        public IServiceLocator Locator
        {
            get { return _parent; }
        }

        // enumerate
        public bool HasRegistered<TService>() { return HasRegistered(typeof(TService)); }
        public bool HasRegistered(Type serviceType) { return _containers.Any(x => x.Value.Any(y => y.Key == serviceType)); }
        public IEnumerable<ServiceRegistration> GetRegistrationsFor(Type serviceType)
        {
            return _containers.SelectMany(x => x.Value, (a, b) => new { Name = a.Key, Services = b })
                .Where(x => serviceType.IsAssignableFrom(x.Services.Key))
                .Select(x => new ServiceRegistration { ServiceType = x.Services.Key, ImplementationType = x.Services.Value.GetType(), Name = x.Name });
        }
        public IEnumerable<ServiceRegistration> Registrations
        {
            get
            {
                return _containers.SelectMany(x => x.Value, (a, b) => new { Name = a.Key, Services = b })
                    .Select(x => new ServiceRegistration { ServiceType = x.Services.Key, ImplementationType = x.Services.Value.GetType(), Name = x.Name });
            }
        }

        // register type
        public ServiceRegistrarLifetime LifetimeForRegisters { get; private set; }
        public void Register(Type serviceType) { RegisterInternal(serviceType, SetLifestyle(new Trampoline { Type = serviceType }), string.Empty); }
        public void Register(Type serviceType, string name) { RegisterInternal(serviceType, SetLifestyle(new Trampoline { Type = serviceType }), name); }


        // register implementation
        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService { RegisterInternal(typeof(TService), SetLifestyle(new Trampoline { Type = typeof(TImplementation) }), string.Empty); }
        public void Register<TService, TImplementation>(string name)
            where TService : class
            where TImplementation : class, TService { RegisterInternal(typeof(TService), SetLifestyle(new Trampoline { Type = typeof(TImplementation) }), name); }
        public void Register<TService>(Type implementationType)
           where TService : class { RegisterInternal(typeof(TService), SetLifestyle(new Trampoline { Type = implementationType }), string.Empty); }
        public void Register<TService>(Type implementationType, string name)
           where TService : class { RegisterInternal(typeof(TService), SetLifestyle(new Trampoline { Type = implementationType }), name); }
        public void Register(Type serviceType, Type implementationType) { RegisterInternal(serviceType, SetLifestyle(new Trampoline { Type = implementationType }), string.Empty); }
        public void Register(Type serviceType, Type implementationType, string name) { RegisterInternal(serviceType, SetLifestyle(new Trampoline { Type = implementationType }), name); }


        // register instance
        public void RegisterInstance<TService>(TService instance)
            where TService : class { EnsureTransientLifestyle(); RegisterInternal(typeof(TService), instance, string.Empty); }
        public void RegisterInstance<TService>(TService instance, string name)
            where TService : class { EnsureTransientLifestyle(); RegisterInternal(typeof(TService), instance, name); }
        public void RegisterInstance(Type serviceType, object instance) { EnsureTransientLifestyle(); RegisterInternal(serviceType, instance, string.Empty); }
        public void RegisterInstance(Type serviceType, object instance, string name) { EnsureTransientLifestyle(); RegisterInternal(serviceType, instance, name); }

        // register method
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { RegisterInternal(typeof(TService), SetLifestyle(new Trampoline { Factory = l => factoryMethod(l) }), string.Empty); }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod, string name)
            where TService : class { RegisterInternal(typeof(TService), SetLifestyle(new Trampoline { Factory = l => factoryMethod(l) }), name); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod) { RegisterInternal(serviceType, SetLifestyle(new Trampoline { Factory = l => factoryMethod(l) }), string.Empty); }
        public void Register(Type serviceType, Func<IServiceLocator, object> factoryMethod, string name) { RegisterInternal(serviceType, SetLifestyle(new Trampoline { Factory = l => factoryMethod(l) }), name); }

        // interceptor
        public void RegisterInterceptor(IServiceLocatorInterceptor interceptor)
        {
            throw new NotSupportedException();
            //if (_interceptors == null)
            //    _interceptors = new List<IServiceLocatorInterceptor>();
            //_interceptors.Add(interceptor);
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

        private void RegisterInternal(Type serviceType, object concrete, string name)
        {
            IDictionary<Type, object> container;
            if (!_containers.TryGetValue(name ?? string.Empty, out container))
                _containers[name ?? string.Empty] = container = new Dictionary<Type, object>();
            container[serviceType] = concrete;
        }

        private void EnsureTransientLifestyle()
        {
            if (LifetimeForRegisters != ServiceRegistrarLifetime.Transient)
                throw new NotSupportedException();
        }

        private Trampoline SetLifestyle(Trampoline t)
        {
            // must cast to IServiceRegistrar for behavior wrappers
            switch (LifetimeForRegisters)
            {
                case ServiceRegistrarLifetime.Transient: break; // b.InstancePerDependency();
                case ServiceRegistrarLifetime.Singleton: t.AsSingleton = true; break;
                default: throw new NotSupportedException();
            }
            return t;
        }
    }
}

