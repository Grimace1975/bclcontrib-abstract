using System;
using System.Abstract;
using System.Collections.Generic;
using NServiceBus.ObjectBuilder.Common;
using NServiceBus.ObjectBuilder;
using NServiceBus;
using System.Linq;
namespace Contoso.Abstract.NServiceBus
{
    internal class ServiceLocatorObjectBuilder : IContainer, IDisposable
    {
        private readonly IServiceLocator _locator;
        private readonly IServiceRegistrar _registrar;

        public ServiceLocatorObjectBuilder(IServiceLocator locator)
        {
            _locator = locator;
            _registrar = locator.Registrar;
        }

        public void Dispose() { }

        public object Build(Type typeToBuild) { return _locator.Resolve(typeToBuild); }
        public IEnumerable<object> BuildAll(Type typeToBuild) { return _locator.ResolveAll(typeToBuild); }

#if !CLR4
        public void Configure(Type component, ComponentCallModelEnum callModel)
        {
            if (!_registrar.HasRegistered(component))
                _registrar.BehaveAs(GetLifetime(callModel)).Register(component, component);
        }

        private static ServiceRegistrarLifetime GetLifetime(ComponentCallModelEnum callModel)
        {
            switch (callModel)
            {
                case ComponentCallModelEnum.Singleton: return ServiceRegistrarLifetime.Singleton;
                case ComponentCallModelEnum.Singlecall: return ServiceRegistrarLifetime.Transient;
                default: return ServiceRegistrarLifetime.Transient;
            }
        }
#else
        public IContainer BuildChildContainer() { return new ServiceLocatorObjectBuilder(_locator.CreateChild(null)); }

        public void Configure<T>(Func<T> component, DependencyLifecycle dependencyLifecycle)
        {
            if (!_registrar.HasRegistered<T>())
                _registrar.BehaveAs(GetLifetime(dependencyLifecycle)).Register(typeof(T), t => component());
        }

        public void Configure(Type component, DependencyLifecycle dependencyLifecycle)
        {
            if (!_registrar.HasRegistered(component))
            {
                var lifetimeRegistrar = _registrar.BehaveAs(GetLifetime(dependencyLifecycle));
                foreach (Type type in GetAllServiceTypesFor(component))
                    lifetimeRegistrar.Register(component, type);
            }
        }

        private static IEnumerable<Type> GetAllServiceTypesFor(Type t)
        {
            if (t == null)
                return Enumerable.Empty<Type>();
            return t.GetInterfaces()
                .Where(x => x.FullName != null && !x.FullName.StartsWith("System."));
        }

        private static ServiceRegistrarLifetime GetLifetime(DependencyLifecycle dependencyLifecycle)
        {
            switch (dependencyLifecycle)
            {
                case DependencyLifecycle.SingleInstance: return ServiceRegistrarLifetime.Singleton;
                //case DependencyLifecycle.InstancePerUnitOfWork: ServiceRegistrarLifetime.HierarchicalLifetimeManager;
                case DependencyLifecycle.InstancePerCall: return ServiceRegistrarLifetime.Transient;
                default: throw new ArgumentException("Unhandled lifecycle - " + dependencyLifecycle);
            }
        }

        public bool HasComponent(Type componentType) { return _registrar.HasRegistered(componentType); }
#endif

        public void ConfigureProperty(Type component, string property, object value) { throw new NotImplementedException(); }
        public void RegisterSingleton(Type lookupType, object instance) { _registrar.RegisterInstance(lookupType, instance); }
    }
}