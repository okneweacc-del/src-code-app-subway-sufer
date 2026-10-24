using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System;

[Serializable]
[ComVisible(true)]
public class FieldAccessException : MemberAccessException
{
	public FieldAccessException()
		: base(Locale.GetText("Attempt to access a private/protected field failed."))
	{
		base.HResult = -2146233081;
	}

	public FieldAccessException(string message)
		: base(message)
	{
		base.HResult = -2146233081;
	}

	protected FieldAccessException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
