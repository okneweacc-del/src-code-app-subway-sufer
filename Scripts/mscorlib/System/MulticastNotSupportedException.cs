using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System;

[Serializable]
[ComVisible(true)]
public sealed class MulticastNotSupportedException : SystemException
{
	public MulticastNotSupportedException()
		: base(Locale.GetText("This operation cannot be performed with the specified delagates."))
	{
	}

	public MulticastNotSupportedException(string message)
		: base(message)
	{
	}

	internal MulticastNotSupportedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
