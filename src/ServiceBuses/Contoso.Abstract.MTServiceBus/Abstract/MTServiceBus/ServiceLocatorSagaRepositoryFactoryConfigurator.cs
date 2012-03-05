using System;
using System.Abstract;
using Magnum.Reflection;
using MassTransit;
using MassTransit.SubscriptionConfigurators;
using MassTransit.Util;
using MassTransit.Saga;
namespace Contoso.Abstract.MTServiceBus
{
    public class ServiceLocatorSagaFactoryConfigurator
    {
        private readonly SubscriptionBusServiceConfigurator _configurator;
        private readonly IServiceLocator _locator;

        public ServiceLocatorSagaFactoryConfigurator(SubscriptionBusServiceConfigurator configurator, IServiceLocator locator)
        {
            _configurator = configurator;
            _locator = locator;
        }

        public void ConfigureSaga(Type sagaType)
        {
            this.FastInvoke(new[] { sagaType }, "Configure");
        }

        [UsedImplicitly]
        public void Configure<T>()
            where T : class, ISaga
        {
            _configurator.Saga<T>(_locator.Resolve<ISagaRepository<T>>());
        }
    }
}
