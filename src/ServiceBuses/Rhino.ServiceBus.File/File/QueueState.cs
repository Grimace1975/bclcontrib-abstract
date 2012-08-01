using System.Threading;
using Rhino.ServiceBus.FileMessaging;

namespace Rhino.ServiceBus.File
{
    public class QueueState
    {
        public MessageQueue Queue;
        public ManualResetEvent WaitHandle;
    }
}