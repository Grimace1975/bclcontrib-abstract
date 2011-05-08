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
    /// ServiceLogInstance
    /// </summary>
    public class ServiceLogInstance : ServiceInstanceBase<IServiceLog, IServiceLogSetup, Action<IServiceLog>>
    {
        public ServiceLogInstance()
            : base(() => new ServiceLogInstance()) { }
    }

    ///// <summary>
    ///// ServiceLogInstance
    ///// </summary>
    //public class ServiceLogInstance : IServiceLogSetup
    //{
    //    private readonly object _lock = new object();
    //    private Func<IServiceLog> _provider;
    //    private IServiceLog _serviceLog;

    //    public IServiceLogSetup SetLogProvider(Func<IServiceLog> provider) { return SetLogProvider(provider, new ServiceLogInstance()); }
    //    public IServiceLogSetup SetLogProvider(Func<IServiceLog> provider, IServiceLogSetup setup)
    //    {
    //        _provider = provider;
    //        return (Setup = setup);
    //    }

    //    public IServiceLogSetup Setup { get; private set; }

    //    public IServiceLog Current
    //    {
    //        get
    //        {
    //            if (_provider == null)
    //                throw new InvalidOperationException(Local.UndefinedServiceLogProvider);
    //            if (_serviceLog == null)
    //                lock (_lock)
    //                    if (_serviceLog == null)
    //                    {
    //                        _serviceLog = _provider();
    //                        if (_serviceLog == null)
    //                            throw new InvalidOperationException();
    //                        if (Setup != null)
    //                            Setup.Finally(_serviceLog);
    //                    }
    //            return _serviceLog;
    //        }
    //    }

    //    #region IServiceLogSetup

    //    private List<Action<IServiceLog>> _actions = new List<Action<IServiceLog>>();

    //    IServiceLogSetup IServiceLogSetup.Do(Action<IServiceLog> action)
    //    {
    //        _actions.Add(action);
    //        return this;
    //    }

    //    void IServiceLogSetup.Finally(IServiceLog log)
    //    {
    //        foreach (var action in _actions)
    //            action(log);
    //    }

    //    #endregion
    //}
}
