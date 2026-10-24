namespace System.Net.Sockets;

public class TcpClient : IDisposable
{
	private enum Properties : uint
	{
		LingerState = 1u,
		NoDelay = 2u,
		ReceiveBufferSize = 4u,
		ReceiveTimeout = 8u,
		SendBufferSize = 0x10u,
		SendTimeout = 0x20u
	}

	private NetworkStream stream;

	private bool active;

	private Socket client;

	private bool disposed;

	private Properties values;

	private int recv_timeout;

	private int send_timeout;

	private int recv_buffer_size;

	private int send_buffer_size;

	private LingerOption linger_state;

	private bool no_delay;

	public LingerOption LingerState
	{
		set
		{
			if (!client.Connected)
			{
				linger_state = value;
				values |= Properties.LingerState;
			}
			else
			{
				client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, value);
			}
		}
	}

	public bool NoDelay
	{
		set
		{
			if (!client.Connected)
			{
				no_delay = value;
				values |= Properties.NoDelay;
			}
			else
			{
				client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, value ? 1 : 0);
			}
		}
	}

	public int ReceiveBufferSize
	{
		set
		{
			if (!client.Connected)
			{
				recv_buffer_size = value;
				values |= Properties.ReceiveBufferSize;
			}
			else
			{
				client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, value);
			}
		}
	}

	public int ReceiveTimeout
	{
		set
		{
			if (!client.Connected)
			{
				recv_timeout = value;
				values |= Properties.ReceiveTimeout;
			}
			else
			{
				client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, value);
			}
		}
	}

	public int SendBufferSize
	{
		set
		{
			if (!client.Connected)
			{
				send_buffer_size = value;
				values |= Properties.SendBufferSize;
			}
			else
			{
				client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, value);
			}
		}
	}

	public int SendTimeout
	{
		set
		{
			if (!client.Connected)
			{
				send_timeout = value;
				values |= Properties.SendTimeout;
			}
			else
			{
				client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, value);
			}
		}
	}

	public TcpClient()
	{
		Init(AddressFamily.InterNetwork);
		client.Bind(new IPEndPoint(IPAddress.Any, 0));
	}

	void IDisposable.Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Init(AddressFamily family)
	{
		active = false;
		if (client != null)
		{
			client.Close();
			client = null;
		}
		client = new Socket(family, SocketType.Stream, ProtocolType.Tcp);
	}

	public void Connect(IPEndPoint remote_end_point)
	{
		try
		{
			client.Connect(remote_end_point);
			active = true;
		}
		finally
		{
			CheckDisposed();
		}
	}

	private void SetOptions()
	{
		Properties properties = values;
		values = (Properties)0u;
		if ((properties & Properties.LingerState) != 0)
		{
			LingerState = linger_state;
		}
		if ((properties & Properties.NoDelay) != 0)
		{
			NoDelay = no_delay;
		}
		if ((properties & Properties.ReceiveBufferSize) != 0)
		{
			ReceiveBufferSize = recv_buffer_size;
		}
		if ((properties & Properties.ReceiveTimeout) != 0)
		{
			ReceiveTimeout = recv_timeout;
		}
		if ((properties & Properties.SendBufferSize) != 0)
		{
			SendBufferSize = send_buffer_size;
		}
		if ((properties & Properties.SendTimeout) != 0)
		{
			SendTimeout = send_timeout;
		}
	}

	public void Connect(string hostname, int port)
	{
		IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
		Connect(hostAddresses, port);
	}

	public void Connect(IPAddress[] ipAddresses, int port)
	{
		CheckDisposed();
		if (ipAddresses == null)
		{
			throw new ArgumentNullException("ipAddresses");
		}
		for (int i = 0; i < ipAddresses.Length; i++)
		{
			try
			{
				IPAddress iPAddress = ipAddresses[i];
				if (iPAddress.Equals(IPAddress.Any) || iPAddress.Equals(IPAddress.IPv6Any))
				{
					throw new SocketException(10049);
				}
				Init(iPAddress.AddressFamily);
				if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
				{
					client.Bind(new IPEndPoint(IPAddress.Any, 0));
				}
				else
				{
					if (iPAddress.AddressFamily != AddressFamily.InterNetworkV6)
					{
						throw new NotSupportedException("This method is only valid for sockets in the InterNetwork and InterNetworkV6 families");
					}
					client.Bind(new IPEndPoint(IPAddress.IPv6Any, 0));
				}
				Connect(new IPEndPoint(iPAddress, port));
				if (values != 0)
				{
					SetOptions();
				}
				break;
			}
			catch (Exception ex)
			{
				Init(AddressFamily.InterNetwork);
				if (i == ipAddresses.Length - 1)
				{
					throw ex;
				}
			}
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposed)
		{
			return;
		}
		disposed = true;
		if (disposing)
		{
			NetworkStream networkStream = stream;
			stream = null;
			if (networkStream != null)
			{
				networkStream.Close();
				active = false;
				networkStream = null;
			}
			else if (client != null)
			{
				client.Close();
				client = null;
			}
		}
	}

	~TcpClient()
	{
		Dispose(disposing: false);
	}

	public NetworkStream GetStream()
	{
		try
		{
			if (stream == null)
			{
				stream = new NetworkStream(client, owns_socket: true);
			}
			return stream;
		}
		finally
		{
			CheckDisposed();
		}
	}

	private void CheckDisposed()
	{
		if (disposed)
		{
			throw new ObjectDisposedException(GetType().FullName);
		}
	}
}
