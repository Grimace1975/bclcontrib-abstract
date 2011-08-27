using System.Abstract;
using Rhino.ServiceBus.Hosting;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Actions;
using System.Reflection;
using System;
using System.Linq;
using Rhino.ServiceBus.Internal;
namespace Contoso.Abstract
{
    public class ServiceBusBootStrapper : AbstractBootStrapper
    {
        private System.Abstract.IServiceLocator _locator;

        protected ServiceBusBootStrapper() { }
        protected ServiceBusBootStrapper(System.Abstract.IServiceLocator locator)
        {
            _locator = locator;
        }

        protected override void ConfigureBusFacility(AbstractRhinoServiceBusConfiguration configuration)
        {
            configuration.UseServiceLocator(_locator);
        }

        protected virtual void ConfigureConsumer(Type type)
        {
            _locator.Registrar.Register<IMessageConsumer>(type, type.FullName);
        }

        protected virtual void ConfigureContainer()
        {
            //_locator.RegisterTypesFromAssembly<IDeploymentAction>(Assembly).FirstOrDefault();
            //_locator.RegisterTypesFromAssembly<IEnvironmentValidationAction>(Assembly).FirstOrDefault();
            RegisterConsumersFrom(typeof(Rhino.ServiceBus.IServiceBus).Assembly);
            RegisterConsumersFrom(Assembly);
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

        public override T GetInstance<T>() { return (T)_locator.Resolve(typeof(T)); }

        private void RegisterConsumersFrom(Assembly assemblyToScan)
        {
            var types = assemblyToScan.GetTypes()
                .Where(type => typeof(IMessageConsumer).IsAssignableFrom(type) &&
                    !typeof(IOccasionalMessageConsumer).IsAssignableFrom(type) &&
                    IsTypeAcceptableForThisBootStrapper(type));
            foreach (var type in types)
                ConfigureConsumer(type);
        }

        protected System.Abstract.IServiceLocator Container
        {
            get { return _locator; }
        }
    }
}