using System.Runtime.InteropServices;

namespace System.Reflection;

[Serializable]
[Flags]
[ComVisible(true)]
public enum EventAttributes
{
	None = 0,
	SpecialName = 0x200,
	ReservedMask = 0x400,
	RTSpecialName = ReservedMask
}
