using System.Runtime.Serialization;

namespace System.Runtime.InteropServices;

[Serializable]
[ComVisible(true)]
public class InvalidComObjectException : SystemException
{
	public InvalidComObjectException()
		: base(Locale.GetText("Invalid COM object is used"))
	{
		base.HResult = -2146233049;
	}

	public InvalidComObjectException(string message)
		: base(message)
	{
		base.HResult = -2146233049;
	}

	protected InvalidComObjectException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
