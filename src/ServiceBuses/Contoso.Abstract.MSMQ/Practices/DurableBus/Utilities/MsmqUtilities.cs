//#region License
// *
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
//using System.Abstract;
//using System.Security.Principal;
//using System;
//using System.Net;
//namespace Contoso.Practices.DurableBus.Utilities
//{
//    /// <summary>
//    /// MsmqUtilities
//    /// </summary>
//    public class MsmqUtilities
//    {
//        private static readonly IServiceLog ServiceLog = ServiceLogManager.Get<MsmqUtilities>();
//        private const string DIRECTPREFIX = "DIRECT=OS:";
//        private static readonly string DIRECTPREFIX_TCP = "DIRECT=TCP:";
//        private static readonly string PREFIX = "FormatName:DIRECT=OS:";
//        private static readonly string PREFIX_TCP = "FormatName:" + DIRECTPREFIX_TCP;
//        private const string PRIVATE = "\\private$\\";

//        public static void CreateQueue(string queueName)
//        {
//            var localAdministratorsGroupName = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Translate(typeof(NTAccount)).ToString();
//            var localAnonymousLogonName = new SecurityIdentifier(WellKnownSidType.AnonymousSid, null).Translate(typeof(NTAccount)).ToString();
//            var localEveryoneGroupName = new SecurityIdentifier(WellKnownSidType.WorldSid, null).Translate(typeof(NTAccount)).ToString();
//            //
//            MessageQueue createdQueue = MessageQueue.Create(queueName, true);
//            createdQueue.SetPermissions(localAdministratorsGroupName, MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);
//            createdQueue.SetPermissions(localEveryoneGroupName, MessageQueueAccessRights.WriteMessage, AccessControlEntryType.Allow);
//            createdQueue.SetPermissions(localAnonymousLogonName, MessageQueueAccessRights.WriteMessage, AccessControlEntryType.Allow);
//            ServiceLog.Debug("Queue created: " + queueName);
//        }

//        public static void CreateQueueIfNecessary(string queueName)
//        {
//            var logicalName = new LogicalName(queueName);
//            if (!string.IsNullOrEmpty(queueName))
//            {
//                string q = logicalName.FullPathWithoutPrefix;
//                if (!logicalName.IsLocal)
//                {
//                    ServiceLog.Debug("Queue is on remote machine.");
//                    ServiceLog.Debug("If this does not succeed (like if the remote machine is disconnected), processing will continue.");
//                }
//                ServiceLog.Debug(string.Format("Checking if queue exists: {0}.", queueName));
//                try
//                {
//                    if (!MessageQueue.Exists(q))
//                    {
//                        ServiceLog.Warning("Queue " + q + " does not exist.");
//                        ServiceLog.Debug("Going to create queue: " + q);
//                        CreateQueue(q);
//                    }
//                }
//                catch (Exception ex) { ServiceLog.Error(string.Format("Could not create queue {0} or check its existence. Processing will still continue.", queueName), ex); }
//            }
//        }

//        public static string GetIndependentAddressForQueue(MessageQueue q)
//        {
//            string address;
//            if (q == null)
//                return null;
//            string[] arr = q.FormatName.Split(new char[] { '\\' });
//            string queueName = arr[arr.Length - 1];
//            int directPrefixIndex = arr[0].IndexOf("DIRECT=OS:");
//            if (directPrefixIndex >= 0)
//                return (queueName + '@' + arr[0].Substring(directPrefixIndex + "DIRECT=OS:".Length));
//            try
//            {
//                arr = q.QueueName.Split(new char[] { '\\' });
//                queueName = arr[arr.Length - 1];
//                address = queueName + '@' + q.MachineName;
//            }
//            catch { throw new Exception("MessageQueueException: '" + "DIRECT=OS:" + "' is missing. " + "FormatName='" + q.FormatName + "'"); }
//            return address;
//        }

//        public class LogicalName
//        {
//            public LogicalName(string name) { Name = name; }
//            public string Name { get; private set; }

//            public string FullPath
//            {
//                get { IPAddress ipAddress; return (IPAddress.TryParse(MachineName, out ipAddress) ? PREFIX_TCP : PREFIX) + FullPathWithoutPrefix; }
//            }

//            public string FullPathWithoutPrefix
//            {
//                get { return MachineName + PRIVATE + QueueName; }
//            }

//            public string MachineName
//            {
//                get
//                {
//                    string machine = Environment.MachineName;
//                    var parts = Name.Split(new char[] { '@' });
//                    return (((parts.Length >= 2) && (parts[1] != ".")) && (parts[1].ToLowerInvariant() != "localhost") ? parts[1] : machine);
//                }
//            }

//            public string QueueName
//            {
//                get
//                {
//                    var parts = Name.Split(new char[] { '@' });
//                    return (parts.Length >= 1 ? parts[0] : null);
//                }
//            }

//            public bool IsLocal
//            {
//                get
//                {
//                    string machineName = Environment.MachineName.ToLowerInvariant();
//                    var normalizedName = Name.ToLowerInvariant().Replace(PREFIX.ToLowerInvariant(), string.Empty);
//                    int index = normalizedName.IndexOf('\\');
//                    string queueMachineName = normalizedName.Substring(0, index);
//                    return ((machineName == queueMachineName) || (queueMachineName == "localhost") || (queueMachineName == "."));
//                }
//            }
//        }

//    }
//}