using System;

namespace Rhino.ServiceBus.Internal
{
    internal interface ITransaction
    {
        int Commit(int fRetaining, int grfTC, int grfRM);
        int Abort(int pboidReason, int fRetaining, int fAsync);
        int GetTransactionInfo(IntPtr pinfo);
    }
}