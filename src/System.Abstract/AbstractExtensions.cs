﻿#region License
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
namespace System
{
    /// <summary>
    /// AbstractExtensions
    /// </summary>
    public static class AbstractExtensions
    {
        /// <summary>
        /// Toes the short name.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <returns></returns>
        public static string ToShortName(this DeploymentEnvironment environment)
        {
            switch (environment)
            {
                case DeploymentEnvironment.ProofOfConcept: return "proof";
                case DeploymentEnvironment.Local: return "local";
                case DeploymentEnvironment.Development: return "develop";
                case DeploymentEnvironment.AlphaTesting: return "alpha";
                case DeploymentEnvironment.BetaTesting: return "beta";
                case DeploymentEnvironment.Production: return "prod";
                default: throw new ArgumentOutOfRangeException("environment", "unknown target");
            }
        }

        /// <summary>
        /// Determines whether [is external deployment] [the specified environment].
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <returns>
        ///   <c>true</c> if [is external deployment] [the specified environment]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsExternalDeployment(this DeploymentEnvironment environment)
        {
            return (environment == DeploymentEnvironment.Production || environment == DeploymentEnvironment.BetaTesting);
        }

        /// <summary>
        /// Toes the code.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <returns></returns>
        public static string ToCode(this DeploymentEnvironment environment)
        {
            switch (environment)
            {
                case DeploymentEnvironment.ProofOfConcept: return "X";
                case DeploymentEnvironment.Local: return "Z";
                case DeploymentEnvironment.Development: return "D";
                case DeploymentEnvironment.AlphaTesting: return "A";
                case DeploymentEnvironment.BetaTesting: return "B";
                case DeploymentEnvironment.Production: return "P";
                default: throw new ArgumentOutOfRangeException("environment", "unknown target");
            }
        }

        /// <summary>
        /// Toes the code.
        /// </summary>
        /// <param name="stage">The stage.</param>
        /// <returns></returns>
        public static string ToCode(this DevelopmentStage stage)
        {
            switch (stage)
            {
                case DevelopmentStage.PreAlpha: return "D";
                case DevelopmentStage.Alpha: return "A";
                case DevelopmentStage.Beta: return "B";
                case DevelopmentStage.Release: return "P";
                default: throw new ArgumentOutOfRangeException("stage", "unknown target");
            }
        }
    }
}
