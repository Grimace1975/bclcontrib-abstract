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
using System.Reflection;
namespace System.Abstract
{
    /// <summary>
    /// IServiceCacheRegistration
    /// </summary>
    public interface IServiceCacheRegistration
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }
        /// <summary>
        /// Gets the name of the absolute.
        /// </summary>
        /// <value>
        /// The name of the absolute.
        /// </value>
        string AbsoluteName { get; }
        /// <summary>
        /// Gets the use headers.
        /// </summary>
        /// <value>
        /// The use headers.
        /// </value>
        bool UseHeaders { get; }
        /// <summary>
        /// Gets the registrar.
        /// </summary>
        /// <value>
        /// The registrar.
        /// </value>
        ServiceCacheRegistrar Registrar { get; }
        /// <summary>
        /// Attaches the registrar.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="absoluteName">Name of the absolute.</param>
        void AttachRegistrar(ServiceCacheRegistrar registrar, string absoluteName);
    }

    /// <summary>
    /// ServiceCacheRegistration
    /// </summary>
    public class ServiceCacheRegistration : IServiceCacheRegistration
    {
        private Dictionary<Type, List<ConsumerAction>> _consumers = new Dictionary<Type, List<ConsumerAction>>();

        internal struct ConsumerAction
        {
            public Type TType;
            public MethodInfo Method;
            public object Target;

            public ConsumerAction(Type tType, MethodInfo method, object target)
            {
                TType = tType;
                Method = method;
                Target = target;
            }
        }

        /// <summary>
        /// ConsumerInfo
        /// </summary>
        public struct ConsumerInfo
        {
            private static MethodInfo _invokeInternalInfo = typeof(ConsumerInfo).GetMethod("InvokeInternal", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            internal ConsumerAction Action { get; set; }
            /// <summary>
            /// Gets the message.
            /// </summary>
            /// <value>
            /// The message.
            /// </value>
            public object Message { get; internal set; }
            /// <summary>
            /// Actions the invoke.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="message">The message.</param>
            /// <param name="tag">The tag.</param>
            /// <param name="values">The values.</param>
            /// <param name="get">The get.</param>
            /// <returns></returns>
            public object ActionInvoke<T>(object message, object tag, object[] values, Func<T> get) { return Action.Method.Invoke(Action.Target, new object[] { message, tag, values, get }); }
            /// <summary>
            /// Invokes the specified cache.
            /// </summary>
            /// <param name="cache">The cache.</param>
            /// <param name="tag">The tag.</param>
            /// <param name="header">The header.</param>
            /// <returns></returns>
            public object Invoke(IServiceCache cache, object tag, CacheItemHeader header) { return _invokeInternalInfo.MakeGenericMethod(Action.TType).Invoke(this, new[] { cache, tag, header }); }
            private object InvokeInternal<T>(IServiceCache cache, object tag, CacheItemHeader header) { return ActionInvoke<T>(Message, tag, header.Values, () => (T)cache.Get(null, header.Item)); }
        }

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
            T Get<T>(IServiceCache cache, IServiceCacheRegistration registration, object tag, object[] values);
            /// <summary>
            /// Sends the specified cache.
            /// </summary>
            /// <param name="cache">The cache.</param>
            /// <param name="registration">The registration.</param>
            /// <param name="tag">The tag.</param>
            /// <param name="messages">The messages.</param>
            void Send(IServiceCache cache, IServiceCacheRegistration registration, object tag, params object[] messages);
            /// <summary>
            /// Removes the specified cache.
            /// </summary>
            /// <param name="cache">The cache.</param>
            /// <param name="registration">The registration.</param>
            void Remove(IServiceCache cache, IServiceCacheRegistration registration);
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
        /// Gets or sets a value indicating whether [use headers].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use headers]; otherwise, <c>false</c>.
        /// </value>
        public bool UseHeaders { get; set; }

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
        public string AbsoluteName { get; private set; }

        /// <summary>
        /// Consumeses the specified action.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public ServiceCacheRegistration ConsumerOf<TMessage>(Func<TMessage, object, object[], Func<object>, object> action) { return ConsumerOf<object, TMessage>(action); }
        /// <summary>
        /// Consumeses the specified action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public ServiceCacheRegistration ConsumerOf<T, TMessage>(Func<TMessage, object, object[], Func<T>, object> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            UseHeaders = true;
            List<ConsumerAction> consumerActions;
            if (!_consumers.TryGetValue(typeof(TMessage), out consumerActions))
                _consumers.Add(typeof(TMessage), consumerActions = new List<ConsumerAction>());
            consumerActions.Add(new ConsumerAction(typeof(T), action.Method, action.Target));
            return this;
        }

        /// <summary>
        /// Gets the consumers for.
        /// </summary>
        /// <param name="messages">The messages.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public IEnumerable<ConsumerInfo> GetConsumersFor(object[] messages)
        {
            if (messages == null)
                throw new ArgumentNullException("messages");
            List<ConsumerAction> consumerActions;
            foreach (var message in messages)
                if (_consumers.TryGetValue(message.GetType(), out consumerActions))
                    foreach (var consumerAction in consumerActions)
                        yield return new ConsumerInfo { Message = message, Action = consumerAction };
        }

        #region Registrar

        /// <summary>
        /// Gets the registrar.
        /// </summary>
        /// <value>
        /// The registrar.
        /// </value>
        public ServiceCacheRegistrar Registrar { get; internal set; }

        /// <summary>
        /// Gets the namespaces.
        /// </summary>
        /// <value>
        /// The namespaces.
        /// </value>
        internal List<string> Namespaces { get; private set; }

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        public IEnumerable<string> Keys
        {
            get
            {
                var name = AbsoluteName;
                if (name != null)
                    yield return name;
                if (Namespaces != null)
                    foreach (var n in Namespaces)
                        yield return n + name;
            }
        }

        /// <summary>
        /// Attaches the registrar.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="absoluteName">Name of the absolute.</param>
        public void AttachRegistrar(ServiceCacheRegistrar registrar, string absoluteName)
        {
            Registrar = registrar;
            AbsoluteName = absoluteName;
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
