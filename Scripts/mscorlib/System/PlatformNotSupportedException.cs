using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System;

[Serializable]
[ComVisible(true)]
public class PlatformNotSupportedException : NotSupportedException
{
	private const int Result = -2146233031;

	public PlatformNotSupportedException()
		: base(Locale.GetText("This platform is not supported."))
	{
		base.HResult = -2146233031;
	}

	protected PlatformNotSupportedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
