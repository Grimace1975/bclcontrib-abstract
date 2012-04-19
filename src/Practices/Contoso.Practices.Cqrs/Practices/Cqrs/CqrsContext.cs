﻿#region License
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
        ICommandBus CommandBus { get; }
        IEventBus EventBus { get; }
		IServiceLocator Locator { get; }
	}

	/// <summary>
	/// CqrsContext
	/// </summary>
	public class CqrsContext : ICqrsContext
	{
		private Lazy<IServiceLocator> _locator;
		private string _name;

		public CqrsContext()
			: this(ServiceLocatorManager.Lazy, null) { }
		public CqrsContext(string name)
            : this(ServiceLocatorManager.Lazy, name) { }
		public CqrsContext(Lazy<IServiceLocator> locator)
			: this(locator, null) { }
		public CqrsContext(Lazy<IServiceLocator> locator, string name)
		{
			_locator = locator;
			_name = name;
		}

		public void Dispose()
		{
			if (HasStarted)
				Stop();
		}
		
		public IAggregateRootRepository AggregateRepository { get; protected set; }
        public ICommandBus CommandBus { get; protected set; }
		public IEventBus EventBus { get; protected set; }
		public IServiceLocator Locator { get; protected set; }

		#region Start/Stop

		public bool HasStarted { get; protected set; }

		public void Start()
		{
			if (HasStarted)
				throw new InvalidOperationException("Already started");
			Locator = _locator.Value;
			LoadServices();
			var registrar = Locator.Registrar;
			RegisterWithServiceLocator(registrar);
			HasStarted = true;
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
        protected virtual ICommandBus GetCommandBus() { return Locator.Resolve<ICommandBus>(); }
        protected virtual IEventBus GetEventBus() { return Locator.Resolve<IEventBus>(); }
		protected virtual IAggregateRootRepository GetAggregateRepository() { return Locator.Resolve<IAggregateRootRepository>(); }

		public void Stop()
		{
			if (!HasStarted)
				throw new InvalidOperationException("Already stopped");
			Locator = null;
			HasStarted = false;
		}

		#endregion
	}
}