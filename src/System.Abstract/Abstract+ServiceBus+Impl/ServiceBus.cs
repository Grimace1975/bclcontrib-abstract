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
using System.Collections.Generic;
namespace System.Abstract
{
    /// <summary>
    /// ServiceBus
    /// </summary>
    public static class ServiceBus
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly static IServiceBusEndpoint SelfEndpoint = new LiteralServiceBusEndpoint("#local");

        /// <summary>
        /// Sends the specified message builder.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="messageBuilder">The message builder.</param>
        /// <returns></returns>
        public static IServiceBusCallback Send<TMessage>(Action<TMessage> messageBuilder)
            where TMessage : class { var serviceBus = ServiceBusManager.Current; return serviceBus.Send(null, serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        /// <summary>
        /// Sends the specified destination.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="destination">The destination.</param>
        /// <param name="messageBuilder">The message builder.</param>
        /// <returns></returns>
        public static IServiceBusCallback Send<TMessage>(string destination, Action<TMessage> messageBuilder)
            where TMessage : class { var serviceBus = ServiceBusManager.Current; return serviceBus.Send(new LiteralServiceBusEndpoint(destination), serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        /// <summary>
        /// Sends the specified destination.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="destination">The destination.</param>
        /// <param name="messageBuilder">The message builder.</param>
        /// <returns></returns>
        public static IServiceBusCallback Send<TMessage>(IServiceBusEndpoint destination, Action<TMessage> messageBuilder)
            where TMessage : class { var serviceBus = ServiceBusManager.Current; return serviceBus.Send(destination, serviceBus.CreateMessage<TMessage>(messageBuilder)); }
        /// <summary>
        /// Sends the specified messages.
        /// </summary>
        /// <param name="messages">The messages.</param>
        /// <returns></returns>
        public static IServiceBusCallback Send(params object[] messages) { var serviceBus = ServiceBusManager.Current; return serviceBus.Send(null, messages); }
        /// <summary>
        /// Sends the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="messages">The messages.</param>
        /// <returns></returns>
        public static IServiceBusCallback Send(string destination, params object[] messages) { var serviceBus = ServiceBusManager.Current; return serviceBus.Send(new LiteralServiceBusEndpoint(destination), messages); }
        /// <summary>
        /// Sends the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="messages">The messages.</param>
        /// <returns></returns>
        public static IServiceBusCallback Send(IServiceBusEndpoint destination, params object[] messages) { var serviceBus = ServiceBusManager.Current; return serviceBus.Send(destination, messages); }
    }
}
