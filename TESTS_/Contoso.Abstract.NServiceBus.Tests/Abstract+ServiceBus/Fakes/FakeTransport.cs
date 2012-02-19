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
using NServiceBus.Unicast.Transport;
namespace Contoso.Abstract.Fakes
{
    public class FakeTransport : ITransport
    {
        public void AbortHandlingCurrentMessage() { }
        public string Address
        {
            get { throw new NotImplementedException(); }
        }
        public void ChangeNumberOfWorkerThreads(int targetNumberOfWorkerThreads) { }
        public event EventHandler FailedMessageProcessing;
        public event EventHandler FinishedMessageProcessing;
        public int GetNumberOfPendingMessages()
        {
            return 0;
        }
        public int NumberOfWorkerThreads
        {
            get { return 1; }
        }
        public void ReceiveMessageLater(TransportMessage m) { }
        public void Send(TransportMessage m, string destination) { }
        public void Start() { }
        public event EventHandler StartedMessageProcessing;
        public event EventHandler<TransportMessageReceivedEventArgs> TransportMessageReceived;
        public void Dispose() { }
    }
}
