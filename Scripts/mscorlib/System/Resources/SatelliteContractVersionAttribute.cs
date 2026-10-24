using System.Runtime.InteropServices;

namespace System.Resources;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class SatelliteContractVersionAttribute : Attribute
{
	private Version ver;

	public SatelliteContractVersionAttribute(string version)
	{
		ver = new Version(version);
	}
}
