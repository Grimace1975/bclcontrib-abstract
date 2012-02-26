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
using System.Abstract;
using System.Abstract.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using AddOpcode = Contoso.Abstract.MemcachedServiceCache.TagMapper.AddOpcode;
using GetOpcode = Contoso.Abstract.MemcachedServiceCache.TagMapper.GetOpcode;
using SetOpcode = Contoso.Abstract.MemcachedServiceCache.TagMapper.SetOpcode;
using System;
namespace Contoso.Abstract
{
    [TestClass]
    public class DispatcherTests
    {
        [TestMethod]
        public void Append_Should_Be_Called()
        {
            var append = new ArraySegment<byte>();
            var appendCas = new CasResult<ArraySegment<byte>> { Cas = 1, Result = append };

            // Test : bool Append(string key, ArraySegment<byte> data);
            var clientMock = new Mock<IMemcachedClient>();
            var service = new MemcachedServiceCache(clientMock.Object);
            service.Add(append, "name", CacheItemPolicy.Infinite, "value");
            clientMock.Verify(x => x.Append("name", It.IsAny<ArraySegment<byte>>()));

            // Test : CasResult<bool> Append(string key, ulong cas, ArraySegment<byte> data);
            var clientMock2 = new Mock<IMemcachedClient>();
            var service2 = new MemcachedServiceCache(clientMock2.Object);
            service2.Add(appendCas, "name", CacheItemPolicy.Infinite, "value");
            clientMock2.Verify(x => x.Append("name", appendCas.Cas, It.IsAny<ArraySegment<byte>>()));
        }

        [TestMethod]
        public void Cas_Should_Be_Called()
        {
            var validFor = new TimeSpan(0, 0, 1);
            var expiresAt = new DateTime(3000, 1, 1);
            var cas = new CasResult<object> { Cas = 1 };

            // Test : CasResult<bool> Cas(StoreMode mode, string key, object value);
            // covered by 0L for cas

            // Test : CasResult<bool> Cas(StoreMode mode, string key, object value, ulong cas);
            var clientMock2 = new Mock<IMemcachedClient>();
            var service2 = new MemcachedServiceCache(clientMock2.Object);
            service2.Add(cas, "name", CacheItemPolicy.Infinite, "value");
            clientMock2.Verify(x => x.Cas(It.IsAny<StoreMode>(), "name", "value", cas.Cas));
            //
            var clientMock2A = new Mock<IMemcachedClient>();
            var service2A = new MemcachedServiceCache(clientMock2A.Object);
            service2A.Set(cas, "name", CacheItemPolicy.Infinite, "value");
            clientMock2A.Verify(x => x.Cas(It.IsAny<StoreMode>(), "name", "value", cas.Cas));

            // Test : CasResult<bool> Cas(StoreMode mode, string key, object value, DateTime expiresAt, ulong cas);
            var clientMock3 = new Mock<IMemcachedClient>();
            var service3 = new MemcachedServiceCache(clientMock3.Object);
            service3.Add(cas, "name", new CacheItemPolicy { AbsoluteExpiration = expiresAt }, "value");
            clientMock3.Verify(x => x.Cas(It.IsAny<StoreMode>(), "name", "value", expiresAt, cas.Cas));
            //
            var clientMock3A = new Mock<IMemcachedClient>();
            var service3A = new MemcachedServiceCache(clientMock3A.Object);
            service3A.Set(cas, "name", new CacheItemPolicy { AbsoluteExpiration = expiresAt }, "value");
            clientMock3A.Verify(x => x.Cas(It.IsAny<StoreMode>(), "name", "value", expiresAt, cas.Cas));

            // Test : CasResult<bool> Cas(StoreMode mode, string key, object value, TimeSpan validFor, ulong cas);
            var clientMock4 = new Mock<IMemcachedClient>();
            var service4 = new MemcachedServiceCache(clientMock4.Object);
            service4.Add(cas, "name", new CacheItemPolicy { SlidingExpiration = validFor }, "value");
            clientMock4.Verify(x => x.Cas(It.IsAny<StoreMode>(), "name", "value", validFor, cas.Cas));
            //
            var clientMock4A = new Mock<IMemcachedClient>();
            var service4A = new MemcachedServiceCache(clientMock4A.Object);
            service4A.Set(cas, "name", new CacheItemPolicy { SlidingExpiration = validFor }, "value");
            clientMock4A.Verify(x => x.Cas(It.IsAny<StoreMode>(), "name", "value", validFor, cas.Cas));
        }

