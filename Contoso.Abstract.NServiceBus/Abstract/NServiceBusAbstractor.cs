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
namespace Contoso.Abstract
{
    /// <summary>
    /// INServiceBus
    /// </summary>
    public interface INServiceBus : IPublishingServiceBus
    {
        void Return<T>(T value);
        IBus Bus { get; }
    }

    /// <summary>
    /// NServiceBusAbstractor
    /// </summary>
    public class NServiceBusAbstractor : INServiceBus, ServiceBusManager.ISetupRegistration
    {
        private IServiceLocator _serviceLocator;

        static NServiceBusAbstractor() { ServiceBusManager.EnsureRegistration(); }
        public NServiceBusAbstractor() { }
        public NServiceBusAbstractor(IServiceLocator serviceLocator)
            : this(serviceLocator, DefaultBusCreator(serviceLocator)) { }
        public NServiceBusAbstractor(IServiceLocator serviceLocator, IStartableBus startableBus)
        {
            if (serviceLocator == null)
                throw new ArgumentNullException("serviceLocator");
            if (startableBus == null)
                throw new ArgumentNullException("startableBus", "The specified NServiceBus bus cannot be null.");
            _serviceLocator = serviceLocator;
            Bus = startableBus.Start();
        }
        public NServiceBusAbstractor(IServiceLocator serviceLocator, IBus bus)
        {
            if (serviceLocator == null)
                throw new ArgumentNullException("serviceLocator");
            if (bus == null)
                throw new ArgumentNullException("bus", "The specified NServiceBus bus cannot be null.");
            _serviceLocator = serviceLocator;
            Bus = bus;
        }

        Action<IServiceLocator, string> ServiceBusManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (locator, name) => ServiceBusManager.RegisterInstance<INServiceBus>(this, locator, name); }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        public TMessage CreateMessage<TMessage>(Action<TMessage> messageBuilder)
            where TMessage : class, IServiceMessage
        {
            var message = (TMessage)Bus.CreateInstance(typeof(TMessage));
            if (messageBuilder != null)
                messageBuilder(message);
            return message;
        }

        public IServiceBusCallback Send(IServiceBusEndpoint endpoint, params IServiceMessage[] messages)
        {
            if (messages == null || messages.Length == 0 || messages[0] == null)
                throw new ArgumentNullException("messages", "Please include at least one message.");
            var transports = messages.Select(x => new Transport { B = x }).ToArray();
            try
            {
                if (endpoint == ServiceBus.Self)
                {
                    Bus.SendLocal(transports);
                    return null;
                }
                Bus.Send(transports);
                return null;
            }
            catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
        }

        public void Reply(params IServiceMessage[] messages)
        {
            if (messages == null || messages.Length == 0 || messages[0] == null)
                throw new ArgumentNullException("messages", "Please include at least one message.");
            var transports = messages.Select(x => new Transport { B = x }).ToArray();
            try { Bus.Reply(transports); }
            catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
        }

        #region Publishing ServiceBus

        public void Publish(params IServiceMessage[] messages)
        {
            if (messages == null || messages.Length == 0 || messages[0] == null)
                throw new ArgumentNullException("messages");
            var transports = messages.Select(x => new Transport { B = x }).ToArray();
            try { Bus.Publish(transports); }
            catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
        }

        public void Subscribe(Type messageType, Predicate<IServiceMessage> predicate)
        {
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            try
            {
                if (predicate == null)
                    Bus.Subscribe(messageType);
                else
                    Bus.Subscribe(messageType, Cast(predicate));
            }
            catch (Exception ex) { throw new ServiceBusMessageException(messageType, ex); }
        }

        public void Unsubscribe(Type messageType)
        {
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            try { Bus.Unsubscribe(messageType); }
            catch (Exception ex) { throw new ServiceBusMessageException(messageType, ex); }
        }

        #endregion

        #region Domain-specific

        public IBus Bus { get; set; }

        public void Return<T>(T value)
        {
            if (typeof(T) != typeof(int))
                throw new NotSupportedException();
            try { Bus.Return(Convert.ToInt32(value)); }
            catch (Exception ex) { throw new ServiceBusMessageException(null, ex); }
        }

        #endregion

        public static IStartableBus DefaultBusCreator(IServiceLocator serviceLocator)
        {
            if (serviceLocator == null)
                serviceLocator = ServiceLocatorManager.Current;
            return Configure.Instance.Builder.Build<IBus>() as IStartableBus;
        }

        #region Casts

        private static Predicate<IMessage> Cast(Predicate<IServiceMessage> predicate)
        {
            return (c => predicate((INServiceMessage)c));
        }

        #endregion
    }
}
