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
using System.Collections.Generic;
namespace Contoso.Practices.Cqrs
{
    /// <summary>
    /// ICommandBus
    /// </summary>
    public interface ICommandBus : IServiceBus { }

    /// <summary>
    /// CommandBus
    /// </summary>
    public struct CommandBus : ICommandBus
    {
        private IServiceBus _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBus"/> struct.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public CommandBus(IServiceBus parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            _parent = parent;
        }
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.
        /// -or-
        /// null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public object GetService(Type serviceType) { return _parent.GetService(serviceType); }
        /// <summary>
        /// Creates the message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="messageBuilder">The message builder.</param>
        /// <returns></returns>
        public TMessage CreateMessage<TMessage>(Action<TMessage> messageBuilder)
            where TMessage : class { return _parent.CreateMessage(messageBuilder); }
        /// <summary>
        /// Sends the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="messages">The messages.</param>
        /// <returns></returns>
        public IServiceBusCallback Send(IServiceBusEndpoint destination, params object[] messages) { return _parent.Send(destination, messages); }
        /// <summary>
        /// Replies the specified messages.
        /// </summary>
        /// <param name="messages">The messages.</param>
        public void Reply(params object[] messages) { _parent.Send(messages); }
    }
}
