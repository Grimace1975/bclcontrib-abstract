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
using System.Linq;
using MassTransit;
namespace Contoso.Abstract
{
    internal class MTServiceBusTransport
    {
        /// <summary>
        /// Transport
        /// </summary>
        public class Transport<T>
        {
            public T D { get; set; }
        }

        /// <summary>
        /// TransportHandler
        /// </summary>
        public class TransportHandler<T> : Consumes<Transport<T>>.All
        {
            private readonly object _bus;
            public TransportHandler(object bus) { _bus = bus; }
            public void Consume(Transport<T> message) { var d = message.D; }
        }

        #region Endpoint-Translation

        //public static string TransportEndpointMapper(IServiceBusEndpoint endpoint)
        //{
        //    return null;
        //}

        #endregion

        #region message casting

        public static object[] Wrap(object[] messages)
        {
            return messages.Select(x =>
            {
                var type = typeof(Transport<>).MakeGenericType(x.GetType());
                var transport = Activator.CreateInstance(type);
                type.GetProperty("D").SetValue(transport, x, null);
                return transport;
            }).ToArray();
        }

        public static Type Wrap(Type messagesType) { return typeof(Transport<>).MakeGenericType(messagesType); }

        public static IEndpoint Cast(IServiceBusEndpoint endpoint)
        {
            //var endpointAsLiteral = (endpoint as LiteralServiceBusEndpoint);
            //if (endpointAsLiteral != null)
            //    return endpointAsLiteral.Value;
            throw new InvalidOperationException("Cast: Endpoint");
        }

        #endregion
    }
}
