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
    /// ServiceLocatorInstance
    /// </summary>
    public class ServiceLocatorInstance : IServiceLocatorSetup
    {
        private readonly object _lock = new object();
        private Func<IServiceLocator> _provider;
        private IServiceLocator _locator;

        public IServiceLocatorSetup SetLocatorProvider(Func<IServiceLocator> provider) { return SetLocatorProvider(provider, new ServiceLocatorInstance()); }
        public IServiceLocatorSetup SetLocatorProvider(Func<IServiceLocator> provider, IServiceLocatorSetup setup)
        {
            _provider = provider;
            return (Setup = setup);
        }

        public IServiceLocatorSetup Setup { get; private set; }

        public IServiceLocator Current
        {
            get
            {
                if (_provider == null)
                    throw new InvalidOperationException(Local.UndefinedServiceLocatorProvider);
                if (_locator == null)
                    lock (_lock)
                        if (_locator == null)
                        {
                            _locator = _provider();
                            if (_locator == null)
                                throw new InvalidOperationException();
                            var registrar = _locator.GetRegistrar();
                            RegisterSelfInLocator(registrar, _locator);
                            if (Setup != null)
                                Setup.Finally(registrar, _locator);
                        }
                return _locator;
            }
        }

        private static void RegisterSelfInLocator(IServiceRegistrar registrar, IServiceLocator locator)
        {
            registrar.RegisterInstance<IServiceLocator>(locator);
        }

        #region IServiceLocatorSetup

        private List<Action<IServiceRegistrar, IServiceLocator>> _actions = new List<Action<IServiceRegistrar, IServiceLocator>>();

        IServiceLocatorSetup IServiceLocatorSetup.Do(Action<IServiceRegistrar, IServiceLocator> action)
        {
            _actions.Add(action);
            return this;
        }

        void IServiceLocatorSetup.Finally(IServiceRegistrar registrar, IServiceLocator locator)
        {
            foreach (var action in _actions)
                action(registrar, locator);
        }

        #endregion
    }
}
