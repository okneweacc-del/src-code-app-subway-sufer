namespace System.Net.Sockets;

public class LingerOption
{
	private bool enabled;

	private int seconds;

	public LingerOption(bool enable, int secs)
	{
		enabled = enable;
		seconds = secs;
	}
}
