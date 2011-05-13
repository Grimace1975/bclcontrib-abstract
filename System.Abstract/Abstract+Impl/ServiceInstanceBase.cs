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
	public interface IServiceInstance<TIService, TServiceSetupAction>
	{
		IServiceSetup<TServiceSetupAction> SetProvider(Func<TIService> provider);
		IServiceSetup<TServiceSetupAction> SetProvider(Func<TIService> provider, IServiceSetup<TServiceSetupAction> setup);
		IServiceSetup<TServiceSetupAction> Setup { get; }
		TIService Current { get; }
	}

	/// <summary>
	/// IServiceSetup
	/// </summary>
	public interface IServiceSetup<TServiceSetupAction>
	{
		IServiceSetup<TServiceSetupAction> Do(TServiceSetupAction action);
		IEnumerable<TServiceSetupAction> ToList();
	}

	/// <summary>
	/// ServiceInstanceBase
	/// </summary>
	public abstract class ServiceInstanceBase<TIService, TServiceSetupAction> : IServiceInstance<TIService, TServiceSetupAction>, IServiceSetup<TServiceSetupAction>
		//where TIServiceSetup : IServiceSetup<TIServiceSetup, TServiceSetupAction>
	{
		private readonly object _lock = new object();
		private Func<TIService> _provider;
		private TIService _service;
		private Func<IServiceSetup<TServiceSetupAction>> _defaultServiceSetup;
		private Action<TIService, IEnumerable<TServiceSetupAction>> _onSetup;

		public ServiceInstanceBase(Func<IServiceSetup<TServiceSetupAction>> defaultServiceSetup, Action<TIService, IEnumerable<TServiceSetupAction>> onSetup)
		{
			_defaultServiceSetup = defaultServiceSetup;
			_onSetup = onSetup;
		}

		public IServiceSetup<TServiceSetupAction> SetProvider(Func<TIService> provider) { return SetProvider(provider, _defaultServiceSetup()); }
		public IServiceSetup<TServiceSetupAction> SetProvider(Func<TIService> provider, IServiceSetup<TServiceSetupAction> setup)
		{
			_provider = provider;
			return (Setup = setup);
		}

		public IServiceSetup<TServiceSetupAction> Setup { get; private set; }

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
							if (_onSetup != null)
								_onSetup(_service, (Setup == null ? Setup.ToList() : null));
						}
				return _service;
			}
		}

		#region IServiceSetup

		private List<TServiceSetupAction> _actions = new List<TServiceSetupAction>();

		IServiceSetup<TServiceSetupAction> IServiceSetup<TServiceSetupAction>.Do(TServiceSetupAction action)
		{
			_actions.Add(action);
			return Setup;
		}

		IEnumerable<TServiceSetupAction> IServiceSetup<TServiceSetupAction>.ToList()
		{
			return _actions;
		}

		#endregion
	}
}
