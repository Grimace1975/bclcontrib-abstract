//#region License
///*
//The MIT License

//Copyright (c) 2008 Sky Morey

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
//*/
//#endregion
//using System;
//namespace Contoso.Practices.DurableBus.Transport
//{
//    public partial class MsmqTransport
//    {
//        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
//        public event EventHandler MessageProcessing;
//        public event EventHandler MessageProcessingFailed;
//        public event EventHandler MessageProcessed;

//        protected virtual bool OnFailedMessageProcessing()
//        {
//            try
//            {
//                var failedMessageProcessing = FailedMessageProcessing;
//                if (failedMessageProcessing != null)
//                    failedMessageProcessing(this, null);
//            }
//            catch (Exception ex) { Logger.Warn("Failed raising 'failed message processing' event.", ex); return false; }
//            return true;
//        }

//        protected virtual bool OnFinishedMessageProcessing()
//        {
//            try
//            {
//                var finishedMessageProcessing = FinishedMessageProcessing;
//                if (finishedMessageProcessing != null)
//                    finishedMessageProcessing(this, null);
//            }
//            catch (Exception ex) { Logger.Error("Failed raising 'finished message processing' event.", ex); return false; }
//            return true;
//        }

//        protected virtual bool OnTransportMessageReceived(TransportMessage m)
//        {
//            try
//            {
//                var transportMessageReceived = TransportMessageReceived;
//                if (transportMessageReceived != null)
//                    transportMessageReceived(this, new MessageReceivedEventArgs(m));
//            }
//            catch (Exception e) { Logger.Warn("Failed raising 'transport message received' event for message with ID=" + m.Id, e); return false; }
//            return true;
//        }
//    }
//}