        [TestMethod]
        public void Decrement_Should_Be_Called()
        {
            var validFor = new TimeSpan(0, 0, 1);
            var expiresAt = new DateTime(3000, 1, 1);
            var decrement = new MemcachedServiceCache.DecrementTag { DefaultValue = 1, Delta = 2 };
            var decrementCas = new CasResult<MemcachedServiceCache.DecrementTag> { Cas = 1, Result = decrement };

            // Test : ulong Decrement(string key, ulong defaultValue, ulong delta);
            var clientMock = new Mock<IMemcachedClient>();
            var service = new MemcachedServiceCache(clientMock.Object);
            service.Set(decrement, "name", CacheItemPolicy.Infinite, "value");
            clientMock.Verify(x => x.Decrement("name", decrement.DefaultValue, decrement.Delta));

            // Test : ulong Decrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt);
            var clientMock2 = new Mock<IMemcachedClient>();
            var service2 = new MemcachedServiceCache(clientMock2.Object);
            service2.Set(decrement, "name", new CacheItemPolicy { AbsoluteExpiration = expiresAt }, "value");
            clientMock2.Verify(x => x.Decrement("name", decrement.DefaultValue, decrement.Delta, expiresAt));

            // Test : ulong Decrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor);
            var clientMock3 = new Mock<IMemcachedClient>();
            var service3 = new MemcachedServiceCache(clientMock3.Object);
            service3.Set(decrement, "name", new CacheItemPolicy { SlidingExpiration = validFor }, "value");
            clientMock3.Verify(x => x.Decrement("name", decrement.DefaultValue, decrement.Delta, validFor));

            // Test : CasResult<ulong> Decrement(string key, ulong defaultValue, ulong delta, ulong cas);
            var clientMock4 = new Mock<IMemcachedClient>();
            var service4 = new MemcachedServiceCache(clientMock4.Object);
            service4.Set(decrementCas, "name", CacheItemPolicy.Infinite, "value");
            clientMock4.Verify(x => x.Decrement("name", decrement.DefaultValue, decrement.Delta, decrementCas.Cas));

            // Test : CasResult<ulong> Decrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas);
            var clientMock5 = new Mock<IMemcachedClient>();
            var service5 = new MemcachedServiceCache(clientMock5.Object);
            service5.Set(decrementCas, "name", new CacheItemPolicy { AbsoluteExpiration = expiresAt }, "value");
            clientMock5.Verify(x => x.Decrement("name", decrement.DefaultValue, decrement.Delta, expiresAt, decrementCas.Cas));

            // Test : CasResult<ulong> Decrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas);
            var clientMock6 = new Mock<IMemcachedClient>();
            var service6 = new MemcachedServiceCache(clientMock6.Object);
            service6.Set(decrementCas, "name", new CacheItemPolicy { SlidingExpiration = validFor }, "value");
            clientMock6.Verify(x => x.Decrement("name", decrement.DefaultValue, decrement.Delta, validFor, decrementCas.Cas));
        }

        [TestMethod]
        public void FlushAll_Should_Be_Called()
        {
            // Test : FlushAll();
            var clientMock = new Mock<IMemcachedClient>();
            var service = new MemcachedServiceCache(clientMock.Object, null);
            service.FlushAll();
            clientMock.Verify(x => x.FlushAll());
        }

