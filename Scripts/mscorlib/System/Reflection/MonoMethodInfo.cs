using System.Runtime.CompilerServices;

namespace System.Reflection;

internal struct MonoMethodInfo
{
	private Type parent;

	private Type ret;

	internal MethodAttributes attrs;

	internal MethodImplAttributes iattrs;

	private CallingConventions callconv;

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_method_info(IntPtr handle, out MonoMethodInfo info);

	internal static MonoMethodInfo GetMethodInfo(IntPtr handle)
	{
		get_method_info(handle, out var info);
		return info;
	}

	internal static Type GetDeclaringType(IntPtr handle)
	{
		return GetMethodInfo(handle).parent;
	}

	internal static Type GetReturnType(IntPtr handle)
	{
		return GetMethodInfo(handle).ret;
	}

	internal static MethodAttributes GetAttributes(IntPtr handle)
	{
		return GetMethodInfo(handle).attrs;
	}

	internal static CallingConventions GetCallingConvention(IntPtr handle)
	{
		return GetMethodInfo(handle).callconv;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ParameterInfo[] get_parameter_info(IntPtr handle, MemberInfo member);

	internal static ParameterInfo[] GetParametersInfo(IntPtr handle, MemberInfo member)
	{
		return get_parameter_info(handle, member);
	}
}
