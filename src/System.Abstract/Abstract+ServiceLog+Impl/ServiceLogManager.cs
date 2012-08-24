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
using System.Abstract.Parts;
using Contoso.Abstract;
namespace System.Abstract
{
    /// <summary>
    /// ServiceLogManager
    /// </summary>
    public class ServiceLogManager : ServiceManagerBase<IServiceLog, Action<IServiceLog>, ServiceLogManagerDebugger>
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly Lazy<IServiceLog> EmptyServiceLog = new Lazy<IServiceLog>(() => new EmptyServiceLog());

        static ServiceLogManager()
        {
            Registration = new ServiceRegistration
            {
                MakeAction = a => x => a(x),
                OnSetup = (service, descriptor) =>
                {
                    if (descriptor != null)
                        foreach (var action in descriptor.Actions)
                            action(service);
                    return service;
                },
                OnChange = (service, descriptor) =>
                {
                    if (descriptor != null)
                        foreach (var action in descriptor.Actions)
                            action(service);
                },
            };
            // default provider
            if (Lazy == null)
                SetProvider(() => new ConsoleServiceLog("Default"));
        }

        /// <summary>
        /// Sets the provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> SetProvider(Func<IServiceLog> provider) { return (Lazy = MakeByProviderProtected(provider, null)); }
        /// <summary>
        /// Sets the provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="setupDescriptor">The setup descriptor.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> SetProvider(Func<IServiceLog> provider, ISetupDescriptor setupDescriptor) { return (Lazy = MakeByProviderProtected(provider, setupDescriptor)); }
        /// <summary>
        /// Makes the by provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> MakeByProvider(Func<IServiceLog> provider) { return MakeByProviderProtected(provider, null); }
        /// <summary>
        /// Makes the by provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="setupDescriptor">The setup descriptor.</param>
        /// <returns></returns>
        public static Lazy<IServiceLog> MakeByProvider(Func<IServiceLog> provider, ISetupDescriptor setupDescriptor) { return MakeByProviderProtected(provider, setupDescriptor); }

        /// <summary>
        /// Gets the current.
        /// </summary>
        public static IServiceLog Current
        {
            get
            {
                if (Lazy == null)
                    throw new InvalidOperationException("Service undefined. Ensure SetProvider");
                return Lazy.Value;
            }
        }

        /// <summary>
        /// Ensures the registration.
        /// </summary>
        public static void EnsureRegistration() { }
        /// <summary>
        /// Gets the setup descriptor.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static ISetupDescriptor GetSetupDescriptor(Lazy<IServiceLog> service) { return GetSetupDescriptorProtected(service, null); }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IServiceLog Get<T>() { return (ServiceLogManager.Lazy ?? EmptyServiceLog).Value.Get<T>(); }
        /// <summary>
        /// Gets the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static IServiceLog Get(string name) { return (ServiceLogManager.Lazy ?? EmptyServiceLog).Value.Get(name); }
    }
}
