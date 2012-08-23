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
        /// <summary>
        /// Gets the log.
        /// </summary>
        StreamWriter Log { get; }
    }

    /// <summary>
    /// StreamServiceLog
    /// </summary>
    public class StreamServiceLog : IStreamLogServiceLog, IDisposable, ServiceLogManager.ISetupRegistration
    {
        static StreamServiceLog() { ServiceLogManager.EnsureRegistration(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamServiceLog"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="s">The s.</param>
        public StreamServiceLog(string name, Stream s)
            : this(name, new StreamWriter(s)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamServiceLog"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="sw">The sw.</param>
        public StreamServiceLog(string name, StreamWriter sw)
        {
            Name = name;
            sw.AutoFlush = true;
            Log = sw;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamServiceLog"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="s">The s.</param>
        /// <param name="encoding">The encoding.</param>
        public StreamServiceLog(string name, Stream s, Encoding encoding)
            : this(name, new StreamWriter(s, encoding)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamServiceLog"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="s">The s.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        public StreamServiceLog(string name, Stream s, Encoding encoding, int bufferSize)
            : this(name, new StreamWriter(s, encoding, bufferSize)) { }
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="StreamServiceLog"/> is reclaimed by garbage collection.
        /// </summary>
        ~StreamServiceLog()
        {
            try { ((IDisposable)this).Dispose(); }
            catch { }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Log != null)
            {
                GC.SuppressFinalize(this);
                try { Log.Dispose(); }
                finally { Log = null; }
            }
        }

        Action<IServiceLocator, string> ServiceLogManager.ISetupRegistration.DefaultServiceRegistrar
        {
            get { return (locator, name) => ServiceLogManager.RegisterInstance<IStreamLogServiceLog>(this, locator, name); }
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
            throw new NotSupportedException("A StreamServiceLog does not support child loggers");
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
            throw new NotSupportedException("A StreamServiceLog does not support child loggers");
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
            Log.WriteLine("[{0}] '{1}' {2} {3}", level, Name, s);
            if (ex != null)
                Log.WriteLine("{0}: {1} {2}", ex.GetType().FullName, ex.Message, ex.StackTrace);
        }

        #region Domain-specific

        /// <summary>
        /// Gets the log.
        /// </summary>
        public StreamWriter Log { get; private set; }

        #endregion
    }
}
