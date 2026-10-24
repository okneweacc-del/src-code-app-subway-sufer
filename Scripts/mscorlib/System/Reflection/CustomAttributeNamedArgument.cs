using System.Runtime.InteropServices;

namespace System.Reflection;

[Serializable]
[ComVisible(true)]
public struct CustomAttributeNamedArgument
{
	private CustomAttributeTypedArgument typedArgument;

	private MemberInfo memberInfo;

	public override string ToString()
	{
		return memberInfo.Name + " = " + typedArgument.ToString();
	}

	public override bool Equals(object obj)
	{
		if (!(obj is CustomAttributeNamedArgument customAttributeNamedArgument))
		{
			return false;
		}
		return customAttributeNamedArgument.memberInfo == memberInfo && typedArgument.Equals(customAttributeNamedArgument.typedArgument);
	}

	public override int GetHashCode()
	{
		return (memberInfo.GetHashCode() << 16) + typedArgument.GetHashCode();
	}
}
