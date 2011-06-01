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
namespace System.Abstract
{
    /// <summary>
    /// IServiceLog
    /// </summary>
    public interface IServiceLog : IServiceProvider
    {
        object GetLogger(object tag);
        void LogEvent(object logger, ServiceLogEventType eventType, string module, string text, params object[] args);
        void LogEvent(object logger, ServiceLogEventType eventType, string module, Exception e);
    }

    /// <summary>
    /// IServiceLogExtensions
    /// </summary>
    public static class IServiceLogExtensions
    {
        #region Lazy Setup

        public static LazyEx<IServiceLog> RegisterWithServiceLocator(this LazyEx<IServiceLog> service) { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator(null); return service; }
        public static LazyEx<IServiceLog> RegisterWithServiceLocator(this LazyEx<IServiceLog> service, string name) { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator(name); return service; }
        public static LazyEx<IServiceLog> RegisterWithServiceLocator(this LazyEx<IServiceLog> service, Func<IServiceLocator> locator) { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator(locator, null); return service; }
        public static LazyEx<IServiceLog> RegisterWithServiceLocator(this LazyEx<IServiceLog> service, Func<IServiceLocator> locator, string name) { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator(locator, name); return service; }

        #endregion
    }
}
