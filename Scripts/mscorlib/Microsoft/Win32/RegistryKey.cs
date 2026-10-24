using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Win32;

[ComVisible(true)]
public sealed class RegistryKey : MarshalByRefObject, IDisposable
{
	private object handle;

	private object hive;

	private readonly string qname;

	private readonly bool isRemoteRoot;

	private readonly bool isWritable;

	private static readonly IRegistryApi RegistryApi;

	public string Name => qname;

	internal bool IsRoot => hive != null;

	private bool IsWritable => isWritable;

	internal RegistryHive Hive
	{
		get
		{
			if (!IsRoot)
			{
				throw new NotSupportedException();
			}
			return (RegistryHive)(int)hive;
		}
	}

	internal object Handle => handle;

	internal RegistryKey(RegistryHive hiveId)
		: this(hiveId, new IntPtr((int)hiveId), remoteRoot: false)
	{
	}

	internal RegistryKey(RegistryHive hiveId, IntPtr keyHandle, bool remoteRoot)
	{
		hive = hiveId;
		handle = keyHandle;
		qname = GetHiveName(hiveId);
		isRemoteRoot = remoteRoot;
		isWritable = true;
	}

	internal RegistryKey(object data, string keyName, bool writable)
	{
		handle = data;
		qname = keyName;
		isWritable = writable;
	}

	static RegistryKey()
	{
		if (Path.DirectorySeparatorChar == '\\')
		{
			RegistryApi = new Win32RegistryApi();
		}
		else
		{
			RegistryApi = new UnixRegistryApi();
		}
	}

	void IDisposable.Dispose()
	{
		GC.SuppressFinalize(this);
		Close();
	}

	~RegistryKey()
	{
		Close();
	}

	public void Flush()
	{
		RegistryApi.Flush(this);
	}

	public void Close()
	{
		Flush();
		if (isRemoteRoot || !IsRoot)
		{
			RegistryApi.Close(this);
			handle = null;
		}
	}

	public void SetValue(string name, object value)
	{
		AssertKeyStillValid();
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (name != null)
		{
			AssertKeyNameLength(name);
		}
		if (!IsWritable)
		{
			throw new UnauthorizedAccessException("Cannot write to the registry key.");
		}
		RegistryApi.SetValue(this, name, value);
	}

	public RegistryKey OpenSubKey(string name)
	{
		return OpenSubKey(name, writable: false);
	}

	public RegistryKey OpenSubKey(string name, bool writable)
	{
		AssertKeyStillValid();
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		AssertKeyNameLength(name);
		return RegistryApi.OpenSubKey(this, name, writable);
	}

	public object GetValue(string name)
	{
		return GetValue(name, null);
	}

	public object GetValue(string name, object defaultValue)
	{
		AssertKeyStillValid();
		return RegistryApi.GetValue(this, name, defaultValue, RegistryValueOptions.None);
	}

	public void DeleteValue(string name, bool throwOnMissingValue)
	{
		AssertKeyStillValid();
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (!IsWritable)
		{
			throw new UnauthorizedAccessException("Cannot write to the registry key.");
		}
		RegistryApi.DeleteValue(this, name, throwOnMissingValue);
	}

	public string[] GetValueNames()
	{
		AssertKeyStillValid();
		return RegistryApi.GetValueNames(this);
	}

	public override string ToString()
	{
		AssertKeyStillValid();
		return RegistryApi.ToString(this);
	}

	private void AssertKeyStillValid()
	{
		if (handle == null)
		{
			throw new ObjectDisposedException("Microsoft.Win32.RegistryKey");
		}
	}

	private void AssertKeyNameLength(string name)
	{
		if (name.Length > 255)
		{
			throw new ArgumentException("Name of registry key cannot be greater than 255 characters");
		}
	}

	internal static string DecodeString(byte[] data)
	{
		string text = Encoding.Unicode.GetString(data);
		int num = text.IndexOf('\0');
		if (num != -1)
		{
			text = text.TrimEnd(default(char));
		}
		return text;
	}

	internal static IOException CreateMarkedForDeletionException()
	{
		throw new IOException("Illegal operation attempted on a registry key that has been marked for deletion.");
	}

	private static string GetHiveName(RegistryHive hive)
	{
		return hive switch
		{
			RegistryHive.ClassesRoot => "HKEY_CLASSES_ROOT", 
			RegistryHive.CurrentConfig => "HKEY_CURRENT_CONFIG", 
			RegistryHive.CurrentUser => "HKEY_CURRENT_USER", 
			RegistryHive.DynData => "HKEY_DYN_DATA", 
			RegistryHive.LocalMachine => "HKEY_LOCAL_MACHINE", 
			RegistryHive.PerformanceData => "HKEY_PERFORMANCE_DATA", 
			RegistryHive.Users => "HKEY_USERS", 
			_ => throw new NotImplementedException($"Registry hive '{hive.ToString()}' is not implemented."), 
		};
	}
}
