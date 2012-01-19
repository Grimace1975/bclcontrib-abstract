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
using System.Diagnostics;
using System.Globalization;
namespace Contoso.Abstract
{
    /// <summary>
    /// IEventLogServiceLog
    /// </summary>
    public interface IEventLogServiceLog : IServiceLog
    {
        EventLog Log { get; }
    }

    /// <summary>
    /// EventLogServiceLog
    /// </summary>
    public class EventLogServiceLog : IEventLogServiceLog, IDisposable, ServiceLogManager.ISetupRegistration
    {
        static EventLogServiceLog() { ServiceLogManager.EnsureRegistration(); }
        public EventLogServiceLog(string name)
            : this(name, "default") { }
        public EventLogServiceLog(string name, string source)
        {
            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, name);
            Name = name;
            Log = new EventLog(name) { Source = source };
        }
        public EventLogServiceLog(string name, string machineName, string source)
        {
            if (!EventLog.SourceExists(source, machineName))
                EventLog.CreateEventSource(new EventSourceCreationData(source, name) { MachineName = machineName });
            Name = name;
            Log = new EventLog(name, machineName, source);
        }
        ~EventLogServiceLog()
        {
            try { ((IDisposable)this).Dispose(); }
            catch { }
        }

        public void Dispose()
        {
            if (Log != null)
            {
                GC.SuppressFinalize(this);
                try { Log.Dispose(); }
                finally { Log = null; }
            }
        }

        Action<IServiceLocator, string> ServiceLogManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (locator, name) => ServiceLogManager.RegisterInstance<IEventLogServiceLog>(this, locator, name); }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        // get
        public string Name { get; private set; }
        public IServiceLog Get(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            return new EventLogServiceLog(Log.Log, Log.MachineName, Log.Source);
        }

        // log
        public void Write(ServiceLog.LogLevel level, Exception ex, string s, params object[] args)
        {
            if (Log == null)
                throw new NullReferenceException("Log");
            s = string.Format(CultureInfo.CurrentCulture, s, args);
            string message;
            if (ex == null)
                message = string.Format(CultureInfo.CurrentCulture, "[{0}] '{1}' message: {2}", level.ToString(), Name, s);
            else
                message = string.Format(CultureInfo.CurrentCulture, "[{0}] '{1}' message: {2} exception: {3} {4} {5}", level.ToString(), Name, s, ex.GetType(), ex.Message, ex.StackTrace);
            Log.WriteEntry(message, ToEventLogEntryType(level));
        }

        #region Domain-specific

        public EventLog Log { get; private set; }

        #endregion

        private static EventLogEntryType ToEventLogEntryType(ServiceLog.LogLevel level)
        {
            switch (level)
            {
                case ServiceLog.LogLevel.Fatal:
                case ServiceLog.LogLevel.Error:
                    return EventLogEntryType.Error;
                case ServiceLog.LogLevel.Warning:
                    return EventLogEntryType.Warning;
                default:
                    return EventLogEntryType.Information;
            }
        }
    }
}
