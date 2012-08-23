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
        /// <param name="cache">The cache.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object Add(this IServiceCache cache, string name, object value) { return cache.Add(null, name, CacheItemPolicy.Default, value, ServiceCacheByDispatcher.Empty); }
        /// <summary>
        /// Adds the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="name">The name.</param>
        /// <param name="itemPolicy">The item policy.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object Add(this IServiceCache cache, string name, CacheItemPolicy itemPolicy, object value) { return cache.Add(null, name, itemPolicy, value, ServiceCacheByDispatcher.Empty); }
        /// <summary>
        /// Adds the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object Add(this IServiceCache cache, object tag, string name, object value) { return cache.Add(tag, name, CacheItemPolicy.Default, value, ServiceCacheByDispatcher.Empty); }
        /// <summary>
        /// Adds the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <param name="itemPolicy">The item policy.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object Add(this IServiceCache cache, object tag, string name, CacheItemPolicy itemPolicy, object value) { return cache.Add(tag, name, itemPolicy, value, ServiceCacheByDispatcher.Empty); }

        /// <summary>
        /// Gets the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, string name) { return cache.Get(null, name); }
        /// <summary>
        /// Gets the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="names">The names.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, IEnumerable<string> names) { return cache.Get(null, names); }
        /// <summary>
        /// Tries the get.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool TryGet(this IServiceCache cache, string name, out object value) { return cache.TryGet(null, name, out value); }

        /// <summary>
        /// Removes the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static object Remove(this IServiceCache cache, string name) { return cache.Remove(null, name); }

        /// <summary>
        /// Sets the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object Set(this IServiceCache cache, string name, object value) { return cache.Set(null, name, CacheItemPolicy.Default, value, ServiceCacheByDispatcher.Empty); }
        /// <summary>
        /// Sets the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="name">The name.</param>
        /// <param name="itemPolicy">The item policy.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object Set(this IServiceCache cache, string name, CacheItemPolicy itemPolicy, object value) { return cache.Set(null, name, itemPolicy, value, ServiceCacheByDispatcher.Empty); }
        /// <summary>
        /// Sets the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object Set(this IServiceCache cache, object tag, string name, object value) { return cache.Set(tag, name, CacheItemPolicy.Default, value, ServiceCacheByDispatcher.Empty); }
        /// <summary>
        /// Sets the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <param name="itemPolicy">The item policy.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object Set(this IServiceCache cache, object tag, string name, CacheItemPolicy itemPolicy, object value) { return cache.Set(tag, name, itemPolicy, value, ServiceCacheByDispatcher.Empty); }

        /// <summary>
        /// Ensures the cache dependency.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="dependency">The dependency.</param>
        public static void EnsureCacheDependency(IServiceCache cache, object tag, CacheItemDependency dependency)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");
            string[] names;
            if (dependency == null || (names = (dependency(cache, null, tag, null) as string[])) == null)
                return;
            EnsureCacheDependency(cache, names);
        }
        /// <summary>
        /// Ensures the cache dependency.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="names">The names.</param>
        public static void EnsureCacheDependency(IServiceCache cache, IEnumerable<string> names)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");
            if (names != null)
                foreach (var name in names)
                    cache.Add(null, name, new CacheItemPolicy { AbsoluteExpiration = ServiceCache.InfiniteAbsoluteExpiration }, string.Empty);
        }

        /// <summary>
        /// Touches the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="names">The names.</param>
        public static void Touch(this IServiceCache cache, params string[] names) { Touch(cache, null, names); }
        /// <summary>
        /// Touches the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="names">The names.</param>
        public static void Touch(this IServiceCache cache, object tag, params string[] names)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");
            var touchable = cache.Settings.Touchable;
            if (touchable == null)
                throw new NotSupportedException("Touchables are not supported");
            touchable.Touch(tag, names);
        }

        /// <summary>
        /// Makes the dependency.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="names">The names.</param>
        /// <returns></returns>
        public static CacheItemDependency MakeDependency(this IServiceCache cache, params string[] names)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");
            var touchable = cache.Settings.Touchable;
            if (touchable == null)
                throw new NotSupportedException("Touchables are not supported");
            return (c, r, tag, values) => touchable.MakeDependency(tag, names);
        }

        #region BehaveAs

        /// <summary>
        /// Behaves as.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="namespace">The @namespace.</param>
        /// <returns></returns>
        public static IServiceCache BehaveAs(this IServiceCache cache, string @namespace)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");
            if (@namespace == null)
                throw new ArgumentNullException("@namespace");
            return new ServiceCacheNamespaceBehaviorWrapper(cache, @namespace);
        }
        /// <summary>
        /// Behaves as.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="values">The values.</param>
        /// <param name="namespace">The @namespace.</param>
        /// <returns></returns>
        public static IServiceCache BehaveAs(this IServiceCache cache, IEnumerable<object> values, out string @namespace)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");
            @namespace = ServiceCache.GetNamespace(values);
            if (@namespace == null)
                throw new ArgumentNullException("@values");
            return new ServiceCacheNamespaceBehaviorWrapper(cache, @namespace);
        }

        #endregion

        #region Registrations

        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, ServiceCacheRegistration registration) { return Get<object>(cache, registration, null, null); }
        /// <summary>
        /// Gets the specified cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        public static T Get<T>(this IServiceCache cache, ServiceCacheRegistration registration) { return Get<T>(cache, registration, null, null); }
        /// <summary>
        /// Gets the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetMany<T>(this IServiceCache cache, ServiceCacheRegistration registration) { return Get<IEnumerable<T>>(cache, registration, null, null); }
        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        public static IQueryable<T> GetQuery<T>(this IServiceCache cache, ServiceCacheRegistration registration) { return Get<IQueryable<T>>(cache, registration, null, null); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, ServiceCacheRegistration registration, object[] values) { return Get<object>(cache, registration, null, values); }
        /// <summary>
        /// Gets the specified cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static T Get<T>(this IServiceCache cache, ServiceCacheRegistration registration, object[] values) { return Get<T>(cache, registration, null, values); }
        /// <summary>
        /// Gets the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetMany<T>(this IServiceCache cache, ServiceCacheRegistration registration, object[] values) { return Get<IEnumerable<T>>(cache, registration, null, values); }
        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IQueryable<T> GetQuery<T>(this IServiceCache cache, ServiceCacheRegistration registration, object[] values) { return Get<IQueryable<T>>(cache, registration, null, values); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, ServiceCacheRegistration registration, object tag) { return Get<object>(cache, registration, tag, null); }
        /// <summary>
        /// Gets the specified cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static T Get<T>(this IServiceCache cache, ServiceCacheRegistration registration, object tag) { return Get<T>(cache, registration, tag, null); }
        /// <summary>
        /// Gets the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetMany<T>(this IServiceCache cache, ServiceCacheRegistration registration, object tag) { return Get<IEnumerable<T>>(cache, registration, tag, null); }
        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static IQueryable<T> GetQuery<T>(this IServiceCache cache, ServiceCacheRegistration registration, object tag) { return Get<IQueryable<T>>(cache, registration, tag, null); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values) { return Get<object>(cache, registration, tag, values); }
        /// <summary>
        /// Gets the many.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetMany<T>(this IServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values) { return Get<IEnumerable<T>>(cache, registration, tag, values); }
        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IQueryable<T> GetQuery<T>(this IServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values) { return Get<IQueryable<T>>(cache, registration, tag, values); }
        /// <summary>
        /// Gets the specified cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static T Get<T>(this IServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values)
        {
            if (registration == null)
                throw new ArgumentNullException("registration");
            var registrationDispatcher = GetRegistrationDispatcher(cache);
            // fetch registration
            var recurses = 0;
            ServiceCacheRegistration foundRegistration;
            if (!ServiceCacheRegistrar.TryGetValue(registration, ref recurses, out foundRegistration))
                throw new InvalidOperationException(string.Format(Local.UndefinedServiceCacheRegistrationAB, (registration.Registrar != null ? registration.Registrar.AnchorType.ToString() : "{unregistered}"), registration.Name));
            if (foundRegistration is ServiceCacheForeignRegistration)
                throw new InvalidOperationException(Local.InvalidDataSource);
            return registrationDispatcher.Get<T>(cache, foundRegistration, tag, values);
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
            ServiceCacheRegistration foundRegistration;
            if (!ServiceCacheRegistrar.TryGetValue(anchorType, registrationName, ref recurses, out foundRegistration))
                throw new InvalidOperationException(string.Format(Local.UndefinedServiceCacheRegistrationAB, anchorType.ToString(), registrationName));
            if (foundRegistration is ServiceCacheForeignRegistration)
                throw new InvalidOperationException(Local.InvalidDataSource);
            return registrationDispatcher.Get<T>(cache, foundRegistration, tag, values);
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
            foreach (var registration in registrar.GetAll())
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
            ServiceCacheRegistration foundRegistration;
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
            ServiceCacheRegistration foundRegistration;
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
        public static Lazy<IServiceCache> RegisterWithServiceLocator<T>(this Lazy<IServiceCache> service) { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, ServiceLocatorManager.Lazy, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceCache> RegisterWithServiceLocator<T>(this Lazy<IServiceCache> service, string name) { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, ServiceLocatorManager.Lazy, name); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        public static Lazy<IServiceCache> RegisterWithServiceLocator<T>(this Lazy<IServiceCache> service, Lazy<IServiceLocator> locator) { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceCache> RegisterWithServiceLocator<T>(this Lazy<IServiceCache> service, Lazy<IServiceLocator> locator, string name) { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, name); return service; }
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
        public static Lazy<IServiceCache> RegisterWithServiceLocator<T>(this Lazy<IServiceCache> service, IServiceLocator locator) { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, null); return service; }
        /// <summary>
        /// Registers the with service locator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <param name="locator">The locator.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Lazy<IServiceCache> RegisterWithServiceLocator<T>(this Lazy<IServiceCache> service, IServiceLocator locator, string name) { ServiceCacheManager.GetSetupDescriptor(service).RegisterWithServiceLocator<T>(service, locator, name); return service; }
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
