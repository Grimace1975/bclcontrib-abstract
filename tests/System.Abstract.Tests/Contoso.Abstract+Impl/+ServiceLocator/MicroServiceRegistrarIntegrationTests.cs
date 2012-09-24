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
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Contoso.Abstract
{
    [TestClass]
    public class MicroServiceRegistrarIntegrationTests : ServiceRegistrarIntegrationTestsBase
    {
        protected override IServiceLocator CreateServiceLocator() { return new MicroServiceLocator(); }

        #region Register Instance

        [TestMethod]
        public override void RegisterInstance_Generic_Should_Return_Same_Object() { }
        [TestMethod]
        public override void RegisterInstance_GenericNamed_Should_Return_Same_Object() { }
        [TestMethod]
        public override void RegisterInstance_Should_Return_Same_Object() { }
        [TestMethod]
        public override void RegisterInstance_Named_Should_Return_Same_Object() { }
        [TestMethod]
        public override void RegisterInstance_Should_Return_Same_Object_For_Same_Type() { }

        #endregion

        #region Register Method

        [TestMethod]
        public override void Register_Generic_With_FactoryMethod_Should_Return_Result_From_Factory() { }
        [TestMethod]
        public override void Register_With_FactoryMethod_Should_Return_Result_From_Factory() { }

        #endregion
    }
}