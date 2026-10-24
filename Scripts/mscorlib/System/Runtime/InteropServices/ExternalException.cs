using System.Runtime.Serialization;

namespace System.Runtime.InteropServices;

[Serializable]
[ComVisible(true)]
public class ExternalException : SystemException
{
	public ExternalException()
		: base(Locale.GetText("External exception"))
	{
		base.HResult = -2147467259;
	}

	public ExternalException(string message)
		: base(message)
	{
		base.HResult = -2147467259;
	}

	protected ExternalException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public ExternalException(string message, int errorCode)
		: base(message)
	{
		base.HResult = errorCode;
	}
}
