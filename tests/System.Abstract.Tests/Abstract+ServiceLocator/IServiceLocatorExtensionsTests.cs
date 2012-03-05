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
using System.Abstract.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
namespace System.Abstract
{
	[TestClass]
    public class IServiceLocatorExtensionsTests
	{
		public interface ITestServiceLocator : IServiceLocator { }

		[TestMethod]
		public void GetServiceLocatorGeneric_Returns_Generic()
		{
			var serviceLocatorMock = new Mock<IServiceLocator>();
			serviceLocatorMock.As<ITestServiceLocator>();
			var serviceLocator = serviceLocatorMock.Object;
			//
			Assert.IsInstanceOfType(serviceLocator.GetServiceLocator<ITestServiceLocator>(), typeof(ITestServiceLocator));
		}

		[TestMethod]
		public void ResolveGeneric_With_ServiceType_Returns_Generic()
		{
			var testServiceType = typeof(TestService);
			var serviceLocatorMock = new Mock<IServiceLocator>();
			serviceLocatorMock.Setup(x => x.Resolve(testServiceType))
				.Returns(new TestService { });
			var serviceLocator = serviceLocatorMock.Object;
			//
			Assert.IsInstanceOfType(serviceLocator.Resolve<TestService>(testServiceType), testServiceType);
		}

		[TestMethod]
		public void ResolveGeneric_With_ServiceType_And_Name_Returns_Generic()
		{
			var testServiceType = typeof(TestService);
			var serviceLocatorMock = new Mock<IServiceLocator>();
			serviceLocatorMock.Setup(x => x.Resolve(testServiceType, "name"))
				.Returns(new TestService { });
			var serviceLocator = serviceLocatorMock.Object;
			//
			Assert.IsInstanceOfType(serviceLocator.Resolve<TestService>(testServiceType, "name"), testServiceType);
		}

		[TestMethod]
		public void ResolveAll_With_ServiceType_And_Name_Returns_Generic()
		{
			var testServiceType = typeof(TestService);
			var serviceLocatorMock = new Mock<IServiceLocator>();
			serviceLocatorMock.Setup(x => x.ResolveAll(testServiceType))
				.Returns(new[] { new TestService { } });
			var serviceLocator = serviceLocatorMock.Object;
			//
			var services = serviceLocator.ResolveAll<TestService>(testServiceType);
			Assert.AreEqual(1, services.Count());
			Assert.IsInstanceOfType(services.First(), testServiceType);
		}
	}
}