#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System;
using System.Abstract;
using System.Globalization;
using Microsoft.Practices.SharePoint.Common.Logging;
using Microsoft.SharePoint.Administration;
namespace Contoso.Abstract
{
    /// <summary>
    /// ISPServiceLog
    /// </summary>
    public interface ISPServiceLog : IServiceLog
    {
        ILogger Log { get; }
        int EventID { get; }
        string AreaName { get; }
        string Category { get; }
    }

    /// <summary>
    /// SPServiceLog
    /// </summary>
    public class SPServiceLog : ISPServiceLog, ServiceLogManager.ISetupRegistration
    {
        static SPServiceLog() { ServiceLogManager.EnsureRegistration(); }
        public SPServiceLog()
            : this(new SharePointLogger(), 0, "Undefined", "General") { }
        public SPServiceLog(string areaName, string category)
            : this(new SharePointLogger(), 0, areaName, category) { }
        public SPServiceLog(int eventID)
            : this(new SharePointLogger(), eventID, "Undefined", "General") { }
        public SPServiceLog(int eventID, string areaName, string category)
            : this(new SharePointLogger(), eventID, areaName, category) { }
        public SPServiceLog(ILogger log, int eventID, string areaName, string category)
        {
            if (log == null)
                throw new ArgumentNullException("log");
            if (string.IsNullOrEmpty(areaName))
                throw new ArgumentNullException("areaName");
            if (string.IsNullOrEmpty(category))
                throw new ArgumentNullException("category");
            Log = log;
            EventID = eventID;
            AreaName = areaName;
            Category = category;
        }

        Action<IServiceLocator, string> ServiceLogManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (locator, name) => ServiceLogManager.RegisterInstance<ISPServiceLog>(this, locator, name); }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        // get
        public string Name
        {
            get { return AreaName + "/" + Category; }
        }
        public IServiceLog Get(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            return new SPServiceLog(Log, EventID, AreaName, name);
        }

        // log
        public void Write(ServiceLog.LogLevel level, Exception ex, string s, params object[] args)
        {
            if (Log == null)
                throw new NullReferenceException("Log");
            s = (!string.IsNullOrEmpty(s) ? string.Format(CultureInfo.CurrentCulture, s, args) : string.Empty);
            if (ex == null)
                switch (level)
                {
                    case ServiceLog.LogLevel.Fatal: Log.LogToOperations(s, EventID, EventSeverity.ErrorCritical, Name); return;
                    case ServiceLog.LogLevel.Error: Log.LogToOperations(s, EventID, EventSeverity.Error, Name); return;
                    case ServiceLog.LogLevel.Warning: Log.LogToOperations(s, EventID, EventSeverity.Warning, Name); return;
                    case ServiceLog.LogLevel.Information: Log.LogToOperations(s, EventID, EventSeverity.Information, Name); return;
                    case ServiceLog.LogLevel.Debug: Log.LogToOperations(s, EventID, EventSeverity.Verbose, Name); return;
                    default: return;
                }
            else
                switch (level)
                {
                    case ServiceLog.LogLevel.Fatal: Log.LogToOperations(ex, s, EventID, EventSeverity.ErrorCritical, Name); return;
                    case ServiceLog.LogLevel.Error: Log.LogToOperations(ex, s, EventID, EventSeverity.Error, Name); return;
                    case ServiceLog.LogLevel.Warning: Log.LogToOperations(ex, s, EventID, EventSeverity.Warning, Name); return;
                    case ServiceLog.LogLevel.Information: Log.LogToOperations(ex, s, EventID, EventSeverity.Information, Name); return;
                    case ServiceLog.LogLevel.Debug: Log.LogToOperations(ex, s, EventID, EventSeverity.Verbose, Name); return;
                    default: return;
                }
        }

        #region Domain-specific

        public ILogger Log { get; private set; }
        public int EventID { get; private set; }
        public string AreaName { get; private set; }
        public string Category { get; private set; }

        #endregion
    }
}
