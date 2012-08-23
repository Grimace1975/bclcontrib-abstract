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
using System.Abstract;
using System.Abstract.EventSourcing;
using Contoso.Abstract.EventSourcing;
using EventStore;
using System.Abstract.Parts;
namespace Contoso.Abstract
{
    /// <summary>
    /// IESEventSource
    /// </summary>
    public interface IESEventSource : IEventSource { }

    /// <summary>
    /// ESEventSource
    /// </summary>
    public class ESEventSource : IESEventSource, EventSourceManager.ISetupRegistration
    {
        private readonly IStoreEvents _store;

        static ESEventSource() { EventSourceManager.EnsureRegistration(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="ESEventSource"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        public ESEventSource(IStoreEvents store)
        {
            _store = store;
        }

        Action<IServiceLocator, string> EventSourceManager.ISetupRegistration.DefaultServiceRegistrar
        {
            get { return (locator, name) => EventSourceManager.RegisterInstance<IESEventSource>(this, locator, name); }
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.
        /// -or-
        /// null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public object GetService(Type serviceType) { throw new NotImplementedException(); }

        /// <summary>
        /// Makes the repository.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg">The arg.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        public IAggregateRootRepository MakeRepository<T>(T arg, ITypeSerializer serializer) { return new AggregateRootRepository(new ESEventStore(_store), new ESAggregateRootSnapshotStore(_store)); }
    }
}
