using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace System.Net.Sockets;

public class Socket : IDisposable
{
	private enum SocketOperation
	{
		Accept = 0,
		Connect = 1,
		Receive = 2,
		ReceiveFrom = 3,
		Send = 4,
		SendTo = 5,
		UsedInManaged1 = 6,
		UsedInManaged2 = 7,
		UsedInProcess = 8,
		UsedInConsole2 = 9,
		Disconnect = 10,
		AcceptReceive = 11,
		ReceiveGeneric = 12,
		SendGeneric = 13
	}

	[StructLayout((LayoutKind)0)]
	private sealed class SocketAsyncResult : IAsyncResult
	{
		public Socket Sock;

		public IntPtr handle;

		private object state;

		private AsyncCallback callback;

		private WaitHandle waithandle;

		private Exception delayedException;

		public EndPoint EndPoint;

		public byte[] Buffer;

		public int Offset;

		public int Size;

		public SocketFlags SockFlags;

		public Socket AcceptSocket;

		public IPAddress[] Addresses;

		public int Port;

		public IList<ArraySegment<byte>> Buffers;

		public bool ReuseSocket;

		private Socket acc_socket;

		private int total;

		private bool completed_sync;

		private bool completed;

		public bool blocking;

		internal int error;

		private SocketOperation operation;

		public object ares;

		public int EndCalled;

