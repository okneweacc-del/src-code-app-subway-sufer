using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.Win32;

internal class Win32RegistryApi : IRegistryApi
{
	private readonly int NativeBytesPerCharacter = Marshal.SystemDefaultCharSize;

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
	private static extern int RegCloseKey(IntPtr keyHandle);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
	private static extern int RegFlushKey(IntPtr keyHandle);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
	private static extern int RegOpenKeyEx(IntPtr keyBase, string keyName, IntPtr reserved, int access, out IntPtr keyHandle);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
	private static extern int RegDeleteValue(IntPtr keyHandle, string valueName);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
	private static extern int RegEnumValue(IntPtr keyBase, int index, StringBuilder nameBuffer, ref int nameLength, IntPtr reserved, ref RegistryValueKind type, IntPtr data, IntPtr dataLength);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
	private static extern int RegSetValueEx(IntPtr keyBase, string valueName, IntPtr reserved, RegistryValueKind type, string data, int rawDataLength);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
	private static extern int RegSetValueEx(IntPtr keyBase, string valueName, IntPtr reserved, RegistryValueKind type, byte[] rawData, int rawDataLength);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
	private static extern int RegSetValueEx(IntPtr keyBase, string valueName, IntPtr reserved, RegistryValueKind type, ref int data, int rawDataLength);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
	private static extern int RegQueryValueEx(IntPtr keyBase, string valueName, IntPtr reserved, ref RegistryValueKind type, IntPtr zero, ref int dataSize);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
	private static extern int RegQueryValueEx(IntPtr keyBase, string valueName, IntPtr reserved, ref RegistryValueKind type, [Out] byte[] data, ref int dataSize);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
	private static extern int RegQueryValueEx(IntPtr keyBase, string valueName, IntPtr reserved, ref RegistryValueKind type, ref int data, ref int dataSize);

	private static IntPtr GetHandle(RegistryKey key)
	{
		return (IntPtr)key.Handle;
	}

	private static bool IsHandleValid(RegistryKey key)
	{
		return key.Handle != null;
	}

	public object GetValue(RegistryKey rkey, string name, object defaultValue, RegistryValueOptions options)
	{
		RegistryValueKind type = RegistryValueKind.Unknown;
		int dataSize = 0;
		object obj = null;
		IntPtr handle = GetHandle(rkey);
		int num = RegQueryValueEx(handle, name, IntPtr.Zero, ref type, IntPtr.Zero, ref dataSize);
		switch (num)
		{
		case 2:
		case 1018:
			return defaultValue;
		default:
			GenerateException(num);
			break;
		case 0:
		case 234:
			break;
		}
		switch (type)
		{
		case RegistryValueKind.String:
		{
			num = GetBinaryValue(rkey, name, type, out var data2, dataSize);
			obj = RegistryKey.DecodeString(data2);
			break;
		}
		case RegistryValueKind.ExpandString:
		{
			num = GetBinaryValue(rkey, name, type, out var data5, dataSize);
			obj = RegistryKey.DecodeString(data5);
			if ((options & RegistryValueOptions.DoNotExpandEnvironmentNames) == 0)
			{
				obj = Environment.ExpandEnvironmentVariables((string)obj);
			}
			break;
		}
		case RegistryValueKind.DWord:
		{
			int data4 = 0;
			num = RegQueryValueEx(handle, name, IntPtr.Zero, ref type, ref data4, ref dataSize);
			obj = data4;
			break;
		}
		case RegistryValueKind.Binary:
		{
			num = GetBinaryValue(rkey, name, type, out var data3, dataSize);
			obj = data3;
			break;
		}
		case RegistryValueKind.MultiString:
		{
			obj = null;
			num = GetBinaryValue(rkey, name, type, out var data, dataSize);
			if (num == 0)
			{
				obj = RegistryKey.DecodeString(data).Split(default(char));
			}
			break;
		}
		default:
			throw new SystemException();
		}
		if (num != 0)
		{
			GenerateException(num);
		}
		return obj;
	}

