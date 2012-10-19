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
    /// <remarks>
    /// [Wrap]SC\\{Anchor.FullName}::{Registration.Name}[#]
    /// ServiceCacheRegistrar._namePrefix - SC\\{Anchor.FullName}::
    /// Registration.AbsoluteName = SC\\{Anchor.FullName}::{Registration.Name}
    /// </remarks>
    public class ServiceCacheRegistrar
    {
        private static ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private static Dictionary<Type, ServiceCacheRegistrar> _items = new Dictionary<Type, ServiceCacheRegistrar>();
        private ReaderWriterLockSlim _setRwLock = new ReaderWriterLockSlim();
        private HashSet<IServiceCacheRegistration> _set = new HashSet<IServiceCacheRegistration>();
        private Dictionary<string, IServiceCacheRegistration> _setAsName = new Dictionary<string, IServiceCacheRegistration>();
        private string _namePrefix;

        /// <summary>
        /// IDispatch
        /// </summary>
        public interface IDispatch { }

        internal ServiceCacheRegistrar(Type anchorType)
        {
            _namePrefix = "SC\\" + anchorType.FullName + "::";
            AnchorType = anchorType;
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="TAnchor">The type of the anchor.</typeparam>
        /// <returns></returns>
        public static ServiceCacheRegistrar Get<TAnchor>() { return Get(typeof(TAnchor)); }
        /// <summary>
        /// Gets the specified anchor type.
        /// </summary>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <returns></returns>
        public static ServiceCacheRegistrar Get(Type anchorType)
        {
            ServiceCacheRegistrar registrar;
            TryGet(anchorType, out registrar, true);
            return registrar;
        }

        /// <summary>
        /// Registers all below.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RegisterAllBelow<T>() { RegisterAllBelow(typeof(T)); }
        /// <summary>
        /// Registers all below.
        /// </summary>
        /// <param name="type">The type.</param>
        public static void RegisterAllBelow(Type type)
        {
            var registrationType = typeof(IServiceCacheRegistration);
            var registrar = Get(type);
            var types = type.GetFields(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => registrationType.IsAssignableFrom(f.FieldType))
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
        public void Register(IServiceCacheRegistration registration)
        {
            if (registration == null)
                throw new ArgumentNullException("registration");
            _setRwLock.EnterWriteLock();
            try
            {
                if (_set.Contains(registration))
                    throw new InvalidOperationException(string.Format(Local.RedefineDataCacheAB, AnchorType.ToString(), registration.Name));
                // add
                var registrationName = registration.Name;
                if (string.IsNullOrEmpty(registrationName))
                    throw new ArgumentNullException("registration.Name");
                if (registrationName.IndexOf("::") > -1)
                    throw new ArgumentException(string.Format(Local.ScopeCharacterNotAllowedA, registrationName), "registration");
                if (_setAsName.ContainsKey(registrationName))
                    throw new ArgumentException(string.Format(Local.RedefineNameA, registrationName), "registration");
                _setAsName.Add(registrationName, registration);
                _set.Add(registration);
                // link-in
                registration.AttachRegistrar(this, _namePrefix + registrationName);
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
        public bool Contains(IServiceCacheRegistration registration)
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
        public bool Remove(IServiceCacheRegistration registration)
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

        internal IEnumerable<IServiceCacheRegistration> All
        {
            get { return _set; }
        }

        /// <summary>
        /// Tries the get.
        /// </summary>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrar">The registrar.</param>
        /// <param name="createIfRequired">if set to <c>true</c> [create if required].</param>
        /// <returns></returns>
        public static bool TryGet(Type anchorType, out ServiceCacheRegistrar registrar, bool createIfRequired)
        {
            if (anchorType == null)
                throw new ArgumentNullException("anchorType");
            _rwLock.EnterUpgradeableReadLock();
            try
            {
                var exists = _items.TryGetValue(anchorType, out registrar);
                if (exists || !createIfRequired)
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

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <param name="recurses">The recurses.</param>
        /// <param name="foundRegistration">The found registration.</param>
        /// <returns></returns>
        public static bool TryGetValue(IServiceCacheRegistration registration, ref int recurses, out IServiceCacheRegistration foundRegistration)
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

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="anchorType">Type of the anchor.</param>
        /// <param name="registrationName">Name of the registration.</param>
        /// <param name="recurses">The recurses.</param>
        /// <param name="foundRegistration">The found registration.</param>
        /// <returns></returns>
        public static bool TryGetValue(Type anchorType, string registrationName, ref int recurses, out IServiceCacheRegistration foundRegistration)
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
                        IServiceCacheRegistration registration;
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
