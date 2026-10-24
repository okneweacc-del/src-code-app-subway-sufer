using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection;

[Serializable]
[ComVisible(true)]
public class TargetException : Exception
{
	public TargetException()
		: base(Locale.GetText("Unable to invoke an invalid target."))
	{
	}

	public TargetException(string message)
		: base(message)
	{
	}

	protected TargetException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
