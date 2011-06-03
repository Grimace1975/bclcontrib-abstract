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
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Reflection;
namespace System.Abstract.Parts
{
	/// <summary>
	/// ServiceManagerBase
	/// </summary>
	public abstract partial class ServiceManagerBase<TIService, TServiceSetupAction>
		where TIService : class
	{
		private static readonly ConditionalWeakTable<Lazy<TIService>, ISetupDescriptor> _setupDescriptors = new ConditionalWeakTable<Lazy<TIService>, ISetupDescriptor>();
		private static readonly object _lock = new object();

		public static Lazy<TIService> Lazy { get; private set; }
		public static Lazy<TIService> SetProvider(Func<TIService> provider) { return Lazy = new Lazy<TIService>(provider); }
		public static Lazy<TIService> SetProvider(Func<TIService> provider, ISetupDescriptor setup) { return Lazy = new Lazy<TIService>(provider); }
		protected static SetupRegistration Registration { get; set; }

		// Force "precise" initialization
		static ServiceManagerBase() { }

		/// <summary>
		/// Current
		/// </summary
		public static TIService Current
		{
			get { return Lazy.Value; }
		}

		public static Lazy<TIService> GetDefaultService()
		{
			try { return Lazy; }
			catch (InvalidOperationException) { return null; }
		}

		#region Setup

		/// <summary>
		/// SetupRegistration
		/// </summary
		protected class SetupRegistration
		{
			public SetupRegistration()
			{
				OnServiceRegistrar = RegisterInstance;
			}

			public Func<TIService, ISetupDescriptor, TIService> OnSetup { get; set; }
			public Action<TIService, IServiceRegistrar, IServiceLocator, string> OnServiceRegistrar { get; set; }
		}

		/// <summary>
		/// RegisterInstance
		/// </summary>
		protected static void RegisterInstance<T>(T service, IServiceRegistrar registrar, IServiceLocator locator, string name)
			where T : class
		{
			if (registrar == null)
				throw new ArgumentNullException("registrar");
			if (name == null)
				registrar.RegisterInstance<T>(service);
			else
				registrar.RegisterInstance<T>(service, name);
		}

		#endregion

		#region IServiceSetup

		/// <summary>
		/// ApplySetup
		/// </summary>
		protected static TIService ApplySetup(Lazy<TIService> service, TIService instance)
		{
			if (service == null)
				throw new ArgumentNullException("service");
			if (instance == null)
				throw new NullReferenceException("instance");
			var registration = Registration;
			if (registration == null)
				throw new NullReferenceException("Registration");
			var onSetup = registration.OnSetup;
			if (onSetup == null)
				return instance;
			// find descriptor
			ISetupDescriptor setupDescriptor;
			_setupDescriptors.TryGetValue(service, out setupDescriptor);
			return onSetup(instance, setupDescriptor);
		}

		/// <summary>
		/// ProtectedGetSetupDescriptor
		/// </summary>
		protected static ISetupDescriptor ProtectedGetSetupDescriptor(Lazy<TIService> service)
		{
			if (service == null)
				throw new ArgumentNullException("service");
			ISetupDescriptor setupDescriptor;
			if (_setupDescriptors.TryGetValue(service, out setupDescriptor))
				return setupDescriptor;
			lock (_lock)
				if (!_setupDescriptors.TryGetValue(service, out setupDescriptor))
				{
					setupDescriptor = new SetupDescriptor(Registration);
					_setupDescriptors.Add(service, setupDescriptor);
					service.HookValueFactory(valueFactory => ApplySetup(service, valueFactory()));
				}
			return setupDescriptor;
		}

		/// <summary>
		/// ISetupDescriptor
		/// </summary>
		public interface ISetupDescriptor
		{
			void Do(TServiceSetupAction action);
			void RegisterWithServiceLocator(Lazy<TIService> service, string name);
			void RegisterWithServiceLocator(Lazy<TIService> service, Lazy<IServiceLocator> locator, string name);
			IEnumerable<TServiceSetupAction> Actions { get; }
		}

		/// <summary>
		/// SetupDescriptor
		/// </summary>
		protected class SetupDescriptor : ISetupDescriptor
		{
			private List<TServiceSetupAction> _actions = new List<TServiceSetupAction>();
			private SetupRegistration _registration;

			public SetupDescriptor(SetupRegistration registration) { _registration = registration; }

			void ISetupDescriptor.Do(TServiceSetupAction action)
			{
				if (action == null)
					throw new ArgumentNullException("action");
				_actions.Add(action);
			}

			void ISetupDescriptor.RegisterWithServiceLocator(Lazy<TIService> service, string name) { ((ISetupDescriptor)this).RegisterWithServiceLocator(service, ServiceLocatorManager.GetDefaultService(), name); }
			void ISetupDescriptor.RegisterWithServiceLocator(Lazy<TIService> service, Lazy<IServiceLocator> locator, string name)
			{
				if (locator == null)
					throw new ArgumentNullException("locator", "Unable to locate ServiceLocator, please ensure this is defined first.");
				var serviceLocatorSetupDescriptor = ServiceLocatorManager.GetSetupDescriptor(locator);
				if (serviceLocatorSetupDescriptor == null)
					throw new NullReferenceException();
				var serviceLocatorRegistrar = _registration.OnServiceRegistrar;
				if (serviceLocatorRegistrar == null)
					throw new NullReferenceException("registration.ServiceLocatorRegistrar");
				serviceLocatorSetupDescriptor.Do((r, l) => serviceLocatorRegistrar(service.Value, r, l, name));
			}

			IEnumerable<TServiceSetupAction> ISetupDescriptor.Actions
			{
				get { return _actions; }
			}
		}

		#endregion
	}
}
