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
using System.Linq;
using System.Abstract.Parts;
namespace System.Abstract
{
    /// <summary>
    /// ServiceCacheExtensions
    /// </summary>
    public static class ServiceCacheExtensions
    {
        /// <summary>
        /// Adds the specified cache.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object Add(this IServiceCache service, string name, object value) { return service.Add(null, name, CacheItemPolicy.Default, value, ServiceCacheByDispatcher.Empty); }
        /// <summary>
        /// Adds the specified cache.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="name">The name.</param>
        /// <param name="itemPolicy">The item policy.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object Add(this IServiceCache service, string name, CacheItemPolicy itemPolicy, object value) { return service.Add(null, name, itemPolicy, value, ServiceCacheByDispatcher.Empty); }
        /// <summary>
        /// Adds the specified cache.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object Add(this IServiceCache service, object tag, string name, object value) { return service.Add(tag, name, CacheItemPolicy.Default, value, ServiceCacheByDispatcher.Empty); }
        /// <summary>
        /// Adds the specified cache.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <param name="itemPolicy">The item policy.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object Add(this IServiceCache service, object tag, string name, CacheItemPolicy itemPolicy, object value) { return service.Add(tag, name, itemPolicy, value, ServiceCacheByDispatcher.Empty); }

        /// <summary>
        /// Gets the specified cache.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache service, string name) { return service.Get(null, name); }
        /// <summary>
        /// Gets the specified cache.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="names">The names.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache service, IEnumerable<string> names) { return service.Get(null, names); }
        /// <summary>
        /// Tries the get.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool TryGet(this IServiceCache service, string name, out object value) { return service.TryGet(null, name, out value); }

        /// <summary>
        /// Removes the specified cache.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static object Remove(this IServiceCache service, string name) { return service.Remove(null, name, null); }
        /// <summary>
        /// Removes the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static object Remove(this IServiceCache service, object tag, string name) { return service.Remove(tag, name, null); }

        /// <summary>
        /// Sets the specified cache.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object Set(this IServiceCache service, string name, object value) { return service.Set(null, name, CacheItemPolicy.Default, value, ServiceCacheByDispatcher.Empty); }
        /// <summary>
        /// Sets the specified cache.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="name">The name.</param>
        /// <param name="itemPolicy">The item policy.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object Set(this IServiceCache service, string name, CacheItemPolicy itemPolicy, object value) { return service.Set(null, name, itemPolicy, value, ServiceCacheByDispatcher.Empty); }
        /// <summary>
        /// Sets the specified cache.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object Set(this IServiceCache service, object tag, string name, object value) { return service.Set(tag, name, CacheItemPolicy.Default, value, ServiceCacheByDispatcher.Empty); }
        /// <summary>
        /// Sets the specified cache.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <param name="itemPolicy">The item policy.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object Set(this IServiceCache service, object tag, string name, CacheItemPolicy itemPolicy, object value) { return service.Set(tag, name, itemPolicy, value, ServiceCacheByDispatcher.Empty); }

