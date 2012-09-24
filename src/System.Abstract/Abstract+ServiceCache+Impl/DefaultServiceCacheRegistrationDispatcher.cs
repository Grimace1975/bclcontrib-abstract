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
using System.Threading;
namespace System.Abstract
{
    /// <summary>
    /// DefaultServiceCacheRegistrationDispatcher
    /// </summary>
    public class DefaultServiceCacheRegistrationDispatcher : ServiceCacheRegistration.IDispatcher
    {
        private static readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Gets the specified cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public T Get<T>(IServiceCache cache, IServiceCacheRegistration registration, object tag, object[] values)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");
            if (registration == null)
                throw new ArgumentNullException("registration");
            var registration2 = (registration as ServiceCacheRegistration);
            if (registration2 == null)
                throw new ArgumentException("must be ServiceCacheRegistration", "registration");
            var itemPolicy = registration2.ItemPolicy;
            if (itemPolicy == null)
                throw new ArgumentNullException("registration.ItemPolicy");
            // fetch from cache
            var name = registration.AbsoluteName;
            string @namespace;
            if (values != null && values.Length > 0)
                cache = cache.BehaveAs(values, out @namespace);
            else
                @namespace = null;
            var useDBNull = ((cache.Settings.Options & ServiceCacheOptions.UseDBNullWithRegistrations) == ServiceCacheOptions.UseDBNullWithRegistrations);
            var distributedServiceCache = cache.BehaveAs<IDistributedServiceCache>();
            if (distributedServiceCache == null)
                return GetUsingLock<T>(cache, registration2, tag, values, name, @namespace, useDBNull);
            return GetUsingCas<T>(distributedServiceCache, registration2, tag, values, name, @namespace, useDBNull);
        }

