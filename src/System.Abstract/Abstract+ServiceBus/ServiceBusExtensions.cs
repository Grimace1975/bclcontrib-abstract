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
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Abstract.Parts;
namespace System.Abstract
{
    /// <summary>
    /// ServiceBusExtensions
    /// </summary>
    public static class ServiceBusExtensions
    {
        /// <summary>
        /// Sends the specified service bus.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="serviceBus">The service bus.</param>
        /// <param name="messageBuilder">The message builder.</param>
        /// <returns></returns>
        public static IServiceBusCallback Send<TMessage>(this IServiceBus serviceBus, Action<TMessage> messageBuilder)
            where TMessage : class { return serviceBus.Send(null, serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        /// <summary>
        /// Sends the specified service bus.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="serviceBus">The service bus.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="messageBuilder">The message builder.</param>
        /// <returns></returns>
        public static IServiceBusCallback Send<TMessage>(this IServiceBus serviceBus, string destination, Action<TMessage> messageBuilder)
            where TMessage : class { return serviceBus.Send(new LiteralServiceBusEndpoint(destination), serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        /// <summary>
        /// Sends the specified service bus.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="serviceBus">The service bus.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="messageBuilder">The message builder.</param>
        /// <returns></returns>
        public static IServiceBusCallback Send<TMessage>(this IServiceBus serviceBus, IServiceBusEndpoint destination, Action<TMessage> messageBuilder)
            where TMessage : class { return serviceBus.Send(destination, serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        /// <summary>
        /// Sends the specified service bus.
        /// </summary>
        /// <param name="serviceBus">The service bus.</param>
        /// <param name="messages">The messages.</param>
        /// <returns></returns>
        public static IServiceBusCallback Send(this IServiceBus serviceBus, params object[] messages) { return serviceBus.Send(null, messages); }
        /// <summary>
        /// Sends the specified service bus.
        /// </summary>
        /// <param name="serviceBus">The service bus.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="messages">The messages.</param>
        /// <returns></returns>
        public static IServiceBusCallback Send(this IServiceBus serviceBus, string destination, params object[] messages) { return serviceBus.Send(new LiteralServiceBusEndpoint(destination), messages); }
        /// <summary>
        /// Replies the specified service bus.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="serviceBus">The service bus.</param>
        /// <param name="messageBuilder">The message builder.</param>
        public static void Reply<TMessage>(this IServiceBus serviceBus, Action<TMessage> messageBuilder)
            where TMessage : class { serviceBus.Reply(serviceBus.CreateMessage(messageBuilder)); }

        // publishing
        /// <summary>
        /// Publishes the specified service bus.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="serviceBus">The service bus.</param>
        /// <param name="messageBuilder">The message builder.</param>
        public static void Publish<TMessage>(this IPublishingServiceBus serviceBus, Action<TMessage> messageBuilder)
            where TMessage : class { serviceBus.Publish(serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        //
        /// <summary>
        /// Subscribes the specified service bus.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="serviceBus">The service bus.</param>
        public static void Subscribe<TMessage>(this IPublishingServiceBus serviceBus)
            where TMessage : class { serviceBus.Subscribe(typeof(TMessage), null); }
        /// <summary>
        /// Subscribes the specified service bus.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="serviceBus">The service bus.</param>
        /// <param name="condition">The condition.</param>
        public static void Subscribe<TMessage>(this IPublishingServiceBus serviceBus, Predicate<TMessage> condition)
            where TMessage : class
        {
            var p = new Predicate<object>(m => (m is TMessage ? condition((TMessage)m) : true));
            serviceBus.Subscribe(typeof(TMessage), p);
        }
        /// <summary>
        /// Subscribes the specified service bus.
        /// </summary>
        /// <param name="serviceBus">The service bus.</param>
        /// <param name="messageType">Type of the message.</param>
        public static void Subscribe(this IPublishingServiceBus serviceBus, Type messageType) { }
        /// <summary>
        /// Unsubscribes the specified service bus.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="serviceBus">The service bus.</param>
        public static void Unsubscribe<TMessage>(this IPublishingServiceBus serviceBus)
            where TMessage : class { serviceBus.Subscribe(typeof(TMessage)); }

        #region BehaveAs

        /// <summary>
        /// Behaves as.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static T BehaveAs<T>(this IServiceBus service)
            where T : class, IServiceBus
        {
            IServiceWrapper<IServiceBus> serviceWrapper;
            do
            {
                serviceWrapper = (service as IServiceWrapper<IServiceBus>);
                if (serviceWrapper != null)
                    service = serviceWrapper.Parent;
            } while (serviceWrapper != null);
            return (service as T);
        }

        #endregion

        #region Lazy Setup

        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static Lazy<IServiceBus> RegisterWithServiceLocator<T>(this Lazy<IServiceBus> service)
            where T : class, IServiceBus { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, ServiceLocatorManager.Lazy, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceBus> RegisterWithServiceLocator<T>(this Lazy<IServiceBus> service, string name)
            where T : class, IServiceBus { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, ServiceLocatorManager.Lazy, name); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Lazy<IServiceBus> RegisterWithServiceLocator<T>(this Lazy<IServiceBus> service, Lazy<IServiceLocator> locator)
            where T : class, IServiceBus { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceBus> RegisterWithServiceLocator<T>(this Lazy<IServiceBus> service, Lazy<IServiceLocator> locator, string name)
            where T : class, IServiceBus { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, name); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, ServiceLocatorManager.Lazy, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service, string name) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, ServiceLocatorManager.Lazy, name); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service, Lazy<IServiceLocator> locator) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service, Lazy<IServiceLocator> locator, string name) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, name); return service; }

        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Lazy<IServiceBus> RegisterWithServiceLocator<T>(this Lazy<IServiceBus> service, IServiceLocator locator)
            where T : class, IServiceBus { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceBus> RegisterWithServiceLocator<T>(this Lazy<IServiceBus> service, IServiceLocator locator, string name)
            where T : class, IServiceBus { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, name); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service, IServiceLocator locator) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service, IServiceLocator locator, string name) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, name); return service; }

        /// <summary>
        /// Adds the endpoint.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns></returns>
        public static Lazy<IServiceBus> AddEndpoint(this Lazy<IServiceBus> service, string endpoint) { return service; }

        /// <summary>
        /// Adds the message handlers by scan.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns></returns>
        public static Lazy<IServiceBus> AddMessageHandlersByScan(this Lazy<IServiceBus> service, params Assembly[] assemblies) { ServiceBusManager.GetSetupDescriptor(service).Do(s => AddMessageHandlersByScan(s, null, assemblies)); return service; }
        /// <summary>
        /// Adds the message handlers by scan.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns></returns>
        public static Lazy<IServiceBus> AddMessageHandlersByScan(this Lazy<IServiceBus> service, Predicate<Type> predicate, params Assembly[] assemblies) { ServiceBusManager.GetSetupDescriptor(service).Do(s => AddMessageHandlersByScan(s, predicate, assemblies)); return service; }
        /// <summary>
        /// Adds the message handler.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="handlerType">Type of the handler.</param>
        /// <returns></returns>
        public static Lazy<IServiceBus> AddMessageHandler(this Lazy<IServiceBus> service, Type handlerType) { ServiceBusManager.GetSetupDescriptor(service).Do(s => AddMessageHandler(s, handlerType)); return service; }

        #endregion

        /// <summary>
        /// Adds the message handlers by scan.
        /// </summary>
        /// <typeparam name="TMessageHandler">The type of the message handler.</typeparam>
        /// <param name="bus">The bus.</param>
        public static void AddMessageHandlersByScan<TMessageHandler>(IServiceBus bus)
            where TMessageHandler : class { AddMessageHandlersByScan(bus, typeof(TMessageHandler)); }
        /// <summary>
        /// Adds the message handlers by scan.
        /// </summary>
        /// <param name="bus">The bus.</param>
        /// <param name="handlerType">Type of the handler.</param>
        public static void AddMessageHandlersByScan(IServiceBus bus, Type handlerType)
        {
            var messageType = GetMessageTypeFromHandler(handlerType);
            if (messageType == null)
                throw new InvalidOperationException("Unable find a message handler");
        }

        //private IEnumerable<Type> GetTypesOfMessageHandlers(Type messageType)
        //{
        //    return Items.Where(x => x.MessageType == messageType)
        //        .Select(x => x.MessageHandlerType);
        //}

        private static IEnumerable<Type> GetMessageTypeFromHandler(Type messageHandlerType)
        {
            //var serviceMessageType = typeof(IServiceMessage);
            //return messageHandlerType.GetInterfaces()
            //    .Where(h => h.IsGenericType && (h.FullName.StartsWith("System.Abstract.IServiceMessageHandler`1") || h.FullName.StartsWith("Contoso.Abstract.IApplicationServiceMessageHandler`1")))
            //    .Select(h => h.GetGenericArguments()[0])
            //    .Where(m => m.GetInterfaces().Any(x => x == serviceMessageType || x == applicationServiceMessageType));
            return null;
        }

        /// <summary>
        /// Adds the message handlers by scan.
        /// </summary>
        /// <param name="bus">The bus.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="assemblies">The assemblies.</param>
        public static void AddMessageHandlersByScan(IServiceBus bus, Predicate<Type> predicate, params Assembly[] assemblies)
        {
            if (assemblies.Count() == 0)
                return;
            //var types = assemblies.SelectMany(a => a.GetTypes())
            //    .Where(t => typeof(basedOnType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && (predicate == null || predicate(t)));
            //foreach (var type in types)
            //    action(basedOnType, type, Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Adds the message handler.
        /// </summary>
        /// <param name="bus">The bus.</param>
        /// <param name="handlerType">Type of the handler.</param>
        public static void AddMessageHandler(IServiceBus bus, Type handlerType)
        {
        }

        //public static string ToString(this IServiceBusLocation location, Func<IServiceBusLocation, object, string> builder, object arg)
        //{
        //    if (location == null)
        //        throw new ArgumentNullException("location");
        //    if (builder == null)
        //        throw new ArgumentNullException("builder");
        //    if (location == ServiceBus.Self)
        //        return null;
        //    var literal = (location as LiteralServiceBusLocation);
        //    if (literal != null)
        //        return literal.Value;
        //    return builder(location, arg);
        //}
    }
}