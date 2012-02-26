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
namespace Contoso.Abstract
{
    [TestClass]
    public class CastleWindsorServiceLocatorIntegrationTests : ServiceLocatorIntegrationTestsBase
    {
        protected override IServiceLocator CreateServiceLocator()
        {
            return new CastleWindsorServiceLocator();
            //var serviceType = typeof(TestService);
            //var serviceType2 = typeof(TestService2);
            //var container = new WindsorContainer();
            ////container.Add(Component.For(typeof(ITestService).ImplementedBy(serviceType).LifeStyle.Transient);
            //container.AddComponent(serviceType.FullName, typeof(ITestService), serviceType);
            //container.AddComponent(serviceType2.FullName, typeof(TestService2), serviceType2);
            //return new WindsorServiceLocator(container);
        }
    }
}