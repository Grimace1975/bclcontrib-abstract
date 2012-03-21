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
    /// ISPTraceServiceLog
    /// </summary>
    public interface ISPTraceServiceLog : IServiceLog
    {
        ITraceLogger Log { get; }
        int EventID { get; }
    }

    /// <summary>
    /// SPTraceServiceLog
    /// </summary>
    public class SPTraceServiceLog : ISPTraceServiceLog, ServiceLogManager.ISetupRegistration
    {
        static SPTraceServiceLog() { ServiceLogManager.EnsureRegistration(); }
        public SPTraceServiceLog()
            : this(new TraceLogger(), 0, null) { }
        public SPTraceServiceLog(string name)
            : this(new TraceLogger(), 0, name) { }
        public SPTraceServiceLog(int eventID)
            : this(new TraceLogger(), eventID, null) { }
        public SPTraceServiceLog(int eventID, string name)
            : this(new TraceLogger(), eventID, name) { }
        public SPTraceServiceLog(ITraceLogger log, int eventID, string name)
        {
            if (log == null)
                throw new ArgumentNullException("log");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            Log = log;
            EventID = eventID;
            Name = name;
        }

        Action<IServiceLocator, string> ServiceLogManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (locator, name) => ServiceLogManager.RegisterInstance<ISPTraceServiceLog>(this, locator, name); }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        // get
        public string Name { get; private set; }
        public IServiceLog Get(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            return new SPTraceServiceLog(Log, EventID, Name + "." + name);
        }

        // log
        public void Write(ServiceLog.LogLevel level, Exception ex, string s, params object[] args)
        {
            if (Log == null)
                throw new NullReferenceException("Log");
            s = (!string.IsNullOrEmpty(s) ? string.Format(CultureInfo.CurrentCulture, s, args) : string.Empty);
            if (ex != null)
                s = GetExceptionMessage(ex, s);
            switch (level)
            {
                case ServiceLog.LogLevel.Fatal: Log.Trace(s, EventID, TraceSeverity.Unexpected, Name); return;
                case ServiceLog.LogLevel.Error: Log.Trace(s, EventID, TraceSeverity.High, Name); return;
                case ServiceLog.LogLevel.Warning: Log.Trace(s, EventID, TraceSeverity.Medium, Name); return;
                case ServiceLog.LogLevel.Information: Log.Trace(s, EventID, TraceSeverity.Monitorable, Name); return;
                case ServiceLog.LogLevel.Debug: Log.Trace(s, EventID, TraceSeverity.Verbose, Name); return;
                default: return;
            }
        }

        #region Domain-specific

        public ITraceLogger Log { get; private set; }
        public int EventID { get; private set; }

        #endregion

        protected virtual string GetExceptionMessage(Exception ex, string s)
        {
            return ex.Message;
        }
    }
}
