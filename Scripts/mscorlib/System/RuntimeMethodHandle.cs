using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System;

[Serializable]
[MonoTODO("Serialization needs tests")]
[ComVisible(true)]
public struct RuntimeMethodHandle : ISerializable
{
	private IntPtr value;

	public IntPtr Value => value;

	internal RuntimeMethodHandle(IntPtr v)
	{
		value = v;
	}

	private RuntimeMethodHandle(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		MonoMethod monoMethod = (MonoMethod)info.GetValue("MethodObj", typeof(MonoMethod));
		value = monoMethod.MethodHandle.Value;
		if (value == IntPtr.Zero)
		{
			throw new SerializationException(Locale.GetText("Insufficient state."));
		}
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		if (value == IntPtr.Zero)
		{
			throw new SerializationException("Object fields may not be properly initialized");
		}
		info.AddValue("MethodObj", (MonoMethod)MethodBase.GetMethodFromHandle(this), typeof(MonoMethod));
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}
		return value == ((RuntimeMethodHandle)obj).Value;
	}

	public override int GetHashCode()
	{
		return value.GetHashCode();
	}
}
