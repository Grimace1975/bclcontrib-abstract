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
    public class AutofacServiceLocatorIntegrationTests : ServiceLocatorIntegrationTestsBase
    {
        protected override IServiceLocator CreateServiceLocator()
        {
            return new AutofacServiceLocator();
            //var serviceType = typeof(TestService);
            //var serviceType2 = typeof(TestService2);
            //var container = new ContainerBuilder();
            //container.RegisterType(serviceType).As<ITestService>().Named(serviceType.FullName, typeof(ITestService));
            //container.RegisterType(serviceType2).As<ITestService>().Named(serviceType2.FullName, typeof(ITestService)).As(typeof(TestService2));
            //return new AutofacServiceLocator(container);
        }
    }
}