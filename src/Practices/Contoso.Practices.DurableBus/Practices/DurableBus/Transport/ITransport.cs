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
namespace Contoso.Practices.DurableBus.Transport
{
    /// <summary>
    /// ITransport
    /// </summary>
    public interface ITransport : IDisposable
    {
        /// <summary>
        /// Occurs when [message received].
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        /// <summary>
        /// Occurs when [message processing].
        /// </summary>
        event EventHandler MessageProcessing;
        /// <summary>
        /// Occurs when [message processing failed].
        /// </summary>
        event EventHandler MessageProcessingFailed;
        /// <summary>
        /// Occurs when [message processed].
        /// </summary>
        event EventHandler MessageProcessed;
        /// <summary>
        /// Aborts the current message.
        /// </summary>
        void AbortCurrentMessage();
        /// <summary>
        /// Gets the pending messages.
        /// </summary>
        int PendingMessages { get; }
        /// <summary>
        /// Receives the message later.
        /// </summary>
        /// <param name="m">The m.</param>
        void ReceiveMessageLater(TransportMessage m);
        /// <summary>
        /// Sends the specified m.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="destination">The destination.</param>
        void Send(TransportMessage m, string destination);
        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start();
        /// <summary>
        /// Gets the address.
        /// </summary>
        string Address { get; }
        /// <summary>
        /// Gets or sets the worker threads.
        /// </summary>
        /// <value>
        /// The worker threads.
        /// </value>
        int WorkerThreads { get; set; }
    }
}