        /// <summary>
        /// Sends the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="messages">The messages.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Send(IServiceCache cache, IServiceCacheRegistration registration, object tag, params object[] messages)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");
            if (registration == null)
                throw new ArgumentNullException("registration");
            var registration2 = (registration as ServiceCacheRegistration);
            if (registration2 == null)
                throw new ArgumentException("must be ServiceCacheRegistration", "registration");
            var consumerInfos = registration2.GetConsumersFor(messages);
            if (!consumerInfos.GetEnumerator().MoveNext())
                return;
            //
            var itemPolicy = registration2.ItemPolicy;
            if (itemPolicy == null)
                throw new ArgumentNullException("registration.ItemPolicy");
            var useDBNull = ((cache.Settings.Options & ServiceCacheOptions.UseDBNullWithRegistrations) == ServiceCacheOptions.UseDBNullWithRegistrations);
            var distributedServiceCache = cache.BehaveAs<IDistributedServiceCache>();
            object value;
            foreach (var header in cache.Get(tag, registration))
                foreach (var consumerInfo in consumerInfos)
                    if ((value = consumerInfo.Invoke(cache, tag, header)) != null)
                        if (distributedServiceCache == null)
                            SetUsingLock(cache, registration2, tag, header, useDBNull, value);
                        else
                            SetUsingCas(distributedServiceCache, registration2, tag, header, useDBNull, value);
        }

        //public void Update(IServiceCache cache, IServiceCacheRegistration registration, object tag, object value)
        //{
        //    if (cache == null)
        //        throw new ArgumentNullException("cache");
        //    if (registration == null)
        //        throw new ArgumentNullException("registration");
        //    var registration2 = (registration as ServiceCacheRegistration);
        //    if (registration2 == null)
        //        throw new ArgumentException("must be ServiceCacheRegistration", "registration");
        //    var itemPolicy = registration2.ItemPolicy;
        //    if (itemPolicy == null)
        //        throw new ArgumentNullException("registration.ItemPolicy");
        //    var useDBNull = ((cache.Settings.Options & ServiceCacheOptions.UseDBNullWithRegistrations) == ServiceCacheOptions.UseDBNullWithRegistrations);
        //    var distributedServiceCache = cache.BehaveAs<IDistributedServiceCache>();
        //    if (distributedServiceCache == null)
        //        SetUsingLock(cache, registration2, tag, header, useDBNull, value);
        //    else
        //        SetUsingCas(distributedServiceCache, registration2, tag, header, useDBNull, value);
        //}

        /// <summary>
        /// Removes the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="registration">The registration.</param>
        public void Remove(IServiceCache cache, IServiceCacheRegistration registration)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");
            if (registration == null)
                throw new ArgumentNullException("registration");
            var registration2 = (registration as ServiceCacheRegistration);
            if (registration2 == null)
                throw new ArgumentException("must be ServiceCacheRegistration", "registration");
            foreach (var name in registration2.Keys)
                cache.Remove(null, name, registration);
        }

        private static T CreateData<T>(string @namespace, ServiceCacheRegistration registration, object tag, object[] values, out CacheItemHeader header)
        {
            if (@namespace != null)
            {
                var namespaces = registration.Namespaces;
                if (!namespaces.Contains(@namespace))
                    namespaces.Add(@namespace);
            }
            header = new CacheItemHeader { Values = values, };
            return (T)registration.Builder(tag, values);
        }

        #region Locks

        private static T GetUsingNoLock<T>(IServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values, string name, string @namespace, bool useDBNull)
        {
            CacheItemHeader header;
            var valueAsCache = cache.Get(tag, name, registration, out header);
            if (valueAsCache != null && (!registration.UseHeaders || header != null))
                return (!useDBNull || !(valueAsCache is DBNull) ? (T)valueAsCache : default(T));
            // create
            var value = CreateData<T>(@namespace, registration, tag, values, out header);
            valueAsCache = (!useDBNull || value != null ? (object)value : DBNull.Value);
            cache.Add(tag, name, registration.ItemPolicy, valueAsCache, new ServiceCacheByDispatcher(registration, values, header));
            return value;
        }
        private static void SetUsingNoLock(IServiceCache cache, ServiceCacheRegistration registration, object tag, CacheItemHeader header, bool useDBNull, object value)
        {
            var valueAsCache = (!useDBNull || value != null ? (object)value : DBNull.Value);
            cache.Add(tag, header.Item, registration.ItemPolicy, valueAsCache, new ServiceCacheByDispatcher(registration, header.Values, header));
        }

        private static T GetUsingLock<T>(IServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values, string name, string @namespace, bool useDBNull)
        {
            CacheItemHeader header;
            var valueAsCache = cache.Get(tag, name, registration, out header);
            if (valueAsCache != null && (!registration.UseHeaders || header != null))
                return (!useDBNull || !(valueAsCache is DBNull) ? (T)valueAsCache : default(T));
            lock (_rwLock)
            {
                valueAsCache = cache.Get(tag, name, registration, out header);
                if (valueAsCache != null && (!registration.UseHeaders || header != null))
                    return (!useDBNull || !(valueAsCache is DBNull) ? (T)valueAsCache : default(T));
                // create
                var value = CreateData<T>(@namespace, registration, tag, values, out header);
                valueAsCache = (!useDBNull || value != null ? (object)value : DBNull.Value);
                cache.Add(tag, name, registration.ItemPolicy, valueAsCache, new ServiceCacheByDispatcher(registration, values, header));
                return value;
            }
        }
        private static void SetUsingLock(IServiceCache cache, ServiceCacheRegistration registration, object tag, CacheItemHeader header, bool useDBNull, object value)
        {
            lock (_rwLock)
            {
                var valueAsCache = (!useDBNull || value != null ? (object)value : DBNull.Value);
                cache.Add(tag, header.Item, registration.ItemPolicy, valueAsCache, new ServiceCacheByDispatcher(registration, header.Values, header));
            }
        }

        private static T GetUsingRwLock<T>(IServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values, string name, string @namespace, bool useDBNull)
        {
            _rwLock.EnterUpgradeableReadLock();
            try
            {
                CacheItemHeader header;
                var valueAsCache = cache.Get(tag, name, registration, out header);
                if (valueAsCache != null && (!registration.UseHeaders || header != null))
                    return (!useDBNull || !(valueAsCache is DBNull) ? (T)valueAsCache : default(T));
                _rwLock.EnterWriteLock();
                try
                {
                    valueAsCache = cache.Get(tag, name, registration, out header);
                    if (valueAsCache != null && (!registration.UseHeaders || header != null))
                        return (!useDBNull || !(valueAsCache is DBNull) ? (T)valueAsCache : default(T));
                    // create
                    var value = CreateData<T>(@namespace, registration, tag, values, out header);
                    valueAsCache = (!useDBNull || value != null ? (object)value : DBNull.Value);
                    cache.Add(tag, name, registration.ItemPolicy, valueAsCache, new ServiceCacheByDispatcher(registration, values, header));
                    return value;
                }
                finally { _rwLock.ExitWriteLock(); }
            }
            finally { _rwLock.ExitUpgradeableReadLock(); }
        }
        private static void SetUsingRwLock(IServiceCache cache, ServiceCacheRegistration registration, object tag, CacheItemPolicy itemPolicy, CacheItemHeader header, bool useDBNull, object value)
        {
            _rwLock.EnterWriteLock();
            try
            {
                var valueAsCache = (!useDBNull || value != null ? (object)value : DBNull.Value);
                cache.Add(tag, header.Item, itemPolicy, valueAsCache, new ServiceCacheByDispatcher(registration, header.Values, header));
            }
            finally { _rwLock.ExitWriteLock(); }
        }

        private static T GetUsingCas<T>(IDistributedServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values, string name, string @namespace, bool useDBNull)
        {
            CacheItemHeader header;
            var valueAsCache = cache.Get(tag, name, registration, out header);
            if (valueAsCache != null && (!registration.UseHeaders || header != null))
                return (!useDBNull || !(valueAsCache is DBNull) ? (T)valueAsCache : default(T));
            // create
            var value = CreateData<T>(@namespace, registration, tag, values, out header);
            valueAsCache = (!useDBNull || value != null ? (object)value : DBNull.Value);
            cache.Add(tag, name, registration.ItemPolicy, valueAsCache, new ServiceCacheByDispatcher(registration, values, header));
            return value;
        }
        private static void SetUsingCas(IDistributedServiceCache cache, ServiceCacheRegistration registration, object tag, CacheItemHeader header, bool useDBNull, object value)
        {
            var valueAsCache = (!useDBNull || value != null ? (object)value : DBNull.Value);
            cache.Add(tag, header.Item, registration.ItemPolicy, valueAsCache, new ServiceCacheByDispatcher(registration, header.Values, header));
        }

        #endregion
    }
}
