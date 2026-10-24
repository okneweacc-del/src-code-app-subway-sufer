namespace Microsoft.Win32;

internal interface IRegistryApi
{
	RegistryKey OpenSubKey(RegistryKey rkey, string keyname, bool writtable);

	void Flush(RegistryKey rkey);

	void Close(RegistryKey rkey);

	object GetValue(RegistryKey rkey, string name, object default_value, RegistryValueOptions options);

	void SetValue(RegistryKey rkey, string name, object value);

	void DeleteValue(RegistryKey rkey, string value, bool throw_if_missing);

	string[] GetValueNames(RegistryKey rkey);

	string ToString(RegistryKey rkey);
}
