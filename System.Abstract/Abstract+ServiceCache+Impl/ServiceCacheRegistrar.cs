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
using System.Reflection;
using System.Threading;
namespace System.Abstract
{
    /// <summary>
    /// ServiceCacheRegistrar
    /// </summary>
    public class ServiceCacheRegistrar
    {
        private static ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private static Dictionary<Type, ServiceCacheRegistrar> _items = new Dictionary<Type, ServiceCacheRegistrar>();
        private ReaderWriterLockSlim _setRwLock = new ReaderWriterLockSlim();
        private HashSet<ServiceCacheRegistration> _set = new HashSet<ServiceCacheRegistration>();
        private Dictionary<string, ServiceCacheRegistration> _setAsName = new Dictionary<string, ServiceCacheRegistration>();
        private string _namePrefix;

        /// <summary>
        /// IDispatch
        /// </summary>
        public interface IDispatch
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceRegistrar"/> class.
        /// </summary>
        /// <param name="anchorType">Type of the anchor.</param>
        internal ServiceCacheRegistrar(Type anchorType)
        {
            _namePrefix = "SC\\" + anchorType.FullName + "::";
            AnchorType = anchorType;
        }

        public static ServiceCacheRegistrar Get<TAnchor>()
            where TAnchor : IServiceRegistrar { return Get(typeof(TAnchor)); }
        public static ServiceCacheRegistrar Get(Type anchorType)
        {
            ServiceCacheRegistrar registrar;
            TryGet(anchorType, out registrar, true);
            return registrar;
        }

        public static void RegisterAllBelow<T>() { RegisterAllBelow(typeof(T)); }
        public static void RegisterAllBelow(Type type)
        {
            var registrationType = typeof(ServiceCacheRegistration);
            var registrar = Get(type);
            var types = type.GetFields(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.FieldType.IsAssignableFrom(registrationType))
                .Select(f => (ServiceCacheRegistration)f.GetValue(null));
            foreach (var t in types)
                registrar.Register(t);
            // recurse down
            foreach (var t in type.GetNestedTypes())
                RegisterAllBelow(t);
        }

        /// <summary>
        /// Gets or sets the type of the anchor.
        /// </summary>
        /// <value>
        /// The type of the anchor.
        /// </value>
        public Type AnchorType { get; private set; }

        /// <summary>
        /// Registers the specified registration.
        /// </summary>
        /// <param name="name">The registration name.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="cacheTags">The cache tags.</param>
        public void Register(string name, CacheItemBuilder builder, params string[] cacheTags) { Register(new ServiceCacheRegistration(name, new CacheItemPolicy(60), builder, cacheTags)); }
        /// <summary>
        /// Registers the specified registration.
        /// </summary>
        /// <param name="name">The registration name.</param>
        /// <param name="minuteTimeout">The minute timeout.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="cacheTags">The cache tags.</param>
        public void Register(string name, int minuteTimeout, CacheItemBuilder builder, params string[] cacheTags) { Register(new ServiceCacheRegistration(name, new CacheItemPolicy(minuteTimeout), builder, cacheTags)); }
        /// <summary>
        /// Registers the specified registration.
        /// </summary>
        /// <param name="name">The registration name.</param>
        /// <param name="itemPolicy">The cache command.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="cacheTags">The cache tags.</param>
        public void Register(string name, CacheItemPolicy itemPolicy, CacheItemBuilder builder, params string[] cacheTags) { Register(new ServiceCacheRegistration(name, itemPolicy, builder, cacheTags)); }
        /// <summary>
        /// Registers the specified registration.
        /// </summary>
        /// <param name="registration">The registration.</param>
        public void Register(ServiceCacheRegistration registration)
        {
            if (registration == null)
                throw new ArgumentNullException("registration");
            _setRwLock.EnterWriteLock();
            try
            {
                if (_set.Contains(registration))
                    throw new InvalidOperationException(string.Format(Local.RedefineDataCacheAB, AnchorType.ToString(), registration.Name));
                // add
                string registrationName = registration.Name;
                if (string.IsNullOrEmpty(registrationName))
                    throw new ArgumentNullException("registration.Name");
                if (registrationName.IndexOf("::") > -1)
                    throw new ArgumentException(string.Format(Local.ScopeCharacterNotAllowedA, registrationName), "registration");
                if (_setAsName.ContainsKey(registrationName))
                    throw new ArgumentException(string.Format(Local.RedefineNameA, registrationName), "registration");
                _setAsName.Add(registrationName, registration);
                _set.Add(registration);
				// link-in
				registration.SetRegistrar(this, _namePrefix + registrationName);
            }
            finally { _setRwLock.ExitWriteLock(); }
        }

