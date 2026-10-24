using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection;

[Serializable]
[ComVisible(true)]
[CLSCompliant(false)]
public sealed class Pointer : ISerializable
{
	private unsafe void* data;

	private Type type;

	private Pointer()
	{
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		throw new NotSupportedException("Pointer deserializatioon not supported.");
	}

	public unsafe static object Box(void* ptr, Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (!type.IsPointer)
		{
			throw new ArgumentException("type");
		}
		Pointer pointer = new Pointer();
		pointer.data = ptr;
		pointer.type = type;
		return pointer;
	}
}
