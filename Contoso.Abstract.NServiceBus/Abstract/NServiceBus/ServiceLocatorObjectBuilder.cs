using System;
using System.Abstract;
using System.Collections.Generic;
using NServiceBus.ObjectBuilder.Common;
using NServiceBus.ObjectBuilder;
namespace Contoso.Abstract.NServiceBus
{
    internal class ServiceLocatorObjectBuilder : IContainer
    {
        private readonly IServiceLocator _locator;
        private readonly IServiceRegistrar _registrar;

        public ServiceLocatorObjectBuilder(IServiceLocator locator)
        {
            _locator = locator;
            _registrar = locator.Registrar;
        }

        public object Build(Type typeToBuild)
        {
            return _locator.Resolve(typeToBuild);
        }

        public IEnumerable<object> BuildAll(Type typeToBuild)
        {
            return _locator.ResolveAll(typeToBuild);
        }

        public void Configure(Type component, ComponentCallModelEnum callModel)
        {
            var v = _registrar.LifetimeForRegisters;
            _registrar.LifetimeForRegisters = GetLifetime(callModel);
            _registrar.Register(component, component);
            _registrar.LifetimeForRegisters = v;
        }

        public void ConfigureProperty(Type component, string property, object value)
        {
            throw new NotImplementedException();
        }

        public void RegisterSingleton(Type lookupType, object instance)
        {
            _registrar.RegisterInstance(lookupType, instance);
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
    }
}