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
    /// ServiceLocatorConsumerFactoryConfigurator
    /// </summary>
    public class ServiceLocatorConsumerFactoryConfigurator
    {
        private readonly SubscriptionBusServiceConfigurator _configurator;
        private readonly IServiceLocator _locator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorConsumerFactoryConfigurator"/> class.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="locator">The locator.</param>
        public ServiceLocatorConsumerFactoryConfigurator(SubscriptionBusServiceConfigurator configurator, IServiceLocator locator)
        {
            _configurator = configurator;
            _locator = locator;
        }

        /// <summary>
        /// Configures the consumer.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        public void ConfigureConsumer(Type messageType)
        {
            this.FastInvoke(new[] { messageType }, "Configure");
        }

        /// <summary>
        /// Configures this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [UsedImplicitly]
        public void Configure<T>()
            where T : class, IConsumer { _configurator.Consumer(new ServiceLocatorConsumerFactory<T>(_locator)); }
    }
}
