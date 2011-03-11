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
using System.Abstract;
using StructureMap.Configuration.DSL;
using StructureMap;
namespace Contoso.Abstract
{
    /// <summary>
    /// IStructureMapServiceRegistrar
    /// </summary>
    public interface IStructureMapServiceRegistrar : IServiceRegistrar
    {
        void RegisterAll<TService>();
    }

    /// <summary>
    /// StructureMapServiceRegistrar
    /// </summary>
    public class StructureMapServiceRegistrar : Registry, IStructureMapServiceRegistrar, IDisposable
    {
        private StructureMapServiceLocator _parent;
        private IContainer _container;

        public StructureMapServiceRegistrar(StructureMapServiceLocator parent, IContainer container)
        {
            _parent = parent;
            _container = container;
        }

        public void Dispose()
        {
            _container.Configure(x => x.AddRegistry(this));
        }

        // locator
        public IServiceLocator GetLocator() { return _parent; }
        public TServiceLocator GetLocator<TServiceLocator>()
            where TServiceLocator : class, IServiceLocator { return (_parent as TServiceLocator); }

        // register implementation
        public void Register<TService, TImplementation>()
            where TImplementation : class, TService { For<TService>().Add<TImplementation>(); }
        public void Register<TService, TImplementation>(string id)
            where TImplementation : class, TService { For(typeof(TService)).Add(typeof(TImplementation)).Named(id); }
        public void Register<TService>(Type implementationType)
            where TService : class { For(typeof(TService)).Add(implementationType); }
        public void Register<TService>(Type implementationType, string id)
            where TService : class { For(typeof(TService)).Add(implementationType).Named(id); }
        public void Register(Type serviceType, Type implementationType) { For(serviceType).Add(implementationType); }
        public void Register(Type serviceType, Type implementationType, string id) { For(serviceType).Add(implementationType).Named(id); }

        // register id
        public void Register(Type serviceType, string id) { For(serviceType).Add(serviceType).Named(id); }

        // register instance
        public new void Register<TService>(TService instance)
            where TService : class { For<TService>().Use(instance); }
        public void Register<TService>(Func<IServiceLocator, TService> factoryMethod)
            where TService : class { For<TService>().Use(x => factoryMethod(_parent)); }

        #region Domain extents

        public void RegisterAll<TService>() { Scan(scanner => scanner.AddAllTypesOf<TService>()); }

        #endregion
    }
}
