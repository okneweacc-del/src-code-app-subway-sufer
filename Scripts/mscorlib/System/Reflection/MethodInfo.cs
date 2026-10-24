using System.Runtime.InteropServices;

namespace System.Reflection;

[Serializable]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(_MethodInfo))]
[ComVisible(true)]
public abstract class MethodInfo : MethodBase, _MethodInfo
{
	public override MemberTypes MemberType => MemberTypes.Method;

	public virtual Type ReturnType => null;

	public override bool IsGenericMethod => false;

	public override bool IsGenericMethodDefinition => false;

	public override bool ContainsGenericParameters => false;

	public abstract MethodInfo GetBaseDefinition();

	public virtual MethodInfo MakeGenericMethod(params Type[] typeArguments)
	{
		throw new NotSupportedException(GetType().ToString());
	}

	[ComVisible(true)]
	public override Type[] GetGenericArguments()
	{
		return Type.EmptyTypes;
	}
}
