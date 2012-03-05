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
	public partial class MsmqTransport
	{
		private void Process()
		{
			//if (MessageInQueue())
			//{
			//    _needToAbort = false;
			//    _messageId = string.Empty;
			//    try
			//    {
			//        if (IsTransactional)
			//            new TransactionWrapper().RunInTransaction(new Action(ReceiveFromQueue), IsolationLevel, TransactionTimeout);
			//        else
			//            ReceiveFromQueue();
			//        ClearFailuresForMessage(_messageId);
			//    }
			//    catch (AbortHandlingCurrentMessageException) { }
			//    catch
			//    {
			//        if (IsTransactional)
			//            IncrementFailuresForMessage(_messageId);
			//        OnFailedMessageProcessing();
			//    }
			//}
		}

		//public void ReceiveFromQueue()
		//{
		//    Message m = ReceiveMessageFromQueueAfterPeekWasSuccessful();
		//    if (m != null)
		//    {
		//        _messageId = m.Id;
		//        if (IsTransactional && HandledMaxRetries(m.Id))
		//        {
		//            Logger.Error(string.Format("Message has failed the maximum number of times allowed, ID={0}.", m.Id));
		//            MoveToErrorQueue(m);
		//            OnFinishedMessageProcessing();
		//        }
		//        else
		//        {
		//            if (StartedMessageProcessing != null)
		//                StartedMessageProcessing(this, null);
		//            TransportMessage result = Convert(m);
		//            if (SkipDeserialization)
		//            {
		//                result.BodyStream = m.BodyStream;
		//            }
		//            else
		//            {
		//                try
		//                {
		//                    result.Body = Extract(m);
		//                }
		//                catch (Exception e)
		//                {
		//                    Logger.Error("Could not extract message data.", e);
		//                    MoveToErrorQueue(m);
		//                    OnFinishedMessageProcessing();
		//                    return;
		//                }
		//            }
		//            bool exceptionNotThrown = OnTransportMessageReceived(result);
		//            bool otherExNotThrown = OnFinishedMessageProcessing();
		//            if (_needToAbort)
		//            {
		//                throw new AbortHandlingCurrentMessageException();
		//            }
		//            if (!exceptionNotThrown || !otherExNotThrown)
		//            {
		//                throw new ApplicationException("Exception occured while processing message.");
		//            }
		//        }
		//    }
		//}
	}
}