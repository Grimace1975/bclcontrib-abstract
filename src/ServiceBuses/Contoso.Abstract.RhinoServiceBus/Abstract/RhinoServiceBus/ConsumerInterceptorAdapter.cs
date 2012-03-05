using System;
using System.Abstract;
using Rhino.ServiceBus.Config;
using Rhino.ServiceBus.Internal;
namespace Contoso.Abstract.RhinoServiceBus
{
    internal class ConsumerInterceptorAdapter : IServiceLocatorInterceptor
    {
        private IConsumerInterceptor _interceptor;

        public ConsumerInterceptorAdapter(IConsumerInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        public void ItemCreated(Type createdItem, bool isTransient) { _interceptor.ItemCreated(createdItem, isTransient); }
        public bool Match(Type type) { return typeof(IMessageConsumer).IsAssignableFrom(type); }
    }
}