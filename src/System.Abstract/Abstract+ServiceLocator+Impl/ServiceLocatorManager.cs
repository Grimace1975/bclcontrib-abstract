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
using System.Reflection;
using System.Text;
namespace System.Abstract
{
    /// <summary>
    /// ServiceLocatorManager
    /// </summary>
    public class ServiceLocatorManager : ServiceManagerBase<IServiceLocator, Action<IServiceLocator>, ServiceLocatorManagerDebugger>
    {
        private static readonly Type _ignoreServiceLocatorType = typeof(IIgnoreServiceLocator);

        static ServiceLocatorManager()
        {
            Registration = new ServiceRegistration
            {
                MakeAction = a => x => a(x),
                OnSetup = (service, descriptor) =>
                {
                    var behavior = (service.Registrar as IServiceRegistrarBehaviorAccessor);
                    if (behavior == null || behavior.RegisterInLocator)
                        RegisterSelfInLocator(service);
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
                SetProvider(() => new MicroServiceLocator());
        }

        /// <summary>
        /// Sets the provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> SetProvider(Func<IServiceLocator> provider) { return (Lazy = MakeByProviderProtected(provider, null)); }
        /// <summary>
        /// Sets the provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="setupDescriptor">The setup descriptor.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> SetProvider(Func<IServiceLocator> provider, ISetupDescriptor setupDescriptor) { return (Lazy = MakeByProviderProtected(provider, setupDescriptor)); }
        /// <summary>
        /// Makes the by provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> MakeByProvider(Func<IServiceLocator> provider) { return MakeByProviderProtected(provider, null); }
        /// <summary>
        /// Makes the by provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="setupDescriptor">The setup descriptor.</param>
        /// <returns></returns>
        public static Lazy<IServiceLocator> MakeByProvider(Func<IServiceLocator> provider, ISetupDescriptor setupDescriptor) { return MakeByProviderProtected(provider, setupDescriptor); }

        /// <summary>
        /// Gets the current.
        /// </summary>
        public static IServiceLocator Current
        {
            get
            {
                if (Lazy == null)
                    throw new InvalidOperationException("Service undefined. Ensure SetProvider");
                if (Lazy.IsValueCreated)
                    return Lazy.Value;
                try { return LazyValue ?? Lazy.Value; }
                catch (ReflectionTypeLoadException ex)
                {
                    var b = new StringBuilder();
                    foreach (var ex2 in ex.LoaderExceptions)
                        b.AppendLine(ex2.Message);
                    throw new Exception(b.ToString(), ex);
                }
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
        public static ISetupDescriptor GetSetupDescriptor(Lazy<IServiceLocator> service) { return GetSetupDescriptorProtected(service, null); }

        private static void RegisterSelfInLocator(IServiceLocator locator)
        {
            locator.Registrar.RegisterInstance<IServiceLocator>(locator);
        }

        /// <summary>
        /// Determines whether [has ignore service locator] [the specified instance].
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///   <c>true</c> if [has ignore service locator] [the specified instance]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasIgnoreServiceLocator(object instance) { return (instance == null || HasIgnoreServiceLocator(instance.GetType())); }
        /// <summary>
        /// Determines whether [has ignore service locator].
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>
        ///   <c>true</c> if [has ignore service locator]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasIgnoreServiceLocator<TService>() { return HasIgnoreServiceLocator(typeof(TService)); }
        /// <summary>
        /// Determines whether [has ignore service locator] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [has ignore service locator] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasIgnoreServiceLocator(Type type)
        {
            return (type == null || _ignoreServiceLocatorType.IsAssignableFrom(type) || IgnoreServiceLocatorAttribute.HasIgnoreServiceLocator(type));
        }
    }
}
