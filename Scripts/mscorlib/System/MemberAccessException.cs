using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System;

[Serializable]
[ComVisible(true)]
public class MemberAccessException : SystemException
{
	public MemberAccessException()
		: base(Locale.GetText("Cannot access a class member."))
	{
		base.HResult = -2146233062;
	}

	public MemberAccessException(string message)
		: base(message)
	{
		base.HResult = -2146233062;
	}

	protected MemberAccessException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
