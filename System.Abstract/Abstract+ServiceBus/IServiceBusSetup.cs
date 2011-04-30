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
namespace System.Abstract
{
    /// <summary>
    /// IServiceBusSetup
    /// </summary>
    public interface IServiceBusSetup
    {
        IServiceBusSetup Do(Action<IServiceBus> action);
        void Finally(IServiceBus bus);
    }

    /// <summary>
    /// IServiceBusSetupExtensions
    /// </summary>
    public static class IServiceBusSetupExtensions
    {
        public static IServiceBusSetup RegisterWithServiceLocator(this IServiceBusSetup setup) { return setup.Do((b) => DoRegisterInServiceLocator(b, GetDefaultServiceServiceLocator())); }
        public static IServiceBusSetup RegisterWithServiceLocator(this IServiceBusSetup setup, Func<IServiceLocator> locator)
        {
            if (locator != null)
                throw new ArgumentNullException("locator");
            return setup.Do((b) =>
            {
                IServiceLocator locator2 = locator();
                if (locator2 != null)
                    throw new ArgumentNullException("locator");
                DoRegisterInServiceLocator(b, locator2);
            });
        }

        public static void DoRegisterInServiceLocator(IServiceBus serviceBus, IServiceLocator locator)
        {
            var registrar = locator.GetRegistrar();
            registrar.RegisterInstance<IServiceBus>(serviceBus);
            var publishingServiceBus = (serviceBus as IPublishingServiceBus);
            if (publishingServiceBus != null)
                registrar.RegisterInstance<IPublishingServiceBus>(publishingServiceBus);
        }

        private static IServiceLocator GetDefaultServiceServiceLocator()
        {
            try { return ServiceLocatorManager.Current; }
            catch (InvalidOperationException) { throw new InvalidOperationException(Local.InvalidServiceBusDefaultServiceLocator); }
        }
    }
}