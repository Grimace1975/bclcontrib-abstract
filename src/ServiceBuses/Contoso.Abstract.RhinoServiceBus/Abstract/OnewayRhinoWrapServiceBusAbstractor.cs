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
using Rhino.ServiceBus;
using Rhino.ServiceBus.Config;
// [Main] http://hibernatingrhinos.com/open-source/rhino-service-bus
namespace Contoso.Abstract
{
    /// <summary>
    /// OnewayRhinoWrapServiceBusAbstractor
    /// </summary>
    public partial class OnewayRhinoWrapServiceBusAbstractor : OnewayRhinoServiceBusAbstractor
    {
        static OnewayRhinoWrapServiceBusAbstractor() { ServiceBusManager.EnsureRegistration(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="OnewayRhinoWrapServiceBusAbstractor"/> class.
        /// </summary>
        public OnewayRhinoWrapServiceBusAbstractor()
            : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OnewayRhinoWrapServiceBusAbstractor"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public OnewayRhinoWrapServiceBusAbstractor(IServiceLocator serviceLocator)
            : base(serviceLocator) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OnewayRhinoWrapServiceBusAbstractor"/> class.
        /// </summary>
        /// <param name="busConfiguration">The bus configuration.</param>
        public OnewayRhinoWrapServiceBusAbstractor(BusConfigurationSection busConfiguration)
            : base(busConfiguration) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OnewayRhinoWrapServiceBusAbstractor"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="busConfiguration">The bus configuration.</param>
        public OnewayRhinoWrapServiceBusAbstractor(IServiceLocator serviceLocator, BusConfigurationSection busConfiguration)
            : base(serviceLocator, busConfiguration) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OnewayRhinoWrapServiceBusAbstractor"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="bus">The bus.</param>
        public OnewayRhinoWrapServiceBusAbstractor(IServiceLocator serviceLocator, IOnewayBus bus)
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
            if (endpoint == null)
                endpoint = RhinoServiceBusTransport.EndpointByMessageType(messages[0].GetType());
            try
            {
                if (endpoint == null) Bus.Send(RhinoServiceBusTransport.Wrap(messages));
                else throw new NotSupportedException();
            }
            catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
            return null;
        }
    }
}