	public void SetValue(RegistryKey rkey, string name, object value)
	{
		Type type = value.GetType();
		IntPtr handle = GetHandle(rkey);
		int num;
		if (type == typeof(int))
		{
			int data = (int)value;
			num = RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.DWord, ref data, 4);
		}
		else if (type == typeof(byte[]))
		{
			byte[] array = (byte[])value;
			num = RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.Binary, array, array.Length);
		}
		else if (type == typeof(string[]))
		{
			string[] array2 = (string[])value;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array3 = array2;
			foreach (string value2 in array3)
			{
				stringBuilder.Append(value2);
				stringBuilder.Append('\0');
			}
			stringBuilder.Append('\0');
			byte[] bytes = Encoding.Unicode.GetBytes(stringBuilder.ToString());
			num = RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.MultiString, bytes, bytes.Length);
		}
		else
		{
			if (type.IsArray)
			{
				throw new ArgumentException("Only string and byte arrays can written as registry values");
			}
			string text = $"{value}{'\0'}";
			num = RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.String, text, text.Length * NativeBytesPerCharacter);
		}
		switch (num)
		{
		case 1018:
			throw RegistryKey.CreateMarkedForDeletionException();
		case 0:
			return;
		}
		GenerateException(num);
	}

	private int GetBinaryValue(RegistryKey rkey, string name, RegistryValueKind type, out byte[] data, int size)
	{
		byte[] array = new byte[size];
		IntPtr handle = GetHandle(rkey);
		int result = RegQueryValueEx(handle, name, IntPtr.Zero, ref type, array, ref size);
		data = array;
		return result;
	}

	public RegistryKey OpenSubKey(RegistryKey rkey, string keyName, bool writable)
	{
		int num = 131097;
		if (writable)
		{
			num |= 0x20006;
		}
		IntPtr handle = GetHandle(rkey);
		int num2 = RegOpenKeyEx(handle, keyName, IntPtr.Zero, num, out var keyHandle);
		switch (num2)
		{
		case 2:
		case 1018:
			return null;
		default:
			GenerateException(num2);
			break;
		case 0:
			break;
		}
		return new RegistryKey(keyHandle, CombineName(rkey, keyName), writable);
	}

	public void Flush(RegistryKey rkey)
	{
		if (IsHandleValid(rkey))
		{
			IntPtr handle = GetHandle(rkey);
			RegFlushKey(handle);
		}
	}

	public void Close(RegistryKey rkey)
	{
		if (IsHandleValid(rkey))
		{
			IntPtr handle = GetHandle(rkey);
			RegCloseKey(handle);
		}
	}

	public void DeleteValue(RegistryKey rkey, string value, bool shouldThrowWhenKeyMissing)
	{
		IntPtr handle = GetHandle(rkey);
		int num = RegDeleteValue(handle, value);
		switch (num)
		{
		case 0:
		case 1018:
			break;
		case 2:
			if (shouldThrowWhenKeyMissing)
			{
				throw new ArgumentException("value " + value);
			}
			break;
		default:
			GenerateException(num);
			break;
		}
	}

	public string[] GetValueNames(RegistryKey rkey)
	{
		IntPtr handle = GetHandle(rkey);
		ArrayList arrayList = new ArrayList();
		int num = 0;
		while (true)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			int nameLength = stringBuilder.Capacity;
			RegistryValueKind type = RegistryValueKind.Unknown;
			int num2 = RegEnumValue(handle, num, stringBuilder, ref nameLength, IntPtr.Zero, ref type, IntPtr.Zero, IntPtr.Zero);
			switch (num2)
			{
			case 0:
			case 234:
				arrayList.Add(stringBuilder.ToString());
				break;
			case 1018:
				throw RegistryKey.CreateMarkedForDeletionException();
			default:
				GenerateException(num2);
				break;
			case 259:
				return (string[])arrayList.ToArray(typeof(string));
			}
			num++;
		}
	}

	private void GenerateException(int errorCode)
	{
		switch (errorCode)
		{
		case 2:
		case 87:
			throw new ArgumentException();
		case 5:
			throw new SecurityException();
		case 53:
			throw new IOException("The network path was not found.");
		default:
			throw new SystemException();
		}
	}

	public string ToString(RegistryKey rkey)
	{
		return rkey.Name;
	}

	internal static string CombineName(RegistryKey rkey, string localName)
	{
		return rkey.Name + "\\" + localName;
	}
}
