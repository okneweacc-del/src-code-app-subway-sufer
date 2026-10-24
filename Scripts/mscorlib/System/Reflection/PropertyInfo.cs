using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection;

[Serializable]
[ComDefaultInterface(typeof(_PropertyInfo))]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
public abstract class PropertyInfo : MemberInfo, _PropertyInfo
{
	public abstract PropertyAttributes Attributes { get; }

	public abstract bool CanRead { get; }

	public abstract bool CanWrite { get; }

	public override MemberTypes MemberType => MemberTypes.Property;

	public abstract Type PropertyType { get; }

	public abstract MethodInfo[] GetAccessors(bool nonPublic);

	public MethodInfo GetGetMethod()
	{
		return GetGetMethod(nonPublic: false);
	}

	public abstract MethodInfo GetGetMethod(bool nonPublic);

	public abstract ParameterInfo[] GetIndexParameters();

	public abstract MethodInfo GetSetMethod(bool nonPublic);

	[DebuggerStepThrough]
	[DebuggerHidden]
	public virtual object GetValue(object obj, object[] index)
	{
		return GetValue(obj, BindingFlags.Default, null, index, null);
	}

	public abstract object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture);

	[DebuggerHidden]
	[DebuggerStepThrough]
	public virtual void SetValue(object obj, object value, object[] index)
	{
		SetValue(obj, value, BindingFlags.Default, null, index, null);
	}

	public abstract void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture);

	public virtual Type[] GetOptionalCustomModifiers()
	{
		return Type.EmptyTypes;
	}

	public virtual Type[] GetRequiredCustomModifiers()
	{
		return Type.EmptyTypes;
	}
}
