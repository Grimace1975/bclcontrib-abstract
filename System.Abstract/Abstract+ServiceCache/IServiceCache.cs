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
    /// IServiceCache
    /// </summary>
    public interface IServiceCache
    {
        object this[string name] { get; set; }

        object Add(object tag, string name, ServiceCacheDependency dependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback, object value);

        /// <summary>
        /// Gets the item from cache associated with the key provided.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The cached item.
        /// </returns>
        object Get(object tag, string name);

        /// <summary>
        /// Removes from cache the item associated with the key provided.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The item removed from the Cache. If the value in the key parameter is not found, returns null.</returns>
        object Remove(object tag, string name);

        /// <summary>
        /// Adds an object into cache based on the parameters provided.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="name">The name.</param>
        /// <param name="dependency">The dependency object defining caching validity dependencies.</param>
        /// <param name="absoluteExpiration">The absolute expiration value used to determine when a cache item must be considerd invalid.</param>
        /// <param name="slidingExpiration">The sliding expiration value used to determine when a cache item is considered invalid due to lack of use.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="onRemoveCallback">The delegate to invoke when the item is removed from cache.</param>
        /// <param name="value">The value to store in cache.</param>
        /// <returns></returns>
        object Insert(object tag, string name, ServiceCacheDependency dependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback, object value);

        /// <summary>
        /// Touches the specified names.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="names">The names.</param>
        void Touch(object tag, params string[] names);

        /// <summary>
        /// Gets the registration dispatch.
        /// </summary>
        ServiceCacheRegistration.IDispatch RegistrationDispatch { get; }
    }

    /// <summary>
    /// IServiceCacheExtensions
    /// </summary>
    public static class IServiceCacheExtensions
    {
        public static object Add(this IServiceCache cache, string name, object value) { return Add(cache, null, name, value); }
        public static object Add(this IServiceCache cache, ServiceCacheCommand cacheCommand, object value) { return Add(cache, null, cacheCommand, cacheCommand.Name, value); }
        public static object Add(this IServiceCache cache, ServiceCacheCommand cacheCommand, string name, object value) { return Add(cache, null, cacheCommand, name, value); }
        public static object Add(this IServiceCache cache, object tag, string name, object value)
        {
            return cache.Add(tag, name, null, DateTime.Now.AddMinutes(60), ServiceCache.NoSlidingExpiration, CacheItemPriority.Normal, null, value);
        }
        public static object Add(this IServiceCache cache, object tag, ServiceCacheCommand cacheCommand, object value) { return Add(cache, tag, cacheCommand, cacheCommand.Name, value); }
        public static object Add(this IServiceCache cache, object tag, ServiceCacheCommand cacheCommand, string name, object value)
        {
            if (cacheCommand == null)
                throw new ArgumentNullException("cacheCommand");
            var itemAddedCallback = cacheCommand.ItemAddedCallback;
            if (itemAddedCallback != null)
                itemAddedCallback(name, value);
            return cache.Add(tag, name, cacheCommand.Dependency, cacheCommand.AbsoluteExpiration, cacheCommand.SlidingExpiration, cacheCommand.Priority, cacheCommand.ItemRemovedCallback, value);
        }

        public static object Get(this IServiceCache cache, string name) { return cache.Get(null, name); }
        public static object Get(this IServiceCache cache, ServiceCacheCommand cacheCommand) { return Get(cache, (object)null, cacheCommand); }
        public static object Get(this IServiceCache cache, object tag, ServiceCacheCommand cacheCommand)
        {
            if (cacheCommand == null)
                throw new ArgumentNullException("cacheCommand");
            return cache.Get(tag, cacheCommand.Name);
        }

        public static object Remove(this IServiceCache cache, string name) { return cache.Remove(null, name); }
        public static object Remove(this IServiceCache cache, ServiceCacheCommand cacheCommand) { return Remove(cache, null, cacheCommand); }
        public static object Remove(this IServiceCache cache, object tag, ServiceCacheCommand cacheCommand)
        {
            if (cacheCommand == null)
                throw new ArgumentNullException("cacheCommand");
            return cache.Remove(tag, cacheCommand.Name);
        }

        public static object Insert(this IServiceCache cache, string name, object value) { return Insert(cache, null, name, value); }
        public static object Insert(this IServiceCache cache, ServiceCacheCommand cacheCommand, object value) { return Insert(cache, null, cacheCommand, cacheCommand.Name, value); }
        public static object Insert(this IServiceCache cache, ServiceCacheCommand cacheCommand, string name, object value) { return Insert(cache, null, cacheCommand, cacheCommand.Name, value); }
        public static object Insert(this IServiceCache cache, object tag, string name, object value)
        {
            return cache.Insert(tag, name, null, DateTime.Now.AddMinutes(60), ServiceCache.NoSlidingExpiration, CacheItemPriority.Normal, null, value);
        }
        public static object Insert(this IServiceCache cache, object tag, ServiceCacheCommand cacheCommand, object value) { return Insert(cache, tag, cacheCommand, cacheCommand.Name, value); }
        public static object Insert(this IServiceCache cache, object tag, ServiceCacheCommand cacheCommand, string name, object value)
        {
            if (cacheCommand == null)
                throw new ArgumentNullException("cacheCommand");
            var itemAddedCallback = cacheCommand.ItemAddedCallback;
            if (itemAddedCallback != null)
                itemAddedCallback(name, value);
            return cache.Insert(tag, name, cacheCommand.Dependency, cacheCommand.AbsoluteExpiration, cacheCommand.SlidingExpiration, cacheCommand.Priority, cacheCommand.ItemRemovedCallback, value);
        }

        public static void EnsureCacheDependency(IServiceCache cache, ServiceCacheDependency dependency)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");
            if (dependency != null)
                return;
            var names = dependency.CacheTags;
            if (names != null)
                foreach (string name in names)
                    cache.Add(null, name, null, ServiceCache.NoAbsoluteExpiration, ServiceCache.NoSlidingExpiration, CacheItemPriority.Normal, null, string.Empty);
        }

        public static void Touch(this IServiceCache cache, params string[] names) { cache.Touch(null, names); }

        public static IServiceCache Wrap(this IServiceCache cache, IEnumerable<object> values, out string @namespace) { @namespace = ServiceCache.GetNamespace(values); return new ServiceCacheNamespaceWrapper(cache, @namespace); }
        public static IServiceCache Wrap(this IServiceCache cache, string @namespace)
        {
            if (@namespace == null)
                throw new ArgumentNullException("@namespace");
            return new ServiceCacheNamespaceWrapper(cache, @namespace);
        }

        #region Registrations

        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, ServiceCacheRegistration registration) { return Get<object>(cache, registration, null, null); }
        public static T Get<T>(this IServiceCache cache, ServiceCacheRegistration registration) { return Get<T>(cache, registration, null, null); }
        public static IEnumerable<T> GetMany<T>(this IServiceCache cache, ServiceCacheRegistration registration) { return Get<IEnumerable<T>>(cache, registration, null, null); }
        public static IQueryable<T> GetQuery<T>(this IServiceCache cache, ServiceCacheRegistration registration) { return Get<IQueryable<T>>(cache, registration, null, null); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, ServiceCacheRegistration registration, object[] values) { return Get<object>(cache, registration, null, values); }
        public static T Get<T>(this IServiceCache cache, ServiceCacheRegistration registration, object[] values) { return Get<T>(cache, registration, null, values); }
        public static IEnumerable<T> GetMany<T>(this IServiceCache cache, ServiceCacheRegistration registration, object[] values) { return Get<IEnumerable<T>>(cache, registration, null, values); }
        public static IQueryable<T> GetQuery<T>(this IServiceCache cache, ServiceCacheRegistration registration, object[] values) { return Get<IQueryable<T>>(cache, registration, null, values); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, ServiceCacheRegistration registration, object tag) { return Get<object>(cache, registration, tag, null); }
        public static T Get<T>(this IServiceCache cache, ServiceCacheRegistration registration, object tag) { return Get<T>(cache, registration, tag, null); }
        public static IEnumerable<T> GetMany<T>(this IServiceCache cache, ServiceCacheRegistration registration, object tag) { return Get<IEnumerable<T>>(cache, registration, tag, null); }
        public static IQueryable<T> GetQuery<T>(this IServiceCache cache, ServiceCacheRegistration registration, object tag) { return Get<IQueryable<T>>(cache, registration, tag, null); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values) { return Get<object>(cache, registration, tag, values); }
        public static IEnumerable<T> GetMany<T>(this IServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values) { return Get<IEnumerable<T>>(cache, registration, tag, values); }
        public static IQueryable<T> GetQuery<T>(this IServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values) { return Get<IQueryable<T>>(cache, registration, tag, values); }
        public static T Get<T>(this IServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values)
        {
            if (cache.RegistrationDispatch == null)
                throw new NullReferenceException("cache.RegistrationDispatch");
            if (registration == null)
                throw new ArgumentNullException("registration");
            // fetch registration
            int recurses = 0;
            ServiceCacheRegistration foundRegistration;
            if (!ServiceCacheRegistrar.TryGetValue(registration, ref recurses, out foundRegistration))
                throw new InvalidOperationException(string.Format(Local.UndefinedServiceCacheRegistrationAB, (registration.Registrar != null ? registration.Registrar.AnchorType.ToString() : "{unregistered}"), registration.Name));
            if (foundRegistration is ServiceCacheForeignRegistration)
                throw new InvalidOperationException(Local.InvalidDataSource);
            return cache.RegistrationDispatch.Get<T>(cache, foundRegistration, tag, values);
        }

        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="anchorType">The type.</param>
        /// <param name="registrationName">The registration id.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, Type anchorType, string registrationName) { return Get<object>(cache, anchorType, registrationName, null, null); }
        public static T Get<T>(this IServiceCache cache, Type anchorType, string registrationName) { return Get<T>(cache, anchorType, registrationName, null, null); }
        public static IEnumerable<T> GetMany<T>(this IServiceCache cache, Type anchorType, string registrationName) { return Get<IEnumerable<T>>(cache, anchorType, registrationName, null, null); }
        public static IQueryable<T> GetQuery<T>(this IServiceCache cache, Type anchorType, string registrationName) { return Get<IQueryable<T>>(cache, anchorType, registrationName, null, null); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="anchorType">The type.</param>
        /// <param name="registrationName">The registration id.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, Type anchorType, string registrationName, object[] values) { return Get<object>(cache, anchorType, registrationName, null, values); }
        public static T Get<T>(this IServiceCache cache, Type anchorType, string registrationName, object[] values) { return Get<T>(cache, anchorType, registrationName, null, values); }
        public static IEnumerable<T> GetMany<T>(this IServiceCache cache, Type anchorType, string registrationName, object[] values) { return Get<IEnumerable<T>>(cache, anchorType, registrationName, string.Empty, values); }
        public static IQueryable<T> GetQuery<T>(this IServiceCache cache, Type anchorType, string registrationName, object[] values) { return Get<IQueryable<T>>(cache, anchorType, registrationName, string.Empty, values); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="anchorType">The type.</param>
        /// <param name="registrationName">The registration id.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, Type anchorType, string registrationName, object tag) { return Get<object>(cache, anchorType, registrationName, tag, null); }
        public static T Get<T>(this IServiceCache cache, Type anchorType, string registrationName, object tag) { return Get<T>(cache, anchorType, registrationName, tag, null); }
        public static IEnumerable<T> GetMany<T>(this IServiceCache cache, Type anchorType, string registrationName, object tag) { return Get<IEnumerable<T>>(cache, anchorType, registrationName, tag, null); }
        public static IQueryable<T> GetQuery<T>(this IServiceCache cache, Type anchorType, string registrationName, object tag) { return Get<IQueryable<T>>(cache, anchorType, registrationName, tag, null); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="anchorType">The type.</param>
        /// <param name="registrationName">The registration id.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static object Get(this IServiceCache cache, Type anchorType, string registrationName, object tag, object[] values) { return Get<object>(cache, anchorType, registrationName, tag, values); }
        public static IEnumerable<T> GetMany<T>(this IServiceCache cache, Type anchorType, string registrationName, object tag, object[] values) { return Get<IEnumerable<T>>(cache, anchorType, registrationName, tag, values); }
        public static IQueryable<T> GetQuery<T>(this IServiceCache cache, Type anchorType, string registrationName, object tag, object[] values) { return Get<IQueryable<T>>(cache, anchorType, registrationName, tag, values); }
        public static T Get<T>(this IServiceCache cache, Type anchorType, string registrationName, object tag, object[] values)
        {
            if (cache.RegistrationDispatch == null)
                throw new NullReferenceException("cache.RegistrationDispatch");
            if (anchorType == null)
                throw new ArgumentNullException("anchorType");
            if (string.IsNullOrEmpty(registrationName))
                throw new ArgumentNullException("registrationName");
            // fetch registration
            int recurses = 0;
            ServiceCacheRegistration foundRegistration;
            if (!ServiceCacheRegistrar.TryGetValue(anchorType, registrationName, ref recurses, out foundRegistration))
                throw new InvalidOperationException(string.Format(Local.UndefinedServiceCacheRegistrationAB, anchorType.ToString(), registrationName));
            if (foundRegistration is ServiceCacheForeignRegistration)
                throw new InvalidOperationException(Local.InvalidDataSource);
            return cache.RegistrationDispatch.Get<T>(cache, foundRegistration, tag, values);
        }

        public static void RemoveAll(this IServiceCache cache, Type anchorType)
        {
            if (cache.RegistrationDispatch == null)
                throw new NullReferenceException("cache.RegistrationDispatch");
            if (anchorType == null)
                throw new ArgumentNullException("anchorType");
            // fetch registrar
            ServiceCacheRegistrar registrar;
            if (!ServiceCacheRegistrar.TryGet(anchorType, out registrar, false))
                throw new InvalidOperationException(string.Format(Local.UndefinedServiceCacheRegistrationA, anchorType.ToString()));
            foreach (var registration in registrar.GetAll())
                cache.RegistrationDispatch.Remove(cache, registration);
        }

        public static void Remove(this IServiceCache cache, ServiceCacheRegistration registration)
        {
            if (cache.RegistrationDispatch == null)
                throw new NullReferenceException("cache.RegistrationDispatch");
            if (registration == null)
                throw new ArgumentNullException("registration");
            // fetch registration
            int recurses = 0;
            ServiceCacheRegistration foundRegistration;
            if (!ServiceCacheRegistrar.TryGetValue(registration, ref recurses, out foundRegistration))
                throw new InvalidOperationException(string.Format(Local.UndefinedServiceCacheRegistrationA, registration.ToString()));
            if (foundRegistration is ServiceCacheForeignRegistration)
                throw new InvalidOperationException(Local.InvalidDataSource);
            cache.RegistrationDispatch.Remove(cache, foundRegistration);
        }

        public static void Remove(this IServiceCache cache, Type anchorType, string registrationName)
        {
            if (cache.RegistrationDispatch == null)
                throw new NullReferenceException("cache.RegistrationDispatch");
            if (anchorType == null)
                throw new ArgumentNullException("anchorType");
            if (string.IsNullOrEmpty(registrationName))
                throw new ArgumentNullException("registrationName");
            // fetch registration
            int recurses = 0;
            ServiceCacheRegistration foundRegistration;
            if (!ServiceCacheRegistrar.TryGetValue(anchorType, registrationName, ref recurses, out foundRegistration))
                throw new InvalidOperationException(string.Format(Local.UndefinedServiceCacheRegistrationAB, anchorType.ToString(), registrationName));
            if (foundRegistration is ServiceCacheForeignRegistration)
                throw new InvalidOperationException(Local.InvalidDataSource);
            cache.RegistrationDispatch.Remove(cache, foundRegistration);
        }

        #endregion
    }
}
