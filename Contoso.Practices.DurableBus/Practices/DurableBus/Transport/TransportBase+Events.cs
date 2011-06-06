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
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler MessageProcessing;
        public event EventHandler MessageProcessingFailed;
        public event EventHandler MessageProcessed;

        protected virtual bool OnMessageReceived(TransportMessage m)
        {
            try
            {
                var messageReceived = MessageReceived;
                if (messageReceived != null)
                    messageReceived(this, new MessageReceivedEventArgs(m));
            }
            catch (Exception e) { ServiceLog.Warning("Failed raising 'transport message received' event for message with ID=" + m.Id, e); return false; }
            return true;
        }

        protected virtual bool OnMessageProcessing()
        {
            try
            {
                var messageProcessing = MessageProcessing;
                if (messageProcessing != null)
                    messageProcessing(this, null);
            }
            catch (Exception ex) { ServiceLog.Error("Failed raising 'finished message processing' event.", ex); return false; }
            return true;
        }

        protected virtual bool OnMessageProcessingFailed()
        {
            try
            {
                var messageProcessingFailed = MessageProcessingFailed;
                if (messageProcessingFailed != null)
                    messageProcessingFailed(this, null);
            }
            catch (Exception ex) { ServiceLog.Warning("Failed raising 'failed message processing' event.", ex); return false; }
            return true;
        }

        protected virtual bool OnMessageProcessed()
        {
            try
            {
                var messageProcessed = MessageProcessed;
                if (messageProcessed != null)
                    messageProcessed(this, null);
            }
            catch (Exception ex) { ServiceLog.Error("Failed raising 'finished message processing' event.", ex); return false; }
            return true;
        }
    }
}