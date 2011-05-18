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
using System;
using System.Linq;
using System.Abstract;
using System.Collections.Generic;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
// https://github.com/enyim/EnyimMemcached/wiki/MemcachedClient-Usage
namespace Contoso.Abstract
{
	/// <summary>
	/// IMemcachedServiceCache
	/// </summary>
	public interface IMemcachedServiceCache : IServiceCache
	{
		MemcachedClient Cache { get; }
		void FlushAll();
		IDictionary<string, CasResult<object>> GetWithCas(IEnumerable<string> keys);
		CasResult<object> GetWithCas(string key);
		CasResult<T> GetWithCas<T>(string key);
		bool TryGetWithCas(string key, out CasResult<object> value);
	}

	/// <summary>
	/// MemcachedServiceCache
	/// </summary>
	public class MemcachedServiceCache : IMemcachedServiceCache, IDisposable
	{
		#region Opcode Decode

		public struct IncrementTag
		{
			public ulong DefaultValue;
			public ulong Delta;
		}

		public struct DecrementTag
		{
			public ulong DefaultValue;
			public ulong Delta;
		}

		private enum Opcode
		{
			Append,
			AppendCas,
			Prepend,
			PrependCas,
			Store,
			CasPlain,
			Cas,
			Decrement,
			DecrementCas,
			Increment,
			IncrementCas
		}

		private static Opcode GetAddOpcode(object tag, out ulong cas)
		{
			cas = 0;
			return Opcode.Store;
		}

		private static Opcode GetInsertOpcode(object tag, ref string name, out ulong cas, out object opvalue, out StoreMode storeMode)
		{
			cas = 0;
			opvalue = null;
			storeMode = StoreMode.Replace;
			return Opcode.Store;
		}

		#endregion

		public MemcachedServiceCache()
			: this((IMemcachedClientConfiguration)null) { }
		public MemcachedServiceCache(IMemcachedClientConfiguration configuration)
			: this(configuration == null ? new MemcachedClient() : new MemcachedClient(configuration)) { }
		public MemcachedServiceCache(MemcachedClient client)
		{
			if (client == null)
				throw new ArgumentNullException("client");
			Cache = client;
			RegistrationDispatch = new DefaultServiceCacheRegistrationDispatcher();
		}
		~MemcachedServiceCache()
		{
			try { ((IDisposable)this).Dispose(); }
			catch { }
		}

		public void Dispose()
		{
			if (Cache != null)
			{
				GC.SuppressFinalize(this);
				try { Cache.Dispose(); }
				finally { Cache = null; }
			}
		}

		public object this[string name]
		{
			get { return Get(null, name); }
			set { Set(null, name, CacheItemPolicy.Default, value); }
		}

		public object Add(object tag, string name, CacheItemPolicy itemPolicy, object value)
		{
			if (itemPolicy == null)
				throw new ArgumentNullException("itemPolicy");
			var updateCallback = itemPolicy.UpdateCallback;
			if (updateCallback != null)
				updateCallback(name, value);
			//
			var absoluteExpiration = itemPolicy.AbsoluteExpiration;
			var slidingExpiration = itemPolicy.SlidingExpiration;
			ulong cas;
			if ((absoluteExpiration == ServiceCache.InfiniteAbsoluteExpiration) && (slidingExpiration == ServiceCache.NoSlidingExpiration))
				switch (GetAddOpcode(tag, out cas))
				{
					case Opcode.CasPlain: return Cache.Cas(StoreMode.Add, name, value);
					case Opcode.Store: return Cache.Store(StoreMode.Add, name, value);
					case Opcode.Cas: return Cache.Cas(StoreMode.Add, name, value, cas);
					default: throw new InvalidOperationException();
				}
			if ((absoluteExpiration != ServiceCache.InfiniteAbsoluteExpiration) && (slidingExpiration == ServiceCache.NoSlidingExpiration))
				switch (GetAddOpcode(tag, out cas))
				{
					case Opcode.Store: return Cache.Store(StoreMode.Add, name, value, absoluteExpiration);
					case Opcode.Cas: return Cache.Cas(StoreMode.Add, name, value, absoluteExpiration, cas);
					default: throw new InvalidOperationException();
				}
			if ((absoluteExpiration == ServiceCache.InfiniteAbsoluteExpiration) && (slidingExpiration != ServiceCache.NoSlidingExpiration))
				switch (GetAddOpcode(tag, out cas))
				{
					case Opcode.Store: return Cache.Store(StoreMode.Add, name, value, slidingExpiration);
					case Opcode.Cas: return Cache.Cas(StoreMode.Add, name, value, slidingExpiration, cas);
					default: throw new InvalidOperationException();
				}
			throw new InvalidOperationException("absoluteExpiration && slidingExpiration");
		}

		public object Get(object tag, string name) { return Cache.Get(name); }
		public object Get(object tag, IEnumerable<string> names)
		{
			var multiGetTag = (tag as Func<IMultiGetOperation, KeyValuePair<string, CacheItem>, object>);
			if (multiGetTag == null)
				return Cache.Get(names);
			return Cache.PerformMultiGet<object>(names, multiGetTag);
		}
		public bool TryGet(object tag, string name, out object value) { return Cache.TryGet(name, out value); }

		public object Remove(object tag, string name) { return Cache.Remove(name); }

