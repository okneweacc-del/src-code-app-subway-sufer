using System.Runtime.Serialization;

namespace System.Runtime.InteropServices;

[Serializable]
[ComVisible(true)]
public class COMException : ExternalException
{
	public COMException()
	{
	}

	public COMException(string message)
		: base(message)
	{
	}

	public COMException(string message, int errorCode)
		: base(message, errorCode)
	{
	}

	protected COMException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public override string ToString()
	{
		return $"{GetType()} (0x{base.HResult:x}): {Message} {((InnerException != null) ? InnerException.ToString() : string.Empty)}{Environment.NewLine}{((StackTrace == null) ? string.Empty : StackTrace)}";
	}
}
