using System.Runtime.InteropServices;

namespace System.Reflection;

[Serializable]
[ComVisible(true)]
public enum MethodImplAttributes
{
	CodeTypeMask = 3,
	IL = 0,
	Native = 1,
	OPTIL = 2,
	Runtime = CodeTypeMask,
	ManagedMask = 4,
	Unmanaged = ManagedMask,
	Managed = IL,
	ForwardRef = 16,
	PreserveSig = 128,
	InternalCall = 4096,
	Synchronized = 32,
	NoInlining = 8,
	MaxMethodImplVal = 65535
}
