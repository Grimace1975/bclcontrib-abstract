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
using System.Linq;
using System.Collections.Generic;
using System.Text;
namespace System.Abstract
{
    /// <summary>
    /// ServiceCache
    /// </summary>
    public static partial class ServiceCache
    {
        /// <summary>
        /// Provides <see cref="System.DateTime"/> instance to be used when no absolute expiration value to be set.
        /// </summary>
        public static readonly DateTime InfiniteAbsoluteExpiration = DateTime.MaxValue;
        /// <summary>
        /// Provides <see cref="System.TimeSpan"/> instance to be used when no sliding expiration value to be set.
        /// </summary>
        public static readonly TimeSpan NoSlidingExpiration = TimeSpan.Zero;

        static ServiceCache()
        {
            var registrar = ServiceCacheRegistrar.Get(typeof(ServiceCache));
            registrar.Register(Primitives.YesNo);
            registrar.Register(Primitives.Gender);
            registrar.Register(Primitives.Integer);
        }

        internal static string GetNamespace(IEnumerable<object> values)
        {
            if (values == null || !values.Any())
                return null;
            var b = new StringBuilder();
            foreach (var v in values)
            {
                if (v != null)
                    b.Append(v.ToString());
                b.Append("\\");
            }
            return b.ToString();
        }

        #region Registrations

        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        public static object Get(ServiceCacheRegistration registration) { return ServiceCacheManager.Current.Get<object>(registration, null, null); }
        public static T Get<T>(ServiceCacheRegistration registration) { return ServiceCacheManager.Current.Get<T>(registration, null, null); }
        public static IEnumerable<T> GetMany<T>(ServiceCacheRegistration registration) { return ServiceCacheManager.Current.Get<IEnumerable<T>>(registration, null, null); }
        public static IQueryable<T> GetQuery<T>(ServiceCacheRegistration registration) { return ServiceCacheManager.Current.Get<IQueryable<T>>(registration, null, null); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static object Get(ServiceCacheRegistration registration, object[] values) { return ServiceCacheManager.Current.Get<object>(registration, null, values); }
        public static T Get<T>(ServiceCacheRegistration registration, object[] values) { return ServiceCacheManager.Current.Get<T>(registration, null, values); }
        public static IEnumerable<T> GetMany<T>(ServiceCacheRegistration registration, object[] values) { return ServiceCacheManager.Current.Get<IEnumerable<T>>(registration, null, values); }
        public static IQueryable<T> GetQuery<T>(ServiceCacheRegistration registration, object[] values) { return ServiceCacheManager.Current.Get<IQueryable<T>>(registration, null, values); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static object Get(ServiceCacheRegistration registration, object tag) { return ServiceCacheManager.Current.Get<object>(registration, tag, null); }
        public static T Get<T>(ServiceCacheRegistration registration, object tag) { return ServiceCacheManager.Current.Get<T>(registration, tag, null); }
        public static IEnumerable<T> GetMany<T>(ServiceCacheRegistration registration, object tag) { return ServiceCacheManager.Current.Get<IEnumerable<T>>(registration, tag, null); }
        public static IQueryable<T> GetQuery<T>(ServiceCacheRegistration registration, object tag) { return ServiceCacheManager.Current.Get<IQueryable<T>>(registration, tag, null); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static object Get(ServiceCacheRegistration registration, object tag, object[] values) { return ServiceCacheManager.Current.Get<object>(registration, tag, values); }
        public static T Get<T>(ServiceCacheRegistration registration, object tag, object[] values) { return ServiceCacheManager.Current.Get<T>(registration, tag, values); }
        public static IEnumerable<T> GetMany<T>(ServiceCacheRegistration registration, object tag, object[] values) { return ServiceCacheManager.Current.Get<IEnumerable<T>>(registration, tag, values); }
        public static IQueryable<T> GetQuery<T>(ServiceCacheRegistration registration, object tag, object[] values) { return ServiceCacheManager.Current.Get<IQueryable<T>>(registration, tag, values); }

        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="anchorType">The type.</param>
        /// <param name="registrationName">The registration id.</param>
        /// <returns></returns>
        public static object Get(Type anchorType, string registrationName) { return ServiceCacheManager.Current.Get<object>(anchorType, registrationName, null, null); }
        public static T Get<T>(Type anchorType, string registrationName) { return ServiceCacheManager.Current.Get<T>(anchorType, registrationName, null, null); }
        public static IEnumerable<T> GetMany<T>(Type anchorType, string registrationName) { return ServiceCacheManager.Current.Get<IEnumerable<T>>(anchorType, registrationName, null, null); }
        public static IQueryable<T> GetQuery<T>(Type anchorType, string registrationName) { return ServiceCacheManager.Current.Get<IQueryable<T>>(anchorType, registrationName, null, null); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="anchorType">The type.</param>
        /// <param name="registrationName">The registration id.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static object Get(Type anchorType, string registrationName, object[] values) { return ServiceCacheManager.Current.Get<object>(anchorType, registrationName, null, values); }
        public static T Get<T>(Type anchorType, string registrationName, object[] values) { return ServiceCacheManager.Current.Get<T>(anchorType, registrationName, null, values); }
        public static IEnumerable<T> GetMany<T>(Type anchorType, string registrationName, object[] values) { return ServiceCacheManager.Current.Get<IEnumerable<T>>(anchorType, registrationName, string.Empty, values); }
        public static IQueryable<T> GetQuery<T>(Type anchorType, string registrationName, object[] values) { return ServiceCacheManager.Current.Get<IQueryable<T>>(anchorType, registrationName, string.Empty, values); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="anchorType">The type.</param>
        /// <param name="registrationName">The registration id.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public static object Get(Type anchorType, string registrationName, object tag) { return ServiceCacheManager.Current.Get<object>(anchorType, registrationName, tag, null); }
        public static T Get<T>(Type anchorType, string registrationName, object tag) { return ServiceCacheManager.Current.Get<T>(anchorType, registrationName, tag, null); }
        public static IEnumerable<T> GetMany<T>(Type anchorType, string registrationName, object tag) { return ServiceCacheManager.Current.Get<IEnumerable<T>>(anchorType, registrationName, tag, null); }
        public static IQueryable<T> GetQuery<T>(Type anchorType, string registrationName, object tag) { return ServiceCacheManager.Current.Get<IQueryable<T>>(anchorType, registrationName, tag, null); }
        /// <summary>
        /// Gets the specified cached item.
        /// </summary>
        /// <param name="anchorType">The type.</param>
        /// <param name="registrationName">The registration id.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static object Get(Type anchorType, string registrationName, object tag, object[] values) { return ServiceCacheManager.Current.Get<object>(anchorType, registrationName, tag, values); }
        public static T Get<T>(Type anchorType, string registrationName, object tag, object[] values) { return ServiceCacheManager.Current.Get<T>(anchorType, registrationName, tag, values); }
        public static IEnumerable<T> GetMany<T>(Type anchorType, string registrationName, object tag, object[] values) { return ServiceCacheManager.Current.Get<IEnumerable<T>>(anchorType, registrationName, tag, values); }
        public static IQueryable<T> GetQuery<T>(Type anchorType, string registrationName, object tag, object[] values) { return ServiceCacheManager.Current.Get<IQueryable<T>>(anchorType, registrationName, tag, values); }

        #endregion
    }
}
