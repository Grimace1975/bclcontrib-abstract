using System.Abstract;
using Rhino.ServiceBus.Hosting;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Actions;
using Rhino.ServiceBus.Internal;
using System.Reflection;
using System;
using System.Linq;
namespace Contoso.Abstract.RhinoServiceBus
{
    public abstract class ServiceLocatorBootStrapper : AbstractBootStrapper
    {
        private System.Abstract.IServiceLocator _locator;

        protected ServiceLocatorBootStrapper() { }
        protected ServiceLocatorBootStrapper(System.Abstract.IServiceLocator locator)
        {
            _locator = locator;
        }

        protected override void ConfigureBusFacility(AbstractRhinoServiceBusConfiguration configuration)
        {
            configuration.UseAbstractServiceLocator(_locator);
            base.ConfigureBusFacility(configuration);
        }

        private void ConfigureConsumers(Assembly assemblyToScan)
        {
            var types = assemblyToScan.GetTypes()
                .Where(type => typeof(IMessageConsumer).IsAssignableFrom(type) &&
                    !typeof(IOccasionalMessageConsumer).IsAssignableFrom(type) &&
                    IsTypeAcceptableForThisBootStrapper(type));
            foreach (var type in types)
                ConfigureConsumer(type);
        }

        protected virtual void ConfigureConsumer(Type type)
        {
            _locator.Registrar.Register<IMessageConsumer>(type, type.FullName);
        }

        protected virtual void ConfigureContainer()
        {
            var registrar = _locator.Registrar;
            registrar.RegisterByTypeMatch<IDeploymentAction>(null, Assembly);
            registrar.RegisterByTypeMatch<IEnvironmentValidationAction>(null, Assembly);
            ConfigureConsumers(typeof(Rhino.ServiceBus.IServiceBus).Assembly);
            ConfigureConsumers(Assembly);
        }

        public override void CreateContainer()
        {
            if (_locator == null)
                _locator = ServiceLocatorManager.Current;
            ConfigureContainer();
        }

        public override void Dispose()
        {
            var disposable = (_locator as IDisposable);
            if (disposable != null)
                disposable.Dispose();
        }

        public override void ExecuteDeploymentActions(string user)
        {
            foreach (var action in _locator.ResolveAll<IDeploymentAction>())
                action.Execute(user);
        }

        public override void ExecuteEnvironmentValidationActions()
        {
            foreach (var action in _locator.ResolveAll<IEnvironmentValidationAction>())
                action.Execute();
        }

        public override T GetInstance<T>()
        {
            return (T)_locator.Resolve(typeof(T));
        }

        protected System.Abstract.IServiceLocator Container
        {
            get { return _locator; }
        }
    }
}