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
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace System.Abstract.Caching
{
    /// <summary>
    /// DataCacheRegistrar
    /// </summary>
    public class DataCacheRegistrar
    {
        private static ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private static Dictionary<Type, DataCacheRegistrar> _items = new Dictionary<Type, DataCacheRegistrar>();
        private ReaderWriterLockSlim _setRwLock = new ReaderWriterLockSlim();
        private HashSet<DataCacheRegistration> _set = new HashSet<DataCacheRegistration>();
        private Dictionary<string, DataCacheRegistration> _setAsID = new Dictionary<string, DataCacheRegistration>();
        private string _idPrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceRegistrar"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        internal DataCacheRegistrar(Type anchorType)
        {
            _idPrefix = "DC::" + anchorType.ToString() + "::";
            AnchorType = anchorType;
        }

        /// <summary>
        /// Gets or sets the type of the anchor.
        /// </summary>
        /// <value>The type of the anchor.</value>
        public Type AnchorType { get; private set; }

        /// <summary>
        /// Adds the data source.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="dependencies">The dependencies.</param>
        public void Register(string registrationId, DataCacheBuilder builder, params string[] dependencies) { Register(new DataCacheRegistration(registrationId, new ServiceCacheCommand(null, 60), builder, dependencies)); }
        /// <summary>
        /// Adds the data source.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="minuteTimeout">The minute timeout.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="dependencies">The dependencies.</param>
        public void Register(string registrationId, int minuteTimeout, DataCacheBuilder builder, params string[] dependencies) { Register(new DataCacheRegistration(registrationId, new ServiceCacheCommand(null, minuteTimeout), builder, dependencies)); }
        /// <summary>
        /// Adds the data source.
        /// </summary>
        /// <param name="cacheCommand">The cache command.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="dependencies">The dependencies.</param>
        public void Register(string registrationId, ServiceCacheCommand cacheCommand, DataCacheBuilder builder, params string[] dependencies) { Register(new DataCacheRegistration(registrationId, cacheCommand, builder, dependencies)); }
        /// <summary>
        /// Adds the data source.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Register(DataCacheRegistration registration)
        {
            if (registration == null)
                throw new ArgumentNullException("registration");
            _setRwLock.EnterWriteLock();
            try
            {
                if (_set.Contains(registration))
                    throw new InvalidOperationException(string.Format(Local.RedefineDataCacheAB, AnchorType.ToString(), registration.ID));
                // add
                string registrationID = registration.ID;
                if (string.IsNullOrEmpty(registrationID))
                    throw new ArgumentNullException("registration.ID");
				if (registrationID.IndexOf("::") > -1)
					throw new ArgumentException(string.Format(Local.ScopeCharacterNotAllowedA, registrationID), "registration");
                if (_setAsID.ContainsKey(registrationID))
					throw new ArgumentException(string.Format(Local.RedefineIDA, registrationID), "registration");
                _setAsID.Add(registrationID, registration);
                _set.Add(registration);
                registration.Registrar = this;
                // adjust cache-command
				registration.CacheCommand.Name = _idPrefix + registrationID;
            }
            finally { _setRwLock.ExitWriteLock(); }
        }

        /// <summary>
        /// Clears this the underlying Hash class instance.
        /// </summary>
        public void Clear()
        {
            _setRwLock.EnterWriteLock();
            try
            {
                _setAsID.Clear();
                _set.Clear();
            }
            finally { _setRwLock.ExitWriteLock(); }
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
		/// <param name="registration">The registration.</param>
        /// <returns>
        /// 	<c>true</c> if the specified key contains key; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(DataCacheRegistration registration)
        {
            _setRwLock.EnterReadLock();
            try { return _set.Contains(registration); }
            finally { _setRwLock.ExitReadLock(); }
        }
        /// <summary>
        /// Determines whether [contains] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string ID)
        {
            _setRwLock.EnterReadLock();
			try { return _setAsID.ContainsKey(ID); }
            finally { _setRwLock.ExitReadLock(); }
        }

        /// <summary>
		/// Removes the specified registration.
        /// </summary>
		/// <param name="registration">The registration.</param>
        /// <returns></returns>
        public bool Remove(DataCacheRegistration registration)
        {
            _setRwLock.EnterWriteLock();
            try
            {
                _setAsID.Remove(registration.ID);
                return _set.Remove(registration);
            }
            finally { _setRwLock.ExitWriteLock(); }
        }
        /// <summary>
        /// Removes the specified registration.
        /// </summary>
		/// <param name="registrationID">The registration ID.</param>
        /// <returns></returns>
        public bool Remove(string registrationID)
        {
            _setRwLock.EnterWriteLock();
            try
            {
                var registration = _setAsID[registrationID];
                _setAsID.Remove(registrationID);
                return _set.Remove(registration);
            }
            finally { _setRwLock.ExitWriteLock(); }
        }

        /// <summary>
        /// Creates the data source.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="headerId">The header id.</param>
        /// <param name="values">The value array.</param>
        /// <returns></returns>
        internal static T CreateData<T>(DataCacheRegistration registration, string tag, object[] values)
        {
            if (registration is DataCacheForeignRegistration)
                throw new InvalidOperationException(Local.InvalidDataSource);
            // append header-list
            var tags = registration.Tags;
            if (!tags.Contains(tag))
                tags.Add(tag);
            // build data-source
            return (T)registration.Builder(tag, values);
        }

        /// <summary>
        /// Gets the registrations.
        /// </summary>
        /// <value>The registrations.</value>
        internal HashSet<DataCacheRegistration> Registrations
        {
            get { return _set; }
        }


		/// <summary>
		/// Tries the get instance.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="registrar">The registrar.</param>
		/// <returns></returns>
		internal static bool TryGetInstance(Type anchorType, out DataCacheRegistrar registrar, bool createIfRequired)
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
						registrar = new DataCacheRegistrar(anchorType);
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
        /// <param name="key">The key.</param>
        /// <param name="recurses">The recurses.</param>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        //: BIND: TryGetValue(System.Type, string, int, DataSourceRegistration)
        internal static bool TryGetValue(DataCacheRegistration registration, ref int recurses, out DataCacheRegistration foundRegistration)
        {
            _rwLock.EnterReadLock();
            try
            {
                var registrar = registration.Registrar;
                if (registrar != null)
                {
                    // local check
					var foreignRegistration = (registration as DataCacheForeignRegistration);
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
                    return TryGetValue(foreignType, foreignRegistration.ForeignID, ref recurses, out foundRegistration);
                }
                foundRegistration = null;
                return false;
            }
            finally { _rwLock.ExitReadLock(); }
        }
        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="key">The key.</param>
        /// <param name="recurses">The recurses.</param>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        //:BIND: TryGetValue(DataSourceRegistration, int, DataSourceRegistration)
        internal static bool TryGetValue(Type type, string registrationID, ref int recurses, out DataCacheRegistration foundRegistration)
        {
            _rwLock.EnterReadLock();
            try
            {
                DataCacheRegistrar registrar;
                if (_items.TryGetValue(type, out registrar))
                {
                    // registration locals
                    var setRwLock = registrar._setRwLock;
                    var setAsId = registrar._setAsID;
                    setRwLock.EnterReadLock();
                    try
                    {
                        DataCacheRegistration registration;
                        if (setAsId.TryGetValue(registrationID, out registration))
                        {
                            // local check
							var foreignRegistration = (registration as DataCacheForeignRegistration);
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
                            return TryGetValue(foreignType, foreignRegistration.ForeignID, ref recurses, out foundRegistration);
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
#endif