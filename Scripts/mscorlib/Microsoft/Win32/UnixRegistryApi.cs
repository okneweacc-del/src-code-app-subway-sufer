using System;
using System.Globalization;

namespace Microsoft.Win32;

internal class UnixRegistryApi : IRegistryApi
{
	private static string ToUnix(string keyname)
	{
		if (keyname.IndexOf('\\') != -1)
		{
			keyname = keyname.Replace('\\', '/');
		}
		return keyname.ToLower();
	}

	private static bool IsWellKnownKey(string parentKeyName, string keyname)
	{
		if (parentKeyName == Registry.CurrentUser.Name || parentKeyName == Registry.LocalMachine.Name)
		{
			return 0 == string.Compare("software", keyname, ignoreCase: true, CultureInfo.InvariantCulture);
		}
		return false;
	}

	public RegistryKey OpenSubKey(RegistryKey rkey, string keyname, bool writable)
	{
		KeyHandler keyHandler = KeyHandler.Lookup(rkey, createNonExisting: true);
		if (keyHandler == null)
		{
			return null;
		}
		RegistryKey registryKey = keyHandler.Probe(rkey, ToUnix(keyname), writable);
		if (registryKey == null && IsWellKnownKey(rkey.Name, keyname))
		{
			registryKey = CreateSubKey(rkey, keyname, writable);
		}
		return registryKey;
	}

	public void Flush(RegistryKey rkey)
	{
		KeyHandler.Lookup(rkey, createNonExisting: false)?.Flush();
	}

	public void Close(RegistryKey rkey)
	{
		KeyHandler.Drop(rkey);
	}

	public object GetValue(RegistryKey rkey, string name, object default_value, RegistryValueOptions options)
	{
		KeyHandler keyHandler = KeyHandler.Lookup(rkey, createNonExisting: true);
		if (keyHandler == null)
		{
			return default_value;
		}
		if (keyHandler.ValueExists(name))
		{
			return keyHandler.GetValue(name, options);
		}
		return default_value;
	}

	public void SetValue(RegistryKey rkey, string name, object value)
	{
		KeyHandler keyHandler = KeyHandler.Lookup(rkey, createNonExisting: true);
		if (keyHandler == null)
		{
			throw RegistryKey.CreateMarkedForDeletionException();
		}
		keyHandler.SetValue(name, value);
	}

	public void DeleteValue(RegistryKey rkey, string name, bool throw_if_missing)
	{
		KeyHandler keyHandler = KeyHandler.Lookup(rkey, createNonExisting: true);
		if (keyHandler != null)
		{
			if (throw_if_missing && !keyHandler.ValueExists(name))
			{
				throw new ArgumentException("the given value does not exist");
			}
			keyHandler.RemoveValue(name);
		}
	}

	public string[] GetValueNames(RegistryKey rkey)
	{
		KeyHandler keyHandler = KeyHandler.Lookup(rkey, createNonExisting: true);
		if (keyHandler == null)
		{
			throw RegistryKey.CreateMarkedForDeletionException();
		}
		return keyHandler.GetValueNames();
	}

	public string ToString(RegistryKey rkey)
	{
		return rkey.Name;
	}

	private RegistryKey CreateSubKey(RegistryKey rkey, string keyname, bool writable)
	{
		KeyHandler keyHandler = KeyHandler.Lookup(rkey, createNonExisting: true);
		if (keyHandler == null)
		{
			throw RegistryKey.CreateMarkedForDeletionException();
		}
		return keyHandler.Ensure(rkey, ToUnix(keyname), writable);
	}
}
