using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Security;

[Serializable]
[ComVisible(true)]
public sealed class XmlSyntaxException : SystemException
{
	public XmlSyntaxException()
	{
	}

	public XmlSyntaxException(string message)
		: base(message)
	{
	}

	public XmlSyntaxException(string message, Exception inner)
		: base(message, inner)
	{
	}

	internal XmlSyntaxException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
