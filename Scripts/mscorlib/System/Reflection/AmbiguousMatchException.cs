using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection;

[Serializable]
[ComVisible(true)]
public sealed class AmbiguousMatchException : SystemException
{
	public AmbiguousMatchException()
		: base("Ambiguous matching in method resolution")
	{
	}

	public AmbiguousMatchException(string message)
		: base(message)
	{
	}

	internal AmbiguousMatchException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
