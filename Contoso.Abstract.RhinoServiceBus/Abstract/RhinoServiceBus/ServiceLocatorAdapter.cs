using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.ServiceBus.Internal;
namespace Contoso.Abstract.RhinoServiceBus
{
    internal class ServiceLocatorAdapter : IServiceLocator
    {
        private readonly System.Abstract.IServiceLocator _locator;

        public ServiceLocatorAdapter(System.Abstract.IServiceLocator locator)
        {
            _locator = locator;
        }

        public bool CanResolve(Type type) { return _locator.Registrar.HasRegistered(type); }

        public IEnumerable<IHandler> GetAllHandlersFor(Type type) { return _locator.Registrar.GetRegistrationsFor(type).Select(x => x.ServiceType as IHandler).Where(x => x != null); }

        public void Release(object item) { }
        public T Resolve<T>() { return (T)_locator.Resolve(typeof(T)); }
        public object Resolve(Type type) { return _locator.Resolve(type); }
        public IEnumerable<T> ResolveAll<T>() { return _locator.ResolveAll(typeof(T)).Cast<T>(); }
    }
}