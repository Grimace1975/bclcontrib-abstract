using Rhino.ServiceBus.FileMessaging;
using Rhino.ServiceBus.Internal;
using MessageType = Rhino.ServiceBus.Transport.MessageType;

namespace Rhino.ServiceBus.File.TransportActions
{
    public class ShutDownAction : AbstractTransportAction
    {
        public override MessageType HandledType
        {
            get { return MessageType.ShutDownMessageMarker; }
        }

        public override bool HandlePeekedMessage(IFileTransport transport, OpenedQueue queue, Message message)
        {
            queue.TryGetMessageFromQueue(message.Id);
            return true;
        }
    }
}