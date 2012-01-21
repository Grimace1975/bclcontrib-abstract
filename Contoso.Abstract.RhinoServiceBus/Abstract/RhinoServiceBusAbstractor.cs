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
        static RhinoServiceBusAbstractor() { ServiceBusManager.EnsureRegistration(); }
        public RhinoServiceBusAbstractor()
            : this(DefaultBusCreator(null)) { }
        public RhinoServiceBusAbstractor(IServiceLocator serviceLocator)
            : this(DefaultBusCreator(serviceLocator)) { }
        public RhinoServiceBusAbstractor(IStartableServiceBus bus)
        {
            if (bus == null)
                throw new ArgumentNullException("bus", "The specified NServiceBus bus cannot be null.");
            Bus = bus;
            bus.Start();
        }

        Action<IServiceLocator, string> ServiceBusManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (locator, name) => ServiceBusManager.RegisterInstance<IRhinoServiceBus>(this, locator, name); }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        public void Publish(params IServiceMessage[] messages)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(Type messageType, Predicate<IServiceMessage> condition)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(Type messageType)
        {
            throw new NotImplementedException();
        }

        public TMessage CreateMessage<TMessage>(Action<TMessage> messageBuilder)
            where TMessage : IServiceMessage
        {
            throw new NotImplementedException();
        }

        public IServiceBusCallback Send(IServiceBusLocation destination, params IServiceMessage[] messages)
        {
            //Bus.Send();
            throw new NotImplementedException();
        }

        public void Reply(params IServiceMessage[] messages)
        {
            //Bus.Reply();
            throw new NotImplementedException();
        }

        public void Return<T>(T value)
        {
            throw new NotImplementedException();
        }

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
