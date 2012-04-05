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
using Contoso.Abstract.RhinoServiceBus;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Config;
using Rhino.ServiceBus.Impl;
// [Main] http://hibernatingrhinos.com/open-source/rhino-service-bus
namespace Contoso.Abstract
{
    /// <summary>
    /// IRhinoServiceBus
    /// </summary>
    public interface IOnewayRhinoServiceBus : System.Abstract.IServiceBus
    {
        IOnewayBus Bus { get; }
    }

    /// <summary>
    /// RhinoServiceBusAbstractor
    /// </summary>
    public partial class OnewayRhinoServiceBusAbstractor : IOnewayRhinoServiceBus, ServiceBusManager.ISetupRegistration
    {
        private IServiceLocator _serviceLocator;
        private bool _passthru = true;

        static OnewayRhinoServiceBusAbstractor() { ServiceBusManager.EnsureRegistration(); }
        public OnewayRhinoServiceBusAbstractor()
            : this(ServiceLocatorManager.Current, DefaultBusCreator(null, null, null)) { }
        public OnewayRhinoServiceBusAbstractor(IServiceLocator serviceLocator)
            : this(serviceLocator, DefaultBusCreator(serviceLocator, null, null)) { }
        public OnewayRhinoServiceBusAbstractor(BusConfigurationSection busConfiguration)
            : this(ServiceLocatorManager.Current, DefaultBusCreator(null, busConfiguration, null)) { }
        public OnewayRhinoServiceBusAbstractor(IServiceLocator serviceLocator, BusConfigurationSection busConfiguration)
            : this(serviceLocator, DefaultBusCreator(serviceLocator, busConfiguration, null)) { }
        public OnewayRhinoServiceBusAbstractor(IServiceLocator serviceLocator, IOnewayBus bus)
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
            get { return (locator, name) => ServiceBusManager.RegisterInstance<IOnewayRhinoServiceBus>(this, locator, name); }
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
                    if (endpoint == null) Bus.Send(messages);
                    else if (endpoint != ServiceBus.SelfEndpoint) Bus.Send(RhinoServiceBusTransport.Cast(endpoint), messages);
                    else throw new NotSupportedException();
                }
                catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
            else
            {
                if (endpoint == null)
                    endpoint = RhinoServiceBusTransport.EndpointByMessageType(messages[0].GetType());
                try
                {
                    if (endpoint == null) Bus.Send(RhinoServiceBusTransport.Cast(messages));
                    else if (endpoint != ServiceBus.SelfEndpoint) Bus.Send(RhinoServiceBusTransport.TransportEndpointMapper(endpoint), RhinoServiceBusTransport.Cast(messages));
                    else throw new NotSupportedException();
                }
                catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
            }
            return null;
        }

        public void Reply(params object[] messages) { throw new NotSupportedException(); }

        #region Domain-specific

        public IOnewayBus Bus { get; private set; }

        #endregion

        public static IOnewayBus DefaultBusCreator(IServiceLocator serviceLocator, BusConfigurationSection busConfiguration, Action<AbstractRhinoServiceBusConfiguration> configurator)
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
            return serviceLocator.Resolve<IOnewayBus>();
        }
    }
}
