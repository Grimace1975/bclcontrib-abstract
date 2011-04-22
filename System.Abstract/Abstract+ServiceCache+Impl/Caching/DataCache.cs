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
#if EXPERIMENTAL
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
namespace System.Abstract.Caching
{
    /// <summary>
    /// DataCache
    /// </summary>
    //: no way to seperate values and state. they are all used for a key
    //: could use generics so don't have to typecast result, maybe autofind the T from the registration
    public static class DataCache
    {
        /// <summary>
        /// NoHeaderId
        /// </summary>
        public const string NoHeaderId = "none";
        private static ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

        #region Class Types
        /// <summary>
        /// Primitive
        /// </summary>
        public class Primitive
        {
            /// <summary>
            /// YesNo
            /// </summary>
            public static readonly DataCacheRegistration YesNo = new DataCacheRegistration("YesNo", (object tag, object[] values) =>
            {
				var values2 = new Dictionary<string, string>(3);
				switch (tag as string)
                {
                    case "":
						values2.Add(string.Empty, "--");
                        break;
                    case NoHeaderId:
                        break;
                    default:
                        throw new InvalidOperationException();
                }
				values2.Add(bool.TrueString, "Yes");
				values2.Add(bool.FalseString, "No");
				return values2;
            });
            /// <summary>
            /// Gender
            /// </summary>
			public static readonly DataCacheRegistration Gender = new DataCacheRegistration("Gender", (object tag, object[] values) =>
            {
                var values2 = new Dictionary<string, string>(3);
                switch (tag as string)
                {
                    case "":
						values2.Add(string.Empty, "--");
                        break;
                    case NoHeaderId:
                        break;
                    default:
                        throw new InvalidOperationException();
                }
				values2.Add("Male", "Male");
				values2.Add("Female", "Female");
				return values2;
            });
            /// <summary>
            /// Integer
            /// </summary>
			public static readonly DataCacheRegistration Integer = new DataCacheRegistration("Integer", (object tag, object[] values) =>
            {
                var values2 = new Dictionary<string, string>(3);
                switch (tag as string)
                {
                    case "":
						values2.Add(string.Empty, "--");
                        break;
                    case NoHeaderId:
                        break;
                    default:
                        throw new InvalidOperationException();
                }
                int startIndex = (int)values[0];
                int endIndex = (int)values[1];
                int indexStep = (int)values[2];
                for (int index = startIndex; index < endIndex; index += indexStep)
					values2.Add(index.ToString(), index.ToString());
				return values2;
            });
        }
        #endregion

        /// <summary>
        /// Initializes the <see cref="DataCache"/> class.
        /// </summary>
        static DataCache()
        {
            var registrar = GetRegistrar(typeof(DataCache));
            registrar.Register(Primitive.YesNo);
            registrar.Register(Primitive.Gender);
            registrar.Register(Primitive.Integer);
        }

        /// <summary>
        /// Gets the registrar.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static DataCacheRegistrar GetRegistrar(Type type)
        {
            DataCacheRegistrar dataSourceRegistrar;
            DataCacheRegistrar.TryGetInstance(type, out dataSourceRegistrar, true);
            return dataSourceRegistrar;
        }

