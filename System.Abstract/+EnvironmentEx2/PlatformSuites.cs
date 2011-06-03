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
		BackOffice = 0x00000004,
		Blade = 0x00000400,
		ComputeServer = 0x00004000,
		DataCenter = 0x00000080,
		Enterprise = 0x00000002,
		EmbeddedNT = 0x00000040,
		Personal = 0x00000200,
		SingleUserTS = 0x00000100,
		SmallBusiness = 0x00000001,
		SmallBusinessRestricted = 0x00000020,
		StorageServer = 0x00002000,
		Terminal = 0x00000010,
		WHServer = 0x00008000,
	}
}
