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
using System.Abstract.EventSourcing;
using System.Collections.Generic;
using EventStore;
namespace Contoso.Abstract.EventSourcing
{
    /// <summary>
    /// ESEventStore
    /// </summary>
    public class ESEventStore : IEventStore
    {
        private readonly IStoreEvents _store;

        /// <summary>
        /// Initializes a new instance of the <see cref="ESEventStore"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        public ESEventStore(IStoreEvents store)
        {
            if (store == null)
                throw new ArgumentNullException("store");
            _store = store;
        }

        /// <summary>
        /// Gets the events by ID.
        /// </summary>
        /// <param name="aggregateID">The aggregate ID.</param>
        /// <param name="startSequence">The start sequence.</param>
        /// <returns></returns>
        public IEnumerable<Event> GetEventsByID(object aggregateID, int startSequence)
        {
            var streamID = (Guid)aggregateID;
            using (var stream = _store.OpenStream(streamID, startSequence, int.MaxValue))
            {
                foreach (var committedEvent in stream.CommittedEvents)
                    yield return (Event)committedEvent.Body;
            }
        }

        /// <summary>
        /// Saves the events.
        /// </summary>
        /// <param name="aggregateID">The aggregate ID.</param>
        /// <param name="events">The events.</param>
        public void SaveEvents(object aggregateID, IEnumerable<Event> events)
        {
            var streamID = (Guid)aggregateID;
            using (var stream = _store.OpenStream(streamID, int.MinValue, int.MaxValue))
            {
                foreach (var @event in events)
                    stream.Add(new EventMessage { Body = @event });
                stream.CommitChanges(Guid.NewGuid());
            }
        }
    }
}