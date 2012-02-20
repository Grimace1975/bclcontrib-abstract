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
    /// ServiceCacheManager
    /// </summary>
    public class ServiceCacheManager : ServiceManagerBase<IServiceCache, Action<IServiceCache>>
    {
        static ServiceCacheManager()
        {
            Registration = new SetupRegistration
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
                OnServiceRegistrar = (service, locator, name) =>
                {
                    RegisterInstance(service, locator, name);
                    var distributedServiceCache = (service as IDistributedServiceCache);
                    if (distributedServiceCache != null)
                        RegisterInstance(distributedServiceCache, locator, name);
                    // specific registration
                    var setupRegistration = (service as ISetupRegistration);
                    if (setupRegistration != null)
                        setupRegistration.OnServiceRegistrar(locator, name);
                },
            };
            // default provider
            if (Lazy == null)
                SetProvider(() => new StaticServiceCache());
        }

        public static Lazy<IServiceCache> SetProvider(Func<IServiceCache> provider) { return (Lazy = MakeByProviderProtected(provider, null)); }
        public static Lazy<IServiceCache> SetProvider(Func<IServiceCache> provider, ISetupDescriptor setupDescriptor) { return (Lazy = MakeByProviderProtected(provider, setupDescriptor)); }
        public static Lazy<IServiceCache> MakeByProvider(Func<IServiceCache> provider) { return MakeByProviderProtected(provider, null); }
        public static Lazy<IServiceCache> MakeByProvider(Func<IServiceCache> provider, ISetupDescriptor setupDescriptor) { return MakeByProviderProtected(provider, setupDescriptor); }

        public static IServiceCache Current
        {
            get
            {
                if (Lazy == null)
                    throw new InvalidOperationException("Service undefined. Ensure SetProvider");
                return Lazy.Value;
            }
        }

        public static void EnsureRegistration() { }
        public static ISetupDescriptor GetSetupDescriptor(Lazy<IServiceCache> service) { return GetSetupDescriptorProtected(service, null); }
    }
}
