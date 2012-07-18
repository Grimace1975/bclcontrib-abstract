//#region License
// *
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
//using System.Collections.Generic;
//using System.Threading;
//using Contoso.Abstract.Practices.DurableBus.Transport;
//using System.Diagnostics;
//using System.Abstract;

//namespace Contoso.Practices.DurableBus.Transport
//{
//    public class MsmqTransport : ITransport, IDisposable
//    {
//        [ThreadStatic]
//        private static volatile string _messageId;
//        [ThreadStatic]
//        private static volatile bool _needToAbort;
//        private MessageQueue errorQueue;
//        private EventHandler FailedMessageProcessing;
//        private static readonly string FAILEDQUEUE = "FailedQ";
//        private readonly Dictionary<string, int> _failuresPerMessage = new Dictionary<string, int>();
//        private readonly ReaderWriterLockSlim _failuresPerMessageRwLock = new ReaderWriterLockSlim();
//        private EventHandler FinishedMessageProcessing;
//        private readonly XmlSerializer headerSerializer = new XmlSerializer(typeof(List<HeaderInfo>));
//        private static readonly string IDFORCORRELATION = "CorrId";
//        private static readonly ILog Logger = LogManager.GetLogger(typeof(MsmqTransport));
//        private int maxRetries = 5;
//        private int numberOfWorkerThreads;
//        private static readonly string ORIGINALID = "OriginalId";
//        private MessageQueue queue;
//        private int secondsToWaitForMessage = 10;
//        private EventHandler StartedMessageProcessing;
//        private EventHandler<TransportMessageReceivedEventArgs> TransportMessageReceived;
//        private static readonly string WINDOWSIDENTITYNAME = "WinIdName";

//        public void AbortHandlingCurrentMessage()
//        {
//            _needToAbort = true;
//        }

//        private void CheckConfiguration()
//        {
//            if (!string.IsNullOrEmpty(InputQueue))
//            {
//                if (MsmqUtilities.GetMachineNameFromLogicalName(InputQueue).ToLower() != Environment.MachineName.ToLower())
//                {
//                    throw new InvalidOperationException("Input queue must be on the same machine as this process.");
//                }
//                if ((MessageSerializer == null) && !SkipDeserialization)
//                {
//                    throw new InvalidOperationException("No message serializer has been configured.");
//                }
//            }
//        }

//        private void ClearFailuresForMessage(string messageId)
//        {
//            _failuresPerMessageRwLock.EnterReadLock();
//            if (_failuresPerMessage.ContainsKey(messageId))
//            {
//                _failuresPerMessageRwLock.ExitReadLock();
//                _failuresPerMessageRwLock.EnterWriteLock();
//                _failuresPerMessage.Remove(messageId);
//                _failuresPerMessageRwLock.ExitWriteLock();
//            }
//            else
//            {
//                _failuresPerMessageRwLock.ExitReadLock();
//            }
//        }

//        public TransportMessage Convert(Message m)
//    {
//        TransportMessage <>g__initLocal6 = new TransportMessage {
//            Id = m.Id,
//            CorrelationId = (m.CorrelationId == @"00000000-0000-0000-0000-000000000000\0") ? null : m.CorrelationId,
//            Recoverable = m.Recoverable,
//            Lifetime = m.TimeToBeReceived,
//            TimeSent = m.SentTime,
//            ReturnAddress = MsmqUtilities.GetIndependentAddressForQueue(m.ResponseQueue),
//            MessageIntent = Enum.IsDefined(typeof(MessageIntentEnum), m.AppSpecific) ? ((MessageIntentEnum) m.AppSpecific) : MessageIntentEnum.Send
//        };
//        TransportMessage result = <>g__initLocal6;
//        UpdateMessageIdBasedOnResponseFromErrorQueue(result, m);
//        FillIdForCorrelationAndWindowsIdentity(result, m);
//        if (string.IsNullOrEmpty(result.IdForCorrelation))
//        {
//            result.IdForCorrelation = result.Id;
//        }
//        if (m.Extension.Length > 0)
//        {
//            MemoryStream stream = new MemoryStream(m.Extension);
//            object o = headerSerializer.Deserialize(stream);
//            result.Headers = o as List<HeaderInfo>;
//            return result;
//        }
//        result.Headers = new List<HeaderInfo>();
//        return result;
//    }

