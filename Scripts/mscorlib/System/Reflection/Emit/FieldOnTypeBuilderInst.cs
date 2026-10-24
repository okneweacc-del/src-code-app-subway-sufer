using System.Globalization;

namespace System.Reflection.Emit;

internal class FieldOnTypeBuilderInst : FieldInfo
{
	internal MonoGenericClass instantiation;

	internal FieldBuilder fb;

	public override Type DeclaringType => instantiation;

	public override string Name => fb.Name;

	public override Type ReflectedType => instantiation;

	public override FieldAttributes Attributes => fb.Attributes;

	public override RuntimeFieldHandle FieldHandle
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public override Type FieldType
	{
		get
		{
			if (!((ModuleBuilder)instantiation.generic_type.Module).assemblyb.IsCompilerContext)
			{
				throw new NotSupportedException();
			}
			return instantiation.InflateType(fb.FieldType);
		}
	}

	public FieldOnTypeBuilderInst(MonoGenericClass instantiation, FieldBuilder fb)
	{
		this.instantiation = instantiation;
		this.fb = fb;
	}

	public override bool IsDefined(Type attributeType, bool inherit)
	{
		throw new NotSupportedException();
	}

	public override object[] GetCustomAttributes(bool inherit)
	{
		throw new NotSupportedException();
	}

	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		throw new NotSupportedException();
	}

	public override string ToString()
	{
		if (!((ModuleBuilder)instantiation.generic_type.Module).assemblyb.IsCompilerContext)
		{
			return fb.FieldType.ToString() + " " + Name;
		}
		return FieldType.ToString() + " " + Name;
	}

	public override object GetValue(object obj)
	{
		throw new NotSupportedException();
	}

	public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}
