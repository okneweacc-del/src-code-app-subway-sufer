using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection;

[Serializable]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(_ConstructorInfo))]
[ComVisible(true)]
public abstract class ConstructorInfo : MethodBase, _ConstructorInfo
{
	[ComVisible(true)]
	public static readonly string ConstructorName = ".ctor";

	[ComVisible(true)]
	public static readonly string TypeConstructorName = ".cctor";

	[ComVisible(true)]
	public override MemberTypes MemberType => MemberTypes.Constructor;

	[DebuggerStepThrough]
	[DebuggerHidden]
	public object Invoke(object[] parameters)
	{
		if (parameters == null)
		{
			parameters = new object[0];
		}
		return Invoke(BindingFlags.CreateInstance, null, parameters, null);
	}

	public abstract object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture);
}
