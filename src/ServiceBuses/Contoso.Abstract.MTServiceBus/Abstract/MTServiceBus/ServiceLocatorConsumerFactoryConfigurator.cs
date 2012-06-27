using System;
using System.Abstract;
using System.Collections.Generic;
using Magnum.Reflection;
using MassTransit;
using MassTransit.SubscriptionConfigurators;
using MassTransit.Util;
namespace Contoso.Abstract.MTServiceBus
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceLocatorConsumerFactoryConfigurator
    {
        private readonly SubscriptionBusServiceConfigurator _configurator;
        private readonly IServiceLocator _locator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="locator"></param>
        public ServiceLocatorConsumerFactoryConfigurator(SubscriptionBusServiceConfigurator configurator, IServiceLocator locator)
        {
            _configurator = configurator;
            _locator = locator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageType"></param>
        public void ConfigureConsumer(Type messageType)
        {
            this.FastInvoke(new[] { messageType }, "Configure");
        }

        [UsedImplicitly]
        public void Configure<T>()
            where T : class, IConsumer { _configurator.Consumer(new ServiceLocatorConsumerFactory<T>(_locator)); }
    }
}
