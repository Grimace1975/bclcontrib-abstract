#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
using System.Threading;
namespace System
{
	internal sealed class System_LazyDebugView<T>
	{
		private readonly Lazy<T> _lazy;

		//[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public System_LazyDebugView(Lazy<T> lazy)
		{
			_lazy = lazy;
		}

		public bool IsValueCreated
		{
			get { return _lazy.IsValueCreated; }
		}

		public bool IsValueFaulted
		{
			get { return _lazy.IsValueFaulted; }
		}

		public LazyThreadSafetyMode Mode
		{
			get { return _lazy.Mode; }
		}

		public T Value
		{
			get { return _lazy.ValueForDebugDisplay; }
		}
	}
}
#endif