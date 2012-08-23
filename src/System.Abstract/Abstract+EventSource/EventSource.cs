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
using System.Abstract.EventSourcing;
using System.Collections.Generic;
using System.Abstract.Parts;
namespace System.Abstract
{
    /// <summary>
    /// IEventSource
    /// </summary>
    public interface IEventSource : IServiceProvider
    {
        /// <summary>
        /// Makes the repository.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg">The arg.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        IAggregateRootRepository MakeRepository<T>(T arg, ITypeSerializer serializer);
    }

    /// <summary>
    /// EventSource
    /// </summary>
    public class EventSource : IEventSource, EventSourceManager.ISetupRegistration
    {
        private readonly IEventStore _eventStore;
        private readonly IAggregateRootSnapshotStore _snapshotStore;
        private readonly Action<IEnumerable<Event>> _eventDispatcher;
        private readonly Func<Type, AggregateRoot> _factory;

        /// <summary>
        /// DefaultFactory
        /// </summary>
        public static class DefaultFactory
        {
            /// <summary>
            /// Factory
            /// </summary>
            public static Func<Type, AggregateRoot> Factory = (t => (AggregateRoot)Activator.CreateInstance(t));
        }

        static EventSource() { EventSourceManager.EnsureRegistration(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="EventSource"/> class.
        /// </summary>
        /// <param name="eventStore">The event store.</param>
        /// <param name="snapshotStore">The snapshot store.</param>
        public EventSource(IEventStore eventStore, IAggregateRootSnapshotStore snapshotStore)
            : this(eventStore, snapshotStore, null, DefaultFactory.Factory) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="EventSource"/> class.
        /// </summary>
        /// <param name="eventStore">The event store.</param>
        /// <param name="snapshotStore">The snapshot store.</param>
        /// <param name="eventDispatcher">The event dispatcher.</param>
        public EventSource(IEventStore eventStore, IAggregateRootSnapshotStore snapshotStore, Action<IEnumerable<Event>> eventDispatcher)
            : this(eventStore, snapshotStore, eventDispatcher, DefaultFactory.Factory) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="EventSource"/> class.
        /// </summary>
        /// <param name="eventStore">The event store.</param>
        /// <param name="snapshotStore">The snapshot store.</param>
        /// <param name="eventDispatcher">The event dispatcher.</param>
        /// <param name="factory">The factory.</param>
        public EventSource(IEventStore eventStore, IAggregateRootSnapshotStore snapshotStore, Action<IEnumerable<Event>> eventDispatcher, Func<Type, AggregateRoot> factory)
        {
            _eventStore = eventStore;
            _snapshotStore = snapshotStore;
            _eventDispatcher = eventDispatcher;
            _factory = factory;
        }

        Action<IServiceLocator, string> EventSourceManager.ISetupRegistration.DefaultServiceRegistrar
        {
            get { return (locator, name) => EventSourceManager.RegisterInstance<IEventSource>(this, locator, name); }
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
        public IAggregateRootRepository MakeRepository<T>(T arg, ITypeSerializer serializer) { return new AggregateRootRepository(_eventStore, _snapshotStore, _eventDispatcher, _factory); }
    }
}
