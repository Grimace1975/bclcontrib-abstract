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
    /// ServiceBusInstance
    /// </summary>
    public class ServiceBusInstance : ServiceInstanceBase<IServiceBus, IServiceBusSetup, Action<IServiceBus>>
    {
        public ServiceBusInstance()
            : base(() => new ServiceBusInstance()) { }
    }

    ///// <summary>
    ///// ServiceBusInstance
    ///// </summary>
    //public class ServiceBusInstance : IServiceBusSetup
    //{
    //    private readonly object _lock = new object();
    //    private Func<IServiceBus> _provider;
    //    private IServiceBus _serviceBus;

    //    public IServiceBusSetup SetBusProvider(Func<IServiceBus> provider) { return SetBusProvider(provider, new ServiceBusInstance()); }
    //    public IServiceBusSetup SetBusProvider(Func<IServiceBus> provider, IServiceBusSetup setup)
    //    {
    //        _provider = provider;
    //        return (Setup = setup);
    //    }

    //    public IServiceBusSetup Setup { get; private set; }

    //    public IServiceBus Current
    //    {
    //        get
    //        {
    //            if (_provider == null)
    //                throw new InvalidOperationException(Local.UndefinedServiceBusProvider);
    //            if (_serviceBus == null)
    //                lock (_lock)
    //                    if (_serviceBus == null)
    //                    {
    //                        _serviceBus = _provider();
    //                        if (_serviceBus == null)
    //                            throw new InvalidOperationException();
    //                        if (Setup != null)
    //                            Setup.Finally(_serviceBus);
    //                    }
    //            return _serviceBus;
    //        }
    //    }

    //    #region IServiceBusSetup

    //    private List<Action<IServiceBus>> _actions = new List<Action<IServiceBus>>();

    //    IServiceBusSetup IServiceBusSetup.Do(Action<IServiceBus> action)
    //    {
    //        _actions.Add(action);
    //        return this;
    //    }

    //    void IServiceBusSetup.Finally(IServiceBus bus)
    //    {
    //        foreach (var action in _actions)
    //            action(bus);
    //    }

    //    #endregion
    //}
}
