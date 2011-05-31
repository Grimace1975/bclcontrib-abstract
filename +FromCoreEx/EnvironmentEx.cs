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
using System.Security;
using System.Globalization;
namespace System
{
    /// <summary>
    /// EnvironmentEx
    /// </summary>
#if COREINTERNAL
    internal
#else
    using System.Patterns.ReleaseManagement.Configuration;
    public
#endif
 static class EnvironmentEx
    {
#if !COREINTERNAL
        public static void LoadFromConfiguration(ReleaseManagementConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");
            EnvironmentEx2.DeploymentEnvironment = configuration.DeploymentEnvironment;
            EnvironmentEx2.DevelopmentStage = configuration.DevelopmentStage;
        }
#endif

        [SecurityCritical]
        internal static string GetResourceFromDefault(string key) { return key; }

        [SecuritySafeCritical] //, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal static string GetResourceString(string key) { return GetResourceFromDefault(key); }

        [SecuritySafeCritical]
        internal static string GetResourceString(string key, params object[] values)
        {
            string resourceFromDefault = GetResourceFromDefault(key);
            return string.Format(CultureInfo.CurrentCulture, resourceFromDefault, values);
        }
    }
}
