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
	public interface IServiceCache : IServiceProvider
	{
		object this[string name] { get; set; }

		object Add(object tag, string name, CacheItemPolicy itemPolicy, object value);

		/// <summary>
		/// Gets the item from cache associated with the key provided.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <param name="name">The name.</param>
		/// <returns>
		/// The cached item.
		/// </returns>
		object Get(object tag, string name);
		object Get(object tag, IEnumerable<string> names);
		bool TryGet(object tag, string name, out object value);

		/// <summary>
		/// Removes from cache the item associated with the key provided.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <param name="name">The name.</param>
		/// <returns>The item removed from the Cache. If the value in the key parameter is not found, returns null.</returns>
		object Remove(object tag, string name);

		/// <summary>
		/// Adds an object into cache based on the parameters provided.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <param name="name">The name.</param>
		/// <param name="itemPolicy">The itemPolicy object.</param>
		/// <param name="value">The value to store in cache.</param>
		/// <returns></returns>
		object Set(object tag, string name, CacheItemPolicy itemPolicy, object value);

		/// <summary>
		/// Touches the specified names.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <param name="names">The names.</param>
		void Touch(object tag, params string[] names);

		ServiceCacheSettings Settings { get; }
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
		public static object Add(this IServiceCache cache, string name, object value) { return cache.Add(null, name, CacheItemPolicy.Default, value); }
		public static object Add(this IServiceCache cache, string name, CacheItemPolicy itemPolicy, object value) { return cache.Add(null, name, itemPolicy, value); }
		public static object Add(this IServiceCache cache, object tag, string name, object value) { return cache.Add(tag, name, CacheItemPolicy.Default, value); }

		public static object Get(this IServiceCache cache, string name) { return cache.Get(null, name); }
		public static object Get(this IServiceCache cache, IEnumerable<string> names) { return cache.Get(null, names); }
		public static bool TryGet(this IServiceCache cache, string name, out object value) { return cache.TryGet(null, name, out value); }

		public static object Remove(this IServiceCache cache, string name) { return cache.Remove(null, name); }

		public static object Set(this IServiceCache cache, string name, object value) { return cache.Set(null, name, CacheItemPolicy.Default, value); }
		public static object Set(this IServiceCache cache, string name, CacheItemPolicy itemPolicy, object value) { return cache.Set(null, name, itemPolicy, value); }
		public static object Set(this IServiceCache cache, object tag, string name, object value) { return cache.Set(tag, name, CacheItemPolicy.Default, value); }

		public static void EnsureCacheDependency(IServiceCache cache, object tag, CacheItemDependency dependency)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");
			string[] cacheTags;
			if ((dependency == null) || ((cacheTags = (dependency(cache, tag) as string[])) == null))
				return;
			EnsureCacheDependency(cache, cacheTags);
		}
		public static void EnsureCacheDependency(IServiceCache cache, IEnumerable<string> cacheTags)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");
			if (cacheTags != null)
				foreach (string cacheTag in cacheTags)
					cache.Add(null, cacheTag, new CacheItemPolicy { AbsoluteExpiration = ServiceCache.InfiniteAbsoluteExpiration }, string.Empty);
		}

		public static void Touch(this IServiceCache cache, params string[] names) { cache.Touch(null, names); }

		public static IServiceCache Wrap(this IServiceCache cache, IEnumerable<object> values, out string @namespace)
		{
			@namespace = ServiceCache.GetNamespace(values);
			if (@namespace == null)
				throw new ArgumentNullException("@values");
			return new ServiceCacheNamespaceWrapper(cache, @namespace);
		}
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

		#region Lazy Setup

		public static LazyEx<IServiceCache> RegisterWithServiceLocator(this LazyEx<IServiceCache> lazy) { ServiceCacheManager.GetSetupDescriptor(lazy).RegisterWithServiceLocator(null); return lazy; }
		public static LazyEx<IServiceCache> RegisterWithServiceLocator(this LazyEx<IServiceCache> lazy, string name) { ServiceCacheManager.GetSetupDescriptor(lazy).RegisterWithServiceLocator(name); return lazy; }
		public static LazyEx<IServiceCache> RegisterWithServiceLocator(this LazyEx<IServiceCache> lazy, Func<IServiceLocator> locator) { ServiceCacheManager.GetSetupDescriptor(lazy).RegisterWithServiceLocator(locator, null); return lazy; }
		public static LazyEx<IServiceCache> RegisterWithServiceLocator(this LazyEx<IServiceCache> lazy, Func<IServiceLocator> locator, string name) { ServiceCacheManager.GetSetupDescriptor(lazy).RegisterWithServiceLocator(locator, name); return lazy; }

		#endregion
	}
}
