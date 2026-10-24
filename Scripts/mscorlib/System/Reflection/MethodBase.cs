using System.Diagnostics;
using System.Globalization;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection;

[Serializable]
[ComDefaultInterface(typeof(_MethodBase))]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
public abstract class MethodBase : MemberInfo, _MethodBase
{
	public abstract RuntimeMethodHandle MethodHandle { get; }

	public abstract MethodAttributes Attributes { get; }

	public virtual CallingConventions CallingConvention => CallingConventions.Standard;

	public bool IsPublic => (Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;

	public bool IsStatic => (Attributes & MethodAttributes.Static) != 0;

	public bool IsVirtual => (Attributes & MethodAttributes.Virtual) != 0;

	public bool IsAbstract => (Attributes & MethodAttributes.Abstract) != 0;

	public virtual bool ContainsGenericParameters => false;

	public virtual bool IsGenericMethodDefinition => false;

	public virtual bool IsGenericMethod => false;

	internal static MethodBase GetMethodFromHandleNoGenericCheck(RuntimeMethodHandle handle)
	{
		return GetMethodFromIntPtr(handle.Value, IntPtr.Zero);
	}

	private static MethodBase GetMethodFromIntPtr(IntPtr handle, IntPtr declaringType)
	{
		if (handle == IntPtr.Zero)
		{
			throw new ArgumentException("The handle is invalid.");
		}
		MethodBase methodFromHandleInternalType = GetMethodFromHandleInternalType(handle, declaringType);
		if (methodFromHandleInternalType == null)
		{
			throw new ArgumentException("The handle is invalid.");
		}
		return methodFromHandleInternalType;
	}

	public static MethodBase GetMethodFromHandle(RuntimeMethodHandle handle)
	{
		MethodBase methodFromIntPtr = GetMethodFromIntPtr(handle.Value, IntPtr.Zero);
		Type declaringType = methodFromIntPtr.DeclaringType;
		if (declaringType.IsGenericType || declaringType.IsGenericTypeDefinition)
		{
			throw new ArgumentException("Cannot resolve method because it's declared in a generic class.");
		}
		return methodFromIntPtr;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern MethodBase GetMethodFromHandleInternalType(IntPtr method_handle, IntPtr type_handle);

	public abstract ParameterInfo[] GetParameters();

	internal virtual int GetParameterCount()
	{
		ParameterInfo[] parameters = GetParameters();
		if (parameters == null)
		{
			return 0;
		}
		return parameters.Length;
	}

	[DebuggerStepThrough]
	[DebuggerHidden]
	public object Invoke(object obj, object[] parameters)
	{
		return Invoke(obj, BindingFlags.Default, null, parameters, null);
	}

	public abstract object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture);

	internal virtual int get_next_table_index(object obj, int table, bool inc)
	{
		if (this is MethodBuilder)
		{
			MethodBuilder methodBuilder = (MethodBuilder)this;
			return methodBuilder.get_next_table_index(obj, table, inc);
		}
		if (this is ConstructorBuilder)
		{
			ConstructorBuilder constructorBuilder = (ConstructorBuilder)this;
			return constructorBuilder.get_next_table_index(obj, table, inc);
		}
		throw new Exception("Method is not a builder method");
	}

	[ComVisible(true)]
	public virtual Type[] GetGenericArguments()
	{
		throw new NotSupportedException();
	}
}
