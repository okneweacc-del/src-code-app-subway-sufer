using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO.IsolatedStorage;

[Serializable]
[ComVisible(true)]
public class IsolatedStorageException : Exception
{
	public IsolatedStorageException()
		: base(Locale.GetText("An Isolated storage operation failed."))
	{
	}

	public IsolatedStorageException(string message)
		: base(message)
	{
	}

	protected IsolatedStorageException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
