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
namespace System
{
	/// <summary>
	/// PlatformSuites
	/// </summary>
	public enum PlatformSuites
	{
        /// <summary>
        /// BackOffice
        /// </summary>
		BackOffice = 0x00000004,
        /// <summary>
        /// Blade
        /// </summary>
		Blade = 0x00000400,
        /// <summary>
        /// ComputeServer
        /// </summary>
		ComputeServer = 0x00004000,
        /// <summary>
        /// DataCenter
        /// </summary>
		DataCenter = 0x00000080,
        /// <summary>
        /// Enterprise
        /// </summary>
		Enterprise = 0x00000002,
        /// <summary>
        /// EmbeddedNT
        /// </summary>
		EmbeddedNT = 0x00000040,
        /// <summary>
        /// Personal
        /// </summary>
		Personal = 0x00000200,
        /// <summary>
        /// SingleUserTS
        /// </summary>
		SingleUserTS = 0x00000100,
        /// <summary>
        /// SmallBusiness
        /// </summary>
		SmallBusiness = 0x00000001,
        /// <summary>
        /// SmallBusinessRestricted
        /// </summary>
		SmallBusinessRestricted = 0x00000020,
        /// <summary>
        /// StorageServer
        /// </summary>
		StorageServer = 0x00002000,
        /// <summary>
        /// Terminal
        /// </summary>
		Terminal = 0x00000010,
        /// <summary>
        /// WHServer
        /// </summary>
		WHServer = 0x00008000,
	}
}
