using System.Collections;
using System;
namespace Rhino.ServiceBus.FileMessaging
{
    public class MessageEnumerator : IEnumerator, IDisposable
    {
        public Message Current
        {
            get { throw new NotImplementedException(); }
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        object IEnumerator.Current
        {
            get { return this.Current; }
        }
    }
}
