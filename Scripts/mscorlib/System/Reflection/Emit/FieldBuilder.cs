using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(_FieldBuilder))]
[ComVisible(true)]
public sealed class FieldBuilder : FieldInfo, _FieldBuilder
{
	private FieldAttributes attrs;

	private Type type;

	private string name;

	private object def_value;

	private int offset;

	private int table_idx;

	internal TypeBuilder typeb;

	private byte[] rva_data;

	private CustomAttributeBuilder[] cattrs;

	private UnmanagedMarshal marshal_info;

	private RuntimeFieldHandle handle;

	private Type[] modReq;

	private Type[] modOpt;

	public override FieldAttributes Attributes => attrs;

	public override Type DeclaringType => typeb;

	public override RuntimeFieldHandle FieldHandle
	{
		get
		{
			throw CreateNotSupportedException();
		}
	}

	public override Type FieldType => type;

	public override string Name => name;

	public override Type ReflectedType => typeb;

	internal override UnmanagedMarshal UMarshal => marshal_info;

	public override Module Module => base.Module;

	internal FieldBuilder(TypeBuilder tb, string fieldName, Type type, FieldAttributes attributes, Type[] modReq, Type[] modOpt)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		attrs = attributes;
		name = fieldName;
		this.type = type;
		this.modReq = modReq;
		this.modOpt = modOpt;
		offset = -1;
		typeb = tb;
		table_idx = tb.get_next_table_index(this, 4, inc: true);
	}

	public override object[] GetCustomAttributes(bool inherit)
	{
		if (typeb.is_created)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, inherit);
		}
		throw CreateNotSupportedException();
	}

	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		if (typeb.is_created)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
		}
		throw CreateNotSupportedException();
	}

	public override object GetValue(object obj)
	{
		throw CreateNotSupportedException();
	}

	public override bool IsDefined(Type attributeType, bool inherit)
	{
		throw CreateNotSupportedException();
	}

	internal override int GetFieldOffset()
	{
		return 0;
	}

	internal void SetRVAData(byte[] data)
	{
		rva_data = (byte[])data.Clone();
	}

	public override void SetValue(object obj, object val, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
	{
		throw CreateNotSupportedException();
	}

	private Exception CreateNotSupportedException()
	{
		return new NotSupportedException("The invoked member is not supported in a dynamic module.");
	}
}
