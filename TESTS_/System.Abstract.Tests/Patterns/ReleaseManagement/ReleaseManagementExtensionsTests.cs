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
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace System.Patterns.ReleaseManagement
{
    [TestClass]
    public class ReleaseManagementExtensionsTests
    {
        [TestMethod]
        public void DeploymentEnvironment_ToShortName_Return_ProperValues()
        {
            Assert.AreEqual("proof", DeploymentEnvironment.ProofOfConcept.ToShortName());
            Assert.AreEqual("local", DeploymentEnvironment.Local.ToShortName());
            Assert.AreEqual("develop", DeploymentEnvironment.Development.ToShortName());
            Assert.AreEqual("alpha", DeploymentEnvironment.AlphaTesting.ToShortName());
            Assert.AreEqual("beta", DeploymentEnvironment.BetaTesting.ToShortName());
            Assert.AreEqual("prod", DeploymentEnvironment.Production.ToShortName());
        }

        [TestMethod]
        public void DeploymentEnvironment_IsExternalDeployment_Return_ProperValues()
        {
            Assert.IsFalse(DeploymentEnvironment.ProofOfConcept.IsExternalDeployment());
            Assert.IsFalse(DeploymentEnvironment.Local.IsExternalDeployment());
            Assert.IsFalse(DeploymentEnvironment.Development.IsExternalDeployment());
            Assert.IsFalse(DeploymentEnvironment.AlphaTesting.IsExternalDeployment());
            Assert.IsTrue(DeploymentEnvironment.BetaTesting.IsExternalDeployment());
            Assert.IsTrue(DeploymentEnvironment.Production.IsExternalDeployment());
        }

        [TestMethod]
        public void DeploymentEnvironment_ToCode_Return_ProperValues()
        {
            Assert.AreEqual("X", DeploymentEnvironment.ProofOfConcept.ToCode());
            Assert.AreEqual("Z", DeploymentEnvironment.Local.ToCode());
            Assert.AreEqual("D", DeploymentEnvironment.Development.ToCode());
            Assert.AreEqual("A", DeploymentEnvironment.AlphaTesting.ToCode());
            Assert.AreEqual("B", DeploymentEnvironment.BetaTesting.ToCode());
            Assert.AreEqual("P", DeploymentEnvironment.Production.ToCode());
        }

        [TestMethod]
        public void DevelopmentStage_ToCode_Return_ProperValues()
        {
            Assert.AreEqual("D", DevelopmentStage.PreAlpha.ToCode());
            Assert.AreEqual("A", DevelopmentStage.Alpha.ToCode());
            Assert.AreEqual("B", DevelopmentStage.Beta.ToCode());
            Assert.AreEqual("P", DevelopmentStage.Release.ToCode());
        }
    }
}
