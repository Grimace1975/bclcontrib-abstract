using System;
using System.Linq;
using System.Collections.Generic;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Impl;
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

        public IEnumerable<IHandler> GetAllHandlersFor(Type type)
        {
            throw new NotImplementedException();
            //return _locator.All()
            //     .Where(r=>type.IsAssignableFrom(r.MappedToType))
            //     .Select(r=> new DefaultHandler(r.RegisteredType, r.MappedToType, () => _locator.Resolve(r.MappedToType));
        }

        public void Release(object item) { }
        public T Resolve<T>() { return (T)_locator.Resolve(typeof(T)); }
        public object Resolve(Type type) { return _locator.Resolve(type); }
        public IEnumerable<T> ResolveAll<T>() { return _locator.ResolveAll(typeof(T)).Cast<T>(); }
    }
}