        /// <summary>
        /// Ensures the cache dependency.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="dependency">The dependency.</param>
        public static void EnsureCacheDependency(IServiceCache service, object tag, CacheItemDependency dependency)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            string[] names;
            if (dependency == null || (names = (dependency(service, null, tag, null) as string[])) == null)
                return;
            EnsureCacheDependency(service, names);
        }
        /// <summary>
        /// Ensures the cache dependency.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="names">The names.</param>
        public static void EnsureCacheDependency(IServiceCache service, IEnumerable<string> names)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (names != null)
                foreach (var name in names)
                    service.Add(null, name, new CacheItemPolicy { AbsoluteExpiration = ServiceCache.InfiniteAbsoluteExpiration }, string.Empty);
        }

        /// <summary>
        /// Touches the specified cache.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="names">The names.</param>
        public static void Touch(this IServiceCache service, params string[] names) { Touch(service, null, names); }
        /// <summary>
        /// Touches the specified cache.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="names">The names.</param>
        public static void Touch(this IServiceCache service, object tag, params string[] names)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            var touchable = service.Settings.Touchable;
            if (touchable == null)
                throw new NotSupportedException("Touchables are not supported");
            touchable.Touch(tag, names);
        }

        /// <summary>
        /// Makes the dependency.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="names">The names.</param>
        /// <returns></returns>
        public static CacheItemDependency MakeDependency(this IServiceCache service, params string[] names)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            var touchable = service.Settings.Touchable;
            if (touchable == null)
                throw new NotSupportedException("Touchables are not supported");
            return (c, r, tag, values) => touchable.MakeDependency(tag, names);
        }

        #region BehaveAs

        /// <summary>
        /// Behaves as.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The cache.</param>
        /// <returns></returns>
        public static T BehaveAs<T>(this IServiceCache service)
            where T : class, IServiceCache
        {
            IServiceWrapper<IServiceCache> serviceWrapper;
            do
            {
                serviceWrapper = (service as IServiceWrapper<IServiceCache>);
                if (serviceWrapper != null)
                    service = serviceWrapper.Parent;
            } while (serviceWrapper != null);
            return (service as T);
        }

        /// <summary>
        /// Behaves as.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="namespace">The @namespace.</param>
        /// <returns></returns>
        public static IServiceCache BehaveAs(this IServiceCache service, string @namespace)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (@namespace == null)
                throw new ArgumentNullException("@namespace");
            return new ServiceCacheNamespaceBehaviorWrapper(service, @namespace);
        }
        /// <summary>
        /// Behaves as.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="values">The values.</param>
        /// <param name="namespace">The @namespace.</param>
        /// <returns></returns>
        public static IServiceCache BehaveAs(this IServiceCache service, IEnumerable<object> values, out string @namespace)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            @namespace = ServiceCache.GetNamespace(values);
            if (@namespace == null)
                throw new ArgumentNullException("@values");
            return new ServiceCacheNamespaceBehaviorWrapper(service, @namespace);
        }

        #endregion

        #region Registrations

        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache service, IServiceCacheRegistration registration) { return Get<object>(service, registration, null, null); }
        /// <summary>
        /// Gets the specified cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        public static T Get<T>(this IServiceCache service, IServiceCacheRegistration registration) { return Get<T>(service, registration, null, null); }
        /// <summary>
        /// Gets the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetMany<T>(this IServiceCache service, IServiceCacheRegistration registration) { return Get<IEnumerable<T>>(service, registration, null, null); }
        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> GetMany<TKey, TValue>(this IServiceCache service, IServiceCacheRegistration registration) { return Get<IDictionary<TKey, TValue>>(service, registration, null, null); }
        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        public static IQueryable<T> GetQuery<T>(this IServiceCache service, IServiceCacheRegistration registration) { return Get<IQueryable<T>>(service, registration, null, null); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache service, IServiceCacheRegistration registration, object[] values) { return Get<object>(service, registration, null, values); }
        /// <summary>
        /// Gets the specified cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static T Get<T>(this IServiceCache service, IServiceCacheRegistration registration, object[] values) { return Get<T>(service, registration, null, values); }
        /// <summary>
        /// Gets the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetMany<T>(this IServiceCache service, IServiceCacheRegistration registration, object[] values) { return Get<IEnumerable<T>>(service, registration, null, values); }
        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> GetMany<TKey, TValue>(this IServiceCache service, IServiceCacheRegistration registration, object[] values) { return Get<IDictionary<TKey, TValue>>(service, registration, null, values); }
        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IQueryable<T> GetQuery<T>(this IServiceCache service, IServiceCacheRegistration registration, object[] values) { return Get<IQueryable<T>>(service, registration, null, values); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache service, IServiceCacheRegistration registration, object tag) { return Get<object>(service, registration, tag, null); }
        /// <summary>
        /// Gets the specified cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static T Get<T>(this IServiceCache service, IServiceCacheRegistration registration, object tag) { return Get<T>(service, registration, tag, null); }
        /// <summary>
        /// Gets the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetMany<T>(this IServiceCache service, IServiceCacheRegistration registration, object tag) { return Get<IEnumerable<T>>(service, registration, tag, null); }
        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> GetMany<TKey, TValue>(this IServiceCache service, IServiceCacheRegistration registration, object tag) { return Get<IDictionary<TKey, TValue>>(service, registration, tag, null); }
        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static IQueryable<T> GetQuery<T>(this IServiceCache service, IServiceCacheRegistration registration, object tag) { return Get<IQueryable<T>>(service, registration, tag, null); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache service, IServiceCacheRegistration registration, object tag, object[] values) { return Get<object>(service, registration, tag, values); }
        /// <summary>
        /// Gets the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetMany<T>(this IServiceCache service, IServiceCacheRegistration registration, object tag, object[] values) { return Get<IEnumerable<T>>(service, registration, tag, values); }
        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> GetMany<TKey, TValue>(this IServiceCache service, IServiceCacheRegistration registration, object tag, object[] values) { return Get<IDictionary<TKey, TValue>>(service, registration, tag, values); }
        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IQueryable<T> GetQuery<T>(this IServiceCache service, ServiceCacheRegistration registration, object tag, object[] values) { return Get<IQueryable<T>>(service, registration, tag, values); }
        /// <summary>
        /// Gets the specified cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static T Get<T>(this IServiceCache service, IServiceCacheRegistration registration, object tag, object[] values)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (registration == null)
                throw new ArgumentNullException("registration");
            var registrationDispatcher = GetRegistrationDispatcher(service);
            // fetch registration
            var recurses = 0;
            IServiceCacheRegistration foundRegistration;
            if (!ServiceCacheRegistrar.TryGetValue(registration, ref recurses, out foundRegistration))
                throw new InvalidOperationException(string.Format(Local.UndefinedServiceCacheRegistrationAB, (registration.Registrar != null ? registration.Registrar.AnchorType.ToString() : "{unregistered}"), registration.Name));
            if (foundRegistration is ServiceCacheForeignRegistration)
                throw new InvalidOperationException(Local.InvalidDataSource);
            return registrationDispatcher.Get<T>(service, foundRegistration, tag, values);
        }


        /// <summary>
        /// Sends the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="messages">The messages.</param>
        public static void Send(this IServiceCache service, IServiceCacheRegistration registration, object[] messages) { Send(service, registration, null, messages); }
        /// <summary>
        /// Sends the specified cache.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="messages">The messages.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static void Send(this IServiceCache service, IServiceCacheRegistration registration, object tag, object[] messages)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (registration == null)
                throw new ArgumentNullException("registration");
            if (messages == null)
                throw new ArgumentNullException("messages");
            var registrationDispatcher = GetRegistrationDispatcher(service);
            // fetch registration
            var recurses = 0;
            IServiceCacheRegistration foundRegistration;
            if (!ServiceCacheRegistrar.TryGetValue(registration, ref recurses, out foundRegistration))
                throw new InvalidOperationException(string.Format(Local.UndefinedServiceCacheRegistrationAB, (registration.Registrar != null ? registration.Registrar.AnchorType.ToString() : "{unregistered}"), registration.Name));
            if (foundRegistration is ServiceCacheForeignRegistration)
                throw new InvalidOperationException(Local.InvalidDataSource);
            registrationDispatcher.Send(service, foundRegistration, tag, messages);
        }

        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">The type.</param>
        /// <param name="registrationName">The registration id.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, Type anchorType, string registrationName) { return Get<object>(cache, anchorType, registrationName, null, null); }
        /// <summary>
        /// Gets the specified cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <returns></returns>
        public static T Get<T>(this IServiceCache cache, Type anchorType, string registrationName) { return Get<T>(cache, anchorType, registrationName, null, null); }
        /// <summary>
        /// Gets the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetMany<T>(this IServiceCache cache, Type anchorType, string registrationName) { return Get<IEnumerable<T>>(cache, anchorType, registrationName, null, null); }
        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> GetMany<TKey, TValue>(this IServiceCache cache, Type anchorType, string registrationName) { return Get<IDictionary<TKey, TValue>>(cache, anchorType, registrationName, null, null); }
        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <returns></returns>
        public static IQueryable<T> GetQuery<T>(this IServiceCache cache, Type anchorType, string registrationName) { return Get<IQueryable<T>>(cache, anchorType, registrationName, null, null); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">The type.</param>
        /// <param name="registrationName">The registration id.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, Type anchorType, string registrationName, object[] values) { return Get<object>(cache, anchorType, registrationName, null, values); }
        /// <summary>
        /// Gets the specified cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static T Get<T>(this IServiceCache cache, Type anchorType, string registrationName, object[] values) { return Get<T>(cache, anchorType, registrationName, null, values); }
        /// <summary>
        /// Gets the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetMany<T>(this IServiceCache cache, Type anchorType, string registrationName, object[] values) { return Get<IEnumerable<T>>(cache, anchorType, registrationName, string.Empty, values); }
        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> GetMany<TKey, TValue>(this IServiceCache cache, Type anchorType, string registrationName, object[] values) { return Get<IDictionary<TKey, TValue>>(cache, anchorType, registrationName, string.Empty, values); }
        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IQueryable<T> GetQuery<T>(this IServiceCache cache, Type anchorType, string registrationName, object[] values) { return Get<IQueryable<T>>(cache, anchorType, registrationName, string.Empty, values); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">The type.</param>
        /// <param name="registrationName">The registration id.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, Type anchorType, string registrationName, object tag) { return Get<object>(cache, anchorType, registrationName, tag, null); }
        /// <summary>
        /// Gets the specified cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static T Get<T>(this IServiceCache cache, Type anchorType, string registrationName, object tag) { return Get<T>(cache, anchorType, registrationName, tag, null); }
        /// <summary>
        /// Gets the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetMany<T>(this IServiceCache cache, Type anchorType, string registrationName, object tag) { return Get<IEnumerable<T>>(cache, anchorType, registrationName, tag, null); }
        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> GetMany<TKey, TValue>(this IServiceCache cache, Type anchorType, string registrationName, object tag) { return Get<IDictionary<TKey, TValue>>(cache, anchorType, registrationName, tag, null); }
        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static IQueryable<T> GetQuery<T>(this IServiceCache cache, Type anchorType, string registrationName, object tag) { return Get<IQueryable<T>>(cache, anchorType, registrationName, tag, null); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">The type.</param>
        /// <param name="registrationName">The registration id.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, Type anchorType, string registrationName, object tag, object[] values) { return Get<object>(cache, anchorType, registrationName, tag, values); }
        /// <summary>
        /// Gets the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetMany<T>(this IServiceCache cache, Type anchorType, string registrationName, object tag, object[] values) { return Get<IEnumerable<T>>(cache, anchorType, registrationName, tag, values); }
        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> GetMany<TKey, TValue>(this IServiceCache cache, Type anchorType, string registrationName, object tag, object[] values) { return Get<IDictionary<TKey, TValue>>(cache, anchorType, registrationName, tag, values); }
        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IQueryable<T> GetQuery<T>(this IServiceCache cache, Type anchorType, string registrationName, object tag, object[] values) { return Get<IQueryable<T>>(cache, anchorType, registrationName, tag, values); }
        /// <summary>
        /// Gets the specified cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static T Get<T>(this IServiceCache cache, Type anchorType, string registrationName, object tag, object[] values)
        {
            if (anchorType == null)
                throw new ArgumentNullException("anchorType");
            if (string.IsNullOrEmpty(registrationName))
                throw new ArgumentNullException("registrationName");
            var registrationDispatcher = GetRegistrationDispatcher(cache);
            // fetch registration
            var recurses = 0;
            IServiceCacheRegistration foundRegistration;
            if (!ServiceCacheRegistrar.TryGetValue(anchorType, registrationName, ref recurses, out foundRegistration))
                throw new InvalidOperationException(string.Format(Local.UndefinedServiceCacheRegistrationAB, anchorType.ToString(), registrationName));
            if (foundRegistration is ServiceCacheForeignRegistration)
                throw new InvalidOperationException(Local.InvalidDataSource);
            return registrationDispatcher.Get<T>(cache, foundRegistration, tag, values);
        }

        /// <summary>
        /// Sends the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <param name="messages">The messages.</param>
        public static void Send(this IServiceCache service, Type anchorType, string registrationName, object[] messages) { Send(service, anchorType, registrationName, null, messages); }
        /// <summary>
        /// Sends the specified cache.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="messages">The messages.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static void Send(this IServiceCache service, Type anchorType, string registrationName, object tag, object[] messages)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (anchorType == null)
                throw new ArgumentNullException("anchorType");
            if (string.IsNullOrEmpty(registrationName))
                throw new ArgumentNullException("registrationName");
            if (messages == null)
                throw new ArgumentNullException("messages");
            var registrationDispatcher = GetRegistrationDispatcher(service);
            // fetch registration
            var recurses = 0;
            IServiceCacheRegistration foundRegistration;
            if (!ServiceCacheRegistrar.TryGetValue(anchorType, registrationName, ref recurses, out foundRegistration))
                throw new InvalidOperationException(string.Format(Local.UndefinedServiceCacheRegistrationAB, anchorType.ToString(), registrationName));
            if (foundRegistration is ServiceCacheForeignRegistration)
                throw new InvalidOperationException(Local.InvalidDataSource);
            registrationDispatcher.Send(service, foundRegistration, tag, messages);
        }

        /// <summary>
        /// Sends the specified service.
        /// </summary>
        /// <typeparam name="TAnchor">The type of the anchor.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="messages">The messages.</param>
        public static void SendAll<TAnchor>(this IServiceCache service, object[] messages) { SendAll(service, typeof(TAnchor), null, messages); }
        /// <summary>
        /// Sends the specified cache.
        /// </summary>
        /// <typeparam name="TAnchor">The type of the anchor.</typeparam>
        /// <param name="service">The cache.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="messages">The messages.</param>
        public static void SendAll<TAnchor>(this IServiceCache service, object tag, object[] messages) { SendAll(service, typeof(TAnchor), tag, messages); }
        /// <summary>
        /// Sends the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="messages">The messages.</param>
        public static void SendAll(this IServiceCache service, Type anchorType, object[] messages) { SendAll(service, anchorType, null, messages); }
        /// <summary>
        /// Sends the specified cache.
        /// </summary>
        /// <param name="service">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="messages">The messages.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static void SendAll(this IServiceCache service, Type anchorType, object tag, object[] messages)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (anchorType == null)
                throw new ArgumentNullException("anchorType");
            if (messages == null)
                throw new ArgumentNullException("messages");
            var registrationDispatcher = GetRegistrationDispatcher(service);
            // fetch all registrations
            ServiceCacheRegistrar registrar;
            ServiceCacheRegistrar.TryGet(anchorType, out registrar, false);
            if (registrar != null)
                foreach (var registration in registrar.All)
                {
                    // fetch registration
                    var recurses = 0;
                    IServiceCacheRegistration foundRegistration;
                    if (!ServiceCacheRegistrar.TryGetValue(registration, ref recurses, out foundRegistration))
                        throw new InvalidOperationException(string.Format(Local.UndefinedServiceCacheRegistrationAB, (registration.Registrar != null ? registration.Registrar.AnchorType.ToString() : "{unregistered}"), registration.Name));
                    if (foundRegistration is ServiceCacheForeignRegistration)
                        throw new InvalidOperationException(Local.InvalidDataSource);
                    registrationDispatcher.Send(service, foundRegistration, tag, messages);
                }
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        public static void RemoveAll(this IServiceCache cache, Type anchorType)
        {
            if (anchorType == null)
                throw new ArgumentNullException("anchorType");
            var registrationDispatcher = GetRegistrationDispatcher(cache);
            // fetch registrar
            ServiceCacheRegistrar registrar;
            if (!ServiceCacheRegistrar.TryGet(anchorType, out registrar, false))
                throw new InvalidOperationException(string.Format(Local.UndefinedServiceCacheRegistrationA, anchorType.ToString()));
            foreach (var registration in registrar.All)
                registrationDispatcher.Remove(cache, registration);
        }

        /// <summary>
        /// Removes the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        public static void Remove(this IServiceCache cache, ServiceCacheRegistration registration)
        {
            if (registration == null)
                throw new ArgumentNullException("registration");
            var registrationDispatcher = GetRegistrationDispatcher(cache);
            // fetch registration
            var recurses = 0;
            IServiceCacheRegistration foundRegistration;
            if (!ServiceCacheRegistrar.TryGetValue(registration, ref recurses, out foundRegistration))
                throw new InvalidOperationException(string.Format(Local.UndefinedServiceCacheRegistrationA, registration.ToString()));
            if (foundRegistration is ServiceCacheForeignRegistration)
                throw new InvalidOperationException(Local.InvalidDataSource);
            registrationDispatcher.Remove(cache, foundRegistration);
        }

        /// <summary>
        /// Removes the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        public static void Remove(this IServiceCache cache, Type anchorType, string registrationName)
        {
            if (anchorType == null)
                throw new ArgumentNullException("anchorType");
            if (string.IsNullOrEmpty(registrationName))
                throw new ArgumentNullException("registrationName");
            var registrationDispatcher = GetRegistrationDispatcher(cache);
            // fetch registration
            var recurses = 0;
            IServiceCacheRegistration foundRegistration;
            if (!ServiceCacheRegistrar.TryGetValue(anchorType, registrationName, ref recurses, out foundRegistration))
                throw new InvalidOperationException(string.Format(Local.UndefinedServiceCacheRegistrationAB, anchorType.ToString(), registrationName));
            if (foundRegistration is ServiceCacheForeignRegistration)
                throw new InvalidOperationException(Local.InvalidDataSource);
            registrationDispatcher.Remove(cache, foundRegistration);
        }

        private static ServiceCacheRegistration.IDispatcher GetRegistrationDispatcher(IServiceCache cache)
        {
            var settings = cache.Settings;
            if (settings == null)
                throw new NullReferenceException("settings");
            var registrationDispatcher = settings.RegistrationDispatcher;
            if (registrationDispatcher == null)
                throw new NullReferenceException("cache.Settings.RegistrationDispatch");
            return registrationDispatcher;
        }

        #endregion

        #region Lazy Setup

        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static Lazy<IServiceCache> RegisterWithServiceLocator<T>(this Lazy<IServiceCache> service)
            where T : class, IServiceCache { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, ServiceLocatorManager.Lazy, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceCache> RegisterWithServiceLocator<T>(this Lazy<IServiceCache> service, string name)
            where T : class, IServiceCache { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, ServiceLocatorManager.Lazy, name); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Lazy<IServiceCache> RegisterWithServiceLocator<T>(this Lazy<IServiceCache> service, Lazy<IServiceLocator> locator)
            where T : class, IServiceCache { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceCache> RegisterWithServiceLocator<T>(this Lazy<IServiceCache> service, Lazy<IServiceLocator> locator, string name)
            where T : class, IServiceCache { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, name); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static Lazy<IServiceCache> RegisterWithServiceLocator(this Lazy<IServiceCache> service) { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, ServiceLocatorManager.Lazy, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceCache> RegisterWithServiceLocator(this Lazy<IServiceCache> service, string name) { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, ServiceLocatorManager.Lazy, name); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Lazy<IServiceCache> RegisterWithServiceLocator(this Lazy<IServiceCache> service, Lazy<IServiceLocator> locator) { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceCache> RegisterWithServiceLocator(this Lazy<IServiceCache> service, Lazy<IServiceLocator> locator, string name) { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, name); return service; }

        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Lazy<IServiceCache> RegisterWithServiceLocator<T>(this Lazy<IServiceCache> service, IServiceLocator locator)
            where T : class, IServiceCache { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceCache> RegisterWithServiceLocator<T>(this Lazy<IServiceCache> service, IServiceLocator locator, string name)
            where T : class, IServiceCache { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, name); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Lazy<IServiceCache> RegisterWithServiceLocator(this Lazy<IServiceCache> service, IServiceLocator locator) { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceCache> RegisterWithServiceLocator(this Lazy<IServiceCache> service, IServiceLocator locator, string name) { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator(service, locator, name); return service; }

        #endregion
    }
}
