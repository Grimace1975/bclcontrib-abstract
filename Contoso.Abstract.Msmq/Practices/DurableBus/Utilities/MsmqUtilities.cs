//#region License
///*
//The MIT License

//Copyright (c) 2008 Sky Morey

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
//*/
//#endregion
//namespace Contoso.Practices.DurableBus.Utilities
//{
//    /// <summary>
//    /// IDurableBusContext
//    /// </summary>
//    public class MsmqUtilities
//{
//    // Fields
//    private const string DIRECTPREFIX = "DIRECT=OS:";
//    private static readonly string DIRECTPREFIX_TCP = "DIRECT=TCP:";
//    private static readonly string LocalAdministratorsGroupName = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Translate(typeof(NTAccount)).ToString();
//    private static readonly string LocalAnonymousLogonName = new SecurityIdentifier(WellKnownSidType.AnonymousSid, null).Translate(typeof(NTAccount)).ToString();
//    private static readonly string LocalEveryoneGroupName = new SecurityIdentifier(WellKnownSidType.WorldSid, null).Translate(typeof(NTAccount)).ToString();
//    private static readonly ILog Logger = LogManager.GetLogger(typeof(MsmqUtilities));
//    private static readonly string PREFIX = "FormatName:DIRECT=OS:";
//    private static readonly string PREFIX_TCP = ("FormatName:" + DIRECTPREFIX_TCP);
//    private const string PRIVATE = @"\private$\";

//    // Methods
//    public static void CreateQueue(string queueName)
//    {
//        MessageQueue createdQueue = MessageQueue.Create(queueName, true);
//        createdQueue.SetPermissions(LocalAdministratorsGroupName, MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);
//        createdQueue.SetPermissions(LocalEveryoneGroupName, MessageQueueAccessRights.WriteMessage, AccessControlEntryType.Allow);
//        createdQueue.SetPermissions(LocalAnonymousLogonName, MessageQueueAccessRights.WriteMessage, AccessControlEntryType.Allow);
//        Logger.Debug("Queue created: " + queueName);
//    }

//    public static void CreateQueueIfNecessary(string queueName)
//    {
//        if (!string.IsNullOrEmpty(queueName))
//        {
//            string q = GetFullPathWithoutPrefix(queueName);
//            if (GetMachineNameFromLogicalName(queueName) != Environment.MachineName)
//            {
//                Logger.Debug("Queue is on remote machine.");
//                Logger.Debug("If this does not succeed (like if the remote machine is disconnected), processing will continue.");
//            }
//            Logger.Debug(string.Format("Checking if queue exists: {0}.", queueName));
//            try
//            {
//                if (!MessageQueue.Exists(q))
//                {
//                    Logger.Warn("Queue " + q + " does not exist.");
//                    Logger.Debug("Going to create queue: " + q);
//                    CreateQueue(q);
//                }
//            }
//            catch (Exception ex)
//            {
//                Logger.Error(string.Format("Could not create queue {0} or check its existence. Processing will still continue.", queueName), ex);
//            }
//        }
//    }

//    public static string GetFullPath(string value)
//    {
//        IPAddress ipAddress;
//        if (IPAddress.TryParse(GetMachineNameFromLogicalName(value), out ipAddress))
//        {
//            return (PREFIX_TCP + GetFullPathWithoutPrefix(value));
//        }
//        return (PREFIX + GetFullPathWithoutPrefix(value));
//    }

//    public static string GetFullPathWithoutPrefix(string value)
//    {
//        return (GetMachineNameFromLogicalName(value) + @"\private$\" + GetQueueNameFromLogicalName(value));
//    }

//    public static string GetIndependentAddressForQueue(MessageQueue q)
//    {
//        string CS$1$0000;
//        if (q == null)
//        {
//            return null;
//        }
//        string[] arr = q.FormatName.Split(new char[] { '\\' });
//        string queueName = arr[arr.Length - 1];
//        int directPrefixIndex = arr[0].IndexOf("DIRECT=OS:");
//        if (directPrefixIndex >= 0)
//        {
//            return (queueName + '@' + arr[0].Substring(directPrefixIndex + "DIRECT=OS:".Length));
//        }
//        try
//        {
//            arr = q.QueueName.Split(new char[] { '\\' });
//            queueName = arr[arr.Length - 1];
//            CS$1$0000 = queueName + '@' + q.MachineName;
//        }
//        catch
//        {
//            throw new Exception("MessageQueueException: '" + "DIRECT=OS:" + "' is missing. " + "FormatName='" + q.FormatName + "'");
//        }
//        return CS$1$0000;
//    }

//    public static string GetMachineNameFromLogicalName(string logicalName)
//    {
//        string[] arr = logicalName.Split(new char[] { '@' });
//        string machine = Environment.MachineName;
//        if (((arr.Length >= 2) && (arr[1] != ".")) && (arr[1].ToLower() != "localhost"))
//        {
//            machine = arr[1];
//        }
//        return machine;
//    }

//    public static string GetQueueNameFromLogicalName(string logicalName)
//    {
//        string[] arr = logicalName.Split(new char[] { '@' });
//        if (arr.Length >= 1)
//        {
//            return arr[0];
//        }
//        return null;
//    }

//    public static bool QueueIsLocal(string value)
//    {
//        string machineName = Environment.MachineName.ToLower();
//        value = value.ToLower().Replace(PREFIX.ToLower(), "");
//        int index = value.IndexOf('\\');
//        string queueMachineName = value.Substring(0, index).ToLower();
//        if (!(machineName == queueMachineName) && !(queueMachineName == "localhost"))
//        {
//            return (queueMachineName == ".");
//        }
//        return true;
//    }
//}
//}