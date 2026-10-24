using System.Diagnostics;
using System.Globalization;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection;

[Serializable]
[ComDefaultInterface(typeof(_FieldInfo))]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
public abstract class FieldInfo : MemberInfo, _FieldInfo
{
	public abstract FieldAttributes Attributes { get; }

	public abstract RuntimeFieldHandle FieldHandle { get; }

	public abstract Type FieldType { get; }

	public override MemberTypes MemberType => MemberTypes.Field;

	public bool IsLiteral => (Attributes & FieldAttributes.Literal) != 0;

	public bool IsStatic => (Attributes & FieldAttributes.Static) != 0;

	public bool IsPublic => (Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public;

	public bool IsNotSerialized => (Attributes & FieldAttributes.NotSerialized) == FieldAttributes.NotSerialized;

	internal virtual UnmanagedMarshal UMarshal => GetUnmanagedMarshal();

	public abstract object GetValue(object obj);

	public abstract void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture);

	[DebuggerStepThrough]
	[DebuggerHidden]
	public void SetValue(object obj, object value)
	{
		SetValue(obj, value, BindingFlags.Default, null, null);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern FieldInfo internal_from_handle_type(IntPtr field_handle, IntPtr type_handle);

	public static FieldInfo GetFieldFromHandle(RuntimeFieldHandle handle)
	{
		if (handle.Value == IntPtr.Zero)
		{
			throw new ArgumentException("The handle is invalid.");
		}
		return internal_from_handle_type(handle.Value, IntPtr.Zero);
	}

	internal virtual int GetFieldOffset()
	{
		throw new SystemException("This method should not be called");
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern UnmanagedMarshal GetUnmanagedMarshal();

	internal object[] GetPseudoCustomAttributes()
	{
		int num = 0;
		if (IsNotSerialized)
		{
			num++;
		}
		if (DeclaringType.IsExplicitLayout)
		{
			num++;
		}
		UnmanagedMarshal uMarshal = UMarshal;
		if (uMarshal != null)
		{
			num++;
		}
		if (num == 0)
		{
			return null;
		}
		object[] array = new object[num];
		num = 0;
		if (IsNotSerialized)
		{
			array[num++] = new NonSerializedAttribute();
		}
		if (DeclaringType.IsExplicitLayout)
		{
			array[num++] = new FieldOffsetAttribute(GetFieldOffset());
		}
		if (uMarshal != null)
		{
			array[num++] = uMarshal.ToMarshalAsAttribute();
		}
		return array;
	}
}
