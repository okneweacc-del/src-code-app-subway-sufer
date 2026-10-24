using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Collections.Generic;

[Serializable]
[ComVisible(true)]
public class KeyNotFoundException : SystemException, ISerializable
{
	public KeyNotFoundException()
		: base("The given key was not present in the dictionary.")
	{
	}

	public KeyNotFoundException(string message)
		: base(message)
	{
	}

	protected KeyNotFoundException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
