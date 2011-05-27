using System;
using MvcTurbine.ComponentModel;
using System.Collections.Generic;

namespace Contoso.MvcTurbine
{
	internal sealed class RegistrationStub : IServiceRegistrar, IDisposable
	{
		public void Dispose() { }
		public void Register<Interface, Implementation>()
			where Implementation : class, Interface { throw new NotImplementedException(); }
		public void Register<Interface>(Func<Interface> factoryMethod)
			where Interface : class { throw new NotImplementedException(); }
		public void Register<Interface, Implementation>(string key)
			where Implementation : class, Interface { throw new NotImplementedException(); }
		public void Register<Interface>(Type implType)
			where Interface : class { throw new NotImplementedException(); }
		public void Register<Interface>(Interface instance)
			where Interface : class { throw new NotImplementedException(); }
		public void Register(string key, Type type) { throw new NotImplementedException(); }
		public void Register(Type serviceType, Type implType) { throw new NotImplementedException(); }
		public void RegisterAll<Interface>() { throw new NotImplementedException(); }
	}


}
