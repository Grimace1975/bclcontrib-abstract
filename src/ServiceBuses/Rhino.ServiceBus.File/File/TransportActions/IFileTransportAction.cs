using Rhino.ServiceBus.FileMessaging;
using Rhino.ServiceBus.Internal;

namespace Rhino.ServiceBus.File.TransportActions
{
    public interface IFileTransportAction
    {
        void Init(IFileTransport transport, OpenedQueue queue);
        bool CanHandlePeekedMessage(Message message);
        bool HandlePeekedMessage(IFileTransport transport, OpenedQueue queue, Message message);
    }
}