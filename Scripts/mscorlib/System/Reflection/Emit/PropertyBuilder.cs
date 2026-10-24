using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

[ClassInterface(ClassInterfaceType.None)]
[ComVisible(true)]
[ComDefaultInterface(typeof(_PropertyBuilder))]
public sealed class PropertyBuilder : PropertyInfo, _PropertyBuilder
{
	private PropertyAttributes attrs;

	private string name;

	private Type type;

	private Type[] parameters;

	private CustomAttributeBuilder[] cattrs;

	private object def_value;

	private MethodBuilder set_method;

	private MethodBuilder get_method;

	private int table_idx;

	internal TypeBuilder typeb;

	private Type[] returnModReq;

	private Type[] returnModOpt;

	private Type[][] paramModReq;

	private Type[][] paramModOpt;

	public override PropertyAttributes Attributes => attrs;

	public override bool CanRead => get_method != null;

	public override bool CanWrite => set_method != null;

	public override Type DeclaringType => typeb;

	public override string Name => name;

	public override Type PropertyType => type;

	public override Type ReflectedType => typeb;

	public override Module Module => base.Module;

	public override MethodInfo[] GetAccessors(bool nonPublic)
	{
		return null;
	}

	public override object[] GetCustomAttributes(bool inherit)
	{
		throw not_supported();
	}

	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		throw not_supported();
	}

	public override MethodInfo GetGetMethod(bool nonPublic)
	{
		return get_method;
	}

	public override ParameterInfo[] GetIndexParameters()
	{
		throw not_supported();
	}

	public override MethodInfo GetSetMethod(bool nonPublic)
	{
		return set_method;
	}

	public override object GetValue(object obj, object[] index)
	{
		return null;
	}

	public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
	{
		throw not_supported();
	}

	public override bool IsDefined(Type attributeType, bool inherit)
	{
		throw not_supported();
	}

	public override void SetValue(object obj, object value, object[] index)
	{
	}

	public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
	{
	}

	private Exception not_supported()
	{
		return new NotSupportedException("The invoked member is not supported in a dynamic module.");
	}
}
