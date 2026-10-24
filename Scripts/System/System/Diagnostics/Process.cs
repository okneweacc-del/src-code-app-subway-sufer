using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.Diagnostics;

[DefaultProperty("StartInfo")]
[DefaultEvent("Exited")]
[Designer("System.Diagnostics.Design.ProcessDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[MonitoringDescription("Represents a system process")]
public class Process : Component
{
	private struct ProcInfo
	{
		public IntPtr process_handle;

		public IntPtr thread_handle;

		public int pid;

		public int tid;

		public string[] envKeys;

		public string[] envValues;

		public string UserName;

		public string Domain;

		public IntPtr Password;

		public bool LoadUserProfile;
	}

	[Flags]
	private enum AsyncModes
	{
		NoneYet = 0,
		SyncOutput = 1,
		SyncError = 2,
		AsyncOutput = 4,
		AsyncError = 8
	}

	[StructLayout((LayoutKind)0)]
	private sealed class ProcessAsyncReader
	{
		public object Sock;

		public IntPtr handle;

		public object state;

		public AsyncCallback callback;

		public ManualResetEvent wait_handle;

		public Exception delayedException;

		public object EndPoint;

		private byte[] buffer;

		public int Offset;

		public int Size;

		public int SockFlags;

		public object AcceptSocket;

		public object[] Addresses;

		public int port;

		public object Buffers;

		public bool ReuseSocket;

		public object acc_socket;

		public int total;

		public bool completed_sync;

		private bool completed;

		private bool err_out;

		internal int error;

		public int operation;

		public object ares;

		public int EndCalled;

		private Process process;

		private Stream stream;

		private StringBuilder sb;

		private Encoding outputEncoding;

		public AsyncReadHandler ReadHandler;

		public void Close()
		{
			stream.Close();
		}
	}

	private delegate void AsyncReadHandler();

	private IntPtr process_handle;

	private int pid;

	private bool enableRaisingEvents;

	private bool already_waiting;

	private ISynchronizeInvoke synchronizingObject;

	private EventHandler exited_event;

	private IntPtr stdout_rd;

	private IntPtr stderr_rd;

	private ProcessModuleCollection module_collection;

	private string process_name;

	private StreamReader error_stream;

	private StreamWriter input_stream;

	private StreamReader output_stream;

	private ProcessStartInfo start_info;

	private AsyncModes async_mode;

	private bool output_canceled;

	private bool error_canceled;

	private ProcessAsyncReader async_output;

	private ProcessAsyncReader async_error;

	private bool disposed;

	private DataReceivedEventHandler OutputDataReceived;

	private DataReceivedEventHandler ErrorDataReceived;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("Handle for this process.")]
	public IntPtr Handle => process_handle;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("The name of this process.")]
	public string ProcessName
	{
		get
		{
			if (process_name == null)
			{
				if (process_handle == IntPtr.Zero)
				{
					throw new InvalidOperationException("No process is associated with this object.");
				}
				process_name = ProcessName_internal(process_handle);
				if (process_name == null)
				{
					throw new InvalidOperationException("Process has exited, so the requested information is not available.");
				}
				if (process_name.EndsWith(".exe") || process_name.EndsWith(".bat") || process_name.EndsWith(".com"))
				{
					process_name = process_name.Substring(0, process_name.Length - 4);
				}
			}
			return process_name;
		}
	}

	[Browsable(false)]
	[MonitoringDescription("Information for the start of this process.")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public ProcessStartInfo StartInfo
	{
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			start_info = value;
		}
	}

	private static bool IsWindows
	{
		get
		{
			PlatformID platform = Environment.OSVersion.Platform;
			if (platform == PlatformID.Win32S || platform == PlatformID.Win32Windows || platform == PlatformID.Win32NT || platform == PlatformID.WinCE)
			{
				return true;
			}
			return false;
		}
	}

	private Process(IntPtr handle, int id)
	{
		process_handle = handle;
		pid = id;
	}

	public Process()
	{
	}

	private void StartExitCallbackIfNeeded()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern string ProcessName_internal(IntPtr handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetProcess_internal(int pid);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetPid_internal();

	public static Process GetCurrentProcess()
	{
		int pid_internal = GetPid_internal();
		IntPtr process_internal = GetProcess_internal(pid_internal);
		if (process_internal == IntPtr.Zero)
		{
			throw new SystemException("Can't find current process");
		}
		return new Process(process_internal, pid_internal);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ShellExecuteEx_internal(ProcessStartInfo startInfo, ref ProcInfo proc_info);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CreateProcess_internal(ProcessStartInfo startInfo, IntPtr stdin, IntPtr stdout, IntPtr stderr, ref ProcInfo proc_info);

	private static bool Start_shell(ProcessStartInfo startInfo, Process process)
	{
		ProcInfo proc_info = default;
		if (startInfo.RedirectStandardInput || startInfo.RedirectStandardOutput || startInfo.RedirectStandardError)
		{
			throw new InvalidOperationException("UseShellExecute must be false when redirecting I/O.");
		}
		if (startInfo.HaveEnvVars)
		{
			throw new InvalidOperationException("UseShellExecute must be false in order to use environment variables.");
		}
		FillUserInfo(startInfo, ref proc_info);
		bool flag;
		try
		{
			flag = ShellExecuteEx_internal(startInfo, ref proc_info);
		}
		finally
		{
			if (proc_info.Password != IntPtr.Zero)
			{
				Marshal.FreeBSTR(proc_info.Password);
			}
			proc_info.Password = IntPtr.Zero;
		}
		if (!flag)
		{
			throw new Win32Exception(-proc_info.pid);
		}
		process.process_handle = proc_info.process_handle;
		process.pid = proc_info.pid;
		process.StartExitCallbackIfNeeded();
		return flag;
	}

	private static bool Start_noshell(ProcessStartInfo startInfo, Process process)
	{
		ProcInfo proc_info = default;
		IntPtr read_handle = IntPtr.Zero;
		IntPtr write_handle = IntPtr.Zero;
		if (startInfo.HaveEnvVars)
		{
			string[] array = new string[startInfo.EnvironmentVariables.Count];
			startInfo.EnvironmentVariables.Keys.CopyTo(array, 0);
			proc_info.envKeys = array;
			array = new string[startInfo.EnvironmentVariables.Count];
			startInfo.EnvironmentVariables.Values.CopyTo(array, 0);
			proc_info.envValues = array;
		}
		bool flag;
		System.IO.MonoIOError error;
		if (startInfo.RedirectStandardInput)
		{
			if (IsWindows)
			{
				int options = 2;
				flag = System.IO.MonoIO.CreatePipe(out read_handle, out var write_handle2);
				if (flag)
				{
					flag = System.IO.MonoIO.DuplicateHandle(GetCurrentProcess().Handle, write_handle2, GetCurrentProcess().Handle, out write_handle, 0, 0, options);
					System.IO.MonoIO.Close(write_handle2, out error);
				}
			}
			else
			{
				flag = System.IO.MonoIO.CreatePipe(out read_handle, out write_handle);
			}
			if (!flag)
			{
				throw new IOException("Error creating standard input pipe");
			}
		}
		else
		{
			read_handle = System.IO.MonoIO.ConsoleInput;
			write_handle = (IntPtr)0;
		}
		IntPtr write_handle3;
		if (startInfo.RedirectStandardOutput)
		{
			IntPtr read_handle2 = IntPtr.Zero;
			if (IsWindows)
			{
				int options2 = 2;
				flag = System.IO.MonoIO.CreatePipe(out var read_handle3, out write_handle3);
				if (flag)
				{
					System.IO.MonoIO.DuplicateHandle(GetCurrentProcess().Handle, read_handle3, GetCurrentProcess().Handle, out read_handle2, 0, 0, options2);
					System.IO.MonoIO.Close(read_handle3, out error);
				}
			}
			else
			{
				flag = System.IO.MonoIO.CreatePipe(out read_handle2, out write_handle3);
			}
			process.stdout_rd = read_handle2;
			if (!flag)
			{
				if (startInfo.RedirectStandardInput)
				{
					System.IO.MonoIO.Close(read_handle, out error);
					System.IO.MonoIO.Close(write_handle, out error);
				}
				throw new IOException("Error creating standard output pipe");
			}
		}
		else
		{
			process.stdout_rd = (IntPtr)0;
			write_handle3 = System.IO.MonoIO.ConsoleOutput;
		}
		IntPtr write_handle4;
		if (startInfo.RedirectStandardError)
		{
			IntPtr read_handle4 = IntPtr.Zero;
			if (IsWindows)
			{
				int options3 = 2;
				flag = System.IO.MonoIO.CreatePipe(out var read_handle5, out write_handle4);
				if (flag)
				{
					System.IO.MonoIO.DuplicateHandle(GetCurrentProcess().Handle, read_handle5, GetCurrentProcess().Handle, out read_handle4, 0, 0, options3);
					System.IO.MonoIO.Close(read_handle5, out error);
				}
			}
			else
			{
				flag = System.IO.MonoIO.CreatePipe(out read_handle4, out write_handle4);
			}
			process.stderr_rd = read_handle4;
			if (!flag)
			{
				if (startInfo.RedirectStandardInput)
				{
					System.IO.MonoIO.Close(read_handle, out error);
					System.IO.MonoIO.Close(write_handle, out error);
				}
				if (startInfo.RedirectStandardOutput)
				{
					System.IO.MonoIO.Close(process.stdout_rd, out error);
					System.IO.MonoIO.Close(write_handle3, out error);
				}
				throw new IOException("Error creating standard error pipe");
			}
		}
		else
		{
			process.stderr_rd = (IntPtr)0;
			write_handle4 = System.IO.MonoIO.ConsoleError;
		}
		FillUserInfo(startInfo, ref proc_info);
		try
		{
			flag = CreateProcess_internal(startInfo, read_handle, write_handle3, write_handle4, ref proc_info);
		}
		finally
		{
			if (proc_info.Password != IntPtr.Zero)
			{
				Marshal.FreeBSTR(proc_info.Password);
			}
			proc_info.Password = IntPtr.Zero;
		}
		if (!flag)
		{
			if (startInfo.RedirectStandardInput)
			{
				System.IO.MonoIO.Close(read_handle, out error);
				System.IO.MonoIO.Close(write_handle, out error);
			}
			if (startInfo.RedirectStandardOutput)
			{
				System.IO.MonoIO.Close(process.stdout_rd, out error);
				System.IO.MonoIO.Close(write_handle3, out error);
			}
			if (startInfo.RedirectStandardError)
			{
				System.IO.MonoIO.Close(process.stderr_rd, out error);
				System.IO.MonoIO.Close(write_handle4, out error);
			}
			throw new Win32Exception(-proc_info.pid, "ApplicationName='" + startInfo.FileName + "', CommandLine='" + startInfo.Arguments + "', CurrentDirectory='" + startInfo.WorkingDirectory + "'");
		}
		process.process_handle = proc_info.process_handle;
		process.pid = proc_info.pid;
		if (startInfo.RedirectStandardInput)
		{
			System.IO.MonoIO.Close(read_handle, out error);
			process.input_stream = new StreamWriter(new System.IO.MonoSyncFileStream(write_handle, FileAccess.Write, ownsHandle: true, 8192), Console.Out.Encoding);
			process.input_stream.AutoFlush = true;
		}
		Encoding encoding = startInfo.StandardOutputEncoding ?? Console.Out.Encoding;
		Encoding encoding2 = startInfo.StandardErrorEncoding ?? Console.Out.Encoding;
		if (startInfo.RedirectStandardOutput)
		{
			System.IO.MonoIO.Close(write_handle3, out error);
			process.output_stream = new StreamReader(new System.IO.MonoSyncFileStream(process.stdout_rd, FileAccess.Read, ownsHandle: true, 8192), encoding, detectEncodingFromByteOrderMarks: true, 8192);
		}
		if (startInfo.RedirectStandardError)
		{
			System.IO.MonoIO.Close(write_handle4, out error);
			process.error_stream = new StreamReader(new System.IO.MonoSyncFileStream(process.stderr_rd, FileAccess.Read, ownsHandle: true, 8192), encoding2, detectEncodingFromByteOrderMarks: true, 8192);
		}
		process.StartExitCallbackIfNeeded();
		return flag;
	}

	private static void FillUserInfo(ProcessStartInfo startInfo, ref ProcInfo proc_info)
	{
		if (startInfo.UserName != null)
		{
			proc_info.UserName = startInfo.UserName;
			proc_info.Domain = startInfo.Domain;
			if (startInfo.Password != null)
			{
				proc_info.Password = Marshal.SecureStringToBSTR(startInfo.Password);
			}
			else
			{
				proc_info.Password = IntPtr.Zero;
			}
			proc_info.LoadUserProfile = startInfo.LoadUserProfile;
		}
	}

	private static bool Start_common(ProcessStartInfo startInfo, Process process)
	{
		if (startInfo.FileName == null || startInfo.FileName.Length == 0)
		{
			throw new InvalidOperationException("File name has not been set");
		}
		if (startInfo.StandardErrorEncoding != null && !startInfo.RedirectStandardError)
		{
			throw new InvalidOperationException("StandardErrorEncoding is only supported when standard error is redirected");
		}
		if (startInfo.StandardOutputEncoding != null && !startInfo.RedirectStandardOutput)
		{
			throw new InvalidOperationException("StandardOutputEncoding is only supported when standard output is redirected");
		}
		if (startInfo.UseShellExecute)
		{
			if (!string.IsNullOrEmpty(startInfo.UserName))
			{
				throw new InvalidOperationException("UserShellExecute must be false if an explicit UserName is specified when starting a process");
			}
			return Start_shell(startInfo, process);
		}
		return Start_noshell(startInfo, process);
	}

	public static Process Start(ProcessStartInfo startInfo)
	{
		if (startInfo == null)
		{
			throw new ArgumentNullException("startInfo");
		}
		Process process = new Process();
		process.StartInfo = startInfo;
		if (Start_common(startInfo, process))
		{
			return process;
		}
		return null;
	}

	public static Process Start(string fileName)
	{
		return Start(new ProcessStartInfo(fileName));
	}

	public static Process Start(string fileName, string arguments)
	{
		return Start(new ProcessStartInfo(fileName, arguments));
	}

	public override string ToString()
	{
		return base.ToString() + " (" + ProcessName + ")";
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void Process_free_internal(IntPtr handle);

	protected override void Dispose(bool disposing)
	{
		if (!disposed)
		{
			disposed = true;
			if (disposing)
			{
				lock (this)
				{
					if (async_output != null)
					{
						async_output.Close();
					}
					if (async_error != null)
					{
						async_error.Close();
					}
				}
			}
			lock (this)
			{
				if (process_handle != IntPtr.Zero)
				{
					Process_free_internal(process_handle);
					process_handle = IntPtr.Zero;
				}
				if (input_stream != null)
				{
					input_stream.Close();
					input_stream = null;
				}
				if (output_stream != null)
				{
					output_stream.Close();
					output_stream = null;
				}
				if (error_stream != null)
				{
					error_stream.Close();
					error_stream = null;
				}
			}
		}
		base.Dispose(disposing);
	}

	~Process()
	{
		Dispose(disposing: false);
	}
}
