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
using NServiceBus;
using System.Linq.Expressions;
using NServiceBus.Unicast;
using Contoso.Abstract.Internal;
namespace Contoso.Abstract
{
    /// <summary>
    /// NServiceBusAbstractor
    /// </summary>
    public class NServiceBusAbstractor : INServiceBus
    {
        private static readonly Type s_domainServiceMessageType = typeof(INServiceMessage);
        private IBus _bus;

        public NServiceBusAbstractor() { }
        public NServiceBusAbstractor(IStartableBus bus)
        {
            if (bus == null)
                throw new ArgumentNullException("bus", "The specified NServiceBus bus cannot be null.");
            Bus = ApplyRequiredBusDependencies(bus).Start();
        }
        public NServiceBusAbstractor(IBus bus)
        {
            if (bus == null)
                throw new ArgumentNullException("bus", "The specified NServiceBus bus cannot be null.");
            Bus = bus;
        }

        public TMessage CreateMessage<TMessage>(Action<TMessage> messageBuilder)
            where TMessage : IServiceMessage
        {
            var message = (TMessage)Bus.CreateInstance(typeof(TMessage));
            if (messageBuilder != null)
                messageBuilder(message);
            return message;
        }

        public IServiceBusCallback Send(IServiceBusLocation location, params IServiceMessage[] messages)
        {
            if (location == null)
                throw new ArgumentNullException("location");
            if ((messages == null) || (messages.Length == 0))
                throw new ArgumentNullException("messages");
            var firstMessage = messages[0];
            if (firstMessage == null)
                throw new ArgumentNullException("messages[0]");
            var transportMessages = MessageCaster.Wrap(messages);
            try
            {
                var locationAsText = location.ToString((x, a) => null, firstMessage);
                if (locationAsText != null)
                    return MessageCaster.Cast(Bus.Send(locationAsText, transportMessages));
                Bus.SendLocal(transportMessages);
                return null;
            }
            catch (Exception ex) { throw new ServiceBusMessageException(firstMessage.GetType(), ex); }
        }

        #region Publishing ServiceBus

        public void Publish(params IServiceMessage[] messages)
        {
            if ((messages == null) || (messages.Length == 0))
                throw new ArgumentNullException("messages");
            var firstMessage = messages[0];
            if (firstMessage == null)
                throw new ArgumentNullException("messages[0]");
            //try { MessageCaster<TMessage>.Publish(Bus, messages); }
            //catch (Exception ex) { throw new ServiceBusMessageException(firstMessage.GetType(), ex); }
        }

        public void Subscribe(Type messageType, Predicate<IServiceMessage> predicate)
        {
            try { Bus.Subscribe(messageType, MessageCaster.Cast(predicate)); }
            catch (Exception ex) { throw new ServiceBusMessageException(messageType, ex); }
        }

        public void Unsubscribe(Type messageType)
        {
            //try { Bus.Unsubscribe(MessageCaster.Cast(messageType)); }
            //catch (Exception ex) { throw new ServiceBusMessageException(messageType, ex); }
        }
        #endregion

        #region Domain-specific

        public IBus Bus
        {
            get
            {
                if (_bus != null)
                    return _bus;
                return (Bus = Configure.Instance.Builder.Build<IBus>());
            }
            private set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "Need to start bus first");
                if (!HasRequiredBusDependencies(value))
                    throw new InvalidOperationException("Required Required Dependencies not met");
                _bus = value;
            }
        }

        public void Reply(params IServiceMessage[] messages)
        {
            //try { Bus.Reply(MessageCaster.Cast(messages)); }
            //catch (Exception ex) { throw new ServiceBusException(ex); }
        }

        public void Return<T>(T value)
        {
            if (typeof(T) != typeof(int))
                throw new NotSupportedException();
            try { Bus.Return(Convert.ToInt32(value)); }
            catch (Exception ex) { throw new ServiceBusMessageException(null, ex); }
        }

        #endregion

        #region RequiredBusDependencies

        public static IStartableBus ApplyRequiredBusDependencies(IStartableBus bus)
        {
            return bus;
        }

        private static bool HasRequiredBusDependencies(IBus bus)
        {
            return true;
        }

        #endregion
    }
}
