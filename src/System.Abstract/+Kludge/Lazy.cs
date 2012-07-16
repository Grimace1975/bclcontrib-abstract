#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Threading;
using System.Diagnostics;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
namespace System
{
    /// <summary>
    /// Lazy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable, DebuggerDisplay("ThreadSafetyMode={Mode}, IsValueCreated={IsValueCreated}, IsValueFaulted={IsValueFaulted}, Value={ValueForDebugDisplay}"), DebuggerTypeProxy(typeof(System_LazyDebugView<>)), ComVisible(false), HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
    public class Lazy<T>
    {
        private volatile object _boxed;
        [NonSerialized]
        private readonly object _threadSafeObj;
        [NonSerialized]
        private Func<T> m_valueFactory;
        private static Func<T> PUBLICATION_ONLY_OR_ALREADY_INITIALIZED = (() => default(T));

        static Lazy() { }

        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy&lt;T&gt;"/> class.
        /// </summary>
        public Lazy()
            : this(LazyThreadSafetyMode.ExecutionAndPublication) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="isThreadSafe">if set to <c>true</c> [is thread safe].</param>
        public Lazy(bool isThreadSafe)
            : this(isThreadSafe ? LazyThreadSafetyMode.ExecutionAndPublication : LazyThreadSafetyMode.None) { }
        //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="valueFactory">The value factory.</param>
        public Lazy(Func<T> valueFactory)
            : this(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="mode">The mode.</param>
        public Lazy(LazyThreadSafetyMode mode)
        {
            _threadSafeObj = GetObjectFromMode(mode);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="valueFactory">The value factory.</param>
        /// <param name="isThreadSafe">if set to <c>true</c> [is thread safe].</param>
        public Lazy(Func<T> valueFactory, bool isThreadSafe)
            : this(valueFactory, isThreadSafe ? LazyThreadSafetyMode.ExecutionAndPublication : LazyThreadSafetyMode.None) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="valueFactory">The value factory.</param>
        /// <param name="mode">The mode.</param>
        public Lazy(Func<T> valueFactory, LazyThreadSafetyMode mode)
        {
            if (valueFactory == null)
                throw new ArgumentNullException("valueFactory");
            _threadSafeObj = GetObjectFromMode(mode);
            m_valueFactory = valueFactory;
        }

        private Boxed CreateValue()
        {
            Boxed boxed = null;
            LazyThreadSafetyMode mode = Mode;
            if (m_valueFactory != null)
            {
                try
                {
                    if ((mode != LazyThreadSafetyMode.PublicationOnly) && (m_valueFactory == PUBLICATION_ONLY_OR_ALREADY_INITIALIZED))
                        throw new InvalidOperationException(EnvironmentEx2.GetResourceString("Lazy_Value_RecursiveCallsToValue"));
                    var valueFactory = m_valueFactory;
                    if (mode != LazyThreadSafetyMode.PublicationOnly)
                        m_valueFactory = PUBLICATION_ONLY_OR_ALREADY_INITIALIZED;
                    return new Boxed(valueFactory());
                }
                catch (Exception exception)
                {
                    if (mode != LazyThreadSafetyMode.PublicationOnly)
                        _boxed = new LazyInternalExceptionHolder(exception.PrepareForRethrow());
                    throw;
                }
            }
            try { boxed = new Boxed((T)Activator.CreateInstance(typeof(T))); }
            catch (MissingMethodException)
            {
                Exception ex = new MissingMemberException(EnvironmentEx2.GetResourceString("Lazy_CreateValue_NoParameterlessCtorForT"));
                if (mode != LazyThreadSafetyMode.PublicationOnly)
                    _boxed = new LazyInternalExceptionHolder(ex);
                throw ex;
            }
            return boxed;
        }

        private static object GetObjectFromMode(LazyThreadSafetyMode mode)
        {
            if (mode == LazyThreadSafetyMode.ExecutionAndPublication)
                return new object();
            if (mode == LazyThreadSafetyMode.PublicationOnly)
                return PUBLICATION_ONLY_OR_ALREADY_INITIALIZED;
            if (mode != LazyThreadSafetyMode.None)
                throw new ArgumentOutOfRangeException("mode", EnvironmentEx2.GetResourceString("Lazy_ctor_ModeInvalid"));
            return null;
        }

        private T LazyInitValue()
        {
            Boxed boxed = null;
            switch (Mode)
            {
                case LazyThreadSafetyMode.None:
                    boxed = CreateValue();
                    _boxed = boxed;
                    break;
                case LazyThreadSafetyMode.PublicationOnly:
                    boxed = CreateValue();
                    #pragma warning disable 420
                    if (Interlocked.CompareExchange(ref _boxed, boxed, null) != null)
                        boxed = (Boxed)_boxed;
                    #pragma warning restore 420
                    break;
                default:
                    lock (_threadSafeObj)
                    {
                        if (_boxed == null)
                        {
                            boxed = CreateValue();
                            _boxed = boxed;
                        }
                        else
                        {
                            boxed = (_boxed as Boxed);
                            if (boxed == null)
                            {
                                var holder = (_boxed as LazyInternalExceptionHolder);
                                throw holder._exception;
                            }
                        }
                    }
                    break;
            }
            return boxed._value;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            T local1 = Value;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (!IsValueCreated)
                return EnvironmentEx2.GetResourceString("Lazy_ToString_ValueNotCreated");
            return this.Value.ToString();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is value created.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is value created; otherwise, <c>false</c>.
        /// </value>
        public bool IsValueCreated
        {
            //[TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            get { return (_boxed != null && _boxed is Boxed); }
        }

        internal bool IsValueFaulted
        {
            get { return (_boxed is LazyInternalExceptionHolder); }
        }

        internal LazyThreadSafetyMode Mode
        {
            get
            {
                if (_threadSafeObj == null)
                    return LazyThreadSafetyMode.None;
                if (_threadSafeObj == (object)PUBLICATION_ONLY_OR_ALREADY_INITIALIZED)
                    return LazyThreadSafetyMode.PublicationOnly;
                return LazyThreadSafetyMode.ExecutionAndPublication;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public T Value
        {
            get
            {
                Boxed boxed = null;
                if (_boxed != null)
                {
                    boxed = (_boxed as Boxed);
                    if (boxed != null)
                        return boxed._value;
                    var holder = (_boxed as LazyInternalExceptionHolder);
                    throw holder._exception;
                }
                //Debugger.NotifyOfCrossThreadDependency();
                return LazyInitValue();
            }
        }

        internal T ValueForDebugDisplay
        {
            get
            {
                if (!this.IsValueCreated)
                    return default(T);
                return ((Boxed)_boxed)._value;
            }
        }

        [Serializable]
        private class Boxed
        {
            internal T _value;

            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            internal Boxed(T value)
            {
                _value = value;
            }
        }

        private class LazyInternalExceptionHolder
        {
            internal Exception _exception;

            //[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            internal LazyInternalExceptionHolder(Exception ex)
            {
                _exception = ex;
            }
        }
    }
}
#endif