using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(_ParameterBuilder))]
[ComVisible(true)]
public class ParameterBuilder : _ParameterBuilder
{
	private MethodBase methodb;

	private string name;

	private CustomAttributeBuilder[] cattrs;

	private UnmanagedMarshal marshal_info;

	private ParameterAttributes attrs;

	private int position;

	private int table_idx;

	private object def_value;

	public virtual int Attributes => (int)attrs;

	public virtual string Name => name;

	public virtual int Position => position;
}