        public static void RegisterAllBelow(Type type)
        {
            var registrationType = typeof(DataCacheRegistration);
            var registrar = GetRegistrar(type);
            type.GetFields(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.FieldType.IsAssignableFrom(registrationType))
                .Select(f => (DataCacheRegistration)f.GetValue(null))
                .ToList()
                .ForEach(r => registrar.Register(r));
            // below
            type.GetNestedTypes()
                .ToList()
                .ForEach(t => RegisterAllBelow(t));
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static object Get(DataCacheRegistration registration) { return Get<object>(registration, string.Empty, null); }
        public static T Get<T>(DataCacheRegistration registration) { return Get<T>(registration, string.Empty, null); }
        public static IEnumerable<T> GetMany<T>(DataCacheRegistration registration) { return Get<IEnumerable<T>>(registration, string.Empty, null); }
        public static IQueryable<T> GetQuery<T>(DataCacheRegistration registration) { return Get<IQueryable<T>>(registration, string.Empty, null); }
        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="key">The key.</param>
        /// <param name="values">The value array.</param>
        /// <returns></returns>
        public static object Get(DataCacheRegistration registration, object[] values) { return Get<object>(registration, string.Empty, values); }
        public static T Get<T>(DataCacheRegistration registration, object[] values) { return Get<T>(registration, string.Empty, values); }
        public static IEnumerable<T> GetMany<T>(DataCacheRegistration registration, object[] values) { return Get<IEnumerable<T>>(registration, string.Empty, values); }
        public static IQueryable<T> GetQuery<T>(DataCacheRegistration registration, object[] values) { return Get<IQueryable<T>>(registration, string.Empty, values); }
        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="key">The key.</param>
        /// <param name="headerId">The header id.</param>
        /// <returns></returns>
        public static object Get(DataCacheRegistration registration, string tag) { return Get<object>(registration, tag, null); }
        public static T Get<T>(DataCacheRegistration registration, string tag) { return Get<T>(registration, tag, null); }
        public static IEnumerable<T> GetMany<T>(DataCacheRegistration registration, string tag) { return Get<IEnumerable<T>>(registration, tag, null); }
        public static IQueryable<T> GetQuery<T>(DataCacheRegistration registration, string tag) { return Get<IQueryable<T>>(registration, tag, null); }
        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static object Get(Type type, string registrationId) { return Get<object>(type, registrationId, string.Empty, null); }
        public static T Get<T>(Type type, string registrationId) { return Get<T>(type, registrationId, string.Empty, null); }
        public static IEnumerable<T> GetMany<T>(Type type, string registrationId) { return Get<IEnumerable<T>>(type, registrationId, string.Empty, null); }
        public static IQueryable<T> GetQuery<T>(Type type, string registrationId) { return Get<IQueryable<T>>(type, registrationId, string.Empty, null); }
        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="key">The key.</param>
        /// <param name="values">The value array.</param>
        /// <returns></returns>
        public static object Get(Type type, string registrationId, object[] values) { return Get<object>(type, registrationId, string.Empty, values); }
        public static T Get<T>(Type type, string registrationId, object[] values) { return Get<T>(type, registrationId, string.Empty, values); }
        public static IEnumerable<T> GetMany<T>(Type type, string registrationId, object[] values) { return Get<IEnumerable<T>>(type, registrationId, string.Empty, values); }
        public static IQueryable<T> GetQuery<T>(Type type, string registrationId, object[] values) { return Get<IQueryable<T>>(type, registrationId, string.Empty, values); }
        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="key">The key.</param>
        /// <param name="headerId">The header id.</param>
        /// <returns></returns>
        public static object Get(Type type, string registrationId, string tag) { return Get<object>(type, registrationId, tag, null); }
        public static T Get<T>(Type type, string registrationId, string tag) { return Get<T>(type, registrationId, tag, null); }
        public static IEnumerable<T> GetMany<T>(Type type, string registrationId, string tag) { return Get<IEnumerable<T>>(type, registrationId, tag, null); }
        public static IQueryable<T> GetQuery<T>(Type type, string registrationId, string tag) { return Get<IQueryable<T>>(type, registrationId, tag, null); }
        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="key">The key.</param>
        /// <param name="headerId">The header id.</param>
        /// <param name="values">The value array.</param>
        /// <returns></returns>
        //:BIND: GetDataSource(Type, string, string, object[])
        public static object Get(DataCacheRegistration registration, string tag, object[] values) { return Get<object>(registration, tag, values); }
        public static IEnumerable<T> GetMany<T>(DataCacheRegistration registration, string tag, object[] values) { return Get<IEnumerable<T>>(registration, tag, values); }
        public static IQueryable<T> GetQuery<T>(DataCacheRegistration registration, string tag, object[] values) { return Get<IQueryable<T>>(registration, tag, values); }
        public static T Get<T>(DataCacheRegistration registration, string tag, object[] values)
        {
            if (registration == null)
                throw new ArgumentNullException("registration");
            if (tag == null)
                throw new ArgumentNullException("headerId");
            tag = tag.ToLowerInvariant();
            // fetch registration
            int recurses = 0;
            DataCacheRegistration foundRegistration;
            if (!DataCacheRegistrar.TryGetValue(registration, ref recurses, out foundRegistration))
                throw new InvalidOperationException(string.Format(Local.UndefinedDataCacheRegistrationAB, (registration.Registrar != null ? registration.Registrar.AnchorType.ToString() : "{unregistered}"), registration.ID));
            // fetch from cache
            var cacheCommand = foundRegistration.CacheCommand;
            string name = cacheCommand.Name + "::" + tag;
            var cache = foundRegistration.GetCacheSystem(values);
            _rwLock.EnterUpgradeableReadLock();
            try
            {
                T value;
                if ((value = (T)cache[name]) == null)
                {
                    _rwLock.EnterWriteLock();
                    try
                    {
                        if ((value = (T)cache[name]) == null)
                        {
                            // create
                            value = DataCacheRegistrar.CreateData<T>(foundRegistration, tag, values);
                            cache.Add(cacheCommand, name, value);
                        }
                    }
                    finally { _rwLock.ExitWriteLock(); }
                }
                return value;
            }
            finally { _rwLock.ExitUpgradeableReadLock(); }
        }
        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="key">The key.</param>
        /// <param name="headerId">The header id.</param>
        /// <param name="values">The value array.</param>
        /// <returns></returns>
        //:BIND: GetDataSource(DataSourceRegistration, string, object[])
        public static object Get(Type type, string registrationId, string tag, object[] values) { return Get<object>(type, registrationId, tag, values); }
        public static IEnumerable<T> GetMany<T>(Type type, string registrationId, string tag, object[] values) { return Get<IEnumerable<T>>(type, registrationId, tag, values); }
        public static IQueryable<T> GetQuery<T>(Type type, string registrationId, string tag, object[] values) { return Get<IQueryable<T>>(type, registrationId, tag, values); }
        public static T Get<T>(Type type, string registrationId, string tag, object[] values)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(registrationId))
                throw new ArgumentNullException("registrationId");
            if (tag == null)
                throw new ArgumentNullException("headerId");
            tag = tag.ToLowerInvariant();
            // fetch registration
            int recurses = 0;
            DataCacheRegistration foundRegistration;
            if (!DataCacheRegistrar.TryGetValue(type, registrationId, ref recurses, out foundRegistration))
                throw new InvalidOperationException(string.Format(Local.UndefinedDataCacheRegistrationAB, type.ToString(), registrationId));
            // fetch from cache
            var cacheCommand = foundRegistration.CacheCommand;
            string name = cacheCommand.Name + "::" + tag;
            var cache = foundRegistration.GetCacheSystem(values);
            _rwLock.EnterUpgradeableReadLock();
            try
            {
                T value;
                if ((value = (T)cache[name]) == null)
                {
                    _rwLock.EnterWriteLock();
                    try
                    {
                        if ((value = (T)cache[name]) == null)
                        {
                            // create
                            value = DataCacheRegistrar.CreateData<T>(foundRegistration, tag, values);
                            cache.Add(cacheCommand, name, value);
                        }
                    }
                    finally { _rwLock.ExitWriteLock(); }
                }
                return value;
            }
            finally { _rwLock.ExitUpgradeableReadLock(); }
        }

