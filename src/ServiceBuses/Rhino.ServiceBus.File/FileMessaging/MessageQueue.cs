namespace Rhino.ServiceBus.FileMessaging
{
    public class MessageQueue
    {
        private string QueuePath;
        private QueueAccessMode access;
        private string queuePath;

        public MessageQueue(string QueuePath, QueueAccessMode access)
        {
            this.QueuePath = QueuePath;
            this.access = access;
        }

        public MessageQueue(string queuePath)
        {
            this.queuePath = queuePath;
        }

        internal static void Delete(string QueuePath)
        {
            throw new System.NotImplementedException();
        }

        public IMessageFormatter Formatter { get; set; }

        internal static bool Exists(string QueuePath)
        {
            throw new System.NotImplementedException();
        }

        public bool Transactional { get; set; }

        internal static MessageQueue Create(string newQueuePath, bool transactional)
        {
            throw new System.NotImplementedException();
        }

        internal void SetPermissions(string administratorsGroupName, MessageQueueAccessRights messageQueueAccessRights, AccessControlEntryType accessControlEntryType)
        {
            throw new System.NotImplementedException();
        }

        public string Path { get; set; }

        internal void Dispose()
        {
            throw new System.NotImplementedException();
        }

        internal Message[] GetAllMessages()
        {
            throw new System.NotImplementedException();
        }

        internal MessageEnumerator GetMessageEnumerator2()
        {
            throw new System.NotImplementedException();
        }

        internal Message ReceiveById(string id, MessageQueueTransactionType messageQueueTransactionType)
        {
            throw new System.NotImplementedException();
        }

        internal void Send(Message message, MessageQueueTransactionType messageQueueTransactionType)
        {
            throw new System.NotImplementedException();
        }

        internal Message Receive(MessageQueueTransactionType messageQueueTransactionType)
        {
            throw new System.NotImplementedException();
        }

        internal Message Peek(System.TimeSpan timeout)
        {
            throw new System.NotImplementedException();
        }

        internal static bool IsFatalError(int num)
        {
            throw new System.NotImplementedException();
        }
    }
}
