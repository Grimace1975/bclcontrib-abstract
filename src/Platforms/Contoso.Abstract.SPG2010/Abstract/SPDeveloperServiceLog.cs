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
    /// ISPDeveloperServiceLog
    /// </summary>
    public interface ISPDeveloperServiceLog : IServiceLog
    {
        /// <summary>
        /// Gets the log.
        /// </summary>
        ILogger Log { get; }
        /// <summary>
        /// Gets the event ID.
        /// </summary>
        int EventID { get; }
        /// <summary>
        /// Gets the name of the area.
        /// </summary>
        /// <value>
        /// The name of the area.
        /// </value>
        string AreaName { get; }
        /// <summary>
        /// Gets the category.
        /// </summary>
        string Category { get; }
    }

    /// <summary>
    /// SPDeveloperServiceLog
    /// </summary>
    public class SPDeveloperServiceLog : ISPDeveloperServiceLog, ServiceLogManager.ISetupRegistration
    {
        static SPDeveloperServiceLog() { ServiceLogManager.EnsureRegistration(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="SPDeveloperServiceLog"/> class.
        /// </summary>
        public SPDeveloperServiceLog()
            : this(new SharePointLogger(), 0, "SharePoint Foundation", "General") { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SPDeveloperServiceLog"/> class.
        /// </summary>
        /// <param name="areaName">Name of the area.</param>
        /// <param name="category">The category.</param>
        public SPDeveloperServiceLog(string areaName, string category)
            : this(new SharePointLogger(), 0, areaName, category) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SPDeveloperServiceLog"/> class.
        /// </summary>
        /// <param name="eventID">The event ID.</param>
        public SPDeveloperServiceLog(int eventID)
            : this(new SharePointLogger(), eventID, "SharePoint Foundation", "General") { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SPDeveloperServiceLog"/> class.
        /// </summary>
        /// <param name="eventID">The event ID.</param>
        /// <param name="areaName">Name of the area.</param>
        /// <param name="category">The category.</param>
        public SPDeveloperServiceLog(int eventID, string areaName, string category)
            : this(new SharePointLogger(), eventID, areaName, category) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SPDeveloperServiceLog"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="eventID">The event ID.</param>
        /// <param name="areaName">Name of the area.</param>
        /// <param name="category">The category.</param>
        public SPDeveloperServiceLog(ILogger log, int eventID, string areaName, string category)
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

        Action<IServiceLocator, string> ServiceLogManager.ISetupRegistration.DefaultServiceRegistrar
        {
            get { return (locator, name) => ServiceLogManager.RegisterInstance<ISPDeveloperServiceLog>(this, locator, name); }
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.
        /// -or-
        /// null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        // get
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { return AreaName + "/" + Category; }
        }
        /// <summary>
        /// Gets the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IServiceLog Get(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            return new SPServiceLog(Log, EventID, AreaName, Name);
        }
        /// <summary>
        /// Gets the specified name.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IServiceLog Get(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return new SPServiceLog(Log, EventID, AreaName, type.Name);
        }

        // log
        /// <summary>
        /// Writes the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="s">The s.</param>
        public void Write(ServiceLog.LogLevel level, Exception ex, string s)
        {
            if (Log == null)
                throw new NullReferenceException("Log");
            if (ex == null)
                switch (level)
                {
                    case ServiceLog.LogLevel.Fatal: Log.TraceToDeveloper(s, EventID, TraceSeverity.Unexpected, Name); return;
                    case ServiceLog.LogLevel.Error: Log.TraceToDeveloper(s, EventID, TraceSeverity.High, Name); return;
                    case ServiceLog.LogLevel.Warning: Log.TraceToDeveloper(s, EventID, TraceSeverity.Medium, Name); return;
                    case ServiceLog.LogLevel.Information: Log.TraceToDeveloper(s, EventID, TraceSeverity.Monitorable, Name); return;
                    case ServiceLog.LogLevel.Debug: Log.TraceToDeveloper(s, EventID, TraceSeverity.Verbose, Name); return;
                    default: return;
                }
            else
                switch (level)
                {
                    case ServiceLog.LogLevel.Fatal: Log.TraceToDeveloper(ex, s, EventID, TraceSeverity.Unexpected, Name); return;
                    case ServiceLog.LogLevel.Error: Log.TraceToDeveloper(ex, s, EventID, TraceSeverity.High, Name); return;
                    case ServiceLog.LogLevel.Warning: Log.TraceToDeveloper(ex, s, EventID, TraceSeverity.Medium, Name); return;
                    case ServiceLog.LogLevel.Information: Log.TraceToDeveloper(ex, s, EventID, TraceSeverity.Monitorable, Name); return;
                    case ServiceLog.LogLevel.Debug: Log.TraceToDeveloper(ex, s, EventID, TraceSeverity.Verbose, Name); return;
                    default: return;
                }
        }

        #region Domain-specific

        /// <summary>
        /// Gets the log.
        /// </summary>
        public ILogger Log { get; private set; }
        /// <summary>
        /// Gets the event ID.
        /// </summary>
        public int EventID { get; private set; }
        /// <summary>
        /// Gets the name of the area.
        /// </summary>
        /// <value>
        /// The name of the area.
        /// </value>
        public string AreaName { get; private set; }
        /// <summary>
        /// Gets the category.
        /// </summary>
        public string Category { get; private set; }

        #endregion
    }
}
