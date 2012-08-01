using System;
using Rhino.ServiceBus.File;
using Rhino.ServiceBus.FileMessaging;
using Rhino.ServiceBus.Impl;

namespace Rhino.ServiceBus.Internal
{
    public interface IFileTransport : ITransport
    {
        void RaiseAdministrativeMessageProcessingCompleted(CurrentMessageInformation information, Exception ex);
        bool RaiseAdministrativeMessageArrived(CurrentMessageInformation information);
        void ReceiveMessageInTransaction(OpenedQueue queue,
            string messageId,
            Func<CurrentMessageInformation, bool> messageArrived,
            Action<CurrentMessageInformation, Exception> messageProcessingCompleted,
            Action<CurrentMessageInformation> beforeMessageTransactionCommit,
            Action<CurrentMessageInformation> beforeMessageTransactionRollback);
        void RaiseMessageSerializationException(OpenedQueue queue, Message msg, string errorMessage);
        OpenedQueue CreateQueue();
    }
}