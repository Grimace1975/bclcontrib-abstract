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
using NLog;
using System.Globalization;
namespace Contoso.Abstract
{
    /// <summary>
    /// INLogServiceLog
    /// </summary>
    public interface INLogServiceLog : IServiceLog
    {
        /// <summary>
        /// Gets the log.
        /// </summary>
        Logger Log { get; }
    }

    /// <summary>
    /// NLogServiceLog
    /// </summary>
    public class NLogServiceLog : INLogServiceLog, ServiceLogManager.ISetupRegistration
    {
        static NLogServiceLog() { ServiceLogManager.EnsureRegistration(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="NLogServiceLog"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public NLogServiceLog(string name)
            : this(LogManager.GetLogger(name)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="NLogServiceLog"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        public NLogServiceLog(Logger log)
        {
            if (log == null)
                throw new ArgumentNullException("log");
            Name = log.Name;
            Log = log;
        }

        Action<IServiceLocator, string> ServiceLogManager.ISetupRegistration.DefaultServiceRegistrar
        {
            get { return (locator, name) => ServiceLogManager.RegisterInstance<INLogServiceLog>(this, locator, name); }
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
        public string Name { get; private set; }
        /// <summary>
        /// Gets the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IServiceLog Get(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            return new NLogServiceLog(LogManager.GetLogger(name));
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
            return new NLogServiceLog(LogManager.GetLogger(string.Empty, type));
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
            var message = string.Format(CultureInfo.CurrentCulture, "[{0}] '{1}' message: {2}", level.ToString(), Name, s);
            if (ex == null)
                switch (level)
                {
                    case ServiceLog.LogLevel.Fatal:
                        Log.Fatal(message);
                        return;
                    case ServiceLog.LogLevel.Error:
                        Log.Error(message);
                        return;
                    case ServiceLog.LogLevel.Warning:
                        Log.Warn(message);
                        return;
                    case ServiceLog.LogLevel.Information:
                        Log.Info(message);
                        return;
                    case ServiceLog.LogLevel.Debug:
                        Log.Debug(message);
                        return;
                    default:
                        return;
                }
            else
                switch (level)
                {
                    case ServiceLog.LogLevel.Fatal:
                        Log.FatalException(message, ex);
                        return;
                    case ServiceLog.LogLevel.Error:
                        Log.ErrorException(message, ex);
                        return;
                    case ServiceLog.LogLevel.Warning:
                        Log.WarnException(message, ex);
                        return;
                    case ServiceLog.LogLevel.Information:
                        Log.InfoException(message, ex);
                        return;
                    case ServiceLog.LogLevel.Debug:
                        Log.DebugException(message, ex);
                        return;
                    default:
                        return;
                }
        }

        #region Domain-specific

        /// <summary>
        /// Gets the log.
        /// </summary>
        public Logger Log { get; private set; }

        #endregion
    }
}
