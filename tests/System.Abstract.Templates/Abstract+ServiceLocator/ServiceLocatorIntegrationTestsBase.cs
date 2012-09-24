#region Foreign-License
//
// Author: Javier Lozano <javier@lozanotek.com>
// Copyright (c) 2009-2010, lozanotek, inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Modified: Sky Morey <moreys@digitalev.com>
//
#endregion
using System.Linq;
using System.Collections.Generic;
using System.Abstract.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace System.Abstract
{
    [TestClass]
    public abstract partial class ServiceLocatorIntegrationTestsBase
    {
        protected IServiceLocator Locator { get; private set; }
        protected IServiceRegistrar Registrar { get; private set; }
        protected abstract IServiceLocator CreateServiceLocator();

        protected virtual void RegisterForTests()
        {
            Registrar.Register<ITestService, TestService>();
            Registrar.Register<ITestNamedService, TestNamedService>(typeof(TestNamedService).FullName);
            Registrar.Register<ITestNamedService, TestNamedService2>(typeof(TestNamedService2).FullName);
        }

        [TestInitialize]
        public void Initialize()
        {
            Locator = CreateServiceLocator();
            Registrar = Locator.Registrar;
            RegisterForTests();
        }

        //Test if child works


        [TestMethod]
        public void Resolve_Should_Return_Valid_Instance()
        {
            var serviceType = typeof(TestService);         
            var service = Locator.Resolve<ITestService>();
            Assert.IsNotNull(service);
            Assert.AreSame(serviceType, service.GetType());
            // non-generic
            var serviceN = Locator.Resolve(typeof(ITestService));
            Assert.IsNotNull(serviceN);
            Assert.AreSame(serviceType, serviceN.GetType());
        }

        [TestMethod]
        public virtual void GenericAndNonGeneric_Resolve_Method_Should_Return_Same_Instance_Type()
        {
            var serviceType = Locator.Resolve<ITestService>().GetType();
            // non-generic
            var serviceTypeN = Locator.Resolve(typeof(ITestService)).GetType();
            Assert.AreEqual(serviceType, serviceTypeN);
        }

        [TestMethod]
        public void Asking_For_UnRegistered_Service_Return_Valid_Instance()
        {
            var service = Locator.Resolve<TestServiceN>();
            Assert.IsNotNull(service);
            // non-generic
            var serviceN = Locator.Resolve(typeof(TestServiceN));
            Assert.IsNotNull(serviceN);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceLocatorResolutionException))]
        public void Asking_For_Invalid_Service_Should_Raise_Exception()
        {
            Locator.Resolve<string>();
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceLocatorResolutionException))]
        public void NonGeneric_Asking_For_Invalid_Service_Should_Raise_Exception()
        {
            Locator.Resolve(typeof(string));
        }

        #region Named Instances

        [TestMethod]
        public void Ask_For_Named_Instance()
        {
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
        //public virtual void GenericAndNonGeneric_Resolve_Named_Instance_Should_Return_Same_Instance_Type()
        //{
        //    Assert.AreEqual(
        //        Locator.Resolve<ITestNamedService>(typeof(TestNamedService).FullName).GetType(),
        //        Locator.Resolve<ITestNamedService>(typeof(TestNamedService)).GetType());
        //}


        [TestMethod]
        [ExpectedException(typeof(ServiceLocatorResolutionException))]
        public void Ask_For_Unknown_Service_Should_Throw_Exception()
        {
            Locator.Resolve<ITestNamedService>("BAD-ID");
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceLocatorResolutionException))]
        public void NonGeneric_Ask_For_Unknown_Service_Should_Throw_Exception()
        {
            Locator.Resolve(typeof(ITestNamedService), "BAD-ID");
        }

        #endregion

        #region ResolveAll

        [TestMethod]
        public virtual void ResolveAll_Should_Return_All_Registered_UnNamed_Services()
        {
            var services = Locator.ResolveAll<ITestService>();
            Assert.AreEqual(1, services.Count());
            // non-generic
            var servicesN = Locator.ResolveAll(typeof(ITestService));
            Assert.AreEqual(1, servicesN.Count());
        }

        [TestMethod]
        public virtual void ResolveAll_Should_Return_All_Registered_Named_Services()
        {
            var services2 = Locator.ResolveAll<ITestNamedService>();
            Assert.AreEqual(2, services2.Count());
            // non-generic
            var servicesN2 = Locator.ResolveAll(typeof(ITestNamedService));
            Assert.AreEqual(2, servicesN2.Count());
        }

        [TestMethod]
        public virtual void ResolveAll_For_Unknown_Type_Should_Return_Empty_Enumerable()
        {
            var services = Locator.ResolveAll<string>();
            Assert.AreEqual(0, services.Count());
            // non-generic
            var servicesN = Locator.ResolveAll(typeof(string));
            Assert.AreEqual(0, servicesN.Count());
        }

        [TestMethod]
        public virtual void GenericAndNonGeneric_ResolveAll_Should_Return_Same_Instace_Types()
        {
            var services = new List<ITestNamedService>(Locator.ResolveAll<ITestNamedService>());
            // non-generic
            var servicesN = new List<ITestNamedService>(Locator.ResolveAll(typeof(ITestNamedService)).Cast<ITestNamedService>());
            for (int index = 0; index < services.Count; index++)
                Assert.AreEqual(services[index].GetType(), servicesN[index].GetType());
        }

        #endregion
    }
}