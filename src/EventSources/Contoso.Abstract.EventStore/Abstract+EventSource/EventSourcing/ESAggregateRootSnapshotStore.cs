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
using EventStore;
namespace System.Abstract.EventSourcing
{
    public class ESAggregateRootSnapshotStore : IAggregateRootSnapshotStore
    {
        private readonly IStoreEvents _store;

        public ESAggregateRootSnapshotStore(IStoreEvents store)
        {
            if (store == null)
                throw new ArgumentNullException("store");
            _store = store;
        }

        public AggregateRootSnapshot GetLatestSnapshot<TAggregateRoot>(object aggregateID)
            where TAggregateRoot : AggregateRoot
        {
            var streamID = (Guid)aggregateID;
            var latestSnapshot = _store.Advanced.GetSnapshot(streamID, int.MaxValue);
            return (latestSnapshot == null ? null : latestSnapshot.Payload as AggregateRootSnapshot);
        }

        public void SaveSnapshot(Type aggregateType, AggregateRootSnapshot snapshot)
        {
            var streamID = (Guid)snapshot.AggregateID;
            _store.Advanced.AddSnapshot(new Snapshot(streamID, snapshot.LastEventSequence, snapshot));
        }

        public Func<IAggregateRootRepository, AggregateRoot, bool> InlineSnapshotPredicate { get; set; }
    }
}