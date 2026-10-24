using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System;

[Serializable]
[ComVisible(true)]
[MonoTODO("Serialization needs tests")]
public struct RuntimeTypeHandle : ISerializable
{
	private IntPtr value;

	public IntPtr Value => value;

	private RuntimeTypeHandle(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		MonoType monoType = (MonoType)info.GetValue("TypeObj", typeof(MonoType));
		value = monoType.TypeHandle.Value;
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
		info.AddValue("TypeObj", Type.GetTypeHandle(this), typeof(MonoType));
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}
		return value == ((RuntimeTypeHandle)obj).Value;
	}

	public override int GetHashCode()
	{
		return value.GetHashCode();
	}
}
