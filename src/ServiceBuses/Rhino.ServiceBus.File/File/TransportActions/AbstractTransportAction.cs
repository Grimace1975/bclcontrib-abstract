using Rhino.ServiceBus.FileMessaging;
using Rhino.ServiceBus.Internal;
using MessageType = Rhino.ServiceBus.Transport.MessageType;

namespace Rhino.ServiceBus.File.TransportActions
{
    public abstract class AbstractTransportAction : IFileTransportAction
    {
        public abstract MessageType HandledType { get; }

        public virtual void Init(IFileTransport transport, OpenedQueue queue) { }

        public bool CanHandlePeekedMessage(Message message)
        {
            var messagType = (MessageType)message.AppSpecific;
            return messagType == HandledType;
        }

        public abstract bool HandlePeekedMessage(IFileTransport transport, OpenedQueue queue, Message message);
    }
}