//        private void CreateQueuesIfNecessary()
//        {
//            if (!DoNotCreateQueues)
//            {
//                MsmqUtilities.CreateQueueIfNecessary(InputQueue);
//                MsmqUtilities.CreateQueueIfNecessary(ErrorQueue);
//            }
//        }
	

//        private IMessage[] Extract(Message message)
//        {
//            return MessageSerializer.Deserialize(message.BodyStream);
//        }

//        #region Label

//        private static void FillIdForCorrelationAndWindowsIdentity(TransportMessage result, Message m)
//        {
//            if (m.Label != null)
//            {
//                if (m.Label.Contains(IDFORCORRELATION))
//                {
//                    int idStartIndex = (m.Label.IndexOf(string.Format("<{0}>", IDFORCORRELATION)) + IDFORCORRELATION.Length) + 2;
//                    int idCount = m.Label.IndexOf(string.Format("</{0}>", IDFORCORRELATION)) - idStartIndex;
//                    result.IdForCorrelation = m.Label.Substring(idStartIndex, idCount);
//                }
//                if (m.Label.Contains(WINDOWSIDENTITYNAME))
//                {
//                    int winStartIndex = (m.Label.IndexOf(string.Format("<{0}>", WINDOWSIDENTITYNAME)) + WINDOWSIDENTITYNAME.Length) + 2;
//                    int winCount = m.Label.IndexOf(string.Format("</{0}>", WINDOWSIDENTITYNAME)) - winStartIndex;
//                    result.WindowsIdentityName = m.Label.Substring(winStartIndex, winCount);
//                }
//            }
//        }

//        private static void FillLabel(Message toSend, TransportMessage m)
//        {
//            toSend.Label = string.Format("<{0}>{2}</{0}><{1}>{3}</{1}>", new object[] { IDFORCORRELATION, WINDOWSIDENTITYNAME, m.IdForCorrelation, m.WindowsIdentityName });
//        }

//        public static string GetLabelWithoutFailedQueue(Message m)
//        {
//            if (m.Label == null)
//            {
//                return null;
//            }
//            if (!m.Label.Contains(FAILEDQUEUE))
//            {
//                return m.Label;
//            }
//            int startIndex = m.Label.IndexOf(string.Format("<{0}>", FAILEDQUEUE));
//            int endIndex = m.Label.IndexOf(string.Format("</{0}>", FAILEDQUEUE)) + (FAILEDQUEUE.Length + 3);
//            return m.Label.Remove(startIndex, endIndex - startIndex);
//        }

//        #endregion

//        public static string GetFailedQueue(Message m)
//        {
//            if (m.Label == null)
//            {
//                return null;
//            }
//            if (!m.Label.Contains(FAILEDQUEUE))
//            {
//                return null;
//            }
//            int startIndex = (m.Label.IndexOf(string.Format("<{0}>", FAILEDQUEUE)) + FAILEDQUEUE.Length) + 2;
//            int count = m.Label.IndexOf(string.Format("</{0}>", FAILEDQUEUE)) - startIndex;
//            return MsmqUtilities.GetFullPath(m.Label.Substring(startIndex, count));
//        }



//        public int GetNumberOfPendingMessages()
//        {
//            MSMQManagementClass qMgmt = new MSMQManagementClass();
//            object machine = Environment.MachineName;
//            object missing = Type.Missing;
//            object formatName = queue.FormatName;
//            qMgmt.Init(ref machine, ref missing, ref formatName);
//            return qMgmt.MessageCount;
//        }

//        #region Msmq

//        private MessageQueueTransactionType GetTransactionTypeForReceive()
//        {
//            return (!IsTransactional ? MessageQueueTransactionType.None : MessageQueueTransactionType.Automatic) ;
//        }

//        private MessageQueueTransactionType GetTransactionTypeForSend()
//        {
//            return (!IsTransactional ? MessageQueueTransactionType.Single : (Transaction.Current == null ? MessageQueueTransactionType.Single : MessageQueueTransactionType.Automatic);
//        }

