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
	/// OperatingSystemEx
	/// </summary>
	public class OperatingSystemEx
	{
		private OperatingSystem _parent;
        /// <summary>
        /// Initializes a new instance of the <see cref="OperatingSystemEx"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
		public OperatingSystemEx(OperatingSystem parent) { _parent = parent; }

        /// <summary>
        /// Gets the platform.
        /// </summary>
		public virtual PlatformID Platform
		{
			get { return (_parent != null ? _parent.Platform : (PlatformID)0); }
		}
        /// <summary>
        /// Gets the service pack.
        /// </summary>
		public virtual string ServicePack
		{
			get { return (_parent != null ? _parent.ServicePack : null); }
		}
        /// <summary>
        /// Gets the version.
        /// </summary>
		public virtual Version Version
		{
			get { return (_parent != null ? _parent.Version : null); }
		}
        /// <summary>
        /// Gets the version string.
        /// </summary>
		public virtual string VersionString
		{
			get { return (_parent != null ? _parent.VersionString : null); }
		}
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
		public override string ToString() { return (_parent != null ? _parent.ToString() : null); }

        /// <summary>
        /// Gets or sets the platform suites.
        /// </summary>
        /// <value>
        /// The platform suites.
        /// </value>
		public virtual PlatformSuites PlatformSuites { get; set; }

        /// <summary>
        /// Gets or sets the platform product ID.
        /// </summary>
        /// <value>
        /// The platform product ID.
        /// </value>
		public virtual PlatformProductID PlatformProductID { get; set; }		
	}
}
