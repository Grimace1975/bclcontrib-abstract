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
            /// <summary>
            /// Gets the specified cache.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="cache">The cache.</param>
            /// <param name="registration">The registration.</param>
            /// <param name="tag">The tag.</param>
            /// <param name="values">The values.</param>
            /// <returns></returns>
            T Get<T>(IServiceCache cache, ServiceCacheRegistration registration, object tag, object[] values);
            /// <summary>
            /// Removes the specified cache.
            /// </summary>
            /// <param name="cache">The cache.</param>
            /// <param name="registration">The registration.</param>
            void Remove(IServiceCache cache, ServiceCacheRegistration registration);
        }

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
        public ServiceCacheRegistration(string name, CacheItemBuilder builder)
            : this(name, new CacheItemPolicy(), builder) { }
        /// <summary>
        /// Adds the data source.
        /// </summary>
        /// <param name="name">The key.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="cacheTags">The dependency array.</param>
        public ServiceCacheRegistration(string name, CacheItemBuilder builder, params string[] cacheTags)
            : this(name, new CacheItemPolicy(), builder) { SetItemPolicyDependency(cacheTags); }
        /// <summary>
        /// Adds the data source.
        /// </summary>
        /// <param name="name">The key.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="cacheTags">The dependency array.</param>
        public ServiceCacheRegistration(string name, CacheItemBuilder builder, Func<object, object[], string[]> cacheTags)
            : this(name, new CacheItemPolicy(), builder) { SetItemPolicyDependency(cacheTags); }
        /// <summary>
        /// Adds the data source.
        /// </summary>
        /// <param name="name">The key.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="dependency">The dependency.</param>
        public ServiceCacheRegistration(string name, CacheItemBuilder builder, CacheItemDependency dependency)
            : this(name, new CacheItemPolicy(), builder) { SetItemPolicyDependency(dependency); }
        /// <summary>
        /// Adds the data source.
        /// </summary>
        /// <param name="name">The key.</param>
        /// <param name="minuteTimeout">The minute timeout.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="cacheTags">The dependency array.</param>
        public ServiceCacheRegistration(string name, int minuteTimeout, CacheItemBuilder builder, params string[] cacheTags)
            : this(name, new CacheItemPolicy(minuteTimeout), builder) { SetItemPolicyDependency(cacheTags); }
        /// <summary>
        /// Adds the data source.
        /// </summary>
        /// <param name="name">The key.</param>
        /// <param name="minuteTimeout">The minute timeout.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="cacheTags">The dependency array.</param>
        public ServiceCacheRegistration(string name, int minuteTimeout, CacheItemBuilder builder, Func<object, object[], string[]> cacheTags)
            : this(name, new CacheItemPolicy(minuteTimeout), builder) { SetItemPolicyDependency(cacheTags); }
        /// <summary>
        /// Adds the data source.
        /// </summary>
        /// <param name="name">The key.</param>
        /// <param name="minuteTimeout">The minute timeout.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="dependency">The dependency.</param>
        public ServiceCacheRegistration(string name, int minuteTimeout, CacheItemBuilder builder, CacheItemDependency dependency)
            : this(name, new CacheItemPolicy(minuteTimeout), builder) { SetItemPolicyDependency(dependency); }
        /// <summary>
        /// Adds the data source.
        /// </summary>
        /// <param name="name">The key.</param>
        /// <param name="itemPolicy">The item policy.</param>
        /// <param name="builder">The builder.</param>
        public ServiceCacheRegistration(string name, CacheItemPolicy itemPolicy, CacheItemBuilder builder)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (itemPolicy == null)
                throw new ArgumentNullException("itemPolicy");
            if (builder == null)
                throw new ArgumentNullException("builder");
            Name = name;
            Builder = builder;
            ItemPolicy = itemPolicy;
            // tacks all namespaces created
            Namespaces = new List<string>();
        }
        /// <summary>
        /// Adds the data source.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="itemPolicy">The cache command.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="cacheTags">The dependency array.</param>
        public ServiceCacheRegistration(string name, CacheItemPolicy itemPolicy, CacheItemBuilder builder, params string[] cacheTags)
            : this(name, itemPolicy, builder) { SetItemPolicyDependency(cacheTags); }
        /// <summary>
        /// Adds the data source.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="itemPolicy">The cache command.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="cacheTags">The dependency array.</param>
        public ServiceCacheRegistration(string name, CacheItemPolicy itemPolicy, CacheItemBuilder builder, Func<object, object[], string[]> cacheTags)
            : this(name, itemPolicy, builder) { SetItemPolicyDependency(cacheTags); }
        /// <summary>
        /// Adds the data source.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="itemPolicy">The cache command.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="dependency">The dependency.</param>
        public ServiceCacheRegistration(string name, CacheItemPolicy itemPolicy, CacheItemBuilder builder, CacheItemDependency dependency)
            : this(name, itemPolicy, builder) { SetItemPolicyDependency(dependency); }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the cache command.
        /// </summary>
        /// <value>
        /// The cache command.
        /// </value>
        public CacheItemPolicy ItemPolicy { get; set; }

        /// <summary>
        /// Gets or sets the builder.
        /// </summary>
        /// <value>
        /// The builder.
        /// </value>
        public CacheItemBuilder Builder { get; set; }

        /// <summary>
        /// AbsoluteName
        /// </summary>
        /// <value>
        /// The name of the absolute.
        /// </value>
        public string AbsoluteName
        {
            get { return _absoluteName; }
        }

        internal List<string> Namespaces;

        #region Registrar

        internal ServiceCacheRegistrar Registrar;

        internal void SetRegistrar(ServiceCacheRegistrar registrar, string absoluteName)
        {
            Registrar = registrar;
            _absoluteName = absoluteName;
        }

        #endregion

        private void SetItemPolicyDependency(string[] cacheTags)
        {
            if (cacheTags != null && cacheTags.Length > 0)
            {
                if (ItemPolicy.Dependency != null)
                    throw new InvalidOperationException(Local.RedefineCacheDependency);
                ItemPolicy.Dependency = (c, r, t, v) => cacheTags;
            }
        }
        private void SetItemPolicyDependency(Func<object, object[], string[]> cacheTags)
        {
            if (cacheTags != null)
            {
                if (ItemPolicy.Dependency != null)
                    throw new InvalidOperationException(Local.RedefineCacheDependency);
                ItemPolicy.Dependency = (c, r, t, v) => cacheTags(t, v);
            }
        }
        private void SetItemPolicyDependency(CacheItemDependency dependency)
        {
            if (dependency != null)
            {
                if (ItemPolicy.Dependency != null)
                    throw new InvalidOperationException(Local.RedefineCacheDependency);
                ItemPolicy.Dependency = dependency;
            }
        }
    }
}