        [TestMethod]
        public void Get_Should_Be_Called()
        {
            var names = new[] { "name" };

            // Test : IDictionary<string, object> Get(IEnumerable<string> keys);
            var clientMock = new Mock<IMemcachedClient>();
            var service = new MemcachedServiceCache(clientMock.Object);
            service.Get("Get", names);
            clientMock.Verify(x => x.Get(names));

            // Test : object Get(string key);
            var clientMock2 = new Mock<IMemcachedClient>();
            var service2 = new MemcachedServiceCache(clientMock2.Object);
            service2.Get(null, "name");
            clientMock2.Verify(x => x.Get("name"));

            // Test : IDictionary<string, CasResult<object>> GetWithCas(IEnumerable<string> keys);
            var clientMock3 = new Mock<IMemcachedClient>();
            var service3 = new MemcachedServiceCache(clientMock3.Object);
            service3.GetWithCas(names);
            clientMock3.Verify(x => x.GetWithCas(names));

            // Test : CasResult<T> GetWithCas<T>(string key);
            var clientMock4 = new Mock<IMemcachedClient>();
            var service4 = new MemcachedServiceCache(clientMock4.Object);
            service4.GetWithCas(names);
            clientMock4.Verify(x => x.GetWithCas(names));

            // Test : CasResult<object> GetWithCas(string key);
            var clientMock5 = new Mock<IMemcachedClient>();
            var service5 = new MemcachedServiceCache(clientMock5.Object, null);
            service5.GetWithCas(names);
            clientMock5.Verify(x => x.GetWithCas(names));

            // Test : bool TryGet(string key, out object value);
            object value6;
            var clientMock6 = new Mock<IMemcachedClient>();
            var service6 = new MemcachedServiceCache(clientMock6.Object, null);
            service6.TryGet(null, "name", out value6);
            clientMock6.Verify(x => x.TryGet("name", out value6));

            // Test : bool TryGetWithCas(string key, out CasResult<object> value);
            CasResult<object> value7;
            var clientMock7 = new Mock<IMemcachedClient>();
            var service7 = new MemcachedServiceCache(clientMock7.Object, null);
            service7.TryGetWithCas("name", out value7);
            clientMock7.Verify(x => x.TryGetWithCas("name", out value7));
        }

        [TestMethod]
        public void Increment_Should_Be_Called()
        {
            var validFor = new TimeSpan(0, 0, 1);
            var expiresAt = new DateTime(3000, 1, 1);
            var increment = new MemcachedServiceCache.IncrementTag { DefaultValue = 1, Delta = 2 };
            var incrementCas = new CasResult<MemcachedServiceCache.IncrementTag> { Cas = 1, Result = increment };

            // Test : ulong Increment(string key, ulong defaultValue, ulong delta);
            var clientMock = new Mock<IMemcachedClient>();
            var service = new MemcachedServiceCache(clientMock.Object);
            service.Set(increment, "name", CacheItemPolicy.Infinite, "value");
            clientMock.Verify(x => x.Increment("name", increment.DefaultValue, increment.Delta));

            // Test : ulong Increment(string key, ulong defaultValue, ulong delta, DateTime expiresAt);
            var clientMock2 = new Mock<IMemcachedClient>();
            var service2 = new MemcachedServiceCache(clientMock2.Object);
            service2.Set(increment, "name", new CacheItemPolicy { AbsoluteExpiration = expiresAt }, "value");
            clientMock2.Verify(x => x.Increment("name", increment.DefaultValue, increment.Delta, expiresAt));

            // Test : ulong Increment(string key, ulong defaultValue, ulong delta, TimeSpan validFor);
            var clientMock3 = new Mock<IMemcachedClient>();
            var service3 = new MemcachedServiceCache(clientMock3.Object);
            service3.Set(increment, "name", new CacheItemPolicy { SlidingExpiration = validFor }, "value");
            clientMock3.Verify(x => x.Increment("name", increment.DefaultValue, increment.Delta, validFor));

            // Test : CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, ulong cas);
            var clientMock4 = new Mock<IMemcachedClient>();
            var service4 = new MemcachedServiceCache(clientMock4.Object);
            service4.Set(incrementCas, "name", CacheItemPolicy.Infinite, "value");
            clientMock4.Verify(x => x.Increment("name", increment.DefaultValue, increment.Delta, incrementCas.Cas));

            // Test : CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas);
            var clientMock5 = new Mock<IMemcachedClient>();
            var service5 = new MemcachedServiceCache(clientMock5.Object);
            service5.Set(incrementCas, "name", new CacheItemPolicy { AbsoluteExpiration = expiresAt }, "value");
            clientMock5.Verify(x => x.Increment("name", increment.DefaultValue, increment.Delta, expiresAt, incrementCas.Cas));

            // Test : CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas);
            var clientMock6 = new Mock<IMemcachedClient>();
            var service6 = new MemcachedServiceCache(clientMock6.Object);
            service6.Set(incrementCas, "name", new CacheItemPolicy { SlidingExpiration = validFor }, "value");
            clientMock6.Verify(x => x.Increment("name", increment.DefaultValue, increment.Delta, validFor, incrementCas.Cas));
        }

