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
    /// IAppServiceBus
    /// </summary>
    public interface IAppServiceBus : IServiceBus
    {
        IAppServiceBus Add<TMessageHandler>()
            where TMessageHandler : class;
        IAppServiceBus Add(Type messageHandlerType);
    }

    /// <summary>
    /// AppServiceBus
    /// </summary>
    public class AppServiceBus : Collection<AppServiceBusRegistration>, IAppServiceBus, ServiceBusManager.ISetupRegistration
    {
        private readonly IServiceMessageHandlerFactory _messageHandlerFactory;

        static AppServiceBus() { ServiceBusManager.EnsureRegistration(); }
        public AppServiceBus()
            : this(null) { }
        public AppServiceBus(IServiceMessageHandlerFactory messageHandlerFactory)
        {
            _messageHandlerFactory = messageHandlerFactory;
        }

        Action<IServiceLocator, string> ServiceBusManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (locator, name) => ServiceBusManager.RegisterInstance<IAppServiceBus>(this, locator, name); }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        public TMessage CreateMessage<TMessage>(Action<TMessage> messageBuilder)
            where TMessage : class { throw new NotImplementedException(); }

        public IAppServiceBus Add<TMessageHandler>()
            where TMessageHandler : class { return Add(typeof(TMessageHandler)); }
        public IAppServiceBus Add(Type messageHandlerType)
        {
            var messageType = GetMessageTypeFromHandler(messageHandlerType);
            if (messageType == null)
                throw new InvalidOperationException("Unable find a message handler");
            base.Add(new AppServiceBusRegistration
            {
                MessageHandlerType = messageHandlerType,
                MessageType = messageType,
            });
            return this;
        }

        public IServiceBusCallback Send(IServiceBusEndpoint destination, params object[] messages)
        {
            if (messages == null)
                throw new ArgumentNullException("messages");
            foreach (var message in messages)
                foreach (var type in GetTypesOfMessageHandlers(message.GetType()))
                    HandleTheMessage(type, message);
            return null;
        }

        private void HandleTheMessage(Type type, object message)
        {
            _messageHandlerFactory.Create<object>(type)
                .Handle(message);
        }

        private IEnumerable<Type> GetTypesOfMessageHandlers(Type messageType)
        {
            return Items.Where(x => x.MessageType == messageType)
                .Select(x => x.MessageHandlerType);
        }

        private static Type GetMessageTypeFromHandler(Type messageHandlerType)
        {
            return null;
            //var serviceMessageType = typeof(IServiceMessage);
            //var applicationServiceMessageType = typeof(IApplicationServiceMessage);
            //return messageHandlerType.GetInterfaces()
            //    .Where(h => h.IsGenericType && (h.FullName.StartsWith("System.Abstract.IServiceMessageHandler`1") || h.FullName.StartsWith("Contoso.Abstract.IApplicationServiceMessageHandler`1")))
            //    .Select(h => h.GetGenericArguments()[0])
            //    .Where(m => m.GetInterfaces().Any(x => x == serviceMessageType || x == applicationServiceMessageType))
            //    .SingleOrDefault();
        }

        public void Reply(params object[] messages) { throw new NotImplementedException(); }
        public void Return<T>(T value) { throw new NotImplementedException(); }
    }
}