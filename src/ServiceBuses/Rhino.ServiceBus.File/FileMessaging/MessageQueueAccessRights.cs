using System;
namespace Rhino.ServiceBus.FileMessaging
{
    [Flags]
    public enum MessageQueueAccessRights
    {
        DeleteMessage = 1,
        PeekMessage = 2,
        ReceiveMessage = 3,
        WriteMessage = 4,
        DeleteJournalMessage = 8,
        ReceiveJournalMessage = 10,
        SetQueueProperties = 16,
        GetQueueProperties = 32,
        DeleteQueue = 65536,
        GetQueuePermissions = 131072,
        GenericWrite = 131108,
        GenericRead = 131115,
        ChangeQueuePermissions = 262144,
        TakeQueueOwnership = 524288,
        FullControl = 983103,
    }
}
