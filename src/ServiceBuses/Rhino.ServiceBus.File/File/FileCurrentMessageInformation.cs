using System.Collections.Specialized;
using Rhino.ServiceBus.FileMessaging;
using Rhino.ServiceBus.Impl;

namespace Rhino.ServiceBus.File
{
    public class FileCurrentMessageInformation : CurrentMessageInformation
    {
        public OpenedQueue Queue { get; set; }
        public Message FileMessage { get; set; }
        public NameValueCollection Headers { get; set; }
        public MessageQueueTransactionType TransactionType { get; set; }
    }
}