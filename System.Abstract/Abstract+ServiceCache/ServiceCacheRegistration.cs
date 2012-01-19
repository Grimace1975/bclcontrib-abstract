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
using System.Collections.Generic;
namespace System.Abstract
{
	/// <summary>
	/// ServiceCacheRegistration
	/// </summary>
	public class ServiceCacheRegistration
	{
		private string _absoluteName;

		/// <summary>
		/// IDispatcher
		/// </summary>
		public interface IDispatcher
		{
			T Get<T>(IServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values);
			void Remove(IServiceCache cache, ServiceCacheRegistration registration);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceCacheRegistration"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		internal ServiceCacheRegistration(string name)
		{
			// used for registration-links only
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			Name = name;
			ItemPolicy = new CacheItemPolicy(-1);
		}
		/// <summary>
		/// Adds the data source.
		/// </summary>
		/// <param name="name">The key.</param>
		/// <param name="builder">The builder.</param>
		/// <param name="cacheTags">The dependency array.</param>
		public ServiceCacheRegistration(string name, CacheItemBuilder builder, params string[] cacheTags)
			: this(name, new CacheItemPolicy(), builder, cacheTags) { }
		/// <summary>
		/// Adds the data source.
		/// </summary>
		/// <param name="name">The key.</param>
		/// <param name="minuteTimeout">The minute timeout.</param>
		/// <param name="builder">The builder.</param>
		/// <param name="cacheTags">The dependency array.</param>
		public ServiceCacheRegistration(string name, int minuteTimeout, CacheItemBuilder builder, params string[] cacheTags)
			: this(name, new CacheItemPolicy(minuteTimeout), builder, cacheTags) { }
		/// <summary>
		/// Adds the data source.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="itemPolicy">The cache command.</param>
		/// <param name="builder">The builder.</param>
		/// <param name="cacheTags">The dependency array.</param>
		public ServiceCacheRegistration(string name, CacheItemPolicy itemPolicy, CacheItemBuilder builder, params string[] cacheTags)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			if (itemPolicy == null)
				throw new ArgumentNullException("itemPolicy");
			if (builder == null)
				throw new ArgumentNullException("builder");
			Name = name;
			Builder = builder;
			if (cacheTags != null && cacheTags.Length > 0)
			{
				if (itemPolicy.Dependency != null)
					throw new InvalidOperationException(Local.RedefineCacheDependency);
				itemPolicy.Dependency = ((a, b) => cacheTags);
			}
			ItemPolicy = itemPolicy;
			// tacks all namespaces created
			Namespaces = new List<string>();
		}

		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		/// <value>The key.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the cache command.
		/// </summary>
		/// <value>The cache command.</value>
		public CacheItemPolicy ItemPolicy { get; set; }

		/// <summary>
		/// Gets or sets the builder.
		/// </summary>
		/// <value>The builder.</value>
		public CacheItemBuilder Builder { get; set; }

		/// <summary>
		/// AbsoluteName
		/// </summary>
		public string AbsoluteName
		{
			get { return _absoluteName; }
		}

		/// <summary>
		/// Namespaces
		/// </summary>
		internal List<string> Namespaces;

		#region Registrar

		/// <summary>
		/// Registrar
		/// </summary>
		internal ServiceCacheRegistrar Registrar;

		internal void SetRegistrar(ServiceCacheRegistrar registrar, string absoluteName)
		{
			Registrar = registrar;
			_absoluteName = absoluteName;
		}

		#endregion
	}
}
