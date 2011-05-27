using System;
using MvcTurbine.ComponentModel;
using System.Collections.Generic;

namespace Contoso.MvcTurbine
{
	public class TurbineServiceLocatorAbstractor : IServiceLocator
	{
		private System.Abstract.IServiceLocator _locator;
		private System.Abstract.IServiceRegistrar _registrar;

		public TurbineServiceLocatorAbstractor()
			: this(System.Abstract.ServiceLocatorManager.Current) { }
		public TurbineServiceLocatorAbstractor(System.Abstract.IServiceLocator locator)
		{
			if (locator == null)
				throw new ArgumentNullException("locator");
			_locator = locator;
			_registrar = locator.GetRegistrar();
		}

		public void Dispose() { }
		public IServiceRegistrar Batch() { return new RegistrationStub(); }

		// Register
		public void Register<Interface>(Func<Interface> factoryMethod)
			where Interface : class { _registrar.Register(l => factoryMethod()); }
		public void Register<Interface>(Interface instance)
			where Interface : class { _registrar.RegisterInstance(instance); }
		public void Register(Type serviceType, Type implType) { _registrar.Register(serviceType, implType); }
		public void Register(string key, Type type) { _registrar.Register(type, key); }
		public void Register<Interface, Implementation>(string key)
			where Implementation : class, Interface { _registrar.Register<Interface, Implementation>(key); }
		public void Register<Interface, Implementation>()
			where Implementation : class, Interface { _registrar.Register<Interface, Implementation>(); }
		public void Register<Interface>(Type implType)
			where Interface : class
		{
			var key = string.Format("{0}-{1}", typeof(Interface).Name, implType.FullName);
			_registrar.Register(typeof(Interface), implType, key);
			// Work-around, also register this implementation to service mapping without the generated key above.
			_registrar.Register(typeof(Interface), implType);
		}

		// Resolve
		public object Resolve(Type type)
		{
			try { return _locator.Resolve(type); }
			catch (System.Abstract.ServiceLocatorResolutionException ex) { throw RepackException(ex); }
		}
		public T Resolve<T>(Type type)
			where T : class
		{
			try { return (T)_locator.Resolve(type); }
			catch (System.Abstract.ServiceLocatorResolutionException ex) { throw RepackException(ex); }
		}
		public T Resolve<T>(string key)
			where T : class
		{
			try { return _locator.Resolve<T>(key); }
			catch (System.Abstract.ServiceLocatorResolutionException ex) { throw RepackException(ex); }
		}
		public T Resolve<T>()
			where T : class
		{
			try { return _locator.Resolve<T>(); }
			catch (System.Abstract.ServiceLocatorResolutionException ex) { throw RepackException(ex); }
		}

		// ResolveAll
		public IList<object> ResolveServices(Type type)
		{
			try { return (IList<object>)_locator.ResolveAll(type); }
			catch (System.Abstract.ServiceLocatorResolutionException ex) { throw RepackException(ex); }
		}
		public IList<T> ResolveServices<T>()
			where T : class
		{
			try { return (IList<T>)_locator.ResolveAll<T>(); }
			catch (System.Abstract.ServiceLocatorResolutionException ex) { throw RepackException(ex); }
		}

		private static ServiceResolutionException RepackException(System.Abstract.ServiceLocatorResolutionException ex)
		{
			return new ServiceResolutionException(ex.ServiceType, ex.InnerException);
		}
	}
}
