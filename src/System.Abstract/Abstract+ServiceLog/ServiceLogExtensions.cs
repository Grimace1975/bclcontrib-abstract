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
using System.Globalization;
using System.Abstract.Parts;
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
        public static IServiceLog Get<T>(this IServiceLog service) { return service.Get(typeof(T)); }

        // log
        /// <summary>
        /// Fatals the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="s">The s.</param>
        public static void Fatal(this IServiceLog service, string s) { service.Write(ServiceLog.LogLevel.Fatal, null, s); }
        /// <summary>
        /// Fatals the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        public static void Fatal(this IServiceLog service, Exception ex) { service.Write(ServiceLog.LogLevel.Fatal, ex, null); }
        /// <summary>
        /// Fatals the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void FatalFormat(this IServiceLog service, string s, params object[] args)
        {
            s = (!string.IsNullOrEmpty(s) ? string.Format(CultureInfo.CurrentCulture, s, args) : string.Empty);
            service.Write(ServiceLog.LogLevel.Fatal, null, s);
        }
        /// <summary>
        /// Fatals the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void FatalFormat(this IServiceLog service, Exception ex, string s, params object[] args)
        {
            s = (!string.IsNullOrEmpty(s) ? string.Format(CultureInfo.CurrentCulture, s, args) : string.Empty);
            service.Write(ServiceLog.LogLevel.Fatal, ex, s);
        }
        /// <summary>
        /// Errors the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="s">The s.</param>
        public static void Error(this IServiceLog service, string s) { service.Write(ServiceLog.LogLevel.Error, null, s); }
        /// <summary>
        /// Errors the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        public static void Error(this IServiceLog service, Exception ex) { service.Write(ServiceLog.LogLevel.Error, ex, null); }
        /// <summary>
        /// Errors the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void ErrorFormat(this IServiceLog service, string s, params object[] args)
        {
            s = (!string.IsNullOrEmpty(s) ? string.Format(CultureInfo.CurrentCulture, s, args) : string.Empty);
            service.Write(ServiceLog.LogLevel.Error, null, s);
        }
        /// <summary>
        /// Errors the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void ErrorFormat(this IServiceLog service, Exception ex, string s, params object[] args)
        {
            s = (!string.IsNullOrEmpty(s) ? string.Format(CultureInfo.CurrentCulture, s, args) : string.Empty);
            service.Write(ServiceLog.LogLevel.Error, ex, s);
        }
        /// <summary>
        /// Warnings the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="s">The s.</param>
        public static void Warning(this IServiceLog service, string s) { service.Write(ServiceLog.LogLevel.Warning, null, s); }
        /// <summary>
        /// Warnings the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        public static void Warning(this IServiceLog service, Exception ex) { service.Write(ServiceLog.LogLevel.Warning, ex, null); }
        /// <summary>
        /// Warnings the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void WarningFormat(this IServiceLog service, string s, params object[] args)
        {
            s = (!string.IsNullOrEmpty(s) ? string.Format(CultureInfo.CurrentCulture, s, args) : string.Empty);
            service.Write(ServiceLog.LogLevel.Warning, null, s);
        }
        /// <summary>
        /// Warnings the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void WarningFormat(this IServiceLog service, Exception ex, string s, params object[] args)
        {
            s = (!string.IsNullOrEmpty(s) ? string.Format(CultureInfo.CurrentCulture, s, args) : string.Empty);
            service.Write(ServiceLog.LogLevel.Warning, ex, s);
        }
        /// <summary>
        /// Informations the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="s">The s.</param>
        public static void Information(this IServiceLog service, string s) { service.Write(ServiceLog.LogLevel.Information, null, s); }
        /// <summary>
        /// Informations the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        public static void Information(this IServiceLog service, Exception ex) { service.Write(ServiceLog.LogLevel.Information, ex, null); }
        /// <summary>
        /// Informations the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void InformationFormat(this IServiceLog service, string s, params object[] args)
        {
            s = (!string.IsNullOrEmpty(s) ? string.Format(CultureInfo.CurrentCulture, s, args) : string.Empty);
            service.Write(ServiceLog.LogLevel.Information, null, s);
        }
        /// <summary>
        /// Informations the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void InformationFormat(this IServiceLog service, Exception ex, string s, params object[] args)
        {
            s = (!string.IsNullOrEmpty(s) ? string.Format(CultureInfo.CurrentCulture, s, args) : string.Empty);
            service.Write(ServiceLog.LogLevel.Information, ex, s);
        }
        /// <summary>
        /// Debugs the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="s">The s.</param>
        public static void Debug(this IServiceLog service, string s) { service.Write(ServiceLog.LogLevel.Debug, null, s); }
        /// <summary>
        /// Debugs the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        public static void Debug(this IServiceLog service, Exception ex) { service.Write(ServiceLog.LogLevel.Debug, ex, null); }
        /// <summary>
        /// Debugs the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void DebugFormat(this IServiceLog service, string s, params object[] args)
        {
            s = (!string.IsNullOrEmpty(s) ? string.Format(CultureInfo.CurrentCulture, s, args) : string.Empty);
            service.Write(ServiceLog.LogLevel.Debug, null, s);
        }
        /// <summary>
        /// Debugs the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="s">The s.</param>
        /// <param name="args">The args.</param>
        public static void DebugFormat(this IServiceLog service, Exception ex, string s, params object[] args)
        {
            s = (!string.IsNullOrEmpty(s) ? string.Format(CultureInfo.CurrentCulture, s, args) : string.Empty);
            service.Write(ServiceLog.LogLevel.Debug, ex, s);
        }

        #region BehaveAs

        /// <summary>
        /// Behaves as.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static T BehaveAs<T>(this IServiceLog service)
            where T : class, IServiceLog
        {
            IServiceWrapper<IServiceLog> serviceWrapper;
            do
            {
                serviceWrapper = (service as IServiceWrapper<IServiceLog>);
                if (serviceWrapper != null)
                    service = serviceWrapper.Parent;
            } while (serviceWrapper != null);
            return (service as T);
        }

        #endregion

        #region Lazy Setup

        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> RegisterWithServiceLocator<T>(this Lazy<IServiceLog> service)
            where T : class, IServiceLog { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, ServiceLocatorManager.Lazy, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> RegisterWithServiceLocator<T>(this Lazy<IServiceLog> service, string name)
            where T : class, IServiceLog { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, ServiceLocatorManager.Lazy, name); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> RegisterWithServiceLocator<T>(this Lazy<IServiceLog> service, Lazy<IServiceLocator> locator)
            where T : class, IServiceLog { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> RegisterWithServiceLocator<T>(this Lazy<IServiceLog> service, Lazy<IServiceLocator> locator, string name)
            where T : class, IServiceLog { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, name); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> RegisterWithServiceLocator(this Lazy<IServiceLog> service) { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, ServiceLocatorManager.Lazy, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> RegisterWithServiceLocator(this Lazy<IServiceLog> service, string name) { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, ServiceLocatorManager.Lazy, name); return service; }
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

        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> RegisterWithServiceLocator<T>(this Lazy<IServiceLog> service, IServiceLocator locator)
            where T : class, IServiceLog { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> RegisterWithServiceLocator<T>(this Lazy<IServiceLog> service, IServiceLocator locator, string name)
            where T : class, IServiceLog { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, name); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> RegisterWithServiceLocator(this Lazy<IServiceLog> service, IServiceLocator locator) { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> RegisterWithServiceLocator(this Lazy<IServiceLog> service, IServiceLocator locator, string name) { ServiceLogManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, name); return service; }

        #endregion
    }
}
