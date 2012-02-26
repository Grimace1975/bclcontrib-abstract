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
using System.Abstract.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace System.Abstract
{
    [TestClass]
    public abstract class ServiceRegistrarIntegrationTestsBase
    {
        protected IServiceLocator Locator { get; private set; }
        protected IServiceRegistrar Registrar { get; private set; }
        protected abstract IServiceLocator CreateServiceLocator();

        [TestInitialize]
        public void Initialize()
        {
            Locator = CreateServiceLocator();
            Registrar = Locator.Registrar;
        }

        #region Enumerate

        [TestMethod]
        public virtual void HasRegistered()
        {
            var serviceType = typeof(TestService);
            Registrar.Register<ITestService>(serviceType);
            //
            Assert.IsFalse(Registrar.HasRegistered<string>());
            Assert.IsFalse(Registrar.HasRegistered(typeof(string)));
            Assert.IsTrue(Registrar.HasRegistered<ITestService>());
            Assert.IsTrue(Registrar.HasRegistered(serviceType));
        }

        [TestMethod]
        public virtual void GetRegistrationsFor()
        {
            var serviceType = typeof(TestService);
            Registrar.Register<ITestService>(serviceType);
            //
            Assert.IsFalse(Registrar.GetRegistrationsFor(typeof(string)).Any());
            Assert.IsTrue(Registrar.GetRegistrationsFor(serviceType).Any());
        }

        #endregion

        #region Register Type

        #endregion

        #region Register Implementation

        [TestMethod]
        public virtual void Register_With_Specified_Type_Should_Return_Same_Type()
        {
            var serviceType = typeof(TestService);
            Registrar.Register<ITestService>(serviceType);
            //
            var service = Locator.Resolve<ITestService>();
            Assert.AreEqual(serviceType, service.GetType());
            var serviceA = Locator.Resolve(serviceType);
            Assert.AreEqual(serviceType, serviceA.GetType());
        }

        [TestMethod]
        public virtual void Register_With_Implementation_Type_Should_Return_Same_Type()
        {
            var serviceType = typeof(TestService);
            Registrar.Register<ITestService, TestService>();
            var serviceType2 = typeof(TestNamedService);
            Registrar.Register<ITestNamedService, TestNamedService>(serviceType2.FullName);
            //
            var service = Locator.Resolve<ITestService>();
            Assert.AreEqual(serviceType, service.GetType());
            var service2 = Locator.Resolve<ITestNamedService>(serviceType2.FullName);
            Assert.AreEqual(serviceType2, service2.GetType());
            var serviceA = Locator.Resolve(serviceType);
            Assert.AreEqual(serviceType, serviceA.GetType());
            var service2A = Locator.Resolve(serviceType, serviceType2.FullName);
            Assert.AreEqual(serviceType2, service2A.GetType());
        }


        [TestMethod]
        public virtual void Register_With_Keyed_Type_Should_Return_Same_Type()
        {
            var serviceType = typeof(TestNamedService);
            Registrar.Register(serviceType, serviceType.FullName);
            //
            var service = Locator.Resolve<TestNamedService>(serviceType.FullName);
            Assert.AreEqual(serviceType, service.GetType());
        }

        [TestMethod]
        public virtual void Register_With_Specified_Service_And_Type_Should_Return_Same_Type()
        {
            var serviceType = typeof(TestService);
            Registrar.Register(serviceType, serviceType);
            //
            var service = Locator.Resolve<ITestService>(serviceType);
            Assert.AreEqual(serviceType, service.GetType());
        }

        [TestMethod]
        public virtual void Register_With_Specified_Service_Should_Return_Same_Type()
        {
            var serviceType = typeof(TestService);
            Registrar.Register(serviceType, serviceType);
            //
            var service = Locator.Resolve(serviceType);
            Assert.AreEqual(serviceType, service.GetType());
        }

        #endregion

        #region Register Instance

        [TestMethod]
        public virtual void RegisterInstance_Generic_Should_Return_Same_Object()
        {
            Registrar.RegisterInstance<ITestService>(new TestService());
            //
            var service = Locator.Resolve<ITestService>();
            Assert.AreEqual(typeof(TestService), service.GetType());
        }
        [TestMethod]
        public virtual void RegisterInstance_GenericNamed_Should_Return_Same_Object()
        {
            Registrar.RegisterInstance<ITestService>(new TestService(), "name");
            //
            var service = Locator.Resolve<ITestService>("name");
            Assert.AreEqual(typeof(TestService), service.GetType());
        }
        [TestMethod]
        public virtual void RegisterInstance_Should_Return_Same_Object()
        {
            Registrar.RegisterInstance(typeof(ITestService), new TestService());
            //
            var service = Locator.Resolve<ITestService>();
            Assert.AreEqual(typeof(TestService), service.GetType());
        }
        [TestMethod]
        public virtual void RegisterInstance_Named_Should_Return_Same_Object()
        {
            Registrar.RegisterInstance(typeof(ITestService), new TestService(), "name");
            //
            var service = Locator.Resolve<ITestService>("name");
            Assert.AreEqual(typeof(TestService), service.GetType());
        }

        //
        [TestMethod]
        public virtual void RegisterInstance_Should_Return_Same_Object_For_Same_Type()
        {
            Registrar.RegisterInstance(new TestService());
            //
            var service = Locator.Resolve<TestService>();
            Assert.AreEqual(typeof(TestService), service.GetType());
        }

        #endregion

        #region Register Method

        [TestMethod]
        public virtual void Register_Generic_With_FactoryMethod_Should_Return_Result_From_Factory()
        {
            var firstExpectedObject = new TestService();
            var secondExpectedObject = new TestService();
            var hasBeenCalled = false;
            Registrar.Register<ITestService>(x =>
            {
                if (!hasBeenCalled)
                {
                    hasBeenCalled = true;
                    return firstExpectedObject;
                }
                return secondExpectedObject;
            });
            //
            var first = Locator.Resolve<ITestService>();
            Assert.AreSame(firstExpectedObject, first);
            //
            var second = Locator.Resolve<ITestService>();
            Assert.AreSame(secondExpectedObject, second);
        }

        [TestMethod]
        public virtual void Register_With_FactoryMethod_Should_Return_Result_From_Factory()
        {
            var firstExpectedObject = new TestService();
            var secondExpectedObject = new TestService();
            var hasBeenCalled = false;
            Registrar.Register(typeof(ITestService), x =>
            {
                if (!hasBeenCalled)
                {
                    hasBeenCalled = true;
                    return firstExpectedObject;
                }
                return secondExpectedObject;
            });
            //
            var first = Locator.Resolve<ITestService>();
            Assert.AreSame(firstExpectedObject, first);
            //
            var second = Locator.Resolve<ITestService>();
            Assert.AreSame(secondExpectedObject, second);
        }

        #endregion
    }
}