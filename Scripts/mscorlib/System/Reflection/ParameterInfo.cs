using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace System.Reflection;

[Serializable]
[ComDefaultInterface(typeof(_ParameterInfo))]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
public class ParameterInfo : ICustomAttributeProvider, _ParameterInfo
{
	protected Type ClassImpl;

	protected object DefaultValueImpl;

	protected MemberInfo MemberImpl;

	protected string NameImpl;

	protected int PositionImpl;

	protected ParameterAttributes AttrsImpl;

	private UnmanagedMarshal marshalAs;

	public virtual Type ParameterType => ClassImpl;

	public virtual ParameterAttributes Attributes => AttrsImpl;

	public bool IsIn => (Attributes & ParameterAttributes.In) != 0;

	public bool IsOptional => (Attributes & ParameterAttributes.Optional) != 0;

	public bool IsOut => (Attributes & ParameterAttributes.Out) != 0;

	public bool IsRetval => (Attributes & ParameterAttributes.Retval) != 0;

	public virtual MemberInfo Member => MemberImpl;

	public virtual string Name => NameImpl;

	public virtual int Position => PositionImpl;

	protected ParameterInfo()
	{
	}

	internal ParameterInfo(ParameterBuilder pb, Type type, MemberInfo member, int position)
	{
		ClassImpl = type;
		MemberImpl = member;
		if (pb != null)
		{
			NameImpl = pb.Name;
			PositionImpl = pb.Position - 1;
			AttrsImpl = (ParameterAttributes)pb.Attributes;
		}
		else
		{
			NameImpl = null;
			PositionImpl = position - 1;
			AttrsImpl = ParameterAttributes.None;
		}
	}

	internal ParameterInfo(ParameterInfo pinfo, MemberInfo member)
	{
		ClassImpl = pinfo.ParameterType;
		MemberImpl = member;
		NameImpl = pinfo.Name;
		PositionImpl = pinfo.Position;
		AttrsImpl = pinfo.Attributes;
	}

	public override string ToString()
	{
		Type type = ClassImpl;
		while (type.HasElementType)
		{
			type = type.GetElementType();
		}
		string text = ((!type.IsPrimitive && ClassImpl != typeof(void) && !(ClassImpl.Namespace == MemberImpl.DeclaringType.Namespace)) ? ClassImpl.FullName : ClassImpl.Name);
		if (!IsRetval)
		{
			text += ' ';
			text += NameImpl;
		}
		return text;
	}

	public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
	}

	public virtual bool IsDefined(Type attributeType, bool inherit)
	{
		return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
	}

	internal object[] GetPseudoCustomAttributes()
	{
		int num = 0;
		if (IsIn)
		{
			num++;
		}
		if (IsOut)
		{
			num++;
		}
		if (IsOptional)
		{
			num++;
		}
		if (marshalAs != null)
		{
			num++;
		}
		if (num == 0)
		{
			return null;
		}
		object[] array = new object[num];
		num = 0;
		if (IsIn)
		{
			array[num++] = new InAttribute();
		}
		if (IsOptional)
		{
			array[num++] = new OptionalAttribute();
		}
		if (IsOut)
		{
			array[num++] = new OutAttribute();
		}
		if (marshalAs != null)
		{
			array[num++] = marshalAs.ToMarshalAsAttribute();
		}
		return array;
	}
}
