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
using System;
using MvcTurbine.ComponentModel;
using System.Collections.Generic;
using System.Reflection;

namespace Contoso.Abstract
{
    /// <summary>
    /// TurbineServiceLocatorAbstractor
    /// </summary
    public class TurbineServiceLocatorAbstractor : IServiceLocator
    {
        private static readonly MethodInfo _coerceRegisterWithKeyMethod = typeof(TurbineServiceLocatorAbstractor).GetMethod("CoerceRegisterWithKey", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo _coerceRegisterMethod = typeof(TurbineServiceLocatorAbstractor).GetMethod("CoerceRegister", BindingFlags.NonPublic | BindingFlags.Instance);
        private System.Abstract.IServiceLocator _locator;
        private System.Abstract.IServiceRegistrar _registrar;

        public TurbineServiceLocatorAbstractor()
            : this(System.Abstract.ServiceLocatorManager.Current) { }
        public TurbineServiceLocatorAbstractor(System.Abstract.IServiceLocator locator)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            _locator = locator;
            _registrar = locator.Registrar;
        }

        public void Dispose() { }
        public IServiceRegistrar Batch() { return new RegistrationStub(); }

        // register
        public void Register<Interface>(Func<Interface> factoryMethod)
            where Interface : class { _registrar.Register(l => factoryMethod()); }
        public void Register<Interface>(Interface instance)
            where Interface : class { _registrar.RegisterInstance(instance); }
        public void Register(Type serviceType, Type implType) { _registrar.Register(serviceType, implType); }
        public void Register(string key, Type type) { _registrar.Register(type, key); }
        public void Register<Interface, Implementation>(string key)
            where Implementation : class, Interface { _coerceRegisterMethod.MakeGenericMethod(typeof(Interface), typeof(Implementation)).Invoke(this, null); }
        public void Register<Interface, Implementation>()
            where Implementation : class, Interface { _coerceRegisterWithKeyMethod.MakeGenericMethod(typeof(Interface), typeof(Implementation)).Invoke(this, null); }
        public void CoerceRegisterWithKey<Interface, Implementation>(string key)
            where Interface : class
            where Implementation : class, Interface { _registrar.Register<Interface, Implementation>(key); }
        public void CoerceRegister<Interface, Implementation>()
            where Interface : class
            where Implementation : class, Interface { _registrar.Register<Interface, Implementation>(); }
        public void Register<Interface>(Type implType)
            where Interface : class
        {
            var key = string.Format("{0}-{1}", typeof(Interface).Name, implType.FullName);
            _registrar.Register(typeof(Interface), implType, key);
            // Work-around, also register this implementation to service mapping without the generated key above.
            _registrar.Register(typeof(Interface), implType);
        }
        public void Register(Type serviceType, Type implType, string key) { _registrar.Register(serviceType, implType, key); }

        // resolve
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
        //
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

#if !CLR4
        // inject
        public TService Inject<TService>(TService instance)
            where TService : class { return _locator.Inject<TService>(instance); }

        // release and teardown
        public void Release(object instance) { _locator.Release(instance); }
        public void TearDown<TService>(TService instance)
            where TService : class { _locator.TearDown<TService>(instance); }
        public void Reset() { Dispose(); }
#endif

        private static ServiceResolutionException RepackException(System.Abstract.ServiceLocatorResolutionException ex) { return new ServiceResolutionException(ex.ServiceType, ex.InnerException); }
    }
}
