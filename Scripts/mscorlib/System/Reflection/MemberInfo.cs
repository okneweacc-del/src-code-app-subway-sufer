using System.Runtime.InteropServices;

namespace System.Reflection;

[Serializable]
[ComDefaultInterface(typeof(_MemberInfo))]
[ClassInterface(ClassInterfaceType.None)]
[ComVisible(true)]
public abstract class MemberInfo : ICustomAttributeProvider, _MemberInfo
{
	public abstract Type DeclaringType { get; }

	public abstract MemberTypes MemberType { get; }

	public abstract string Name { get; }

	public abstract Type ReflectedType { get; }

	public virtual Module Module => DeclaringType.Module;

	public abstract bool IsDefined(Type attributeType, bool inherit);

	public abstract object[] GetCustomAttributes(bool inherit);

	public abstract object[] GetCustomAttributes(Type attributeType, bool inherit);
}
