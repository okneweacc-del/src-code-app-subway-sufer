using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System;

[Serializable]
[ComVisible(true)]
public class RankException : SystemException
{
	public RankException()
		: base(Locale.GetText("Two arrays must have the same number of dimensions."))
	{
		base.HResult = -2146233065;
	}

	public RankException(string message)
		: base(message)
	{
		base.HResult = -2146233065;
	}

	protected RankException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
