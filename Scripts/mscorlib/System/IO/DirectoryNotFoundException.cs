using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO;

[Serializable]
[ComVisible(true)]
public class DirectoryNotFoundException : IOException
{
	public DirectoryNotFoundException()
		: base("Directory not found")
	{
	}

	public DirectoryNotFoundException(string message)
		: base(message)
	{
	}

	protected DirectoryNotFoundException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
