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
using System.IO;
namespace Contoso.Abstract
{
    /// <summary>
    /// IConsoleServiceLog
    /// </summary>
    public interface IConsoleServiceLog : IServiceLog
    {
        TextWriter Log { get; }
    }

    /// <summary>
    /// ConsoleServiceLog
    /// </summary>
    public class ConsoleServiceLog : IConsoleServiceLog, ServiceLogManager.ISetupRegistration
    {
        static ConsoleServiceLog() { ServiceLogManager.EnsureRegistration(); }
        public ConsoleServiceLog(string name)
        {
            Name = name;
            Log = Console.Out;
        }

        Action<IServiceRegistrar, IServiceLocator, string> ServiceLogManager.ISetupRegistration.OnServiceRegistrar
        {
            get { return (registrar, locator, name) => ServiceLogManager.RegisterInstance<IConsoleServiceLog>(this, registrar, locator, name); }
        }

        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        // get
        public string Name { get; private set; }
        public IServiceLog Get(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            return new ConsoleServiceLog(Name + "." + name);
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

        public TextWriter Log { get; set; }

        #endregion
    }
}