using System.Runtime.CompilerServices;

namespace Mono;

internal class Runtime
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern string GetDisplayName();
}
