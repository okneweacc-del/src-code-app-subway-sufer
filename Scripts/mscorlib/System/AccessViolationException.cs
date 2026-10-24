using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System;

[Serializable]
[ComVisible(true)]
public class AccessViolationException : SystemException
{
	public AccessViolationException()
		: base(Locale.GetText("Attempted to read or write protected memory. This is often an indication that other memory has been corrupted."))
	{
		base.HResult = -2147467261;
	}

	protected AccessViolationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
