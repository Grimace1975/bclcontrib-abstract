#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System;
using System.Linq;
using System.Abstract;
using System.Collections.ObjectModel;
using System.Collections.Generic;
namespace Contoso.Abstract
{
    /// <summary>
    /// IApplicationServiceBus
    /// </summary>
    public interface IApplicationServiceBus : IServiceBus
    {
        IApplicationServiceBus Add<TMessageHandler>()
            where TMessageHandler : class;
        IApplicationServiceBus Add(Type messageHandlerType);
    }

    /// <summary>
    /// ApplicationServiceBus
    /// </summary>
    public class ApplicationServiceBus : Collection<ApplicationServiceBusRegistration>, IApplicationServiceBus
    {
        private readonly IServiceMessageHandlerFactory _messageHandlerFactory;

        public ApplicationServiceBus()
            : this(null) { }
        public ApplicationServiceBus(IServiceMessageHandlerFactory messageHandlerFactory)
        {
            _messageHandlerFactory = messageHandlerFactory;
        }

        public TMessage CreateMessage<TMessage>(Action<TMessage> messageBuilder)
            where TMessage : IServiceMessage { throw new NotImplementedException(); }

        public IApplicationServiceBus Add<TMessageHandler>()
            where TMessageHandler : class { return Add(typeof(TMessageHandler)); }
        public IApplicationServiceBus Add(Type messageHandlerType)
        {
            var messageType = GetMessageTypeFromHandler(messageHandlerType);
            if (messageType == null)
                throw new InvalidOperationException("Unable find a message handler");
            base.Add(new ApplicationServiceBusRegistration
            {
                MessageHandlerType = messageHandlerType,
                MessageType = messageType,
            });
            return this;
        }

        public IServiceBusCallback Send(IServiceBusLocation destination, params IServiceMessage[] messages)
        {
            if (messages == null)
                throw new ArgumentNullException("messages");
            foreach (var message in messages)
                foreach (var type in GetTypesOfMessageHandlers(message.GetType()))
                    HandleTheMessage(type, (IApplicationServiceMessage)message);
            return null;
        }

        private void HandleTheMessage(Type type, IApplicationServiceMessage message)
        {
            _messageHandlerFactory.Create<IApplicationServiceMessage>(type)
                .Handle(message);
        }

        private IEnumerable<Type> GetTypesOfMessageHandlers(Type messageType)
        {
            return Items.Where(x => (x.MessageType == messageType))
                .Select(x => x.MessageHandlerType);
        }

        private static Type GetMessageTypeFromHandler(Type messageHandlerType)
        {
            var serviceMessageType = typeof(IServiceMessage);
            var applicationServiceMessageType = typeof(IApplicationServiceMessage);
            return messageHandlerType.GetInterfaces()
                .Where(h => (h.IsGenericType) && (h.FullName.StartsWith("System.Abstract.IServiceMessageHandler`1") || h.FullName.StartsWith("Contoso.Abstract.IApplicationServiceMessageHandler`1")))
                .Select(h => h.GetGenericArguments()[0])
                .Where(m => m.GetInterfaces().Any(x => (x == serviceMessageType) || (x == applicationServiceMessageType)))
                .SingleOrDefault();
        }
    }
}
