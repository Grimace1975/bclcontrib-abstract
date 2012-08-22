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
namespace Contoso.Practices.DurableBus.Transport
{
    public abstract partial class TransportBase<T>
    {
        /// <summary>
        /// Occurs when [message received].
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        /// <summary>
        /// Occurs when [message processing].
        /// </summary>
        public event EventHandler MessageProcessing;
        /// <summary>
        /// Occurs when [message processing failed].
        /// </summary>
        public event EventHandler MessageProcessingFailed;
        /// <summary>
        /// Occurs when [message processed].
        /// </summary>
        public event EventHandler MessageProcessed;

        /// <summary>
        /// Called when [message received].
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        protected virtual bool OnMessageReceived(TransportMessage m)
        {
            try
            {
                var messageReceived = MessageReceived;
                if (messageReceived != null)
                    messageReceived(this, new MessageReceivedEventArgs(m));
            }
            catch (Exception ex) { ServiceLog.WarningFormat("Failed raising 'transport message received' event for message with ID=" + m.Id, ex); return false; }
            return true;
        }

        /// <summary>
        /// Called when [message processing].
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnMessageProcessing()
        {
            try
            {
                var messageProcessing = MessageProcessing;
                if (messageProcessing != null)
                    messageProcessing(this, null);
            }
            catch (Exception ex) { ServiceLog.ErrorFormat("Failed raising 'finished message processing' event.", ex); return false; }
            return true;
        }

        /// <summary>
        /// Called when [message processing failed].
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnMessageProcessingFailed()
        {
            try
            {
                var messageProcessingFailed = MessageProcessingFailed;
                if (messageProcessingFailed != null)
                    messageProcessingFailed(this, null);
            }
            catch (Exception ex) { ServiceLog.WarningFormat("Failed raising 'failed message processing' event.", ex); return false; }
            return true;
        }

        /// <summary>
        /// Called when [message processed].
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnMessageProcessed()
        {
            try
            {
                var messageProcessed = MessageProcessed;
                if (messageProcessed != null)
                    messageProcessed(this, null);
            }
            catch (Exception ex) { ServiceLog.ErrorFormat("Failed raising 'finished message processing' event.", ex); return false; }
            return true;
        }
    }
}