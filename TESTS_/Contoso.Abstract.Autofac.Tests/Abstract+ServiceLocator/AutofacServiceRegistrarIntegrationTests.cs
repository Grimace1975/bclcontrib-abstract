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
    public class AutofacServiceRegistrarIntegrationTests : ServiceRegistrarIntegrationTestsBase
    {
        protected override IServiceLocator CreateServiceLocator() { return new AutofacServiceLocator(); }

        //[TestMethod]
        //public void Registering_with_implementation_should_override_the_previous_registration()
        //{
        //    Registrar.Register<IRepository, ProductRepository>();
        //    Registrar.Register<IRepository, AccountRepository>();
        //    var service = Locator.Resolve<IRepository>();
        //    Assert.IsInstanceOfType(service, typeof(AccountRepository));
        //}

        //[TestMethod]
        //public void Registering_with_implementation_should_override_the_previous_registration_when_resolve_is_called_inbetween()
        //{
        //    Registrar.Register<IRepository, ProductRepository>();
        //    Locator.Resolve<IRepository>();
        //    Registrar.Register<IRepository, AccountRepository>();
        //    var service = Locator.Resolve<IRepository>();
        //    Assert.IsInstanceOfType(service, typeof(AccountRepository));
        //}

        //[TestMethod]
        //public void Registering_with_specified_type_should_override_the_previous_registration_when_resolve_is_called_inbetween()
        //{
        //    Registrar.Register<IRepository>(typeof(ProductRepository));
        //    Locator.Resolve<IRepository>();
        //    Registrar.Register<IRepository>(typeof(AccountRepository));
        //    var service = Locator.Resolve<IRepository>();
        //    Assert.IsInstanceOfType(service, typeof(AccountRepository));
        //}

        //[TestMethod]
        //public void Registering_with_keyed_implementation_should_override_the_previous_registration_when_resolve_is_called_inbetween()
        //{
        //    Registrar.Register<IRepository, ProductRepository>("key");
        //    Locator.Resolve<IRepository>("key");
        //    Registrar.Register<IRepository, AccountRepository>("key");
        //    var service = Locator.Resolve<IRepository>("key");
        //    Assert.IsInstanceOfType(service, typeof(AccountRepository));
        //}

        //[TestMethod]
        //public void Registering_with_specified_service_and_type_should_override_the_previous_registration_when_resolve_is_called_inbetween()
        //{
        //    Registrar.Register(typeof(IRepository), typeof(ProductRepository));
        //    Locator.Resolve<IRepository>();
        //    Registrar.Register(typeof(IRepository), typeof(AccountRepository));
        //    var service = Locator.Resolve<IRepository>();
        //    Assert.IsInstanceOfType(service, typeof(AccountRepository));
        //}
    }
}