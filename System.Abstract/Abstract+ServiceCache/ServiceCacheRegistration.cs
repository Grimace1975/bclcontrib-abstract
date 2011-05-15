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
        /// <summary>
        /// IDispatch
        /// </summary>
        public interface IDispatch
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
            CacheCommand = new ServiceCacheCommand("ServiceCacheRegistration", -1);
        }
        /// <summary>
        /// Adds the data source.
        /// </summary>
        /// <param name="name">The key.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="cacheTags">The dependency array.</param>
        public ServiceCacheRegistration(string name, ServiceCacheBuilder builder, params string[] cacheTags)
            : this(name, new ServiceCacheCommand(name), builder, cacheTags) { }
        /// <summary>
        /// Adds the data source.
        /// </summary>
        /// <param name="name">The key.</param>
        /// <param name="minuteTimeout">The minute timeout.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="cacheTags">The dependency array.</param>
        public ServiceCacheRegistration(string name, int minuteTimeout, ServiceCacheBuilder builder, params string[] cacheTags)
            : this(name, new ServiceCacheCommand(name, minuteTimeout), builder, cacheTags) { }
        /// <summary>
        /// Adds the data source.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="cacheCommand">The cache command.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="cacheTags">The dependency array.</param>
        public ServiceCacheRegistration(string name, ServiceCacheCommand cacheCommand, ServiceCacheBuilder builder, params string[] cacheTags)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (cacheCommand == null)
                throw new ArgumentNullException("cacheCommand");
            if (builder == null)
                throw new ArgumentNullException("builder");
            Name = name;
            Builder = builder;
            if ((cacheTags != null) && (cacheTags.Length > 0))
            {
                if (cacheCommand.Dependency != null)
                    throw new InvalidOperationException(Local.RedefineCacheDependency);
                cacheCommand.Dependency = new ServiceCacheDependency { CacheTags = cacheTags };
            }
            CacheCommand = cacheCommand;
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
        public ServiceCacheCommand CacheCommand { get; set; }

        /// <summary>
        /// Gets or sets the builder.
        /// </summary>
        /// <value>The builder.</value>
        public ServiceCacheBuilder Builder { get; set; }

        /// <summary>
        /// Registrar
        /// </summary>
        internal ServiceCacheRegistrar Registrar;

        /// <summary>
        /// Salts
        /// </summary>
        internal List<string> Namespaces;
    }
}
