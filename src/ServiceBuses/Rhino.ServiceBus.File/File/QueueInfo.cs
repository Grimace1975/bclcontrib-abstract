using System;
using System.ComponentModel;
using Rhino.ServiceBus.Transport;
using Rhino.ServiceBus.FileMessaging;

namespace Rhino.ServiceBus.File
{
    public class QueueInfo
    {
        private string queuePath;
        public string QueuePath
        {
            get { return queuePath; }
            set
            {
                if (value.Contains(";"))
                {
                    var parts = value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    queuePath = parts[0];
                    SubQueue = (SubQueue)Enum.Parse(typeof(SubQueue), parts[1], true);
                }
                else
                    queuePath = value;
            }
        }

        public SubQueue? SubQueue { get; set; }
        public Uri QueueUri { get; set; }
        public bool? Transactional { get; set; }
        public bool IsLocal { get; set; }

        public bool Exists
        {
            get
            {
                if (IsLocal)
                    return MessageQueue.Exists(QueuePath);
                return true; // we assume that remote queues exists
            }
        }

        public string QueuePathWithSubQueue
        {
            get
            {
                if (SubQueue == null)
                    return QueuePath;
                return QueuePath + ";" + SubQueue;
            }
        }

        public OpenedQueue Open() { return Open(QueueAccessMode.SendAndReceive); }
        public OpenedQueue Open(QueueAccessMode access) { return Open(access, null); }
        public OpenedQueue Open(QueueAccessMode access, IMessageFormatter formatter)
        {
            var messageQueue = new MessageQueue(QueuePath, access);
            if (formatter != null)
                messageQueue.Formatter = formatter;
            var openedQueue = new OpenedQueue(this, messageQueue, QueueUri.ToString(), Transactional)
            {
                Formatter = formatter
            };
            if (SubQueue != null)
                return openedQueue.OpenSubQueue(SubQueue.Value, access);
            return openedQueue;
        }

        public void Delete()
        {
            if (Exists && IsLocal)
                MessageQueue.Delete(QueuePath);
        }

        public MessageQueue Create()
        {
            if (!IsLocal || Exists)
                return new MessageQueue(queuePath);
            try { return FileUtil.CreateQueue(queuePath, Transactional ?? true); }
            catch (Exception e) { throw new InvalidAsynchronousStateException("Could not create queue: " + QueueUri, e); }
        }

        public override string ToString() { return this.QueueUri.ToString(); }
    }
}
