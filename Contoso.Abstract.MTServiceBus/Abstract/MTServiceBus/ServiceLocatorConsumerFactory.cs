using System;
using System.Abstract;
using System.Collections.Generic;
using MassTransit;
using MassTransit.Exceptions;
using MassTransit.Pipeline;
namespace Contoso.Abstract.MTServiceBus
{
    public class ServiceLocatorConsumerFactory<T> : IConsumerFactory<T>
        where T : class
    {
        private readonly IServiceLocator _locator;

        public ServiceLocatorConsumerFactory(IServiceLocator locator)
        {
            _locator = locator;
        }

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
