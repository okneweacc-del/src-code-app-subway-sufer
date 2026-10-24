using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy;

[Serializable]
[ComVisible(true)]
public sealed class Zone : IBuiltInEvidence, IIdentityPermissionFactory
{
	private SecurityZone zone;

	public SecurityZone SecurityZone => zone;

	public IPermission CreateIdentityPermission(Evidence evidence)
	{
		return new ZoneIdentityPermission(zone);
	}

	public override bool Equals(object o)
	{
		if (!(o is Zone zone))
		{
			return false;
		}
		return zone.zone == this.zone;
	}

	public override int GetHashCode()
	{
		return (int)zone;
	}

	public override string ToString()
	{
		SecurityElement securityElement = new SecurityElement("System.Security.Policy.Zone");
		securityElement.AddAttribute("version", "1");
		securityElement.AddChild(new SecurityElement("Zone", zone.ToString()));
		return securityElement.ToString();
	}
}
