#region Foreign-License
// .Net40 Kludge
#endregion
#if !CLR4
namespace System.Threading
{
	public enum LazyThreadSafetyMode
	{
		None,
		PublicationOnly,
		ExecutionAndPublication
	}
}
#endif