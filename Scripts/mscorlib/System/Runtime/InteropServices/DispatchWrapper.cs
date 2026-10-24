namespace System.Runtime.InteropServices;

[Serializable]
[ComVisible(true)]
public sealed class DispatchWrapper
{
	private object wrappedObject;

	public object WrappedObject => wrappedObject;
}
