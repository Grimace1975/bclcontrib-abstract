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
using System.Collections.Generic;
namespace System.Abstract
{
    /// <summary>
    /// ServiceBusManager
    /// </summary>
    public class ServiceBusManager : IServiceBusSetup
    {
        private static readonly object _lock = new object();
        private static Func<IServiceBus> _provider;
        private static Func<IServiceLocator> _locator;
        private static IServiceBus _serviceBus;

        public static IServiceBusSetup SetBusProvider(Func<IServiceBus> provider) { return SetBusProvider(provider, GetDefaultServiceServiceLocator, new ServiceBusManager()); }
        public static IServiceBusSetup SetBusProvider(Func<IServiceBus> provider, IServiceBusSetup setup) { return SetBusProvider(provider, GetDefaultServiceServiceLocator, setup); }
        public static IServiceBusSetup SetBusProvider(Func<IServiceBus> provider, Func<IServiceLocator> locator) { return SetBusProvider(provider, locator, new ServiceBusManager()); }
        public static IServiceBusSetup SetBusProvider(Func<IServiceBus> provider, Func<IServiceLocator> locator, IServiceBusSetup setup)
        {
            _provider = provider;
            _locator = locator;
            return (Setup = setup);
        }

        public static IServiceBusSetup Setup { get; private set; }

        public static IServiceBus Current
        {
            get
            {
                if (_provider == null)
                    throw new InvalidOperationException(Local.UndefinedServiceBusProvider);
                if (_serviceBus == null)
                    lock (_lock)
                        if (_serviceBus == null)
                        {
                            _serviceBus = _provider();
                            if (_serviceBus == null)
                                throw new InvalidOperationException();
                            IServiceLocator locator;
                            if ((_locator != null) && ((locator = _locator()) != null))
                            {
                                var registrar = locator.GetRegistrar();
                                RegisterSelfInLocator(registrar, _serviceBus);
                            }
                            if (Setup != null)
                                Setup.Finally(_serviceBus);
                        }
                return _serviceBus;
            }
        }

        private static void RegisterSelfInLocator(IServiceRegistrar registrar, IServiceBus serviceBus)
        {
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


        #region IServiceBusSetup

        private List<Action<IServiceBus>> _actions = new List<Action<IServiceBus>>();

        IServiceBusSetup IServiceBusSetup.Do(Action<IServiceBus> action)
        {
            _actions.Add(action);
            return this;
        }

        void IServiceBusSetup.Finally(IServiceBus bus)
        {
            foreach (var action in _actions)
                action(bus);
        }

        #endregion
    }
}
