using System.IO;

namespace System.Net.Security;

public abstract class AuthenticatedStream : Stream
{
	private Stream innerStream;

	private bool leaveStreamOpen;

	protected Stream InnerStream => innerStream;

	public abstract bool IsAuthenticated { get; }

	public bool LeaveInnerStreamOpen => leaveStreamOpen;

	protected AuthenticatedStream(Stream innerStream, bool leaveInnerStreamOpen)
	{
		this.innerStream = innerStream;
		leaveStreamOpen = leaveInnerStreamOpen;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && innerStream != null)
		{
			if (!leaveStreamOpen)
			{
				innerStream.Close();
			}
			innerStream = null;
		}
	}
}
