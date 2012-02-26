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
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Abstract.Fakes;
namespace System.Abstract
{
    [TestClass]
    public class IServiceBusExtensionsTests
    {
        [TestMethod]
        public void SendGeneric_With_Destination_And_MessageBuilder_Sends()
        {
            var message = new TestMessage { Body = "Body" };
            var serviceBusMock = new Mock<IServiceBus>();
            serviceBusMock.Setup(x => x.CreateMessage<TestMessage>(It.IsAny<Action<TestMessage>>()))
                .Returns(message);
            var serviceBus = serviceBusMock.Object;
            //
            serviceBus.Send<TestMessage>(new LiteralServiceBusEndpoint("dest"), x => x.Body = "...");
            //
            serviceBusMock.Verify(x => x.Send(It.IsAny<IServiceBusEndpoint>(), It.Is<object[]>(y => y[0] == message)));
        }

        [TestMethod]
        public void SendGeneric_With_MessageBuilder_Sends()
        {
            var message = new TestMessage { Body = "Body" };
            var serviceBusMock = new Mock<IServiceBus>();
            serviceBusMock.Setup(x => x.CreateMessage<TestMessage>(It.IsAny<Action<TestMessage>>()))
                .Returns(message);
            var serviceBus = serviceBusMock.Object;
            //
            serviceBus.Send<TestMessage>(x => x.Body = "...");
            //
            serviceBusMock.Verify(x => x.Send(null, It.Is<object[]>(y => y[0] == message)));
        }

        [TestMethod]
        public void SendGeneric_With_StringDestination_And_MessageBuilder_Sends()
        {
            var message = new TestMessage { Body = "Body" };
            var serviceBusMock = new Mock<IServiceBus>();
            serviceBusMock.Setup(x => x.CreateMessage<TestMessage>(It.IsAny<Action<TestMessage>>()))
                .Returns(message);
            var serviceBus = serviceBusMock.Object;
            //
            serviceBus.Send<TestMessage>("dest", x => x.Body = "...");
            //
            serviceBusMock.Verify(x => x.Send(It.IsAny<LiteralServiceBusEndpoint>(), It.Is<object[]>(y => y[0] == message)));
        }

        [TestMethod]
        public void Send_With_Message_Sends()
        {
            var messages = new[] { new TestMessage { Body = "Body" } };
            var serviceBusMock = new Mock<IServiceBus>();
            var serviceBus = serviceBusMock.Object;
            //
            serviceBus.Send(messages);
            //
            serviceBusMock.Verify(x => x.Send(null, messages));
        }

        [TestMethod]
        public void Send_With_StringDestination_And_Message_Sends()
        {
            var messages = new[] { new TestMessage { Body = "Body" } };
            var serviceBusMock = new Mock<IServiceBus>();
            var serviceBus = serviceBusMock.Object;
            //
            serviceBus.Send("dest", messages);
            //
            serviceBusMock.Verify(x => x.Send(It.IsAny<LiteralServiceBusEndpoint>(), messages));
        }
    }
}