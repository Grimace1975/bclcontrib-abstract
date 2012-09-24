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
using System;
using System.Collections.Generic;
using System.Linq;
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
            return _locator.Registrar.GetRegistrationsFor(type)
                .Select(x => (IHandler)new DefaultHandler(x.ServiceType, x.ImplementationType, () => _locator.Resolve(x.ServiceType, x.Name)));
        }

        public void Release(object item) { }
        public T Resolve<T>() { return (T)_locator.Resolve(typeof(T)); }
        public object Resolve(Type type) { return _locator.Resolve(type); }
        public IEnumerable<T> ResolveAll<T>() { return _locator.ResolveAll(typeof(T)).Cast<T>(); }
    }
}