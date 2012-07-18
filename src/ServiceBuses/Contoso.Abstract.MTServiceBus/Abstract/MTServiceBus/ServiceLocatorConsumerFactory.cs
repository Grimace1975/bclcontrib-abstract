using System;
using System.Abstract;
using System.Collections.Generic;
using MassTransit;
using MassTransit.Exceptions;
using MassTransit.Pipeline;
namespace Contoso.Abstract.MTServiceBus
{
    /// <summary>
    /// ServiceLocatorConsumerFactory
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceLocatorConsumerFactory<T> : IConsumerFactory<T>
        where T : class
    {
        private readonly IServiceLocator _locator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorConsumerFactory&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="locator">The locator.</param>
        public ServiceLocatorConsumerFactory(IServiceLocator locator)
        {
            _locator = locator;
        }

        /// <summary>
        /// Gets the consumer.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public IEnumerable<Action<IConsumeContext<TMessage>>> GetConsumer<TMessage>(IConsumeContext<TMessage> context, InstanceHandlerSelector<T, TMessage> selector)
            where TMessage : class
        {
            var consumer = _locator.Resolve<T>();
            if (consumer == null)
                throw new ConfigurationException(string.Format("Unable to resolve type '{0}' from container: ", typeof(T)));
            foreach (var handler in selector(consumer, context))
                yield return handler;
        }
    }
}
