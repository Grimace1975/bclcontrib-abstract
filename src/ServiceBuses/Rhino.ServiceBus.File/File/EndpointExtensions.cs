using System;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.FileMessaging;

namespace Rhino.ServiceBus.File
{
    public static class EndpointExtensions
    {
        public static OpenedQueue InitalizeQueue(this Endpoint endpoint)
        {
            try { return FileUtil.GetQueuePath(endpoint).Open(QueueAccessMode.SendAndReceive); }
            catch (Exception e)
            {
                throw new TransportException(
                    "Could not open queue: " + endpoint + Environment.NewLine +
                    "Queue path: " + FileUtil.GetQueuePath(endpoint) + Environment.NewLine +
                    "Did you forget to create the queue or disable the queue initialization module?", e);
            }
        }
    }
}