        [TestMethod]
        public void Prepend_Should_Be_Called()
        {
            var prepend = new ArraySegment<byte>();
            var prependCas = new CasResult<ArraySegment<byte>> { Cas = 1, Result = prepend };

            // Test : bool Prepend(string key, ArraySegment<byte> data);
            var clientMock = new Mock<IMemcachedClient>();
            var service = new MemcachedServiceCache(clientMock.Object); //, setTagMapperMock.Object);
            service.Set(prepend, "name", CacheItemPolicy.Infinite, "value");
            clientMock.Verify(x => x.Prepend("name", It.IsAny<ArraySegment<byte>>()));

            // Test : CasResult<bool> Prepend(string key, ulong cas, ArraySegment<byte> data);
            var clientMock2 = new Mock<IMemcachedClient>();
            var service2 = new MemcachedServiceCache(clientMock2.Object); //, setTagMapperMock.Object);
            service2.Set(prependCas, "name", CacheItemPolicy.Infinite, "value");
            clientMock2.Verify(x => x.Prepend("name", prependCas.Cas, It.IsAny<ArraySegment<byte>>()));
        }

        [TestMethod]
        public void Remove_Should_Be_Called()
        {
            // Test : bool Remove(string key);
            var clientMock = new Mock<IMemcachedClient>();
            var service = new MemcachedServiceCache(clientMock.Object, null);
            service.Remove("name");
            clientMock.Verify(x => x.Remove("name"));
        }

        [TestMethod]
        public void Store_Should_Be_Called()
        {
            var validFor = new TimeSpan(0, 0, 1);
            var expiresAt = new DateTime(3000, 1, 1);
  
            // Test : bool Store(StoreMode mode, string key, object value);
            var clientMock = new Mock<IMemcachedClient>();
            var service = new MemcachedServiceCache(clientMock.Object);
            service.Add(null, "name", CacheItemPolicy.Infinite, "value");
            clientMock.Verify(x => x.Store(It.IsAny<StoreMode>(), "name", "value"));
            //
            var clientMockA = new Mock<IMemcachedClient>();
            var serviceA = new MemcachedServiceCache(clientMockA.Object);
            serviceA.Set(null, "name", CacheItemPolicy.Infinite, "value");
            clientMockA.Verify(x => x.Store(It.IsAny<StoreMode>(), "name", "value"));

            // Test : bool Store(StoreMode mode, string key, object value, DateTime expiresAt);
            var clientMock2 = new Mock<IMemcachedClient>();
            var service2 = new MemcachedServiceCache(clientMock2.Object);
            service2.Add(null, "name", new CacheItemPolicy { AbsoluteExpiration = expiresAt }, "value");
            clientMock2.Verify(x => x.Store(It.IsAny<StoreMode>(), "name", "value", expiresAt));
            //
            var clientMock2A = new Mock<IMemcachedClient>();
            var service2A = new MemcachedServiceCache(clientMock2A.Object);
            service2A.Set(null, "name", new CacheItemPolicy { AbsoluteExpiration = expiresAt }, "value");
            clientMock2A.Verify(x => x.Store(It.IsAny<StoreMode>(), "name", "value", expiresAt));

            // Test : bool Store(StoreMode mode, string key, object value, TimeSpan validFor);
            var clientMock3 = new Mock<IMemcachedClient>();
            var service3 = new MemcachedServiceCache(clientMock3.Object);
            service3.Add(null, "name", new CacheItemPolicy { SlidingExpiration = validFor }, "value");
            clientMock3.Verify(x => x.Store(It.IsAny<StoreMode>(), "name", "value", validFor));
            //
            var clientMock3A = new Mock<IMemcachedClient>();
            var service3A = new MemcachedServiceCache(clientMock3A.Object);
            service3A.Set(null, "name", new CacheItemPolicy { SlidingExpiration = validFor }, "value");
            clientMock3A.Verify(x => x.Store(It.IsAny<StoreMode>(), "name", "value", validFor));
        }
    }
}

