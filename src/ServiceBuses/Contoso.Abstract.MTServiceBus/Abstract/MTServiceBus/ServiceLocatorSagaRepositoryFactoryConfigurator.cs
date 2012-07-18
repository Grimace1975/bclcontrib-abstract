using System;
using System.Abstract;
using Magnum.Reflection;
using MassTransit;
using MassTransit.SubscriptionConfigurators;
using MassTransit.Util;
using MassTransit.Saga;
namespace Contoso.Abstract.MTServiceBus
{
    /// <summary>
    /// ServiceLocatorSagaFactoryConfigurator
    /// </summary>
    public class ServiceLocatorSagaFactoryConfigurator
    {
        private readonly SubscriptionBusServiceConfigurator _configurator;
        private readonly IServiceLocator _locator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorSagaFactoryConfigurator"/> class.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="locator">The locator.</param>
        public ServiceLocatorSagaFactoryConfigurator(SubscriptionBusServiceConfigurator configurator, IServiceLocator locator)
        {
            _configurator = configurator;
            _locator = locator;
        }

        /// <summary>
        /// Configures the saga.
        /// </summary>
        /// <param name="sagaType">Type of the saga.</param>
        public void ConfigureSaga(Type sagaType)
        {
            this.FastInvoke(new[] { sagaType }, "Configure");
        }

        /// <summary>
        /// Configures this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [UsedImplicitly]
        public void Configure<T>()
            where T : class, ISaga
        {
            _configurator.Saga<T>(_locator.Resolve<ISagaRepository<T>>());
        }
    }
}
