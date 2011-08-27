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
//using System.Linq;
//using System.Reflection;
//using System.Abstract;
//using NServiceBus;
//namespace Contoso.Abstract
//{
//    internal class MessageCaster<TMessage>
//        where TMessage : IServiceMessage
//    {
//        //private static readonly Type _wrappedType = new DynamicProxyBuilder().CreateProxiedType(typeof(TMessage), new[] { typeof(INServiceMessage) });
//        //private static readonly MethodInfo _publishMessageBuilderMethod = MessageCaster.PublishMessageBuilderMethod.MakeGenericMethod(_wrappedType);
//        //private static readonly MethodInfo _publishMessagesMethod = MessageCaster.PublishMessagesMethod.MakeGenericMethod(_wrappedType);
//        //private static readonly MethodInfo _replyMessageBuilderMethod = MessageCaster.ReplyMessageBuilderMethod.MakeGenericMethod(_wrappedType);
//        //private static readonly MethodInfo _sendMessageBuilderMethod = MessageCaster.SendMessageBuilderMethod.MakeGenericMethod(_wrappedType);
//        //private static readonly MethodInfo _sendMessagesMethod = MessageCaster.SendMessagesMethod.MakeGenericMethod(_wrappedType);
//        //private static readonly MethodInfo _sendLocalMessageBuilderMethod = MessageCaster.SendLocalMessageBuilderMethod.MakeGenericMethod(_wrappedType);
//        //private static readonly MethodInfo _subscribeMethod = MessageCaster.SubscribeMethod.MakeGenericMethod(_wrappedType);
//        //private static readonly MethodInfo _subscribeConditionMethod = MessageCaster.SubscribeConditionMethod.MakeGenericMethod(_wrappedType);
//        //private static readonly MethodInfo _unsubscribeMethod = MessageCaster.UnsubscribeMethod.MakeGenericMethod(_wrappedType);
//        ////
//        //public static void Publish(IBus bus, Action<TMessage> messageBuilder) { _publishMessageBuilderMethod.Invoke(bus, new object[] { Cast(messageBuilder) }); }
//        //public static void Publish(IBus bus, TMessage[] messages) { _publishMessagesMethod.Invoke(bus, new object[] { Cast(messages) }); }
//        //public static void Reply(IBus bus, Action<TMessage> messageBuilder) { _replyMessageBuilderMethod.Invoke(bus, new object[] { Cast(messageBuilder) }); }
//        //public static IServiceBusCallback Send(IBus bus, Action<TMessage> messageBuilder) { return MessageCaster.Cast((ICallback)_sendMessageBuilderMethod.Invoke(bus, new object[] { Cast(messageBuilder) })); }
//        //public static IServiceBusCallback Send(IBus bus, string destination, Action<TMessage> messageBuilder) { return MessageCaster.Cast((ICallback)_sendMessagesMethod.Invoke(bus, new object[] { destination, Cast(messageBuilder) })); }
//        //public static void SendLocal(IBus bus, Action<TMessage> messageBuilder) { _sendLocalMessageBuilderMethod.Invoke(bus, new object[] { Cast(messageBuilder) }); }
//        //public static void Subscribe(IBus bus) { _subscribeMethod.Invoke(bus, null); }
//        //public static void Subscribe(IBus bus, Predicate<TMessage> condition) { _subscribeConditionMethod.Invoke(bus, new object[] { Cast(condition) }); }
//        //public static void Unsubscribe(IBus bus) { _unsubscribeMethod.Invoke(bus, null); }

//        //public static Action<IMessage> Cast(Action<TMessage> messageBuilder)
//        //{
//        //    return (c => messageBuilder((TMessage)((object)c)));
//        //}

//        //private static Predicate<IMessage> Cast(Predicate<TMessage> condition)
//        //{
//        //    return (c => condition((TMessage)((object)c)));
//        //}

//        //public static IMessage[] Cast(TMessage[] messages)
//        //{
//        //    return messages.Cast<INServiceMessage>().ToArray();
//        //}

//        //public static TMessage MakeMessage()
//        //{
//        //    return (TMessage)Activator.CreateInstance(_wrappedType);
//        //}
//    }
//}
