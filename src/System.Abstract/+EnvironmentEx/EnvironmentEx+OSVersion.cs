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
using System.Runtime.InteropServices;
namespace System
{
    public static partial class EnvironmentEx
    {
        private static OperatingSystemEx _osVersionEx;
        [ThreadStatic]
        private static OperatingSystemEx _osVersionExMock;

        /// <summary>
        /// Gets or sets the OS version ex mock.
        /// </summary>
        /// <value>
        /// The OS version ex mock.
        /// </value>
        public static OperatingSystemEx OSVersionExMock
        {
            get { return _osVersionExMock; }
            set { _osVersionExMock = value; }
        }

        /// <summary>
        /// Gets the OS version ex.
        /// </summary>
        public static OperatingSystemEx OSVersionEx
        {
            get { return (_osVersionExMock == null ? GetOSVersionEx() : _osVersionExMock); }
        }

        private static OperatingSystemEx GetOSVersionEx()
        {
            if (_osVersionEx != null)
                return _osVersionEx;
            var osvi = new OSVersionInfoEx { OSVersionInfoSize = (uint)Marshal.SizeOf(typeof(OSVersionInfoEx)) };
            GetVersionEx(osvi);
            var suiteMask = osvi.SuiteMask;
            var productType = osvi.ProductType;
            return _osVersionEx = new OperatingSystemEx(Environment.OSVersion)
            {
                PlatformSuites = (PlatformSuites)suiteMask,
                PlatformProductID = (PlatformProductID)productType,
            };
        }

        #region Native

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class OSVersionInfo
        {
            public uint OSVersionInfoSize = ((uint)Marshal.SizeOf(typeof(OSVersionInfo)));
            public uint MajorVersion;
            public uint MinorVersion;
            public uint BuildNumber;
            public uint PlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x80)]
            public string CSDVersion;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class OSVersionInfoEx : OSVersionInfo
        {

            public ushort SuiteMask;
            public byte ProductType;
            public byte Reserved;
        }

        [DllImport("Kernel32", CharSet = CharSet.Auto)]
        private static extern bool GetVersionEx([In, Out] OSVersionInfo versionInformation);

        #endregion
    }
}
