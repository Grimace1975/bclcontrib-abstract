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
using System.Abstract.EventSourcing;
using System;
using System.Abstract;
namespace Contoso.Practices.Cqrs
{
	/// <summary>
	/// ICqrsContext
	/// </summary>
	public interface ICqrsContext : IDisposable
	{
		void Start();
		void Stop();
		IAggregateRootRepository AggregateRepository { get; }
		IServiceBus CommandBus { get; }
		IServiceBus EventBus { get; }
		IServiceLocator Locator { get; }
	}

	/// <summary>
	/// CqrsContext
	/// </summary>
	public class CqrsContext : ICqrsContext
	{
		private Lazy<IServiceLocator> _lazyLocator;
		private string _name;

		public CqrsContext()
			: this(ServiceLocatorManager.GetDefaultService(), null) { }
		public CqrsContext(string name)
			: this(ServiceLocatorManager.GetDefaultService(), name) { }
		public CqrsContext(Lazy<IServiceLocator> locator)
			: this(locator, null) { }
		public CqrsContext(Lazy<IServiceLocator> locator, string name)
		{
			_lazyLocator = locator;
			_name = name;
		}

		public void Dispose()
		{
			if (IsStarted)
				Stop();
		}
		
		public IAggregateRootRepository AggregateRepository { get; protected set; }
		public IServiceBus CommandBus { get; protected set; }
		public IServiceBus EventBus { get; protected set; }
		public IServiceLocator Locator { get; protected set; }

		#region Start/Stop

		public bool IsStarted { get; protected set; }

		public void Start()
		{
			if (IsStarted)
				throw new InvalidOperationException("Already started");
			Locator = _lazyLocator.Value;
			LoadServices();
			var registrar = Locator.GetRegistrar();
			RegisterWithServiceLocator(registrar);
			IsStarted = true;
		}

		private void LoadServices()
		{
			AggregateRepository = GetAggregateRepository();
			EventBus = GetEventBus();
			CommandBus = GetCommandBus();
		}
		private void RegisterWithServiceLocator(IServiceRegistrar registrar)
		{
			registrar.RegisterInstance<ICqrsContext>(this, _name);
		}
		protected virtual IServiceBus GetEventBus() { return Locator.Resolve<IServiceBus>("EventBus"); }
		protected virtual IServiceBus GetCommandBus() { return Locator.Resolve<IServiceBus>("CommandBus"); }
		protected virtual IAggregateRootRepository GetAggregateRepository() { return Locator.Resolve<IAggregateRootRepository>(); }

		public void Stop()
		{
			if (!IsStarted)
				throw new InvalidOperationException("Already stopped");
			Locator = null;
			IsStarted = false;
		}

		#endregion
	}
}