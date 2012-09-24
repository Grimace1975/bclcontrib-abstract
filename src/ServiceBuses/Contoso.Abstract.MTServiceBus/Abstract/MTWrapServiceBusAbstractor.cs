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
using System.Abstract;
using System.Linq;
using MassTransit;
using IServiceBus = MassTransit.IServiceBus;
using ServiceBus = System.Abstract.ServiceBus;
namespace Contoso.Abstract
{
    /// <summary>
    /// MTWrapServiceBusAbstractor
    /// </summary>
    public class MTWrapServiceBusAbstractor : MTServiceBusAbstractor
    {
        static MTWrapServiceBusAbstractor() { ServiceBusManager.EnsureRegistration(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="MTWrapServiceBusAbstractor"/> class.
        /// </summary>
        public MTWrapServiceBusAbstractor()
            : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MTWrapServiceBusAbstractor"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public MTWrapServiceBusAbstractor(IServiceLocator serviceLocator)
            : base(serviceLocator) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MTWrapServiceBusAbstractor"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="bus">The bus.</param>
        public MTWrapServiceBusAbstractor(IServiceLocator serviceLocator, IServiceBus bus)
            : base(serviceLocator, bus) { }

        /// <summary>
        /// Sends the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="messages">The messages.</param>
        /// <returns></returns>
        public override IServiceBusCallback Send(IServiceBusEndpoint endpoint, params object[] messages)
        {
            if (messages == null || messages.Length == 0 || messages[0] == null)
                throw new ArgumentNullException("messages", "Please include at least one message.");
            try
            {
                //if (endpoint == null) Bus.Endpoint.Send(MTServiceBusTransport.Wrap(messages));
                //else if (endpoint != ServiceBus.SelfEndpoint) MTServiceBusTransport.Cast(endpoint).Send(MTServiceBusTransport.Wrap(messages));
                //else Bus.Endpoint.Send(MTServiceBusTransport.Wrap(messages));
            }
            catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
            return null;
        }

        /// <summary>
        /// Replies the specified messages.
        /// </summary>
        /// <param name="messages">The messages.</param>
        public override void Reply(params object[] messages)
        {
            if (messages == null || messages.Length == 0 || messages[0] == null)
                throw new ArgumentNullException("messages", "Please include at least one message.");
            //try { Bus.Reply(MTServiceBusTransport.Wrap(messages)); }
            //catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
        }

        #region Publishing ServiceBus

        /// <summary>
        /// Publishes the specified messages.
        /// </summary>
        /// <param name="messages">The messages.</param>
        public override void Publish(params object[] messages)
        {
            if (messages == null || messages.Length == 0 || messages[0] == null)
                throw new ArgumentNullException("messages");
            //try { Bus.Publish(MTServiceBusTransport.Wrap(messages)); }
            //catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
        }

        /// <summary>
        /// Subscribes the specified message type.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="predicate">The predicate.</param>
        public override void Subscribe(Type messageType, Predicate<object> predicate)
        {
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            try
            {
                //if (predicate == null) Bus.SubscribeConsumer(MTServiceBusTransport.Wrap(messageType));
                //else Bus.SubscribeConsumer(MTServiceBusTransport.Wrap(messageType), predicate);
            }
            catch (Exception ex) { throw new ServiceBusMessageException(messageType, ex); }
        }

        /// <summary>
        /// Unsubscribes the specified message type.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        public override void Unsubscribe(Type messageType)
        {
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            //try { Bus.Unsubscribe(MTServiceBusTransport.Wrap(messageType)); }
            //catch (Exception ex) { throw new ServiceBusMessageException(MTServiceBusTransport.Wrap(messageType), ex); }
        }

        #endregion
    }
}
