using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

[ComVisible(true)]
[ComDefaultInterface(typeof(_EventBuilder))]
[ClassInterface(ClassInterfaceType.None)]
public sealed class EventBuilder : _EventBuilder
{
	internal string name;

	private Type type;

	private TypeBuilder typeb;

	private CustomAttributeBuilder[] cattrs;

	internal MethodBuilder add_method;

	internal MethodBuilder remove_method;

	internal MethodBuilder raise_method;

	internal MethodBuilder[] other_methods;

	internal EventAttributes attrs;

	private int table_idx;
}
