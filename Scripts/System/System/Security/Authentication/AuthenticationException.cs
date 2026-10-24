using System.Runtime.Serialization;

namespace System.Security.Authentication;

[Serializable]
public class AuthenticationException : SystemException
{
	public AuthenticationException()
		: base(global::Locale.GetText("Authentication exception."))
	{
	}

	protected AuthenticationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
		: base(serializationInfo, streamingContext)
	{
	}
}
