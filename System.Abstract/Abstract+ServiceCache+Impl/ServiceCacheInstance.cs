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
    /// ServiceCacheInstance
    /// </summary>
    public class ServiceCacheInstance : IServiceCacheSetup
    {
        private readonly object _lock = new object();
        private Func<IServiceCache> _provider;
        private IServiceCache _serviceCache;

        public IServiceCacheSetup SetCacheProvider(Func<IServiceCache> provider) { return SetCacheProvider(provider, new ServiceCacheInstance()); }
		public IServiceCacheSetup SetCacheProvider(Func<IServiceCache> provider, IServiceCacheSetup setup)
        {
            _provider = provider;
            return (Setup = setup);
        }

        public IServiceCacheSetup Setup { get; private set; }

        public IServiceCache Current
        {
            get
            {
                if (_provider == null)
                    throw new InvalidOperationException(Local.UndefinedServiceBusProvider);
				if (_serviceCache == null)
                    lock (_lock)
						if (_serviceCache == null)
                        {
                            _serviceCache = _provider();
							if (_serviceCache == null)
                                throw new InvalidOperationException();
                            if (Setup != null)
								Setup.Finally(_serviceCache);
                        }
                return _serviceCache;
            }
        }

        #region IServiceCacheSetup

		private List<Action<IServiceCache>> _actions = new List<Action<IServiceCache>>();

        IServiceCacheSetup IServiceCacheSetup.Do(Action<IServiceCache> action)
        {
            _actions.Add(action);
            return this;
        }

        void IServiceCacheSetup.Finally(IServiceCache bus)
        {
            foreach (var action in _actions)
                action(bus);
        }

        #endregion
    }
}
