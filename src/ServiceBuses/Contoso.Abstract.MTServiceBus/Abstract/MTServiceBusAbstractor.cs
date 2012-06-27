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
using MassTransit;
using IServiceBus = MassTransit.IServiceBus;
namespace Contoso.Abstract
{
    /// <summary>
    /// IMTServiceBus
    /// </summary>
    public interface IMTServiceBus : IPublishingServiceBus
    {
        IServiceBus Bus { get; }
    }

    /// <summary>
    /// MTServiceBusAbstractor
    /// </summary>
    /// http://readthedocs.org/docs/masstransit/en/develop/index.html
    /// http://mikehadlow.blogspot.com/2009/07/first-look-at-masstransit.html
    public class MTServiceBusAbstractor : IMTServiceBus, ServiceBusManager.ISetupRegistration
    {
        private IServiceLocator _serviceLocator;

        static MTServiceBusAbstractor() { ServiceBusManager.EnsureRegistration(); }
        public MTServiceBusAbstractor() { }
        public MTServiceBusAbstractor(IServiceLocator serviceLocator)
            : this(serviceLocator, DefaultBusCreator(serviceLocator)) { }
        public MTServiceBusAbstractor(IServiceLocator serviceLocator, IServiceBus bus)
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
            get { return (locator, name) => ServiceBusManager.RegisterInstance<IMTServiceBus>(this, locator, name); }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        public TMessage CreateMessage<TMessage>(Action<TMessage> messageBuilder)
            where TMessage : class
        {
            throw new NotImplementedException();
            //var message = (TMessage)Bus.CreateInstance(typeof(TMessage));
            //if (messageBuilder != null)
            //    messageBuilder(message);
            //return message;
        }

        public virtual IServiceBusCallback Send(IServiceBusEndpoint endpoint, params object[] messages)
        {
            if (messages == null || messages.Length == 0 || messages[0] == null)
                throw new ArgumentNullException("messages", "Please include at least one message.");
            try
            {
                //if (endpoint == null) Bus.Endpoint.Send(messages);
                //else if (endpoint != ServiceBus.SelfEndpoint) MTServiceBusTransport.Cast(endpoint).Send(messages);
                //else Bus.Endpoint.Send(messages);
            }
            catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
            return null;
        }

        public virtual void Reply(params object[] messages)
        {
            if (messages == null || messages.Length == 0 || messages[0] == null)
                throw new ArgumentNullException("messages", "Please include at least one message.");
            //try { Bus.Reply(messages); }
            //catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
        }

        #region Publishing ServiceBus

        public virtual void Publish(params object[] messages)
        {
            if (messages == null || messages.Length == 0 || messages[0] == null)
                throw new ArgumentNullException("messages");
            //try { Bus.Publish(messages); }
            //catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
        }

        public virtual void Subscribe(Type messageType, Predicate<object> predicate)
        {
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            try
            {
                //if (predicate == null) Bus.SubscribeConsumer(messageType);
                //else Bus.SubscribeConsumer(messageType, predicate);
            }
            catch (Exception ex) { throw new ServiceBusMessageException(messageType, ex); }
        }

        public virtual void Unsubscribe(Type messageType)
        {
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            //try { Bus.Unsubscribe(messageType); }
            //catch (Exception ex) { throw new ServiceBusMessageException(messageType, ex); }
        }

        #endregion

        #region Domain-specific

        public IServiceBus Bus { get; set; }

        #endregion

        public static IServiceBus DefaultBusCreator(IServiceLocator serviceLocator)
        {
            if (serviceLocator == null)
                serviceLocator = ServiceLocatorManager.Current;
            var bus = ServiceBusFactory.New(sbc =>
            {
                sbc.UseMsmq();
                sbc.UseMulticastSubscriptionClient();
                sbc.ReceiveFrom("msmq://localhost/test_queue");
                sbc.Subscribe(subs =>
                {
                    //subs.Handler<YourMessage>(msg=>Console.WriteLine(msg.Text));
                });
            });
            return bus;
        }
    }
}
