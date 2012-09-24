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
using MvcTurbine.ComponentModel;
using System.Collections.Generic;
using System.Reflection;

namespace Contoso.Abstract
{
    /// <summary>
    /// TurbineServiceLocatorAbstractor
    /// </summary>
    public class TurbineServiceLocatorAbstractor : IServiceLocator
    {
        private static readonly MethodInfo _coerceRegisterWithKeyMethod = typeof(TurbineServiceLocatorAbstractor).GetMethod("CoerceRegisterWithKey", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo _coerceRegisterMethod = typeof(TurbineServiceLocatorAbstractor).GetMethod("CoerceRegister", BindingFlags.NonPublic | BindingFlags.Instance);
        private System.Abstract.IServiceLocator _locator;
        private System.Abstract.IServiceRegistrar _registrar;

        /// <summary>
        /// Initializes a new instance of the <see cref="TurbineServiceLocatorAbstractor"/> class.
        /// </summary>
        public TurbineServiceLocatorAbstractor()
            : this(System.Abstract.ServiceLocatorManager.Current) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TurbineServiceLocatorAbstractor"/> class.
        /// </summary>
        /// <param name="locator">The locator.</param>
        public TurbineServiceLocatorAbstractor(System.Abstract.IServiceLocator locator)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            _locator = locator;
            _registrar = locator.Registrar;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() { }
        /// <summary>
        /// Creates a <see cref="T:MvcTurbine.ComponentModel.IServiceRegistrar"/> to process queued
        /// registration of types.
        /// </summary>
        /// <returns></returns>
        public IServiceRegistrar Batch() { return new RegistrationStub(); }

        // register
        /// <summary>
        /// Registers the specified factory method.
        /// </summary>
        /// <typeparam name="Interface">The type of the nterface.</typeparam>
        /// <param name="factoryMethod">The factory method.</param>
        public void Register<Interface>(Func<Interface> factoryMethod)
            where Interface : class { _registrar.Register(l => factoryMethod()); }
        /// <summary>
        /// Registers the specified instance.
        /// </summary>
        /// <typeparam name="Interface">The type of the nterface.</typeparam>
        /// <param name="instance">The instance.</param>
        public void Register<Interface>(Interface instance)
            where Interface : class { _registrar.RegisterInstance(instance); }
        /// <summary>
        /// Registers the implementation type, <paramref name="implType"/>, with the locator
        /// by the given service type, <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Type of the service to register.</param>
        /// <param name="implType">Implementation to associate with the service.</param>
        public void Register(Type serviceType, Type implType) { _registrar.Register(serviceType, implType); }
        /// <summary>
        /// Registers the implementation type, <paramref name="type"/>, with the locator
        /// by the given string key.
        /// </summary>
        /// <param name="key">Unique key to distinguish the service.</param>
        /// <param name="type">Implementation type to use.</param>
        public void Register(string key, Type type) { _registrar.Register(type, key); }
        /// <summary>
        /// Registers the specified key.
        /// </summary>
        /// <typeparam name="Interface">The type of the nterface.</typeparam>
        /// <typeparam name="Implementation">The type of the mplementation.</typeparam>
        /// <param name="key">The key.</param>
        public void Register<Interface, Implementation>(string key)
            where Implementation : class, Interface { _coerceRegisterMethod.MakeGenericMethod(typeof(Interface), typeof(Implementation)).Invoke(this, null); }
        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="Interface">The type of the nterface.</typeparam>
        /// <typeparam name="Implementation">The type of the mplementation.</typeparam>
        public void Register<Interface, Implementation>()
            where Implementation : class, Interface { _coerceRegisterWithKeyMethod.MakeGenericMethod(typeof(Interface), typeof(Implementation)).Invoke(this, null); }
        /// <summary>
        /// Coerces the register with key.
        /// </summary>
        /// <typeparam name="Interface">The type of the nterface.</typeparam>
        /// <typeparam name="Implementation">The type of the mplementation.</typeparam>
        /// <param name="key">The key.</param>
        public void CoerceRegisterWithKey<Interface, Implementation>(string key)
            where Interface : class
            where Implementation : class, Interface { _registrar.Register<Interface, Implementation>(key); }
        /// <summary>
        /// Coerces the register.
        /// </summary>
        /// <typeparam name="Interface">The type of the nterface.</typeparam>
        /// <typeparam name="Implementation">The type of the mplementation.</typeparam>
        public void CoerceRegister<Interface, Implementation>()
            where Interface : class
            where Implementation : class, Interface { _registrar.Register<Interface, Implementation>(); }
        /// <summary>
        /// Registers the specified impl type.
        /// </summary>
        /// <typeparam name="Interface">The type of the nterface.</typeparam>
        /// <param name="implType">Type of the impl.</param>
        public void Register<Interface>(Type implType)
            where Interface : class
        {
            var key = string.Format("{0}-{1}", typeof(Interface).Name, implType.FullName);
            _registrar.Register(typeof(Interface), implType, key);
            // Work-around, also register this implementation to service mapping without the generated key above.
            _registrar.Register(typeof(Interface), implType);
        }
        /// <summary>
        /// Registers the implementation type, <paramref name="implType"/>, with the locator
        /// by the given service type, <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Type of the service to register.</param>
        /// <param name="implType">Implementation to associate with the service.</param>
        /// <param name="key">Unique key to distinguish the service.</param>
        public void Register(Type serviceType, Type implType, string key) { _registrar.Register(serviceType, implType, key); }

        // resolve
        /// <summary>
        /// Resolves the service of the specified type by the given type key.
        /// </summary>
        /// <param name="type">Type of service to resolve.</param>
        /// <returns>
        /// An instance of the type, null otherwise
        /// </returns>
        public object Resolve(Type type)
        {
            try { return _locator.Resolve(type); }
            catch (System.Abstract.ServiceLocatorResolutionException ex) { throw RepackException(ex); }
        }
        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public T Resolve<T>(Type type)
            where T : class
        {
            try { return (T)_locator.Resolve(type); }
            catch (System.Abstract.ServiceLocatorResolutionException ex) { throw RepackException(ex); }
        }
        /// <summary>
        /// Resolves the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T Resolve<T>(string key)
            where T : class
        {
            try { return _locator.Resolve<T>(key); }
            catch (System.Abstract.ServiceLocatorResolutionException ex) { throw RepackException(ex); }
        }
        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>()
            where T : class
        {
            try { return _locator.Resolve<T>(); }
            catch (System.Abstract.ServiceLocatorResolutionException ex) { throw RepackException(ex); }
        }
        //
        /// <summary>
        /// Resolves the services.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IList<object> ResolveServices(Type type)
        {
            try { return (IList<object>)_locator.ResolveAll(type); }
            catch (System.Abstract.ServiceLocatorResolutionException ex) { throw RepackException(ex); }
        }
        /// <summary>
        /// Resolves the services.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> ResolveServices<T>()
            where T : class
        {
            try { return (IList<T>)_locator.ResolveAll<T>(); }
            catch (System.Abstract.ServiceLocatorResolutionException ex) { throw RepackException(ex); }
        }

#if !CLR4
        // inject
        /// <summary>
        /// Injects the specified instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public TService Inject<TService>(TService instance)
            where TService : class { return _locator.Inject<TService>(instance); }

        // release and teardown
        /// <summary>
        /// Releases the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void Release(object instance) { _locator.Release(instance); }
        /// <summary>
        /// Tears down.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        public void TearDown<TService>(TService instance)
            where TService : class { _locator.TearDown<TService>(instance); }
        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset() { Dispose(); }
#endif

        private static ServiceResolutionException RepackException(System.Abstract.ServiceLocatorResolutionException ex) { return new ServiceResolutionException(ex.ServiceType, ex.InnerException); }
    }
}