        /// <summary>
        /// Clears this all registrations.
        /// </summary>
        public void Clear()
        {
            _setRwLock.EnterWriteLock();
            try
            {
                _setAsName.Clear();
                _set.Clear();
            }
            finally { _setRwLock.ExitWriteLock(); }
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns>
        ///   <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(ServiceCacheRegistration registration)
        {
            _setRwLock.EnterReadLock();
            try { return _set.Contains(registration); }
            finally { _setRwLock.ExitReadLock(); }
        }
        /// <summary>
        /// Determines whether the specified name has been registered.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        ///   <c>true</c> if the specified name has been registered; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string name)
        {
            _setRwLock.EnterReadLock();
            try { return _setAsName.ContainsKey(name); }
            finally { _setRwLock.ExitReadLock(); }
        }

        /// <summary>
        /// Removes the specified registration.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        public bool Remove(ServiceCacheRegistration registration)
        {
            _setRwLock.EnterWriteLock();
            try { _setAsName.Remove(registration.Name); return _set.Remove(registration); }
            finally { _setRwLock.ExitWriteLock(); }
        }
        /// <summary>
        /// Removes the specified registration.
        /// </summary>
        /// <param name="name">The registration name.</param>
        /// <returns></returns>
        public bool Remove(string name)
        {
            _setRwLock.EnterWriteLock();
            try { var registration = _setAsName[name]; _setAsName.Remove(name); return _set.Remove(registration); }
            finally { _setRwLock.ExitWriteLock(); }
        }

        internal IEnumerable<ServiceCacheRegistration> GetAll()
        {
            return _set;
        }

        public static bool TryGet(Type anchorType, out ServiceCacheRegistrar registrar, bool createIfRequired)
        {
            if (anchorType == null)
                throw new ArgumentNullException("anchorType");
            _rwLock.EnterUpgradeableReadLock();
            try
            {
                bool exists = _items.TryGetValue(anchorType, out registrar);
                if ((exists) || (!createIfRequired))
                    return exists;
                _rwLock.EnterWriteLock();
                try
                {
                    if (!_items.TryGetValue(anchorType, out registrar))
                    {
                        // create
                        registrar = new ServiceCacheRegistrar(anchorType);
                        _items.Add(anchorType, registrar);
                    }
                }
                finally { _rwLock.ExitWriteLock(); }
                return true;
            }
            finally { _rwLock.ExitUpgradeableReadLock(); }
        }

        public static bool TryGetValue(ServiceCacheRegistration registration, ref int recurses, out ServiceCacheRegistration foundRegistration)
        {
            _rwLock.EnterReadLock();
            try
            {
                var registrar = registration.Registrar;
                if (registrar != null)
                {
                    // local check
                    var foreignRegistration = (registration as ServiceCacheForeignRegistration);
                    if (foreignRegistration == null)
                    {
                        foundRegistration = registration;
                        return true;
                    }
                    // foreign recurse
                    if (recurses++ > 4)
                        throw new InvalidOperationException(Local.ExceedRecurseCount);
                    // touch - starts foreign static constructor
                    var foreignType = foreignRegistration.ForeignType;
                    foreignType.InvokeMember("Touch", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static, null, null, null);
                    return TryGetValue(foreignType, foreignRegistration.ForeignName, ref recurses, out foundRegistration);
                }
                foundRegistration = null;
                return false;
            }
            finally { _rwLock.ExitReadLock(); }
        }

        public static bool TryGetValue(Type anchorType, string registrationName, ref int recurses, out ServiceCacheRegistration foundRegistration)
        {
            _rwLock.EnterReadLock();
            try
            {
                ServiceCacheRegistrar registrar;
                if (_items.TryGetValue(anchorType, out registrar))
                {
                    // registration locals
                    var setRwLock = registrar._setRwLock;
                    var setAsId = registrar._setAsName;
                    setRwLock.EnterReadLock();
                    try
                    {
                        ServiceCacheRegistration registration;
                        if (setAsId.TryGetValue(registrationName, out registration))
                        {
                            // local check
                            var foreignRegistration = (registration as ServiceCacheForeignRegistration);
                            if (foreignRegistration == null)
                            {
                                foundRegistration = registration;
                                return true;
                            }
                            // foreign recurse
                            if (recurses++ > 4)
                                throw new InvalidOperationException(Local.ExceedRecurseCount);
                            // touch - starts foreign static constructor
                            var foreignType = foreignRegistration.ForeignType;
                            foreignType.InvokeMember("Touch", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static, null, null, null);
                            return TryGetValue(foreignType, foreignRegistration.ForeignName, ref recurses, out foundRegistration);
                        }
                    }
                    finally { setRwLock.ExitReadLock(); }
                }
                foundRegistration = null;
                return false;
            }
            finally { _rwLock.ExitReadLock(); }
        }
    }
}
