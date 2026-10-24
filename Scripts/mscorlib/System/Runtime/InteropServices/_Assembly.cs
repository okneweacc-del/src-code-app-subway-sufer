using System.Reflection;

namespace System.Runtime.InteropServices;

[Guid("17156360-2F1A-384A-BC52-FDE93C215C5B")]
[CLSCompliant(false)]
[ComVisible(true)]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
[TypeLibImportClass(typeof(Assembly))]
public interface _Assembly
{
	new Type GetType();
}
