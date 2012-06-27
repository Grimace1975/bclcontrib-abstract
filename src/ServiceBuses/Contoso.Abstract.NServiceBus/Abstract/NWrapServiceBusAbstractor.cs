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
using System.Reflection;
using NServiceBus;
namespace Contoso.Abstract
{
    /// <summary>
    /// NWrapServiceBusAbstractor
    /// </summary>
    public class NWrapServiceBusAbstractor : NServiceBusAbstractor
    {
        static NWrapServiceBusAbstractor() { ServiceBusManager.EnsureRegistration(); }
        public NWrapServiceBusAbstractor(params Assembly[] assemblies)
            : base(assemblies) { }
        public NWrapServiceBusAbstractor(IServiceLocator serviceLocator, params Assembly[] assemblies)
            : base(serviceLocator, assemblies) { }
        public NWrapServiceBusAbstractor(IServiceLocator serviceLocator, IStartableBus startableBus)
            : base(serviceLocator, startableBus) { }
        public NWrapServiceBusAbstractor(IServiceLocator serviceLocator, IBus bus)
            : base(serviceLocator, bus) { }

        public override IServiceBusCallback Send(IServiceBusEndpoint endpoint, params object[] messages)
        {
            if (messages == null || messages.Length == 0 || messages[0] == null)
                throw new ArgumentNullException("messages", "Please include at least one message.");
            try
            {
                if (endpoint == null) Bus.Send(NServiceBusTransport.Wrap(messages));
                else if (endpoint != ServiceBus.SelfEndpoint) Bus.Send(NServiceBusTransport.Cast(endpoint), NServiceBusTransport.Wrap(messages));
                else Bus.SendLocal(NServiceBusTransport.Wrap(messages));
            }
            catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
            return null;
        }

        public override void Reply(params object[] messages)
        {
            if (messages == null || messages.Length == 0 || messages[0] == null)
                throw new ArgumentNullException("messages", "Please include at least one message.");
            try { Bus.Reply(NServiceBusTransport.Wrap(messages)); }
            catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
        }

        #region Publishing ServiceBus

        public override void Publish(params object[] messages)
        {
            if (messages == null || messages.Length == 0 || messages[0] == null)
                throw new ArgumentNullException("messages");
            try { Bus.Publish(NServiceBusTransport.Wrap(messages)); }
            catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
        }

        public override void Subscribe(Type messageType, Predicate<object> predicate)
        {
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            try
            {
                if (predicate == null) Bus.Subscribe(NServiceBusTransport.Wrap(messageType));
                else Bus.Subscribe(NServiceBusTransport.Wrap(messageType), NServiceBusTransport.Cast(predicate));
            }
            catch (Exception ex) { throw new ServiceBusMessageException(messageType, ex); }
        }

        public override void Unsubscribe(Type messageType)
        {
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            try { Bus.Unsubscribe(NServiceBusTransport.Wrap(messageType)); }
            catch (Exception ex) { throw new ServiceBusMessageException(messageType, ex); }
        }

        #endregion

        #region Domain-specific

        public override int SendWithReturn(int executeTimeout, IServiceBusEndpoint endpoint, params object[] messages)
        {
            if (messages == null || messages.Length == 0 || messages[0] == null)
                throw new ArgumentNullException("messages", "Please include at least one message.");
            IAsyncResult asyncResult;
            try
            {
                if (endpoint == null) asyncResult = Bus.Send(NServiceBusTransport.Wrap(messages)).Register(state => { }, null);
                if (endpoint != ServiceBus.SelfEndpoint) asyncResult = Bus.Send(NServiceBusTransport.Cast(endpoint), NServiceBusTransport.Wrap(messages)).Register(state => { }, null);
                else throw new NotSupportedException();
            }
            catch (Exception ex) { throw new ServiceBusMessageException(messages[0].GetType(), ex); }
            if (!asyncResult.AsyncWaitHandle.WaitOne(executeTimeout))
                throw new TimeoutException();
            return ((CompletionResult)asyncResult.AsyncState).ErrorCode;
        }

        #endregion
    }
}