        /// <summary>
        /// Invalidates the data source.
        /// </summary>
        /// <param name="type">The type.</param>
        public static void InvalidateData(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            // fetch registration-hash
            DataCacheRegistrar registrar;
            if (!DataCacheRegistrar.TryGetInstance(type, out registrar, false))
                throw new InvalidOperationException(string.Format(Local.UndefinedDataCacheRegistrationA, type.ToString()));
            foreach (var registration in registrar.Registrations)
                InvalidateData(registration);
        }
        /// <summary>
        /// Invalidates the data source.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="key">The key.</param>
        public static void InvalidateData(Type type, string registrationID)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(registrationID))
				throw new ArgumentNullException("registrationID");
            // fetch registration
            int recurses = 0;
            DataCacheRegistration foundRegistration;
            if (!DataCacheRegistrar.TryGetValue(type, registrationID, ref recurses, out foundRegistration))
                throw new InvalidOperationException(string.Format(Local.UndefinedDataCacheRegistrationAB, type.ToString(), registrationID));
            InvalidateData(foundRegistration);
        }
        /// <summary>
        /// Invalidates the data source.
        /// </summary>
        /// <param name="registration">The registration.</param>
        public static void InvalidateData(DataCacheRegistration registration)
        {
            if (registration is DataCacheForeignRegistration)
                throw new InvalidOperationException(Local.InvalidDataSource);
            // remove from cache
            var cache = registration.GetCacheSystem(null);
            foreach (string tags in registration.Tags)
				cache.Remove(registration.CacheCommand.Name + "::" + tags);
        }
    }
}
#endif