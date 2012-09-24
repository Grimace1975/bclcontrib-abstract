using System.ComponentModel;
using System;
using Rhino.ServiceBus.FileMessaging.Design;

namespace Rhino.ServiceBus.FileMessaging
{
    [TypeConverter(typeof(MessageFormatterConverter))]
    public interface IMessageFormatter : ICloneable
    {
        bool CanRead(Message message);
        object Read(Message message);
        void Write(Message message, object obj);
    }
}
