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
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus;
using Contoso.Abstract.RhinoServiceBus;
namespace Contoso.Abstract
{
    /// <summary>
    /// IRhinoServiceBus
    /// </summary>
    public interface IRhinoServiceBus : IPublishingServiceBus
    {
        Rhino.ServiceBus.IServiceBus Bus { get; }
    }

    /// <summary>
    /// RhinoServiceBusAbstractor
    /// </summary>
    public class RhinoServiceBusAbstractor : IRhinoServiceBus, ServiceBusManager.ISetupRegistration
    {
        private IServiceLocator _serviceLocator;

        static RhinoServiceBusAbstractor() { ServiceBusManager.EnsureRegistration(); }
        public RhinoServiceBusAbstractor()
            : this(ServiceLocatorManager.Current, DefaultBusCreator(null)) { }
        public RhinoServiceBusAbstractor(IServiceLocator serviceLocator)
            : this(serviceLocator, DefaultBusCreator(serviceLocator)) { }
        public RhinoServiceBusAbstractor(IServiceLocator serviceLocator, IStartableServiceBus bus)
        {
            if (serviceLocator == null)
                throw new ArgumentNullException("serviceLocator");
            if (bus == null)
                throw new ArgumentNullException("bus", "The specified NServiceBus bus cannot be null.");
            _serviceLocator = serviceLocator;
            Bus = bus;
            bus.Start();
        }

        Action<IServiceLocator, string> ServiceBusManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (locator, name) => ServiceBusManager.RegisterInstance<IRhinoServiceBus>(this, locator, name); }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        public TMessage CreateMessage<TMessage>(Action<TMessage> messageBuilder)
            where TMessage : class, IServiceMessage
        {
            var message = _serviceLocator.Resolve<TMessage>();
            if (messageBuilder != null)
                messageBuilder(message);
            return message;
        }

        public IServiceBusCallback Send(IServiceBusEndpoint location, params IServiceMessage[] messages)
        {
            if (messages == null || messages.Length == 0 || messages[0] == null)
                throw new ArgumentNullException("messages", "Please include at least one message.");
            var transports = messages.Select(x => new Transport { B = x });
            try
            {
                if (location == ServiceBus.Self)
                {
                    Bus.SendToSelf(transports);
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
            var transports = messages.Select(x => new Transport { B = x });
            try { Bus.Reply(transports); }
            catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
        }

        #region Publishing ServiceBus

        public void Publish(params IServiceMessage[] messages)
        {
            var transports = messages.Select(x => new Transport { B = x });
            try { Bus.Publish(transports); }
            catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
        }

        public void Subscribe(Type messageType, Predicate<IServiceMessage> condition)
        {
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            if (condition != null)
                throw new ArgumentException("condition", "Must be null.");
            try { Bus.Subscribe(messageType); }
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

        public Rhino.ServiceBus.IServiceBus Bus { get; private set; }

        #endregion

        public static IStartableServiceBus DefaultBusCreator(IServiceLocator serviceLocator)
        {
            if (serviceLocator == null)
                serviceLocator = ServiceLocatorManager.Current;
            new RhinoServiceBusConfiguration()
                .UseAbstractServiceLocator(serviceLocator)
                .Configure();
            return serviceLocator.Resolve<IStartableServiceBus>();
        }
    }
}
