using System;
using System.Abstract;
using System.Collections.Generic;
using Magnum.Reflection;
using MassTransit;
using MassTransit.SubscriptionConfigurators;
using MassTransit.Util;
namespace Contoso.Abstract.MTServiceBus
{
    public class ServiceLocatorConsumerFactoryConfigurator
    {
        private readonly SubscriptionBusServiceConfigurator _configurator;
        private readonly IServiceLocator _locator;

        public ServiceLocatorConsumerFactoryConfigurator(SubscriptionBusServiceConfigurator configurator, IServiceLocator locator)
        {
            _configurator = configurator;
            _locator = locator;
        }

        public void ConfigureConsumer(Type messageType)
        {
            this.FastInvoke(new[] { messageType }, "Configure");
        }

        [UsedImplicitly]
        public void Configure<T>()
            where T : class { _configurator.Consumer(new ServiceLocatorConsumerFactory<T>(_locator)); }
    }
}
