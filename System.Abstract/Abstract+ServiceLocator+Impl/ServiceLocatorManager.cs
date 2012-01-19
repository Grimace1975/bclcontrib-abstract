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
namespace System.Abstract
{
    /// <summary>
    /// ServiceLocatorManager
    /// </summary>
    public class ServiceLocatorManager : ServiceManagerBase<IServiceLocator, Action<IServiceLocator>>
    {
        private static readonly Type _wantToSkipServiceLocatorType = typeof(IWantToSkipServiceLocator);
        private static readonly Type _wantToSkipServiceRegistrationType = typeof(IWantToSkipServiceRegistration);

        static ServiceLocatorManager()
        {
            Registration = new SetupRegistration
            {
                OnSetup = (service, descriptor) =>
                {
                    RegisterSelfInLocator(service);
                    if (descriptor != null)
                        foreach (var action in descriptor.Actions)
                            action(service);
                    return service;
                },
            };
        }

        public static void EnsureRegistration() { }
        public static ISetupDescriptor GetSetupDescriptor(Lazy<IServiceLocator> service) { return ProtectedGetSetupDescriptor(service, null); }

        private static void RegisterSelfInLocator(IServiceLocator locator)
        {
            locator.Registrar.RegisterInstance<IServiceLocator>(locator);
        }

        public static bool GetWantsToSkipLocator(object instance) { return (instance == null || GetWantsToSkipLocator(instance.GetType())); }
        public static bool GetWantsToSkipLocator<TService>() { return GetWantsToSkipLocator(typeof(TService)); }
        public static bool GetWantsToSkipLocator(Type type)
        {
            return (type == null || _wantToSkipServiceLocatorType.IsAssignableFrom(type));
        }

        public static bool GetWantsToSkipRegistration(object instance) { return (instance == null || GetWantsToSkipRegistration(instance.GetType())); }
        public static bool GetWantsToSkipRegistration<TService>() { return GetWantsToSkipRegistration(typeof(TService)); }
        public static bool GetWantsToSkipRegistration(Type type)
        {
            return (type == null || _wantToSkipServiceRegistrationType.IsAssignableFrom(type));
        }
    }
}
