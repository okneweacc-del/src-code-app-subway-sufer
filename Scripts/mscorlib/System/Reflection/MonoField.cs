using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Reflection;

[Serializable]
internal class MonoField : FieldInfo, ISerializable
{
	internal IntPtr klass;

	internal RuntimeFieldHandle fhandle;

	private string name;

	private Type type;

	private FieldAttributes attrs;

	public override FieldAttributes Attributes => attrs;

	public override RuntimeFieldHandle FieldHandle => fhandle;

	public override Type FieldType => type;

	public override Type ReflectedType => GetParentType(declaring: false);

	public override Type DeclaringType => GetParentType(declaring: true);

	public override string Name => name;

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern Type GetParentType(bool declaring);

	public override bool IsDefined(Type attributeType, bool inherit)
	{
		return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
	}

	public override object[] GetCustomAttributes(bool inherit)
	{
		return MonoCustomAttrs.GetCustomAttributes(this, inherit);
	}

	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal override extern int GetFieldOffset();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern object GetValueInternal(object obj);

	public override object GetValue(object obj)
	{
		if (!IsStatic)
		{
			if (obj == null)
			{
				throw new TargetException("Non-static field requires a target");
			}
			if (!DeclaringType.IsAssignableFrom(obj.GetType()))
			{
				throw new ArgumentException($"Field {Name} defined on type {DeclaringType} is not a field on the target object which is of type {obj.GetType()}.", "obj");
			}
		}
		if (!IsLiteral)
		{
			CheckGeneric();
		}
		return GetValueInternal(obj);
	}

	public override string ToString()
	{
		return $"{type} {name}";
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetValueInternal(FieldInfo fi, object obj, object value);

	public override void SetValue(object obj, object val, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
	{
		if (!IsStatic)
		{
			if (obj == null)
			{
				throw new TargetException("Non-static field requires a target");
			}
			if (!DeclaringType.IsAssignableFrom(obj.GetType()))
			{
				throw new ArgumentException($"Field {Name} defined on type {DeclaringType} is not a field on the target object which is of type {obj.GetType()}.", "obj");
			}
		}
		if (IsLiteral)
		{
			throw new FieldAccessException("Cannot set a constant field");
		}
		if (binder == null)
		{
			binder = Binder.DefaultBinder;
		}
		CheckGeneric();
		if (val != null)
		{
			object obj2 = binder.ChangeType(val, type, culture);
			if (obj2 == null)
			{
				throw new ArgumentException(string.Concat("Object type ", val.GetType(), " cannot be converted to target type: ", type), "val");
			}
			val = obj2;
		}
		SetValueInternal(this, obj, val);
	}

	internal MonoField Clone(string newName)
	{
		MonoField monoField = new MonoField();
		monoField.name = newName;
		monoField.type = type;
		monoField.attrs = attrs;
		monoField.klass = klass;
		monoField.fhandle = fhandle;
		return monoField;
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		MemberInfoSerializationHolder.Serialize(info, Name, ReflectedType, ToString(), MemberTypes.Field);
	}

	private void CheckGeneric()
	{
		if (DeclaringType.ContainsGenericParameters)
		{
			throw new InvalidOperationException("Late bound operations cannot be performed on fields with types for which Type.ContainsGenericParameters is true.");
		}
	}
}
