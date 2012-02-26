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
using System.Abstract.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace System.Abstract
{
	[TestClass]
	public abstract class ServiceBusIntegrationTestsBase
	{
		protected IServiceBus Bus { get; private set; }
		protected abstract IServiceBus CreateServiceBus();

		[TestInitialize]
		public void Initialize()
		{
			Bus = CreateServiceBus();
		}

		[TestMethod]
		public void CreateMessage_Should_Return_Valid_Instance()
		{
			var message = Bus.CreateMessage<TestMessage>(null);
			Assert.IsNotNull(message);
		}

		[TestMethod]
		public void CreateMessage_With_Action_Should_Return_Valid_Instance()
		{
			var message = Bus.CreateMessage<TestMessage>(x => x.Body = "APPLY");
			Assert.IsNotNull(message);
			Assert.AreEqual(message.Body, "APPLY");
		}


		[TestMethod]
		public void Send_Should_Return_Valid_Instance()
		{
		}

		//IServiceBusCallback Send(IServiceBusLocation destination, params IServiceMessage[] messages);
		//public static IServiceBusCallback Send<TMessage>(this IServiceBus serviceBus, IServiceBusLocation destination, Action<TMessage> messageBuilder)
		//    where TMessage : IServiceMessage { return serviceBus.Send(destination, serviceBus.CreateMessage<TMessage>(messageBuilder)); }
		//public static IServiceBusCallback Send<TMessage>(this IServiceBus serviceBus, Action<TMessage> messageBuilder)
		//    where TMessage : IServiceMessage { return serviceBus.Send(null, serviceBus.CreateMessage<TMessage>(messageBuilder)); }
		//public static IServiceBusCallback Send<TMessage>(this IServiceBus serviceBus, string destination, Action<TMessage> messageBuilder)
		//    where TMessage : IServiceMessage { return serviceBus.Send(new LiteralServiceBusLocation(destination), serviceBus.CreateMessage<TMessage>(messageBuilder)); }
		//// send
		//public static IServiceBusCallback Send(this IServiceBus serviceBus, params IServiceMessage[] messages) { return serviceBus.Send(null, messages); }
		//public static IServiceBusCallback Send(this IServiceBus serviceBus, string destination, params IServiceMessage[] messages) { return serviceBus.Send(new LiteralServiceBusLocation(destination), messages); }
	}
}