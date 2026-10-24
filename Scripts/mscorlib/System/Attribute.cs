using System.Reflection;
using System.Runtime.InteropServices;

namespace System;

[Serializable]
[AttributeUsage(AttributeTargets.All)]
[ClassInterface(ClassInterfaceType.None)]
[ComVisible(true)]
[ComDefaultInterface(typeof(_Attribute))]
public abstract class Attribute : _Attribute
{
	private static void CheckParameters(object element, Type attributeType)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		if (attributeType == null)
		{
			throw new ArgumentNullException("attributeType");
		}
		if (!typeof(Attribute).IsAssignableFrom(attributeType))
		{
			throw new ArgumentException(Locale.GetText("Type is not derived from System.Attribute."), "attributeType");
		}
	}

	public static Attribute GetCustomAttribute(MemberInfo element, Type attributeType)
	{
		return GetCustomAttribute(element, attributeType, inherit: true);
	}

	public static Attribute GetCustomAttribute(MemberInfo element, Type attributeType, bool inherit)
	{
		CheckParameters(element, attributeType);
		return MonoCustomAttrs.GetCustomAttribute(element, attributeType, inherit);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static bool IsDefined(ParameterInfo element, Type attributeType)
	{
		return IsDefined(element, attributeType, inherit: true);
	}

	public static bool IsDefined(MemberInfo element, Type attributeType)
	{
		return IsDefined(element, attributeType, inherit: true);
	}

	public static bool IsDefined(MemberInfo element, Type attributeType, bool inherit)
	{
		CheckParameters(element, attributeType);
		MemberTypes memberType = element.MemberType;
		if (memberType != MemberTypes.Constructor && memberType != MemberTypes.Event && memberType != MemberTypes.Field && memberType != MemberTypes.Method && memberType != MemberTypes.Property && memberType != MemberTypes.TypeInfo && memberType != MemberTypes.NestedType)
		{
			throw new NotSupportedException(Locale.GetText("Element is not a constructor, method, property, event, type or field."));
		}
		if (memberType == MemberTypes.Property)
		{
			return MonoCustomAttrs.IsDefined(element, attributeType, inherit);
		}
		return element.IsDefined(attributeType, inherit);
	}

	public static bool IsDefined(ParameterInfo element, Type attributeType, bool inherit)
	{
		CheckParameters(element, attributeType);
		if (element.IsDefined(attributeType, inherit))
		{
			return true;
		}
		return IsDefined(element.Member, attributeType, inherit);
	}

	public override bool Equals(object obj)
	{
		if (obj == null || !(obj is Attribute))
		{
			return false;
		}
		return ValueType.DefaultEquals(this, obj);
	}
}
