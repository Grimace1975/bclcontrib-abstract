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

        public T Get<T>(IServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");
            if (registration == null)
                throw new ArgumentNullException("registration");
            var itemPolicy = registration.ItemPolicy;
            if (itemPolicy == null)
                throw new ArgumentNullException("registration.CacheCommand");
            // fetch from cache
            var name = registration.AbsoluteName;
            string @namespace;
            if (values != null && values.Length > 0)
                cache = cache.BehaveAs(values, out @namespace);
            else
                @namespace = null;
            var useDBNull = ((cache.Settings.Options & ServiceCacheOptions.UseDBNullWithRegistrations) == ServiceCacheOptions.UseDBNullWithRegistrations);
            var distributedServiceCache = (cache as IDistributedServiceCache);
            if (distributedServiceCache == null)
                return GetUsingLock<T>(cache, registration, tag, values, itemPolicy, name, @namespace, useDBNull);
            return GetUsingCas<T>(distributedServiceCache, registration, tag, values, itemPolicy, name, @namespace, useDBNull);
        }

        private static T GetUsingNoLock<T>(IServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values, CacheItemPolicy itemPolicy, string name, string @namespace, bool useDBNull)
        {
            var valueAsCache = cache.Get(tag, name);
            if (valueAsCache != null)
                return (!useDBNull || !(valueAsCache is DBNull) ? (T)valueAsCache : default(T));
            // create
            var value = CreateData<T>(@namespace, registration, tag, values);
            valueAsCache = (!useDBNull || value != null ? (object)value : DBNull.Value);
            cache.Add(tag, name, itemPolicy, valueAsCache, new ServiceCacheByDispatcher(registration, values));
            return value;
        }

        private static T GetUsingLock<T>(IServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values, CacheItemPolicy itemPolicy, string name, string @namespace, bool useDBNull)
        {
            var valueAsCache = cache.Get(tag, name);
            if (valueAsCache != null)
                return (!useDBNull || !(valueAsCache is DBNull) ? (T)valueAsCache : default(T));
            lock (_rwLock)
            {
                valueAsCache = cache.Get(tag, name);
                if (valueAsCache != null)
                    return (!useDBNull || !(valueAsCache is DBNull) ? (T)valueAsCache : default(T));
                // create
                var value = CreateData<T>(@namespace, registration, tag, values);
                valueAsCache = (!useDBNull || value != null ? (object)value : DBNull.Value);
                cache.Add(tag, name, itemPolicy, valueAsCache, new ServiceCacheByDispatcher(registration, values));
                return value;
            }
        }

        private static T GetUsingRwLock<T>(IServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values, CacheItemPolicy itemPolicy, string name, string @namespace, bool useDBNull)
        {
            _rwLock.EnterUpgradeableReadLock();
            try
            {
                var valueAsCache = cache.Get(tag, name);
                if (valueAsCache != null)
                    return (!useDBNull || !(valueAsCache is DBNull) ? (T)valueAsCache : default(T));
                _rwLock.EnterWriteLock();
                try
                {
                    valueAsCache = cache.Get(tag, name);
                    if (valueAsCache != null)
                        return (!useDBNull || !(valueAsCache is DBNull) ? (T)valueAsCache : default(T));
                    // create
                    var value = CreateData<T>(@namespace, registration, tag, values);
                    valueAsCache = (!useDBNull || value != null ? (object)value : DBNull.Value);
                    cache.Add(tag, name, itemPolicy, valueAsCache, new ServiceCacheByDispatcher(registration, values));
                    return value;
                }
                finally { _rwLock.ExitWriteLock(); }
            }
            finally { _rwLock.ExitUpgradeableReadLock(); }
        }

        private static T GetUsingCas<T>(IDistributedServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values, CacheItemPolicy itemPolicy, string name, string @namespace, bool useDBNull)
        {
            var valueAsCache = cache.Get(tag, name);
            if (valueAsCache != null)
                return (!useDBNull || !(valueAsCache is DBNull) ? (T)valueAsCache : default(T));
            // create
            var value = CreateData<T>(@namespace, registration, tag, values);
            valueAsCache = (!useDBNull || value != null ? (object)value : DBNull.Value);
            cache.Add(tag, name, itemPolicy, valueAsCache, new ServiceCacheByDispatcher(registration, values));
            return value;
        }

        public void Remove(IServiceCache cache, ServiceCacheRegistration registration)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");
            if (registration == null)
                throw new ArgumentNullException("registration");
            // remove from cache
            var name = registration.AbsoluteName;
            cache.Remove(name, null);
            var namespaces = registration.Namespaces;
            if (namespaces != null)
                foreach (var n in namespaces)
                    cache.Remove(n + name, null);
        }

        private static T CreateData<T>(string @namespace, ServiceCacheRegistration registration, object tag, object[] values)
        {
            if (@namespace != null)
            {
                var namespaces = registration.Namespaces;
                if (!namespaces.Contains(@namespace))
                    namespaces.Add(@namespace);
            }
            return (T)registration.Builder(tag, values);
        }
    }
}
