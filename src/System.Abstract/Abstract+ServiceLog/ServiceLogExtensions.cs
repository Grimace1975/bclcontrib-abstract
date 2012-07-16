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
    /// ServiceLogExtensions
    /// </summary>
    public static class ServiceLogExtensions
    {
        // get
        /// <summary>
        /// Gets the specified service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static IServiceLog Get<T>(this IServiceLog service) { return service.Get(typeof(T).Name); }
        /// <summary>
        /// Gets the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IServiceLog Get(this IServiceLog service, Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return service.Get(type.Name);
        }

        // log
        /// <summary>
        /// Fatals the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void Fatal(this IServiceLog service, string s, params object[] args) { service.Write(ServiceLog.LogLevel.Fatal, null, s, args); }
        /// <summary>
        /// Fatals the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        public static void Fatal(this IServiceLog service, Exception ex) { service.Write(ServiceLog.LogLevel.Fatal, ex, null, null); }
        /// <summary>
        /// Fatals the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void Fatal(this IServiceLog service, Exception ex, string s, params object[] args) { service.Write(ServiceLog.LogLevel.Fatal, ex, s, args); }
        /// <summary>
        /// Errors the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void Error(this IServiceLog service, string s, params object[] args) { service.Write(ServiceLog.LogLevel.Error, null, s, args); }
        /// <summary>
        /// Errors the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        public static void Error(this IServiceLog service, Exception ex) { service.Write(ServiceLog.LogLevel.Error, ex, null, null); }
        /// <summary>
        /// Errors the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void Error(this IServiceLog service, Exception ex, string s, params object[] args) { service.Write(ServiceLog.LogLevel.Error, ex, s, args); }
        /// <summary>
        /// Warnings the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void Warning(this IServiceLog service, string s, params object[] args) { service.Write(ServiceLog.LogLevel.Warning, null, s, args); }
        /// <summary>
        /// Warnings the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        public static void Warning(this IServiceLog service, Exception ex) { service.Write(ServiceLog.LogLevel.Warning, ex, null, null); }
        /// <summary>
        /// Warnings the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void Warning(this IServiceLog service, Exception ex, string s, params object[] args) { service.Write(ServiceLog.LogLevel.Warning, ex, s, args); }
        /// <summary>
        /// Informations the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void Information(this IServiceLog service, string s, params object[] args) { service.Write(ServiceLog.LogLevel.Information, null, s, args); }
        /// <summary>
        /// Informations the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        public static void Information(this IServiceLog service, Exception ex) { service.Write(ServiceLog.LogLevel.Information, ex, null, null); }
        /// <summary>
        /// Informations the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void Information(this IServiceLog service, Exception ex, string s, params object[] args) { service.Write(ServiceLog.LogLevel.Information, ex, s, args); }
        /// <summary>
        /// Debugs the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void Debug(this IServiceLog service, string s, params object[] args) { service.Write(ServiceLog.LogLevel.Debug, null, s, args); }
        /// <summary>
        /// Debugs the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        public static void Debug(this IServiceLog service, Exception ex) { service.Write(ServiceLog.LogLevel.Debug, ex, null, null); }
        /// <summary>
        /// Debugs the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void Debug(this IServiceLog service, Exception ex, string s, params object[] args) { service.Write(ServiceLog.LogLevel.Debug, ex, s, args); }

        #region Lazy Setup

        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> RegisterWithServiceLocator(this Lazy<IServiceLog> service) { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> RegisterWithServiceLocator(this Lazy<IServiceLog> service, string name) { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, name); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> RegisterWithServiceLocator(this Lazy<IServiceLog> service, Lazy<IServiceLocator> locator) { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> RegisterWithServiceLocator(this Lazy<IServiceLog> service, Lazy<IServiceLocator> locator, string name) { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, name); return service; }

        #endregion
    }
}
