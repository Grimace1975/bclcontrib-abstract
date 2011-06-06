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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
namespace Contoso.Abstract
{
    /// <summary>
    /// ITraceSourceServiceLog
    /// </summary>
    public interface ITraceSourceServiceLog : IServiceLog
    {
        TraceSource Log { get; }
    }

    /// <summary>
    /// TraceSourceServiceLog
    /// </summary>
    public class TraceSourceServiceLog : ITraceSourceServiceLog, ServiceLogManager.ISetupRegistration
    {
        private static readonly Dictionary<string, TraceSource> _logs = new Dictionary<string, TraceSource>();

        static TraceSourceServiceLog() { ServiceLogManager.EnsureRegistration(); }
        public TraceSourceServiceLog(string name)
            : this(name, SourceLevels.Off) { }
        public TraceSourceServiceLog(string name, SourceLevels defaultLevel)
        {
            Name = name;
            Log = GetAndCache(name, defaultLevel);
        }

        Action<IServiceRegistrar, IServiceLocator, string> ServiceLogManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (registrar, locator, name) => ServiceLogManager.RegisterInstance<ITraceSourceServiceLog>(this, registrar, locator, name); }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        // get
        public string Name { get; private set; }
        public IServiceLog Get(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            return new TraceSourceServiceLog(Name + "." + name);
        }

        // log
        public void Write(ServiceLog.LogLevel level, Exception ex, string s, params object[] args)
        {
            if (Log == null)
                throw new NullReferenceException("Log");
            s = string.Format(CultureInfo.CurrentCulture, s, args);
            if (ex == null)
                Log.TraceEvent(ToTraceEventType(level), 0, s);
            else
                Log.TraceData(ToTraceEventType(level), 0, new object[] { s, ex });
        }

        #region Domain-specific

        public TraceSource Log { get; private set; }

        #endregion

        private static TraceEventType ToTraceEventType(ServiceLog.LogLevel level)
        {
            switch (level)
            {
                case ServiceLog.LogLevel.Fatal:
                    return TraceEventType.Critical;
                case ServiceLog.LogLevel.Error:
                    return TraceEventType.Error;
                case ServiceLog.LogLevel.Warning:
                    return TraceEventType.Warning;
                case ServiceLog.LogLevel.Information:
                    return TraceEventType.Information;
                case ServiceLog.LogLevel.Debug:
                    return TraceEventType.Verbose;
                default:
                    return TraceEventType.Verbose;
            }
        }

        private static TraceSource GetAndCache(string name, SourceLevels defaultLevel)
        {
            TraceSource log;
            if (_logs.TryGetValue(name, out log))
                return log;
            lock (_logs)
            {
                if (_logs.TryGetValue(name, out log))
                    return log;
                log = new TraceSource(name);
                if (!HasDefaultSource(log))
                {
                    var source = new TraceSource("Default", defaultLevel);
                    for (string shortName = ShortenName(name); !string.IsNullOrEmpty(shortName); shortName = ShortenName(shortName))
                    {
                        var source2 = new TraceSource(shortName, defaultLevel);
                        if (!HasDefaultSource(source2))
                        {
                            source = source2;
                            break;
                        }
                    }
                    log.Switch = source.Switch;
                    var listeners = log.Listeners;
                    listeners.Clear();
                    foreach (TraceListener listener in source.Listeners)
                        listeners.Add(listener);
                }
                _logs.Add(name, log);
            }
            return log;
        }

        private static string ShortenName(string name)
        {
            int length = name.LastIndexOf('.');
            return (length != -1 ? name.Substring(0, length) : null);
        }

        private static bool HasDefaultSource(TraceSource source)
        {
            return (((source.Listeners.Count == 1) && (source.Listeners[0] is DefaultTraceListener)) && (source.Listeners[0].Name == "Default"));
        }
    }
}
