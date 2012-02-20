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
namespace System.Abstract
{
    /// <summary>
    /// ServiceBusExtensions
    /// </summary>
    public static class ServiceBusExtensions
    {
        public static IServiceBusCallback Send<TMessage>(this IServiceBus serviceBus, IServiceBusEndpoint destination, Action<TMessage> messageBuilder)
            where TMessage : class { return serviceBus.Send(destination, serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        public static IServiceBusCallback Send<TMessage>(this IServiceBus serviceBus, Action<TMessage> messageBuilder)
            where TMessage : class { return serviceBus.Send(null, serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        public static IServiceBusCallback Send<TMessage>(this IServiceBus serviceBus, string destination, Action<TMessage> messageBuilder)
            where TMessage : class { return serviceBus.Send(new LiteralServiceBusEndpoint(destination), serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        public static IServiceBusCallback Send(this IServiceBus serviceBus, params object[] messages) { return serviceBus.Send(null, messages); }
        public static IServiceBusCallback Send(this IServiceBus serviceBus, string destination, params object[] messages) { return serviceBus.Send(new LiteralServiceBusEndpoint(destination), messages); }
        public static void Reply<TMessage>(this IServiceBus serviceBus, Action<TMessage> messageBuilder)
            where TMessage : class { serviceBus.Reply(serviceBus.CreateMessage(messageBuilder)); }

        // publishing
        public static void Publish<TMessage>(this IPublishingServiceBus serviceBus, Action<TMessage> messageBuilder)
            where TMessage : class { serviceBus.Publish(serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        //
        public static void Subscribe<TMessage>(this IPublishingServiceBus serviceBus)
            where TMessage : class { serviceBus.Subscribe(typeof(TMessage), null); }
        public static void Subscribe<TMessage>(this IPublishingServiceBus serviceBus, Predicate<TMessage> condition)
            where TMessage : class
        {
            var p = new Predicate<object>(m => (m is TMessage ? condition((TMessage)m) : true));
            serviceBus.Subscribe(typeof(TMessage), p);
        }
        public static void Subscribe(this IPublishingServiceBus serviceBus, Type messageType) { }
        public static void Unsubscribe<TMessage>(this IPublishingServiceBus serviceBus)
            where TMessage : class { serviceBus.Subscribe(typeof(TMessage)); }

        #region Lazy Setup

        public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, null); return service; }
        public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service, string name) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, name); return service; }
        public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service, Lazy<IServiceLocator> locator) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, null); return service; }
        public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service, Lazy<IServiceLocator> locator, string name) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, name); return service; }
        public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service, IServiceLocator locator) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, null); return service; }
        public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service, IServiceLocator locator, string name) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, name); return service; }
        public static Lazy<IServiceBus> AddEndpoint(this Lazy<IServiceBus> service, string endpoint) { return service; }

        public static Lazy<IServiceBus> AddMessageHandlersByScan(this Lazy<IServiceBus> service, params Assembly[] assemblies) { ServiceBusManager.GetSetupDescriptor(service).Do(s => AddMessageHandlersByScan(s, null, assemblies)); return service; }
        public static Lazy<IServiceBus> AddMessageHandlersByScan(this Lazy<IServiceBus> service, Predicate<Type> predicate, params Assembly[] assemblies) { ServiceBusManager.GetSetupDescriptor(service).Do(s => AddMessageHandlersByScan(s, predicate, assemblies)); return service; }
        public static Lazy<IServiceBus> AddMessageHandler(this Lazy<IServiceBus> service, Type handlerType) { ServiceBusManager.GetSetupDescriptor(service).Do(s => AddMessageHandler(s, handlerType)); return service; }

        #endregion

        public static void AddMessageHandlersByScan<TMessageHandler>(IServiceBus bus)
            where TMessageHandler : class { AddMessageHandlersByScan(bus, typeof(TMessageHandler)); }
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

        public static void AddMessageHandlersByScan(IServiceBus bus, Predicate<Type> predicate, params Assembly[] assemblies)
        {
            if (assemblies.Count() == 0)
                return;
            //var types = assemblies.SelectMany(a => a.GetTypes())
            //    .Where(t => typeof(basedOnType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && (predicate == null || predicate(t)));
            //foreach (var type in types)
            //    action(basedOnType, type, Guid.NewGuid().ToString());
        }

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