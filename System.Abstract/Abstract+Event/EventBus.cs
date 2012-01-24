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
using System.Collections.Generic;
namespace System.Abstract
{
    /// <summary>
    /// IEventBus
    /// </summary>
    public interface IEventBus : IServiceBus { }

    /// <summary>
    /// EventBus
    /// </summary>
    public struct EventBus : IEventBus
    {
        private IServiceBus _parent;

        public EventBus(IServiceBus parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            _parent = parent;
        }
        public object GetService(Type serviceType) { return _parent.GetService(serviceType); }
        public TMessage CreateMessage<TMessage>(Action<TMessage> messageBuilder)
            where TMessage : class, IServiceMessage { return _parent.CreateMessage(messageBuilder); }
        public IServiceBusCallback Send(IServiceBusEndpoint destination, params IServiceMessage[] messages) { return _parent.Send(destination, messages); }
        public void Reply(params IServiceMessage[] messages) { _parent.Send(messages); }
    }
}