//        [DebuggerNonUserCode]
//        private bool MessageInQueue()
//        {
//            try { queue.Peek(TimeSpan.FromSeconds((double)SecondsToWaitForMessage)); return true; }
//            catch (MessageQueueException mqe)
//            {
//                if (mqe.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
//                {
//                    if (mqe.MessageQueueErrorCode == MessageQueueErrorCode.AccessDenied)
//                    {
//                        Logger.Fatal(string.Format("Do not have permission to access queue [{0}]. Make sure that the current user [{1}] has permission to Send, Receive, and Peek  from this queue. NServiceBus will now exit.", Address, WindowsIdentity.GetCurrent().Name));
//                        Thread.Sleep(0x2710);
//                        Process.GetCurrentProcess().Kill();
//                    }
//                    Logger.Error("Problem in peeking a message from queue: " + Enum.GetName(typeof(MessageQueueErrorCode), mqe.MessageQueueErrorCode), mqe);
//                }
//                return false;
//            }
//            catch (ObjectDisposedException)
//            {
//                Logger.Fatal("Queue has been disposed. Cannot continue operation. Please restart this process.");
//                Thread.Sleep(0x2710);
//                Process.GetCurrentProcess().Kill();
//                return false;
//            }
//            catch (Exception e) { Logger.Error("Error in peeking a message from queue.", e); return false; }
//        }

//        #endregion

//        private bool HandledMaxRetries(string messageId)
//        {
//            _failuresPerMessageRwLock.EnterReadLock();
//            if (_failuresPerMessage.ContainsKey(messageId) && (_failuresPerMessage[messageId] >= maxRetries))
//            {
//                _failuresPerMessageRwLock.ExitReadLock();
//                _failuresPerMessageRwLock.EnterWriteLock();
//                _failuresPerMessage.Remove(messageId);
//                _failuresPerMessageRwLock.ExitWriteLock();
//                return true;
//            }
//            _failuresPerMessageRwLock.ExitReadLock();
//            return false;
//        }

//        private void IncrementFailuresForMessage(string messageId)
//        {
//            try
//            {
//                _failuresPerMessageRwLock.EnterWriteLock();
//                if (!_failuresPerMessage.ContainsKey(messageId))
//                    _failuresPerMessage[messageId] = 1;
//                else
//                    _failuresPerMessage[messageId] += 1;
//            }
//            catch { }
//            finally { _failuresPerMessageRwLock.ExitWriteLock(); }
//        }


//        protected void MoveToErrorQueue(IServiceMessage m)
//        {
//            m.Label = m.Label + string.Format("<{0}>{1}</{0}>", FAILEDQUEUE, MsmqUtilities.GetIndependentAddressForQueue(queue));
//            if (!m.Label.Contains(ORIGINALID))
//                m.Label = m.Label + string.Format("<{0}>{1}</{0}>", ORIGINALID, m.Id);
//            if (errorQueue != null)
//                errorQueue.Send(m, MessageQueueTransactionType.Single);
//        }


//        [DebuggerNonUserCode]
//        private IServiceMessage ReceiveMessageFromQueueAfterPeekWasSuccessful()
//        {
//            try { return queue.Receive(TimeSpan.FromSeconds((double)SecondsToWaitForMessage), GetTransactionTypeForReceive()); }
//            catch (MessageQueueException mqe)
//            {
//                if (mqe.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
//                    Logger.Error("Problem in receiving message from queue: " + Enum.GetName(typeof(MessageQueueErrorCode), mqe.MessageQueueErrorCode), mqe);
//                return null;
//            }
//            catch (Exception e)
//            {
//                Logger.Error("Error in receiving message from queue.", e);
//                return null;
//            }
//        }

//        public void ReceiveMessageLater(TransportMessage m)
//        {
//            if (!string.IsNullOrEmpty(InputQueue))
//                Send(m, InputQueue);
//        }

