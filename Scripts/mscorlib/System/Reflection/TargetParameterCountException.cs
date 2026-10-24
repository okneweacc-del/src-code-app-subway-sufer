using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection;

[Serializable]
[ComVisible(true)]
public sealed class TargetParameterCountException : Exception
{
	public TargetParameterCountException()
		: base(Locale.GetText("Number of parameter does not match expected count."))
	{
	}

	public TargetParameterCountException(string message)
		: base(message)
	{
	}

	internal TargetParameterCountException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
