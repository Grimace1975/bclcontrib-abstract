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
using System;
using System.Collections.Generic;
namespace System.Abstract
{
	/// <summary>
	/// ServiceCacheSettings
	/// </summary>
	public class ServiceCacheSettings
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCacheSettings"/> class.
        /// </summary>
		public ServiceCacheSettings()
		{
			RegionMarker = "@";
            RegistrationDispatcher = new DefaultServiceCacheRegistrationDispatcher();
            Options = ServiceCacheOptions.UseDBNullWithRegistrations;
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCacheSettings"/> class.
        /// </summary>
        /// <param name="touchable">The touchable.</param>
		public ServiceCacheSettings(ITouchableCacheItem touchable)
			: this() { Touchable = touchable; }

		/// <summary>
		/// Gets or sets the RegionMarker.
		/// </summary>
		public string RegionMarker { get; set; }

		/// <summary>
		/// Gets or sets the options.
		/// </summary>
		public ServiceCacheOptions Options { get; set; }

		/// <summary>
		/// Gets or sets the registration dispatcher.
		/// </summary>
		public ServiceCacheRegistration.IDispatcher RegistrationDispatcher { get; set; }

		/// <summary>
		/// Gets or sets the touchable.
		/// </summary>
		public ITouchableCacheItem Touchable { get; set; }

		/// <summary>
		/// Tries to get the region.
		/// </summary>
		public bool TryGetRegion(ref string name, out string regionName)
		{
			var index = name.IndexOf(RegionMarker);
			if (index == -1)
			{
				regionName = null;
				return false;
			}
			string originalName = name;
			regionName = originalName.Substring(0, index);
			name = originalName.Substring(index + RegionMarker.Length);
			return true;
		}
	}
}
