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
namespace System.Abstract
{
    /// <summary>
    /// ServiceBusExtensions
    /// </summary>
    public static class ServiceBusExtensions
    {
        public static IServiceBusCallback Send<TMessage>(this IServiceBus serviceBus, IServiceBusLocation destination, Action<TMessage> messageBuilder)
            where TMessage : IServiceMessage { return serviceBus.Send(destination, serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        public static IServiceBusCallback Send<TMessage>(this IServiceBus serviceBus, Action<TMessage> messageBuilder)
            where TMessage : IServiceMessage { return serviceBus.Send(null, serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        public static IServiceBusCallback Send<TMessage>(this IServiceBus serviceBus, string destination, Action<TMessage> messageBuilder)
            where TMessage : IServiceMessage { return serviceBus.Send(new LiteralServiceBusLocation(destination), serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        public static IServiceBusCallback Send(this IServiceBus serviceBus, params IServiceMessage[] messages) { return serviceBus.Send(null, messages); }
        public static IServiceBusCallback Send(this IServiceBus serviceBus, string destination, params IServiceMessage[] messages) { return serviceBus.Send(new LiteralServiceBusLocation(destination), messages); }
		public static void Reply<TMessage>(this IServiceBus serviceBus, Action<TMessage> messageBuilder)
			where TMessage : IServiceMessage { serviceBus.Reply(serviceBus.CreateMessage(messageBuilder)); }

        // publishing
        public static void Publish<TMessage>(this IPublishingServiceBus serviceBus, Action<TMessage> messageBuilder)
            where TMessage : IServiceMessage { serviceBus.Publish(serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        //
        public static void Subscribe<TMessage>(this IPublishingServiceBus serviceBus)
            where TMessage : IServiceMessage { serviceBus.Subscribe(typeof(TMessage), null); }
        public static void Subscribe<TMessage>(this IPublishingServiceBus serviceBus, Predicate<TMessage> condition)
            where TMessage : IServiceMessage
        {
            var p = new Predicate<IServiceMessage>(m => (m is TMessage ? condition((TMessage)m) : true));
            serviceBus.Subscribe(typeof(TMessage), p);
        }
        public static void Subscribe(this IPublishingServiceBus serviceBus, Type messageType) { }
        public static void Unsubscribe<TMessage>(this IPublishingServiceBus serviceBus)
            where TMessage : IServiceMessage { }

        #region Lazy Setup

		public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, null); return service; }
		public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service, string name) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, name); return service; }
		public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service, Lazy<IServiceLocator> locator) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, null); return service; }
		public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service, Lazy<IServiceLocator> locator, string name) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, name); return service; }
		public static Lazy<IServiceBus> AddEndpoint(this Lazy<IServiceBus> service, string endpoint) { return service; }

        #endregion

        public static string ToString(this IServiceBusLocation location, Func<IServiceBusLocation, object, string> builder, object arg)
        {
            if (location == null)
                throw new ArgumentNullException("location");
            if (builder == null)
                throw new ArgumentNullException("builder");
            if (location == ServiceBus.Self)
                return null;
            var literal = (location as LiteralServiceBusLocation);
            if (literal != null)
                return literal.Value;
            return builder(location, arg);
        }
    }
}