		public object Set(object tag, string name, CacheItemPolicy itemPolicy, object value)
		{
			if (itemPolicy == null)
				throw new ArgumentNullException("itemPolicy");
			var updateCallback = itemPolicy.UpdateCallback;
			if (updateCallback != null)
				updateCallback(name, value);
			//
			var absoluteExpiration = itemPolicy.AbsoluteExpiration;
			var slidingExpiration = itemPolicy.SlidingExpiration;
			ulong cas;
			object opvalue;
			StoreMode storeMode;
			IncrementTag increment;
			DecrementTag decrement;
			if ((absoluteExpiration == ServiceCache.InfiniteAbsoluteExpiration) && (slidingExpiration == ServiceCache.NoSlidingExpiration))
				switch (GetInsertOpcode(tag, ref name, out cas, out opvalue, out storeMode))
				{
					case Opcode.Append: return Cache.Append(name, (ArraySegment<byte>)opvalue);
					case Opcode.AppendCas: return Cache.Append(name, cas, (ArraySegment<byte>)opvalue);
					case Opcode.Prepend: return Cache.Prepend(name, (ArraySegment<byte>)opvalue);
					case Opcode.PrependCas: return Cache.Prepend(name, cas, (ArraySegment<byte>)opvalue);
					case Opcode.CasPlain: return Cache.Cas(StoreMode.Set, name, value);
					case Opcode.Store: return Cache.Store(StoreMode.Set, name, value);
					case Opcode.Cas: return Cache.Cas(StoreMode.Set, name, value, cas);
					case Opcode.Decrement: decrement = (DecrementTag)opvalue; return Cache.Decrement(name, decrement.DefaultValue, decrement.Delta);
					case Opcode.DecrementCas: decrement = (DecrementTag)opvalue; return Cache.Decrement(name, decrement.DefaultValue, decrement.Delta, cas);
					case Opcode.Increment: increment = (IncrementTag)opvalue; return Cache.Increment(name, increment.DefaultValue, increment.Delta);
					case Opcode.IncrementCas: increment = (IncrementTag)opvalue; return Cache.Increment(name, increment.DefaultValue, increment.Delta, cas);
					default: throw new InvalidOperationException();
				}
			if ((absoluteExpiration != ServiceCache.InfiniteAbsoluteExpiration) && (slidingExpiration == ServiceCache.NoSlidingExpiration))
				switch (GetInsertOpcode(tag, ref name, out cas, out opvalue, out storeMode))
				{
					case Opcode.Append:
					case Opcode.AppendCas:
					case Opcode.Prepend:
					case Opcode.PrependCas:
					case Opcode.CasPlain:
						throw new NotSupportedException("Operation not supported with absoluteExpiration && slidingExpiration");
					case Opcode.Store: return Cache.Store(StoreMode.Set, name, value, absoluteExpiration);
					case Opcode.Cas: return Cache.Cas(StoreMode.Set, name, value, absoluteExpiration, cas);
					case Opcode.Decrement: decrement = (DecrementTag)opvalue; return Cache.Decrement(name, decrement.DefaultValue, decrement.Delta, absoluteExpiration);
					case Opcode.DecrementCas: decrement = (DecrementTag)opvalue; return Cache.Decrement(name, decrement.DefaultValue, decrement.Delta, absoluteExpiration, cas);
					case Opcode.Increment: increment = (IncrementTag)opvalue; return Cache.Increment(name, increment.DefaultValue, increment.Delta, absoluteExpiration);
					case Opcode.IncrementCas: increment = (IncrementTag)opvalue; return Cache.Increment(name, increment.DefaultValue, increment.Delta, absoluteExpiration, cas);
					default: throw new InvalidOperationException();
				}
			if ((absoluteExpiration == ServiceCache.InfiniteAbsoluteExpiration) && (slidingExpiration != ServiceCache.NoSlidingExpiration))
				switch (GetInsertOpcode(tag, ref name, out cas, out opvalue, out storeMode))
				{
					case Opcode.Append:
					case Opcode.AppendCas:
					case Opcode.Prepend:
					case Opcode.PrependCas:
					case Opcode.CasPlain:
						throw new NotSupportedException("Operation not supported with absoluteExpiration && slidingExpiration");
					case Opcode.Store: return Cache.Store(StoreMode.Set, name, value, slidingExpiration);
					case Opcode.Cas: return Cache.Cas(StoreMode.Set, name, value, slidingExpiration, cas);
					case Opcode.Decrement: decrement = (DecrementTag)opvalue; return Cache.Decrement(name, decrement.DefaultValue, decrement.Delta, slidingExpiration);
					case Opcode.DecrementCas: decrement = (DecrementTag)opvalue; return Cache.Decrement(name, decrement.DefaultValue, decrement.Delta, slidingExpiration, cas);
					case Opcode.Increment: increment = (IncrementTag)opvalue; return Cache.Increment(name, increment.DefaultValue, increment.Delta, slidingExpiration);
					case Opcode.IncrementCas: increment = (IncrementTag)opvalue; return Cache.Increment(name, increment.DefaultValue, increment.Delta, slidingExpiration, cas);
					default: throw new InvalidOperationException();
				}
			throw new InvalidOperationException("absoluteExpiration && slidingExpiration");
		}

		public void Touch(object tag, params string[] names)
		{
			throw new NotSupportedException();
		}

		public ServiceCacheSettings Settings
		{
			get { return null; }
		}
		public ServiceCacheRegistration.IDispatch RegistrationDispatch { get; private set; }

		#region Domain-specific

		public MemcachedClient Cache { get; private set; }

		public void FlushAll() { Cache.FlushAll(); }
		public IDictionary<string, CasResult<object>> GetWithCas(IEnumerable<string> keys) { return Cache.GetWithCas(keys); }
		public CasResult<object> GetWithCas(string key) { return Cache.GetWithCas(key); }
		public CasResult<T> GetWithCas<T>(string key) { return Cache.GetWithCas<T>(key); }
		public bool TryGetWithCas(string key, out CasResult<object> value) { return TryGetWithCas(key, out value); }

		#endregion
	}
}
