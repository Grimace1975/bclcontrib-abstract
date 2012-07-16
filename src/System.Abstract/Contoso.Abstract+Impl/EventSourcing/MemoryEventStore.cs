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
using System.Collections.Generic;
using System.Abstract.EventSourcing;
namespace Contoso.Abstract.EventSourcing
{
    /// <summary>
    /// MemoryEventStore
    /// </summary>
    public class MemoryEventStore : IEventStore
    {
        private readonly List<Event> _events = new List<Event>();

        /// <summary>
        /// Gets the events by ID.
        /// </summary>
        /// <param name="aggregateID">The aggregate ID.</param>
        /// <param name="startSequence">The start sequence.</param>
        /// <returns></returns>
        public IEnumerable<Event> GetEventsByID(object aggregateID, int startSequence)
        {
            return _events
                .Where(x => x.AggregateID.Equals(aggregateID) && x.EventSequence > startSequence)
                .ToList();
        }

        /// <summary>
        /// Gets the events by event types.
        /// </summary>
        /// <param name="eventTypes">The event types.</param>
        /// <returns></returns>
        public IEnumerable<Event> GetEventsByEventTypes(IEnumerable<Type> eventTypes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the events.
        /// </summary>
        /// <param name="aggregateID">The aggregate ID.</param>
        /// <param name="events">The events.</param>
        public void SaveEvents(object aggregateID, IEnumerable<Event> events)
        {
            _events.AddRange(events);
        }
    }
}