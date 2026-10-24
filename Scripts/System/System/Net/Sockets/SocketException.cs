using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Net.Sockets;

[Serializable]
public class SocketException : Win32Exception
{
	public SocketError SocketErrorCode => (SocketError)base.NativeErrorCode;

	public override string Message => base.Message;

	public SocketException()
		: base(WSAGetLastError_internal())
	{
	}

	public SocketException(int error)
		: base(error)
	{
	}

	protected SocketException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	internal SocketException(int error, string message)
		: base(error, message)
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int WSAGetLastError_internal();
}
