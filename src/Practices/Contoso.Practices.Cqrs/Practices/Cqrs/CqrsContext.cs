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
        /// <summary>
        /// Starts this instance.
        /// </summary>
		void Start();
        /// <summary>
        /// Stops this instance.
        /// </summary>
		void Stop();
        /// <summary>
        /// Gets the aggregate repository.
        /// </summary>
		IAggregateRootRepository AggregateRepository { get; }
        /// <summary>
        /// Gets the command bus.
        /// </summary>
        ICommandBus CommandBus { get; }
        /// <summary>
        /// Gets the event bus.
        /// </summary>
        IEventBus EventBus { get; }
        /// <summary>
        /// Gets the locator.
        /// </summary>
		IServiceLocator Locator { get; }
	}

	/// <summary>
	/// CqrsContext
	/// </summary>
	public class CqrsContext : ICqrsContext
	{
		private Lazy<IServiceLocator> _locator;
		private string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="CqrsContext"/> class.
        /// </summary>
		public CqrsContext()
			: this(ServiceLocatorManager.Lazy, null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CqrsContext"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
		public CqrsContext(string name)
            : this(ServiceLocatorManager.Lazy, name) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CqrsContext"/> class.
        /// </summary>
        /// <param name="locator">The locator.</param>
		public CqrsContext(Lazy<IServiceLocator> locator)
			: this(locator, null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CqrsContext"/> class.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
		public CqrsContext(Lazy<IServiceLocator> locator, string name)
		{
			_locator = locator;
			_name = name;
		}

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
		public void Dispose()
		{
			if (HasStarted)
				Stop();
		}

        /// <summary>
        /// Gets or sets the aggregate repository.
        /// </summary>
        /// <value>
        /// The aggregate repository.
        /// </value>
		public IAggregateRootRepository AggregateRepository { get; protected set; }
        /// <summary>
        /// Gets or sets the command bus.
        /// </summary>
        /// <value>
        /// The command bus.
        /// </value>
        public ICommandBus CommandBus { get; protected set; }
        /// <summary>
        /// Gets or sets the event bus.
        /// </summary>
        /// <value>
        /// The event bus.
        /// </value>
		public IEventBus EventBus { get; protected set; }
        /// <summary>
        /// Gets or sets the locator.
        /// </summary>
        /// <value>
        /// The locator.
        /// </value>
		public IServiceLocator Locator { get; protected set; }

		#region Start/Stop

        /// <summary>
        /// Gets or sets a value indicating whether this instance has started.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has started; otherwise, <c>false</c>.
        /// </value>
		public bool HasStarted { get; protected set; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
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
        /// <summary>
        /// Gets the command bus.
        /// </summary>
        /// <returns></returns>
        protected virtual ICommandBus GetCommandBus() { return Locator.Resolve<ICommandBus>(); }
        /// <summary>
        /// Gets the event bus.
        /// </summary>
        /// <returns></returns>
        protected virtual IEventBus GetEventBus() { return Locator.Resolve<IEventBus>(); }
        /// <summary>
        /// Gets the aggregate repository.
        /// </summary>
        /// <returns></returns>
		protected virtual IAggregateRootRepository GetAggregateRepository() { return Locator.Resolve<IAggregateRootRepository>(); }

        /// <summary>
        /// Stops this instance.
        /// </summary>
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