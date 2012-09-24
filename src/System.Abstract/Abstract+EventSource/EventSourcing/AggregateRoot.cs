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
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Threading;
namespace System.Abstract.EventSourcing
{
    /// <summary>
    /// IAggregateRoot
    /// </summary>
    public interface IAggregateRoot
    {
        /// <summary>
        /// Gets the aggregate ID.
        /// </summary>
        object AggregateID { get; }
    }

    /// <summary>
    /// AggregateRoot
    /// </summary>
    public abstract class AggregateRoot : IAggregateRoot, IAggregateRootStateAccessor
    {
        /// <summary>
        /// EmptyEventDispatcher
        /// </summary>
        public static readonly IAggregateRootEventDispatcher EmptyEventDispatcher = new EmptyAggregateRootEventDispatcher();
        private readonly List<Event> _changes = new List<Event>();
        private IAggregateRootEventDispatcher _eventDispatcher;
        private bool _useStorageBasedSequencing;

        private class EmptyAggregateRootEventDispatcher : IAggregateRootEventDispatcher
        {
            public void ApplyEvent(AggregateRoot aggregate, Event e) { }
            public IEnumerable<Type> GetEventTypes() { return null; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
        /// </summary>
        public AggregateRoot() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public AggregateRoot(AggregateRootOptions options)
        {
            _useStorageBasedSequencing = ((options & AggregateRootOptions.UseStorageBasedSequencing) != 0);
        }

        /// <summary>
        /// Gets or sets the aggregate ID.
        /// </summary>
        /// <value>
        /// The aggregate ID.
        /// </value>
        public object AggregateID { get; protected set; }
        /// <summary>
        /// Gets the last event date.
        /// </summary>
        protected internal DateTime LastEventDate { get; private set; }
        /// <summary>
        /// Gets the last event sequence.
        /// </summary>
        protected internal int LastEventSequence { get; private set; }

        /// <summary>
        /// Gets or sets the event dispatcher.
        /// </summary>
        /// <value>
        /// The event dispatcher.
        /// </value>
        protected IAggregateRootEventDispatcher EventDispatcher
        {
            get { return _eventDispatcher; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _eventDispatcher = value;
            }
        }

        /// <summary>
        /// Applies the event.
        /// </summary>
        /// <param name="e">The e.</param>
        protected void ApplyEvent(Event e)
        {
            if (e == null)
                throw new ArgumentNullException("e");
            if (_eventDispatcher == null)
                throw new InvalidOperationException("EventDispatcher must be set first.");
            e.AggregateID = AggregateID;
            e.EventDate = DateTime.Now;
            if (!_useStorageBasedSequencing)
                e.EventSequence = ++LastEventSequence;
            _eventDispatcher.ApplyEvent(this, e);
            _changes.Add(e); // trackAsChange
        }

        /// <summary>
        /// Gets a value indicating whether this instance has changed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has changed; otherwise, <c>false</c>.
        /// </value>
        public bool HasChanged
        {
            get { return (_changes.Count > 0); }
        }

        #region Access State

        bool IAggregateRootStateAccessor.LoadFromHistory(IEnumerable<Event> events)
        {
            if (events == null)
                throw new ArgumentNullException("events");
            if (_eventDispatcher == null)
                throw new InvalidOperationException("EventDispatcher must be set first.");
            Event lastEvent = null;
            foreach (var e in events.OrderBy(x => x.EventSequence))
            {
                _eventDispatcher.ApplyEvent(this, e);
                lastEvent = e;
            }
            if (lastEvent != null)
            {
                LastEventDate = lastEvent.EventDate;
                LastEventSequence = (int)(lastEvent.EventSequence ?? 0);
                return true;
            }
            LastEventDate = DateTime.Now;
            LastEventSequence = 0;
            return false;
        }

        IEnumerable<Event> IAggregateRootStateAccessor.GetUncommittedChanges() { return _changes; }

        void IAggregateRootStateAccessor.MarkChangesAsCommitted() { _changes.Clear(); }

        #endregion
    }
}