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
using System.Abstract;
using System.Abstract.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;
namespace Contoso.Abstract
{
    [TestClass]
    public class UnityServiceLocatorIntegrationTests : ServiceLocatorIntegrationTestsBase
    {
        protected override IServiceLocator CreateServiceLocator()
        {
            return new UnityServiceLocator();
            //var container = new UnityContainer();
            //var serviceType = typeof(TestService);
            //var serviceType2 = typeof(TestService2);
            //container.RegisterType(typeof(ITestService), serviceType, new InjectionMember[0]);
            //container.RegisterType(typeof(ITestService), serviceType, serviceType.FullName, new InjectionMember[0]);
            //container.RegisterType(typeof(ITestService), serviceType2, serviceType2.FullName, new InjectionMember[0]);
            //return new UnityServiceLocator(container);
        }

        //[TestMethod]
        //public void Inject_Should_Set_Dependencies_On_Instance_When_Dependencies_Are_Not_Defined_On_The_Interface_And_Resolved_As_The_Interface_Type()
        //{
        //    Registrar.Register(typeof(ITestDependency), typeof(TestDependency));
        //    //
        //    var serviceAsInterface = (ITestService)new TestServiceWithServiceDependency();
        //    Locator.Inject(serviceAsInterface);
        //    //
        //    var service = (TestServiceWithServiceDependency)serviceAsInterface;
        //    Assert.IsNotNull(service.DependencyThatDoesNotExistOnInterface);
        //}

        // skip this test
        public override void ResolveAll_Should_Return_All_Registered_UnNamed_Services() { }
    }
}
