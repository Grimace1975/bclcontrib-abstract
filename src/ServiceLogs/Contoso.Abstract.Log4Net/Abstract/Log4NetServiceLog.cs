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
using log4net;
using System.Globalization;
namespace Contoso.Abstract
{
    /// <summary>
    /// ILog4NetServiceLog
    /// </summary>
    public interface ILog4NetServiceLog : IServiceLog
    {
        ILog Log { get; }
    }

    /// <summary>
    /// Log4NetServiceLog
    /// </summary>
    public class Log4NetServiceLog : ILog4NetServiceLog, ServiceLogManager.ISetupRegistration
    {
        static Log4NetServiceLog() { ServiceLogManager.EnsureRegistration(); }
        public Log4NetServiceLog(string name)
            : this(LogManager.GetLogger(name)) { }
        public Log4NetServiceLog(ILog log)
        {
            if (log == null)
                throw new ArgumentNullException("log");
            Name = log.Logger.Name;
            Log = log;
        }

        Action<IServiceLocator, string> ServiceLogManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (locator, name) => ServiceLogManager.RegisterInstance<ILog4NetServiceLog>(this, locator, name); }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        // get
        public string Name { get; private set; }
        public IServiceLog Get(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            return new Log4NetServiceLog(LogManager.GetLogger(Name + "." + name));
        }

        // log
        public void Write(ServiceLog.LogLevel level, Exception ex, string s, params object[] args)
        {
            if (Log == null)
                throw new NullReferenceException("Log");
            s = (!string.IsNullOrEmpty(s) ? string.Format(CultureInfo.CurrentCulture, s, args) : string.Empty);
            switch (level)
            {
                case ServiceLog.LogLevel.Fatal: Log.Fatal(s, ex); return;
                case ServiceLog.LogLevel.Error: Log.Error(s, ex); return;
                case ServiceLog.LogLevel.Warning: Log.Warn(s, ex); return;
                case ServiceLog.LogLevel.Information: Log.Info(s, ex); return;
                case ServiceLog.LogLevel.Debug: Log.Debug(s, ex); return;
                default: return;
            }
        }

        #region Domain-specific

        public ILog Log { get; private set; }

        #endregion
    }
}
