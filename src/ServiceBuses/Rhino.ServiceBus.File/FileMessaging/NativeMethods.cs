using System.IO;
using Rhino.ServiceBus.Internal;
using System.Text;
using System;
namespace Rhino.ServiceBus.FileMessaging
{
    public static class NativeMethods
    {
        internal static int MQBeginTransaction(out ITransaction iTransaction)
        {
            iTransaction = null;
            return -1;
        }

        internal static int FormatMessage(int p, IntPtr intPtr, int error, int p_2, StringBuilder lpBuffer, int p_3, IntPtr intPtr_2)
        {
            return -1;
        }
    }
}
