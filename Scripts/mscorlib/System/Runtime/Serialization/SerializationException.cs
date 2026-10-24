using System.Runtime.InteropServices;

namespace System.Runtime.Serialization;

[Serializable]
[ComVisible(true)]
public class SerializationException : SystemException
{
	public SerializationException()
		: base("An error occurred during (de)serialization")
	{
	}

	public SerializationException(string message)
		: base(message)
	{
	}

	protected SerializationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
