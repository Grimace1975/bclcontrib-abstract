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
    public abstract partial class ServiceLocatorIntegrationTestsBase
    {
        protected virtual void RegisterForFutureTests()
        {
            Registrar.Register<ITestService, TestServiceFuture>();
            Registrar.Register<ITestFutureService, TestFutureService>();
            //Registrar.Register<ITestNamedService, TestNamedService>(typeof(TestNamedService).FullName);
            //Registrar.Register<ITestNamedService, TestNamedService2>(typeof(TestNamedService2).FullName);
        }

        //[TestMethod]
        public void Future_Registration_Resolve_Should_Return_Valid_Instance()
        {
            var serviceType = typeof(TestServiceFuture);
            var futureServiceType = typeof(TestFutureService);
            RegisterForFutureTests();
            //
            var service = Locator.Resolve<ITestService>();
            Assert.IsNotNull(service);
            Assert.AreSame(serviceType, service.GetType());
            var futureService = Locator.Resolve<ITestFutureService>();
            Assert.IsNotNull(futureService);
            Assert.AreSame(serviceType, futureServiceType);
            // non-generic
            var serviceN = Locator.Resolve(typeof(ITestService));
            Assert.IsNotNull(serviceN);
            Assert.AreSame(serviceType, serviceN.GetType());
            var futureServiceN = Locator.Resolve(typeof(ITestFutureService));
            Assert.IsNotNull(futureServiceN);
            Assert.AreSame(futureServiceType, futureServiceN.GetType());
        }

        //[TestMethod]
        public void Future_Registration_Ask_For_Named_Instance()
        {
            RegisterForFutureTests();
            //
            var serviceType = typeof(TestNamedService);
            var serviceType2 = typeof(TestNamedService2);
            var service = Locator.Resolve<ITestNamedService>(serviceType.FullName);
            Assert.AreSame(service.GetType(), serviceType);
            var service2 = Locator.Resolve<ITestNamedService>(serviceType2.FullName);
            Assert.AreSame(service2.GetType(), serviceType2);
            // non-generic
            var serviceN = Locator.Resolve(typeof(ITestNamedService), serviceType.FullName);
            Assert.AreSame(serviceN.GetType(), serviceType);
            var serviceN2 = Locator.Resolve(typeof(ITestNamedService), serviceType2.FullName);
            Assert.AreSame(serviceN2.GetType(), serviceType2);
        }

        //[TestMethod]
        public virtual void Future_Registration_ResolveAll_Should_Return_All_Registered_Services()
        {
            RegisterForFutureTests();
            //
            var services = Locator.ResolveAll<ITestNamedService>();
            Assert.AreEqual(2, services.Count());
            // non-generic
            var servicesN = Locator.ResolveAll(typeof(ITestNamedService));
            Assert.AreEqual(2, servicesN.Count());
        }
    }
}