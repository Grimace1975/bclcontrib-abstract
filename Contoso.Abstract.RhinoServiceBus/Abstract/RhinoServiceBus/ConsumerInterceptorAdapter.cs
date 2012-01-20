using System;
using System.Linq;
using System.Abstract;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Config;

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