//        public void Send(TransportMessage m, string destination)
//        {
//            using (MessageQueue q = new MessageQueue(MsmqUtilities.GetFullPath(destination), QueueAccessMode.Send))
//            {
//                Message toSend = new Message();
//                if ((m.Body == null) && (m.BodyStream != null))
//                    toSend.BodyStream = m.BodyStream;
//                else
//                    MessageSerializer.Serialize(m.Body, toSend.BodyStream);
//                if (m.CorrelationId != null)
//                    toSend.CorrelationId = m.CorrelationId;
//                toSend.Recoverable = m.Recoverable;
//                toSend.UseDeadLetterQueue = UseDeadLetterQueue;
//                toSend.UseJournalQueue = UseJournalQueue;
//                if (!string.IsNullOrEmpty(m.ReturnAddress))
//                    toSend.ResponseQueue = new MessageQueue(MsmqUtilities.GetFullPath(m.ReturnAddress));
//                FillLabel(toSend, m);
//                if (m.Lifetime < MessageQueue.InfiniteTimeout)
//                    toSend.TimeToBeReceived = m.Lifetime;
//                if ((m.Headers != null) && (m.Headers.Count > 0))
//                    using (MemoryStream stream = new MemoryStream())
//                    {
//                        headerSerializer.Serialize((Stream)stream, m.Headers);
//                        toSend.Extension = stream.GetBuffer();
//                    }
//                toSend.AppSpecific = (int)m.MessageIntent;
//                try { q.Send(toSend, GetTransactionTypeForSend()); }
//                catch (MessageQueueException ex)
//                {
//                    if (ex.MessageQueueErrorCode == MessageQueueErrorCode.QueueNotFound)
//                        throw new ConfigurationErrorsException("The destination queue '" + destination + "' could not be found. You may have misconfigured the destination for this kind of message (" + m.Body[0].GetType().FullName + ") in the MessageEndpointMappings of the UnicastBusConfig section in your configuration file.It may also be the case that the given queue just hasn't been created yet, or has been deleted.", ex);
//                    throw;
//                }
//                m.Id = toSend.Id;
//            }
//        }

//        private void SetLocalQueue(MessageQueue q)
//        {
//            bool queueIsTransactional;
//            try { queueIsTransactional = q.Transactional; }
//            catch (Exception ex) { throw new InvalidOperationException(string.Format("There is a problem with the input queue given: {0}. See the enclosed exception for details.", q.Path), ex); }
//            if (IsTransactional && !queueIsTransactional)
//                throw new ArgumentException("Queue must be transactional (" + q.Path + ").");
//            queue = q;
//            MessagePropertyFilter mpf = new MessagePropertyFilter();
//            mpf.SetAll();
//            queue.MessageReadPropertyFilter = mpf;
//        }

//        public void Start()
//        {
//            CheckConfiguration();
//            CreateQueuesIfNecessary();
//            if (!string.IsNullOrEmpty(ErrorQueue))
//                errorQueue = new MessageQueue(MsmqUtilities.GetFullPath(ErrorQueue));
//            if (!string.IsNullOrEmpty(InputQueue))
//            {
//                MessageQueue q = new MessageQueue(MsmqUtilities.GetFullPath(InputQueue));
//                SetLocalQueue(q);
//                if (PurgeOnStartup)
//                    queue.Purge();
//                LimitWorkerThreadsToOne();
//                for (int i = 0; i < numberOfWorkerThreads; i++)
//                    AddWorkerThread().Start();
//            }
//        }

//        private static void UpdateMessageIdBasedOnResponseFromErrorQueue(TransportMessage result, Message m)
//        {
//            if ((m.Label != null) && m.Label.Contains(ORIGINALID))
//            {
//                int idStartIndex = (m.Label.IndexOf(string.Format("<{0}>", ORIGINALID)) + ORIGINALID.Length) + 2;
//                int idCount = m.Label.IndexOf(string.Format("</{0}>", ORIGINALID)) - idStartIndex;
//                result.Id = m.Label.Substring(idStartIndex, idCount);
//            }
//        }

//        public string Address
//        {
//            get { return InputQueue; }
//        }

//        public bool DoNotCreateQueues { get; set; }

//        public string ErrorQueue { get; set; }

//        public string InputQueue { get; set; }

//        public IsolationLevel IsolationLevel { get; set; }

//        public bool IsTransactional { get; set; }

//        public int MaxRetries
//        {
//            get
//            {
//                return maxRetries;
//            }
//            set
//            {
//                maxRetries = value;
//            }
//        }

//        public IMessageSerializer MessageSerializer { get; set; }

//        public virtual int NumberOfWorkerThreads
//        {
//            get
//            {
//                lock (workerThreads)
//                {
//                    return workerThreads.Count;
//                }
//            }
//            set
//            {
//                numberOfWorkerThreads = value;
//            }
//        }

//        public bool PurgeOnStartup { get; set; }

//        public int SecondsToWaitForMessage
//        {
//            get
//            {
//                return secondsToWaitForMessage;
//            }
//            set
//            {
//                secondsToWaitForMessage = value;
//            }
//        }

//        public bool SkipDeserialization { get; set; }
//        public TimeSpan TransactionTimeout { get; set; }
//        public bool UseDeadLetterQueue { get; set; }
//        public bool UseJournalQueue { get; set; }
//    }
//}