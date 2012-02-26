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
using Contoso.Abstract.RhinoServiceBus;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Config;
// [Main] http://hibernatingrhinos.com/open-source/rhino-service-bus
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
        private bool _passthru = true;

        static RhinoServiceBusAbstractor() { ServiceBusManager.EnsureRegistration(); }
        public RhinoServiceBusAbstractor()
            : this(ServiceLocatorManager.Current, DefaultBusCreator(null, null, null)) { }
        public RhinoServiceBusAbstractor(IServiceLocator serviceLocator)
            : this(serviceLocator, DefaultBusCreator(serviceLocator, null, null)) { }
        public RhinoServiceBusAbstractor(BusConfigurationSection busConfiguration)
            : this(ServiceLocatorManager.Current, DefaultBusCreator(null, busConfiguration, null)) { }
        public RhinoServiceBusAbstractor(IServiceLocator serviceLocator, BusConfigurationSection busConfiguration)
            : this(serviceLocator, DefaultBusCreator(serviceLocator, busConfiguration, null)) { }
        public RhinoServiceBusAbstractor(IServiceLocator serviceLocator, IStartableServiceBus bus)
            : this(serviceLocator, (Rhino.ServiceBus.IServiceBus)bus) { bus.Start(); }
        public RhinoServiceBusAbstractor(IServiceLocator serviceLocator, Rhino.ServiceBus.IServiceBus bus)
        {
            if (serviceLocator == null)
                throw new ArgumentNullException("serviceLocator");
            if (bus == null)
                throw new ArgumentNullException("bus", "The specified bus cannot be null.");
            _serviceLocator = serviceLocator;
            Bus = bus;
        }

        Action<IServiceLocator, string> ServiceBusManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (locator, name) => ServiceBusManager.RegisterInstance<IRhinoServiceBus>(this, locator, name); }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        public TMessage CreateMessage<TMessage>(Action<TMessage> messageBuilder)
            where TMessage : class
        {
            var message = _serviceLocator.Resolve<TMessage>();
            if (messageBuilder != null)
                messageBuilder(message);
            return message;
        }

        public IServiceBusCallback Send(IServiceBusEndpoint endpoint, params object[] messages)
        {
            if (messages == null || messages.Length == 0 || messages[0] == null)
                throw new ArgumentNullException("messages", "Please include at least one message.");
            if (_passthru)
                try
                {
                    if (endpoint == null)
                        Bus.Send(messages);
                    else if (endpoint != ServiceBus.SelfEndpoint)
                        Bus.Send(Caster.Cast(endpoint), messages);
                    else
                        Bus.SendToSelf(messages);
                }
                catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
            else
            {
                if (endpoint == null)
                    endpoint = EndpointByMessageType(messages[0].GetType());
                try
                {
                    if (endpoint == null)
                        Bus.Send(Caster.Cast(messages));
                    else if (endpoint != ServiceBus.SelfEndpoint)
                        Bus.Send(TransportEndpointMapper(endpoint), Caster.Cast(messages));
                    else
                        Bus.SendToSelf(Caster.Cast(messages));
                }
                catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
            }
            return null;
        }

        public void Reply(params object[] messages)
        {
            if (messages == null || messages.Length == 0 || messages[0] == null)
                throw new ArgumentNullException("messages", "Please include at least one message.");
            if (_passthru)
                try { Bus.Reply(messages); }
                catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
            else
                try { Bus.Reply(Caster.Cast(messages)); }
                catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
        }

        #region Publishing ServiceBus

        public void Publish(params object[] messages)
        {
            if (_passthru)
                try { Bus.Publish(messages); }
                catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
            else
                try { Bus.Publish(Caster.Cast(messages)); }
                catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
        }

        public void Subscribe(Type messageType, Predicate<object> condition)
        {
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            if (condition != null)
                throw new ArgumentException("condition", "Must be null.");
            if (_passthru)
                try { Bus.Subscribe(messageType); }
                catch (Exception ex) { throw new ServiceBusMessageException(messageType, ex); }
            else
                try { Bus.Subscribe(Caster.Cast(messageType)); }
                catch (Exception ex) { throw new ServiceBusMessageException(messageType, ex); }
        }

        public void Unsubscribe(Type messageType)
        {
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            if (_passthru)
                try { Bus.Unsubscribe(messageType); }
                catch (Exception ex) { throw new ServiceBusMessageException(messageType, ex); }
            else
                try { Bus.Unsubscribe(Caster.Cast(messageType)); }
                catch (Exception ex) { throw new ServiceBusMessageException(messageType, ex); }
        }

        #endregion

        #region Domain-specific

        public Rhino.ServiceBus.IServiceBus Bus { get; private set; }

        #endregion

        #region Endpoint-Translation

        private IServiceBusEndpoint EndpointByMessageType(Type messageType)
        {
            return null;
        }

        private Endpoint TransportEndpointMapper(IServiceBusEndpoint endpoint)
        {
            return null;
        }

        #endregion

        public static IStartableServiceBus DefaultBusCreator(IServiceLocator serviceLocator, BusConfigurationSection busConfiguration, Action<AbstractRhinoServiceBusConfiguration> configurator)
        {
            if (serviceLocator == null)
                serviceLocator = ServiceLocatorManager.Current;
            var configuration = new RhinoServiceBusConfiguration()
                .UseAbstractServiceLocator(serviceLocator);
            if (busConfiguration != null)
                configuration.UseConfiguration(busConfiguration);
            if (configurator != null)
                configurator(configuration);
            configuration.Configure();
            return serviceLocator.Resolve<IStartableServiceBus>();
        }

        private class Caster
        {
            public static object[] Cast(object[] messages)
            {
                return messages.Select(x =>
                {
                    var type = typeof(Transport<>).MakeGenericType(x.GetType());
                    var transport = Activator.CreateInstance(type);
                    type.GetProperty("B").SetValue(transport, x, null);
                    return transport;
                }).ToArray();
                //return messages.Select(x => new Transport<object> { B = x }).ToArray();
            }

            public static Type Cast(Type messagesType) { return typeof(Transport<>).MakeGenericType(messagesType); }

            public static Endpoint Cast(IServiceBusEndpoint endpoint)
            {
                var endpointAsLiteral = (endpoint as LiteralServiceBusEndpoint);
                if (endpointAsLiteral != null)
                    return new Endpoint { Uri = new Uri(endpointAsLiteral.Value) };
                throw new InvalidOperationException("Cast: Endpoint");
            }
        }
    }
}