		public object AsyncState => state;

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				lock (this)
				{
					if (waithandle == null)
					{
						waithandle = new ManualResetEvent(completed);
					}
				}
				return waithandle;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return completed;
			}
			set
			{
				completed = value;
				lock (this)
				{
					if (waithandle != null && value)
					{
						((ManualResetEvent)waithandle).Set();
					}
				}
			}
		}

		public int Total
		{
			get
			{
				return total;
			}
			set
			{
				total = value;
			}
		}

		public SocketError ErrorCode
		{
			get
			{
				if (delayedException is SocketException ex)
				{
					return ex.SocketErrorCode;
				}
				if (error != 0)
				{
					return (SocketError)error;
				}
				return SocketError.Success;
			}
		}

		public SocketAsyncResult(Socket sock, object state, AsyncCallback callback, SocketOperation operation)
		{
			Sock = sock;
			blocking = sock.blocking;
			handle = sock.socket;
			this.state = state;
			this.callback = callback;
			this.operation = operation;
			SockFlags = SocketFlags.None;
		}

		public void CheckIfThrowDelayedException()
		{
			if (delayedException != null)
			{
				Sock.connected = false;
				throw delayedException;
			}
			if (error != 0)
			{
				Sock.connected = false;
				throw new SocketException(error);
			}
		}

		private void CompleteAllOnDispose(Queue queue)
		{
			object[] array = queue.ToArray();
			queue.Clear();
			for (int i = 0; i < array.Length; i++)
			{
				SocketAsyncResult socketAsyncResult = (SocketAsyncResult)array[i];
				WaitCallback callBack = socketAsyncResult.CompleteDisposed;
				ThreadPool.QueueUserWorkItem(callBack, null);
			}
			if (array.Length == 0)
			{
				Buffer = null;
			}
		}

		private void CompleteDisposed(object unused)
		{
			Complete();
		}

		public void Complete()
		{
			if (operation != SocketOperation.Receive && Sock.disposed)
			{
				delayedException = new ObjectDisposedException(Sock.GetType().ToString());
			}
			IsCompleted = true;
			Queue queue = null;
			if (operation == SocketOperation.Receive || operation == SocketOperation.ReceiveFrom || operation == SocketOperation.ReceiveGeneric)
			{
				queue = Sock.readQ;
			}
			else if (operation == SocketOperation.Send || operation == SocketOperation.SendTo || operation == SocketOperation.SendGeneric)
			{
				queue = Sock.writeQ;
			}
			if (queue != null)
			{
				SocketAsyncCall socketAsyncCall = null;
				SocketAsyncResult socketAsyncResult = null;
				lock (queue)
				{
					queue.Dequeue();
					if (queue.Count > 0)
					{
						socketAsyncResult = (SocketAsyncResult)queue.Peek();
						if (!Sock.disposed)
						{
							Worker worker = new Worker(socketAsyncResult);
							socketAsyncCall = GetDelegate(worker, socketAsyncResult.operation);
						}
						else
						{
							CompleteAllOnDispose(queue);
						}
					}
				}
				socketAsyncCall?.BeginInvoke(null, socketAsyncResult);
			}
			if (callback != null)
			{
				callback(this);
			}
			Buffer = null;
		}

		private SocketAsyncCall GetDelegate(Worker worker, SocketOperation op)
		{
			return op switch
			{
				SocketOperation.Receive => worker.Receive, 
				SocketOperation.ReceiveFrom => worker.ReceiveFrom, 
				SocketOperation.Send => worker.Send, 
				SocketOperation.SendTo => worker.SendTo, 
				_ => null, 
			};
		}

		public void Complete(int total)
		{
			this.total = total;
			Complete();
		}

		public void Complete(Exception e)
		{
			delayedException = e;
			Complete();
		}
	}

	private sealed class Worker
	{
		private SocketAsyncResult result;

		private bool requireSocketSecurity;

		private int send_so_far;

		public Worker(SocketAsyncResult ares)
			: this(ares, requireSocketSecurity: true)
		{
		}

		public Worker(SocketAsyncResult ares, bool requireSocketSecurity)
		{
			result = ares;
			this.requireSocketSecurity = requireSocketSecurity;
		}

		public void Receive()
		{
			result.Complete();
		}

		public void ReceiveFrom()
		{
			int num = 0;
			try
			{
				num = result.Sock.ReceiveFrom_nochecks(result.Buffer, result.Offset, result.Size, result.SockFlags, ref result.EndPoint);
			}
			catch (Exception e)
			{
				result.Complete(e);
				return;
			}
			result.Complete(num);
		}

		private void UpdateSendValues(int last_sent)
		{
			if (result.error == 0)
			{
				send_so_far += last_sent;
				result.Offset += last_sent;
				result.Size -= last_sent;
			}
		}

		public void Send()
		{
			if (result.error == 0)
			{
				UpdateSendValues(result.Total);
				if (result.Sock.disposed)
				{
					result.Complete();
					return;
				}
				if (result.Size > 0)
				{
					SocketAsyncCall socketAsyncCall = Send;
					socketAsyncCall.BeginInvoke(null, result);
					return;
				}
				result.Total = send_so_far;
			}
			result.Complete();
		}

		public void SendTo()
		{
			int num = 0;
			try
			{
				num = result.Sock.SendTo_nochecks(result.Buffer, result.Offset, result.Size, result.SockFlags, result.EndPoint);
				UpdateSendValues(num);
				if (result.Size > 0)
				{
					SocketAsyncCall socketAsyncCall = SendTo;
					socketAsyncCall.BeginInvoke(null, result);
					return;
				}
				result.Total = send_so_far;
			}
			catch (Exception e)
			{
				result.Complete(e);
				return;
			}
			result.Complete();
		}
	}

	private delegate void SocketAsyncCall();

	private Queue readQ = new Queue(2);

	private Queue writeQ = new Queue(2);

	private bool islistening;

	private bool useoverlappedIO;

	private readonly int MinListenPort = 7100;

	private readonly int MaxListenPort = 7150;

	private static int ipv4Supported;

	private static int ipv6Supported;

	private int linger_timeout;

	private IntPtr socket;

	private AddressFamily address_family;

	private SocketType socket_type;

	private ProtocolType protocol_type;

	internal bool blocking = true;

	private Thread blocking_thread;

	private bool isbound;

	private static int current_bind_count;

	private readonly int max_bind_count = 50;

	private bool connected;

	private bool closed;

	internal bool disposed;

	internal EndPoint seed_endpoint;

	private static MethodInfo check_socket_policy;

	public SocketType SocketType => socket_type;

	public int ReceiveTimeout
	{
		set
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (value < -1)
			{
				throw new ArgumentOutOfRangeException("value", "The value specified for a set operation is less than -1");
			}
			if (value == -1)
			{
				value = 0;
			}
			SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, value);
		}
	}

	public static bool SupportsIPv4
	{
		get
		{
			CheckProtocolSupport();
			return ipv4Supported == 1;
		}
	}

	[Obsolete("Use OSSupportsIPv6 instead")]
	public static bool SupportsIPv6
	{
		get
		{
			CheckProtocolSupport();
			return ipv6Supported == 1;
		}
	}

	public bool Blocking => blocking;

	public bool Connected => connected;

	public Socket(AddressFamily family, SocketType type, ProtocolType proto)
	{
		if (family == AddressFamily.Unspecified)
		{
			throw new ArgumentException("family");
		}
		address_family = family;
		socket_type = type;
		protocol_type = proto;
		socket = Socket_internal(family, type, proto, out var error);
		if (error != 0)
		{
			throw new SocketException(error);
		}
	}

	static Socket()
	{
		ipv4Supported = -1;
		ipv6Supported = -1;
		CheckProtocolSupport();
	}

	public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socket_flags, AsyncCallback callback, object state)
	{
		if (disposed && closed)
		{
			throw new ObjectDisposedException(GetType().ToString());
		}
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (offset < 0 || offset > buffer.Length)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		if (size < 0 || offset + size > buffer.Length)
		{
			throw new ArgumentOutOfRangeException("size");
		}
		SocketAsyncResult socketAsyncResult;
		lock (readQ)
		{
			socketAsyncResult = new SocketAsyncResult(this, state, callback, SocketOperation.Receive);
			socketAsyncResult.Buffer = buffer;
			socketAsyncResult.Offset = offset;
			socketAsyncResult.Size = size;
			socketAsyncResult.SockFlags = socket_flags;
			readQ.Enqueue(socketAsyncResult);
			if (readQ.Count == 1)
			{
				Worker worker = new Worker(socketAsyncResult);
				SocketAsyncCall socketAsyncCall = worker.Receive;
				socketAsyncCall.BeginInvoke(null, socketAsyncResult);
			}
		}
		return socketAsyncResult;
	}

	public IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socket_flags, AsyncCallback callback, object state)
	{
		if (disposed && closed)
		{
			throw new ObjectDisposedException(GetType().ToString());
		}
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset", "offset must be >= 0");
		}
		if (size < 0)
		{
			throw new ArgumentOutOfRangeException("size", "size must be >= 0");
		}
		if (offset + size > buffer.Length)
		{
			throw new ArgumentOutOfRangeException("offset, size", "offset + size exceeds the buffer length");
		}
		if (!connected)
		{
			throw new SocketException(10057);
		}
		SocketAsyncResult socketAsyncResult;
		lock (writeQ)
		{
			socketAsyncResult = new SocketAsyncResult(this, state, callback, SocketOperation.Send);
			socketAsyncResult.Buffer = buffer;
			socketAsyncResult.Offset = offset;
			socketAsyncResult.Size = size;
			socketAsyncResult.SockFlags = socket_flags;
			writeQ.Enqueue(socketAsyncResult);
			if (writeQ.Count == 1)
			{
				Worker worker = new Worker(socketAsyncResult);
				SocketAsyncCall socketAsyncCall = worker.Send;
				socketAsyncCall.BeginInvoke(null, socketAsyncResult);
			}
		}
		return socketAsyncResult;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Bind_internal(IntPtr sock, SocketAddress sa, out int error);

	public void Bind(EndPoint local_end)
	{
		if (disposed && closed)
		{
			throw new ObjectDisposedException(GetType().ToString());
		}
		if (local_end == null)
		{
			throw new ArgumentNullException("local_end");
		}
		if (Environment.SocketSecurityEnabled && current_bind_count >= max_bind_count)
		{
			throw new SecurityException("Too many sockets are bound, maximum count in the webplayer is " + max_bind_count);
		}
		Bind_internal(socket, local_end.Serialize(), out var error);
		if (error != 0)
		{
			throw new SocketException(error);
		}
		if (error == 0)
		{
			isbound = true;
		}
		if (Environment.SocketSecurityEnabled)
		{
			current_bind_count++;
		}
		seed_endpoint = local_end;
	}

	public int EndReceive(IAsyncResult result)
	{
		SocketError errorCode;
		return EndReceive(result, out errorCode);
	}

	public int EndReceive(IAsyncResult asyncResult, out SocketError errorCode)
	{
		if (disposed && closed)
		{
			throw new ObjectDisposedException(GetType().ToString());
		}
		if (asyncResult == null)
		{
			throw new ArgumentNullException("asyncResult");
		}
		if (!(asyncResult is SocketAsyncResult socketAsyncResult))
		{
			throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
		}
		if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
		{
			throw InvalidAsyncOp("EndReceive");
		}
		if (!asyncResult.IsCompleted)
		{
			asyncResult.AsyncWaitHandle.WaitOne();
		}
		errorCode = socketAsyncResult.ErrorCode;
		socketAsyncResult.CheckIfThrowDelayedException();
		return socketAsyncResult.Total;
	}

	public int EndSend(IAsyncResult result)
	{
		SocketError errorCode;
		return EndSend(result, out errorCode);
	}

	public int EndSend(IAsyncResult asyncResult, out SocketError errorCode)
	{
		if (disposed && closed)
		{
			throw new ObjectDisposedException(GetType().ToString());
		}
		if (asyncResult == null)
		{
			throw new ArgumentNullException("asyncResult");
		}
		if (!(asyncResult is SocketAsyncResult socketAsyncResult))
		{
			throw new ArgumentException("Invalid IAsyncResult", "result");
		}
		if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
		{
			throw InvalidAsyncOp("EndSend");
		}
		if (!asyncResult.IsCompleted)
		{
			asyncResult.AsyncWaitHandle.WaitOne();
		}
		errorCode = socketAsyncResult.ErrorCode;
		socketAsyncResult.CheckIfThrowDelayedException();
		return socketAsyncResult.Total;
	}

	private Exception InvalidAsyncOp(string method)
	{
		return new InvalidOperationException(method + " can only be called once per asynchronous operation");
	}

	public int Receive(byte[] buffer, int size, SocketFlags flags)
	{
		if (disposed && closed)
		{
			throw new ObjectDisposedException(GetType().ToString());
		}
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (size < 0 || size > buffer.Length)
		{
			throw new ArgumentOutOfRangeException("size");
		}
		int result = Receive_nochecks(buffer, 0, size, flags, out var error);
		if (error != SocketError.Success)
		{
			if (error == SocketError.WouldBlock && blocking)
			{
				throw new SocketException((int)error, "Operation timed out.");
			}
			throw new SocketException((int)error);
		}
		return result;
	}

	public int Receive(byte[] buffer, int offset, int size, SocketFlags flags)
	{
		if (disposed && closed)
		{
			throw new ObjectDisposedException(GetType().ToString());
		}
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (offset < 0 || offset > buffer.Length)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		if (size < 0 || offset + size > buffer.Length)
		{
			throw new ArgumentOutOfRangeException("size");
		}
		int result = Receive_nochecks(buffer, offset, size, flags, out var error);
		if (error != SocketError.Success)
		{
			if (error == SocketError.WouldBlock && blocking)
			{
				throw new SocketException((int)error, "Operation timed out.");
			}
			throw new SocketException((int)error);
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int RecvFrom_internal(IntPtr sock, byte[] buffer, int offset, int count, SocketFlags flags, ref SocketAddress sockaddr, out int error);

	internal int ReceiveFrom_nochecks(byte[] buf, int offset, int size, SocketFlags flags, ref EndPoint remote_end)
	{
		int error;
		return ReceiveFrom_nochecks_exc(buf, offset, size, flags, ref remote_end, throwOnError: true, out error);
	}

	internal int ReceiveFrom_nochecks_exc(byte[] buf, int offset, int size, SocketFlags flags, ref EndPoint remote_end, bool throwOnError, out int error)
	{
		SocketAddress sockaddr = remote_end.Serialize();
		int result = RecvFrom_internal(socket, buf, offset, size, flags, ref sockaddr, out error);
		SocketError socketError = (SocketError)error;
		if (socketError != SocketError.Success)
		{
			if (socketError != SocketError.WouldBlock && socketError != SocketError.InProgress)
			{
				connected = false;
			}
			else if (socketError == SocketError.WouldBlock && blocking)
			{
				if (throwOnError)
				{
					throw new SocketException(10060, "Operation timed out");
				}
				error = 10060;
				return 0;
			}
			if (throwOnError)
			{
				throw new SocketException(error);
			}
			return 0;
		}
		if (Environment.SocketSecurityEnabled && !CheckEndPoint(sockaddr))
		{
			buf.Initialize();
			throw new SecurityException("Unable to connect, as no valid crossdomain policy was found");
		}
		connected = true;
		isbound = true;
		if (sockaddr != null)
		{
			remote_end = remote_end.Create(sockaddr);
		}
		seed_endpoint = remote_end;
		return result;
	}

	public int Send(byte[] buf, int size, SocketFlags flags)
	{
		if (disposed && closed)
		{
			throw new ObjectDisposedException(GetType().ToString());
		}
		if (buf == null)
		{
			throw new ArgumentNullException("buf");
		}
		if (size < 0 || size > buf.Length)
		{
			throw new ArgumentOutOfRangeException("size");
		}
		int result = Send_nochecks(buf, 0, size, flags, out var error);
		if (error != SocketError.Success)
		{
			throw new SocketException((int)error);
		}
		return result;
	}

	public int Send(byte[] buf, int offset, int size, SocketFlags flags)
	{
		if (disposed && closed)
		{
			throw new ObjectDisposedException(GetType().ToString());
		}
		if (buf == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (offset < 0 || offset > buf.Length)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		if (size < 0 || offset + size > buf.Length)
		{
			throw new ArgumentOutOfRangeException("size");
		}
		int result = Send_nochecks(buf, offset, size, flags, out var error);
		if (error != SocketError.Success)
		{
			throw new SocketException((int)error);
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int SendTo_internal_real(IntPtr sock, byte[] buffer, int offset, int count, SocketFlags flags, SocketAddress sa, out int error);

	private static int SendTo_internal(IntPtr sock, byte[] buffer, int offset, int count, SocketFlags flags, SocketAddress sa, out int error)
	{
		if (Environment.SocketSecurityEnabled && !CheckEndPoint(sa))
		{
			SecurityException ex = new SecurityException("SendTo request refused by Unity webplayer security model");
			Console.WriteLine("Throwing the following security exception: " + ex);
			throw ex;
		}
		return SendTo_internal_real(sock, buffer, offset, count, flags, sa, out error);
	}

	internal int SendTo_nochecks(byte[] buffer, int offset, int size, SocketFlags flags, EndPoint remote_end)
	{
		SocketAddress sa = remote_end.Serialize();
		int result = SendTo_internal(socket, buffer, offset, size, flags, sa, out var error);
		SocketError socketError = (SocketError)error;
		if (socketError != SocketError.Success)
		{
			if (socketError != SocketError.WouldBlock && socketError != SocketError.InProgress)
			{
				connected = false;
			}
			throw new SocketException(error);
		}
		connected = true;
		isbound = true;
		seed_endpoint = remote_end;
		return result;
	}

	public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, object optionValue)
	{
		if (disposed && closed)
		{
			throw new ObjectDisposedException(GetType().ToString());
		}
		if (optionValue == null)
		{
			throw new ArgumentNullException("optionValue");
		}
		int error;
		if (optionLevel == SocketOptionLevel.Socket && optionName == SocketOptionName.Linger)
		{
			if (!(optionValue is LingerOption obj_val))
			{
				throw new ArgumentException("A 'LingerOption' value must be specified.", "optionValue");
			}
			SetSocketOption_internal(socket, optionLevel, optionName, obj_val, null, 0, out error);
		}
		else if (optionLevel == SocketOptionLevel.IP && (optionName == SocketOptionName.AddMembership || optionName == SocketOptionName.DropMembership))
		{
			if (!(optionValue is MulticastOption obj_val2))
			{
				throw new ArgumentException("A 'MulticastOption' value must be specified.", "optionValue");
			}
			SetSocketOption_internal(socket, optionLevel, optionName, obj_val2, null, 0, out error);
		}
		else
		{
			if (optionLevel != SocketOptionLevel.IPv6 || (optionName != SocketOptionName.AddMembership && optionName != SocketOptionName.DropMembership))
			{
				throw new ArgumentException("Invalid value specified.", "optionValue");
			}
			if (!(optionValue is IPv6MulticastOption obj_val3))
			{
				throw new ArgumentException("A 'IPv6MulticastOption' value must be specified.", "optionValue");
			}
			SetSocketOption_internal(socket, optionLevel, optionName, obj_val3, null, 0, out error);
		}
		switch (error)
		{
		case 10022:
			throw new ArgumentException();
		default:
			throw new SocketException(error);
		case 0:
			break;
		}
	}

	internal static void CheckProtocolSupport()
	{
		if (ipv4Supported == -1)
		{
			try
			{
				Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Close();
				ipv4Supported = 1;
			}
			catch
			{
				ipv4Supported = 0;
			}
		}
		if (ipv6Supported == -1 && ipv6Supported != 0)
		{
			try
			{
				Socket socket2 = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
				socket2.Close();
				ipv6Supported = 1;
			}
			catch
			{
				ipv6Supported = 0;
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern IntPtr Socket_internal(AddressFamily family, SocketType type, ProtocolType proto, out int error);

	~Socket()
	{
		Dispose(explicitDisposing: false);
	}

	private void Linger(IntPtr handle)
	{
		if (!connected || linger_timeout <= 0)
		{
			return;
		}
		Shutdown_internal(handle, SocketShutdown.Receive, out var error);
		if (error != 0)
		{
			return;
		}
		int num = linger_timeout / 1000;
		int num2 = linger_timeout % 1000;
		if (num2 > 0)
		{
			Poll_internal(handle, SelectMode.SelectRead, num2 * 1000, out error);
			if (error != 0)
			{
				return;
			}
		}
		if (num > 0)
		{
			LingerOption obj_val = new LingerOption(enable: true, num);
			SetSocketOption_internal(handle, SocketOptionLevel.Socket, SocketOptionName.Linger, obj_val, null, 0, out error);
		}
	}

	protected virtual void Dispose(bool explicitDisposing)
	{
		if (disposed)
		{
			return;
		}
		disposed = true;
		bool flag = connected;
		connected = false;
		if ((int)socket != -1)
		{
			if (Environment.SocketSecurityEnabled && current_bind_count > 0)
			{
				current_bind_count--;
			}
			closed = true;
			IntPtr handle = socket;
			socket = (IntPtr)(-1);
			Thread thread = blocking_thread;
			if (thread != null)
			{
				thread.Abort();
				blocking_thread = null;
			}
			if (flag)
			{
				Linger(handle);
			}
			Close_internal(handle, out var error);
			if (error != 0)
			{
				throw new SocketException(error);
			}
		}
	}

	public void Dispose()
	{
		Dispose(explicitDisposing: true);
		GC.SuppressFinalize(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Close_internal(IntPtr socket, out int error);

	public void Close()
	{
		linger_timeout = 0;
		((IDisposable)this).Dispose();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Connect_internal_real(IntPtr sock, SocketAddress sa, out int error);

	private static void Connect_internal(IntPtr sock, SocketAddress sa, out int error, bool requireSocketPolicyFile)
	{
		if (requireSocketPolicyFile && !CheckEndPoint(sa))
		{
			throw new SecurityException("Unable to connect, as no valid crossdomain policy was found");
		}
		Connect_internal_real(sock, sa, out error);
	}

	internal static bool CheckEndPoint(SocketAddress sa)
	{
		if (!Environment.SocketSecurityEnabled)
		{
			return true;
		}
		try
		{
			IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Loopback, 123);
			IPEndPoint iPEndPoint2 = (IPEndPoint)iPEndPoint.Create(sa);
			if (check_socket_policy == null)
			{
				check_socket_policy = GetUnityCrossDomainHelperMethod("CheckSocketEndPoint");
			}
			return (bool)check_socket_policy.Invoke(null, new object[2]
			{
				iPEndPoint2.Address.ToString(),
				iPEndPoint2.Port
			});
		}
		catch (Exception ex)
		{
			Console.WriteLine("Unexpected error while trying to CheckEndPoint() : " + ex);
			return false;
		}
	}

	private static MethodInfo GetUnityCrossDomainHelperMethod(string methodname)
	{
		Type type = Type.GetType("UnityEngine.UnityCrossDomainHelper, CrossDomainPolicyParser, Version=1.0.0.0, Culture=neutral");
		if (type == null)
		{
			throw new SecurityException("Cant find type UnityCrossDomainHelper");
		}
		MethodInfo method = type.GetMethod(methodname);
		if (method == null)
		{
			throw new SecurityException("Cant find " + methodname);
		}
		return method;
	}

	public void Connect(EndPoint remoteEP)
	{
		Connect(remoteEP, requireSocketPolicy: true);
	}

	internal void Connect(EndPoint remoteEP, bool requireSocketPolicy)
	{
		SocketAddress socketAddress = null;
		if (disposed && closed)
		{
			throw new ObjectDisposedException(GetType().ToString());
		}
		if (remoteEP == null)
		{
			throw new ArgumentNullException("remoteEP");
		}
		if (remoteEP is IPEndPoint iPEndPoint && (iPEndPoint.Address.Equals(IPAddress.Any) || iPEndPoint.Address.Equals(IPAddress.IPv6Any)))
		{
			throw new SocketException(10049);
		}
		if (islistening)
		{
			throw new InvalidOperationException();
		}
		socketAddress = remoteEP.Serialize();
		int error = 0;
		blocking_thread = Thread.CurrentThread;
		try
		{
			Connect_internal(socket, socketAddress, out error, requireSocketPolicy);
		}
		catch (ThreadAbortException)
		{
			if (disposed)
			{
				Thread.ResetAbort();
				error = 10004;
			}
		}
		finally
		{
			blocking_thread = null;
		}
		if (error != 0)
		{
			throw new SocketException(error);
		}
		connected = true;
		isbound = true;
		seed_endpoint = remoteEP;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Poll_internal(IntPtr socket, SelectMode mode, int timeout, out int error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int Receive_internal(IntPtr sock, byte[] buffer, int offset, int count, SocketFlags flags, out int error);

	internal int Receive_nochecks(byte[] buf, int offset, int size, SocketFlags flags, out SocketError error)
	{
		if (protocol_type == ProtocolType.Udp && Environment.SocketSecurityEnabled)
		{
			IPAddress address = IPAddress.Any;
			if (address_family == AddressFamily.InterNetworkV6)
			{
				address = IPAddress.IPv6Any;
			}
			EndPoint remote_end = new IPEndPoint(address, 0);
			int error2 = 0;
			int result = ReceiveFrom_nochecks_exc(buf, offset, size, flags, ref remote_end, throwOnError: false, out error2);
			error = (SocketError)error2;
			return result;
		}
		int result2 = Receive_internal(socket, buf, offset, size, flags, out var error3);
		error = (SocketError)error3;
		if (error != SocketError.Success && error != SocketError.WouldBlock && error != SocketError.InProgress)
		{
			connected = false;
		}
		else
		{
			connected = true;
		}
		return result2;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int Send_internal(IntPtr sock, byte[] buf, int offset, int count, SocketFlags flags, out int error);

	internal int Send_nochecks(byte[] buf, int offset, int size, SocketFlags flags, out SocketError error)
	{
		if (size == 0)
		{
			error = SocketError.Success;
			return 0;
		}
		int result = Send_internal(socket, buf, offset, size, flags, out var error2);
		error = (SocketError)error2;
		if (error != SocketError.Success && error != SocketError.WouldBlock && error != SocketError.InProgress)
		{
			connected = false;
		}
		else
		{
			connected = true;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Shutdown_internal(IntPtr socket, SocketShutdown how, out int error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetSocketOption_internal(IntPtr socket, SocketOptionLevel level, SocketOptionName name, object obj_val, byte[] byte_val, int int_val, out int error);

	public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue)
	{
		if (disposed && closed)
		{
			throw new ObjectDisposedException(GetType().ToString());
		}
		SetSocketOption_internal(socket, optionLevel, optionName, null, null, optionValue, out var error);
		if (error != 0)
		{
			throw new SocketException(error);
		}
	}
}
