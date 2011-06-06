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
using System.IO;
using System.Text;
using System.Globalization;
namespace Contoso.Abstract
{
    /// <summary>
    /// IStreamLogServiceLog
    /// </summary>
    public interface IStreamLogServiceLog : IServiceLog
    {
        StreamWriter Log { get; }
    }

    /// <summary>
    /// StreamServiceLog
    /// </summary>
    public class StreamServiceLog : IStreamLogServiceLog, IDisposable, ServiceLogManager.ISetupRegistration
    {
        static StreamServiceLog() { ServiceLogManager.EnsureRegistration(); }
        public StreamServiceLog(string name, Stream s)
            : this(name, new StreamWriter(s)) { }
        public StreamServiceLog(string name, StreamWriter sw)
        {
            Name = name;
            sw.AutoFlush = true;
            Log = sw;
        }
        public StreamServiceLog(string name, Stream s, Encoding encoding)
            : this(name, new StreamWriter(s, encoding)) { }
        public StreamServiceLog(string name, Stream s, Encoding encoding, int bufferSize)
            : this(name, new StreamWriter(s, encoding, bufferSize)) { }
        ~StreamServiceLog()
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

        Action<IServiceRegistrar, IServiceLocator, string> ServiceLogManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (registrar, locator, name) => ServiceLogManager.RegisterInstance<IStreamLogServiceLog>(this, registrar, locator, name); }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        // get
        public string Name { get; private set; }
        public IServiceLog Get(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            throw new NotSupportedException("A StreamServiceLog does not support child loggers");
        }

        // log
        public void Write(ServiceLog.LogLevel level, Exception ex, string s, params object[] args)
        {
            if (Log == null)
                throw new NullReferenceException("Log");
            s = string.Format(CultureInfo.CurrentCulture, s, args);
            Log.WriteLine("[{0}] '{1}' {2} {3}", level, Name, s);
            if (ex != null)
                Log.WriteLine("{0}: {1} {2}", ex.GetType().FullName, ex.Message, ex.StackTrace);
        }

        #region Domain-specific

        public StreamWriter Log { get; private set; }

        #endregion
    }
}
