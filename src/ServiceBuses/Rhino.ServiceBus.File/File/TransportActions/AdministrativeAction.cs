using System;
using Rhino.ServiceBus.FileMessaging;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using MessageType = Rhino.ServiceBus.Transport.MessageType;

namespace Rhino.ServiceBus.File.TransportActions
{
    public class AdministrativeAction : AbstractTransportAction
    {
        private IFileTransport transport;

        public override MessageType HandledType
        {
            get { return MessageType.AdministrativeMessageMarker; }
        }

        public override void Init(IFileTransport parentTransport, OpenedQueue queue)
        {
            transport = parentTransport;
        }

        public override bool HandlePeekedMessage(IFileTransport transport1, OpenedQueue queue, Message message)
        {
            Func<CurrentMessageInformation, bool> messageRecieved = information =>
            {
                transport.RaiseAdministrativeMessageArrived(information);
                return true;
            };
            transport.ReceiveMessageInTransaction(queue, message.Id, messageRecieved, transport.RaiseAdministrativeMessageProcessingCompleted, null, null);
            return true;
        }
    }
}