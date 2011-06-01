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
    /// IServiceBus
    /// </summary>
    public interface IServiceBus : IServiceProvider
    {
        TMessage CreateMessage<TMessage>(Action<TMessage> messageBuilder) where TMessage : IServiceMessage;
        IServiceBusCallback Send(IServiceBusLocation destination, params IServiceMessage[] messages);
    }

    /// <summary>
    /// IServiceBusExtensions
    /// </summary>
    public static class IServiceBusExtensions
    {
        public static IServiceBusCallback Send<TMessage>(this IServiceBus serviceBus, IServiceBusLocation destination, Action<TMessage> messageBuilder)
            where TMessage : IServiceMessage { return serviceBus.Send(destination, serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        public static IServiceBusCallback Send<TMessage>(this IServiceBus serviceBus, Action<TMessage> messageBuilder)
            where TMessage : IServiceMessage { return serviceBus.Send(null, serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        public static IServiceBusCallback Send<TMessage>(this IServiceBus serviceBus, string destination, Action<TMessage> messageBuilder)
            where TMessage : IServiceMessage { return serviceBus.Send(new LiteralServiceBusLocation(destination), serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        // send
        public static IServiceBusCallback Send(this IServiceBus serviceBus, params IServiceMessage[] messages) { return serviceBus.Send(null, messages); }
        public static IServiceBusCallback Send(this IServiceBus serviceBus, string destination, params IServiceMessage[] messages) { return serviceBus.Send(new LiteralServiceBusLocation(destination), messages); }

        #region Lazy Setup

		public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(null); return service; }
		public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service, string name) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(name); return service; }
		public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service, Func<IServiceLocator> locator) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(locator, null); return service; }
		public static Lazy<IServiceBus> RegisterWithServiceLocator(this Lazy<IServiceBus> service, Func<IServiceLocator> locator, string name) { ServiceBusManager.GetSetupDescriptor(service).RegisterWithServiceLocator(locator, name); return service; }

		public static Lazy<IServiceBus> AddEndpoint(this Lazy<IServiceBus> service, string endpoint) { return service; }

        #endregion
    }
}