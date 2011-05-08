//#region License
///*
//The MIT License

//Copyright (c) 2008 Sky Morey

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
//*/
//#endregion
//using System;
//using System.Abstract;
//using NServiceBus;
//using System.Collections.Generic;
//namespace Contoso.Abstract.Internal
//{
//    /// <summary>
//    /// BusWrapper
//    /// </summary>
//    internal abstract class BusWrapper : IBus
//    {
//        private IBus _bus;

//        public BusWrapper(IBus bus)
//        {
//            _bus = bus;
//        }

//        public IMessageContext CurrentMessageContext
//        {
//            get { return _bus.CurrentMessageContext; }
//        }
//        public void DoNotContinueDispatchingCurrentMessageToHandlers() { _bus.DoNotContinueDispatchingCurrentMessageToHandlers(); }
//        public void ForwardCurrentMessageTo(string destination) { _bus.ForwardCurrentMessageTo(destination); }
//        public void HandleCurrentMessageLater() { _bus.HandleCurrentMessageLater(); }
//        public IDictionary<string, string> OutgoingHeaders
//        {
//            get { return _bus.OutgoingHeaders; }
//        }
//        public void Publish<T>(Action<T> messageConstructor) where T : IMessage { _bus.Publish<T>(messageConstructor); }
//        public void Publish<T>(params T[] messages) where T : IMessage { _bus.Publish<T>(messages); }
//        public void Reply<T>(Action<T> messageConstructor) where T : IMessage { _bus.Reply<T>(messageConstructor); }
//        public void Reply(params IMessage[] messages) { _bus.Reply(messages); }
//        public void Return(int errorCode) { _bus.Return(errorCode); }
//        public void Send<T>(string destination, string correlationId, Action<T> messageConstructor) where T : IMessage { _bus.Send<T>(destination, correlationId, messageConstructor); }
//        public void Send(string destination, string correlationId, params IMessage[] messages) { _bus.Send(destination, correlationId, messages); }
//        public ICallback Send<T>(string destination, Action<T> messageConstructor) where T : IMessage { return _bus.Send<T>(destination, messageConstructor); }
//        public ICallback Send(string destination, params IMessage[] messages) { return _bus.Send(destination, messages); }
//        public ICallback Send<T>(Action<T> messageConstructor) where T : IMessage { return _bus.Send<T>(messageConstructor); }
//        public ICallback Send(params IMessage[] messages) { return _bus.Send(messages); }
//        public void SendLocal<T>(Action<T> messageConstructor) where T : IMessage { _bus.SendLocal<T>(messageConstructor); }
//        public void SendLocal(params IMessage[] messages) { _bus.SendLocal(messages); }
//        public void Subscribe<T>(Predicate<T> condition) where T : IMessage { _bus.Subscribe<T>(condition); }
//        public void Subscribe(Type messageType, Predicate<IMessage> condition) { _bus.Subscribe(messageType, condition); }
//        public void Subscribe<T>() where T : IMessage { _bus.Subscribe<T>(); }
//        public void Subscribe(Type messageType) { _bus.Subscribe(messageType); }
//        public void Unsubscribe<T>() where T : IMessage { _bus.Unsubscribe<T>(); }
//        public void Unsubscribe(Type messageType) { _bus.Unsubscribe(messageType); }
//        public object CreateInstance(Type messageType) { return _bus.CreateInstance(messageType); }
//        public T CreateInstance<T>(Action<T> action) where T : IMessage { return _bus.CreateInstance<T>(action); }
//        public T CreateInstance<T>() where T : IMessage { return _bus.CreateInstance<T>(); }
//    }
//}
