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
    /// IServiceInstance
    /// </summary>
    public interface IServiceInstance<TIService, TIServiceSetup>
    {
        TIServiceSetup SetProvider(Func<TIService> provider);
        TIServiceSetup SetProvider(Func<TIService> provider, TIServiceSetup setup);
        TIServiceSetup Setup { get; }
        TIService Current { get; }
    }

    /// <summary>
    /// IServiceSetup
    /// </summary>
    public interface IServiceSetup<TIServiceSetup, TServiceSetupAction>
    {
        TIServiceSetup Do(TServiceSetupAction action);
        //void Finally(IEnumerable<TServiceSetupAction> actions, object tag);
    }

    /// <summary>
    /// ServiceInstanceBase
    /// </summary>
    public abstract class ServiceInstanceBase<TIService, TIServiceSetup, TServiceSetupAction> : IServiceInstance<TIService, TIServiceSetup>, IServiceSetup<TIServiceSetup, TServiceSetupAction>
        where TIServiceSetup : IServiceSetup<TIServiceSetup, TServiceSetupAction>
    {
        private readonly object _lock = new object();
        private Func<TIService> _provider;
        private TIService _service;
        private Func<TIServiceSetup> _defaultServiceSetupBuilder;

        public ServiceInstanceBase(Func<ServiceInstanceBase<TIService, TIServiceSetup, TServiceSetupAction>> defaultServiceBuilder)
        {
            _defaultServiceSetupBuilder = () => (TIServiceSetup)(object)defaultServiceBuilder();
        }
        public ServiceInstanceBase(Func<TIServiceSetup> defaultServiceSetupBuilder)
        {
            _defaultServiceSetupBuilder = defaultServiceSetupBuilder;
        }

        public TIServiceSetup SetProvider(Func<TIService> provider) { return SetProvider(provider, _defaultServiceSetupBuilder()); }
        public TIServiceSetup SetProvider(Func<TIService> provider, TIServiceSetup setup)
        {
            _provider = provider;
            return (Setup = setup);
        }

        public TIServiceSetup Setup { get; private set; }

        public TIService Current
        {
            get
            {
                if (_provider == null)
                    throw new InvalidOperationException(Local.UndefinedServiceBusProvider);
                if (_service == null)
                    lock (_lock)
                        if (_service == null)
                        {
                            _service = _provider();
                            if (_service == null)
                                throw new InvalidOperationException();
                            //if (Setup != null)
                            //    Setup.Finally(_service);
                        }
                return _service;
            }
        }

        #region IServiceSetup

        private List<TServiceSetupAction> _actions = new List<TServiceSetupAction>();

        TIServiceSetup IServiceSetup<TIServiceSetup, TServiceSetupAction>.Do(TServiceSetupAction action)
        {
            _actions.Add(action);
            return Setup;
        }

        //void IServiceSetup<TService, TServiceSetupAction>.Finally(IServiceRegistrar registrar, IServiceLocator locator)
        //{
        //    foreach (var action in _actions)
        //        action(registrar, locator);
        //}

        #endregion
    }
}
