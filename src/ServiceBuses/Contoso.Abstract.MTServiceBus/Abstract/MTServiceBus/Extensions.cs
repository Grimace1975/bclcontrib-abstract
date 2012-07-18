using System;
using System.Linq;
using System.Abstract;
using System.Collections.Generic;
using Magnum.Extensions;
using MassTransit;
using MassTransit.SubscriptionConfigurators;
using MassTransit.Saga;
using MassTransit.Saga.SubscriptionConfigurators;
namespace Contoso.Abstract.MTServiceBus
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Consumers the specified configurator.
        /// </summary>
        /// <typeparam name="TConsumer">The type of the consumer.</typeparam>
        /// <param name="configurator">The configurator.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(this SubscriptionBusServiceConfigurator configurator, IServiceLocator locator)
            where TConsumer : class, IConsumer
        {
            var consumerFactory = new ServiceLocatorConsumerFactory<TConsumer>(locator);
            return configurator.Consumer<TConsumer>(consumerFactory);
        }

        private static IList<Type> FindTypes<T>(IServiceLocator locator, Func<Type, bool> filter)
        {
            return locator.Registrar.GetRegistrationsFor(typeof(T)).Select(x => x.ServiceType).Where(filter).ToList();
        }

        /// <summary>
        /// Loads from.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="locator">The locator.</param>
        public static void LoadFrom(this SubscriptionBusServiceConfigurator configurator, IServiceLocator locator)
        {
            var concreteTypes = FindTypes<IConsumer>(locator, x => !x.Implements<ISaga>());
            if (concreteTypes.Count > 0)
            {
                var consumerConfigurator = new ServiceLocatorConsumerFactoryConfigurator(configurator, locator);
                foreach (Type concreteType in concreteTypes)
                    consumerConfigurator.ConfigureConsumer(concreteType);
            }
            var sagaTypes = FindTypes<ISaga>(locator, x => true);
            if (sagaTypes.Count > 0)
            {
                var sagaConfigurator = new ServiceLocatorSagaFactoryConfigurator(configurator, locator);
                foreach (Type type in sagaTypes)
                    sagaConfigurator.ConfigureSaga(type);
            }
        }

        /// <summary>
        /// Sagas the specified configurator.
        /// </summary>
        /// <typeparam name="TSaga">The type of the saga.</typeparam>
        /// <param name="configurator">The configurator.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static SagaSubscriptionConfigurator<TSaga> Saga<TSaga>(this SubscriptionBusServiceConfigurator configurator, IServiceLocator locator)
            where TSaga : class, ISaga
        {
            return configurator.Saga<TSaga>(locator.Resolve<ISagaRepository<TSaga>>());
        }
    }
}