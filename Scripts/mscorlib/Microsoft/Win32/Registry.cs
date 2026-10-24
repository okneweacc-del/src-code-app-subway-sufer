using System.Runtime.InteropServices;

namespace Microsoft.Win32;

[ComVisible(true)]
public static class Registry
{
	public static readonly RegistryKey ClassesRoot = new RegistryKey(RegistryHive.ClassesRoot);

	public static readonly RegistryKey CurrentConfig = new RegistryKey(RegistryHive.CurrentConfig);

	public static readonly RegistryKey CurrentUser = new RegistryKey(RegistryHive.CurrentUser);

	public static readonly RegistryKey DynData = new RegistryKey(RegistryHive.DynData);

	public static readonly RegistryKey LocalMachine = new RegistryKey(RegistryHive.LocalMachine);

	public static readonly RegistryKey PerformanceData = new RegistryKey(RegistryHive.PerformanceData);

	public static readonly RegistryKey Users = new RegistryKey(RegistryHive.Users);
}
