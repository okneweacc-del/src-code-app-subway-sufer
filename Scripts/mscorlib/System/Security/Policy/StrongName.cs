using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy;

[Serializable]
[ComVisible(true)]
public sealed class StrongName : IBuiltInEvidence, IIdentityPermissionFactory
{
	private StrongNamePublicKeyBlob publickey;

	private string name;

	private Version version;

	public string Name => name;

	public StrongNamePublicKeyBlob PublicKey => publickey;

	public Version Version => version;

	public IPermission CreateIdentityPermission(Evidence evidence)
	{
		return new StrongNameIdentityPermission(publickey, name, version);
	}

	public override bool Equals(object o)
	{
		if (!(o is StrongName strongName))
		{
			return false;
		}
		if (name != strongName.Name)
		{
			return false;
		}
		if (!Version.Equals(strongName.Version))
		{
			return false;
		}
		return PublicKey.Equals(strongName.PublicKey);
	}

	public override int GetHashCode()
	{
		return publickey.GetHashCode();
	}

	public override string ToString()
	{
		SecurityElement securityElement = new SecurityElement(typeof(StrongName).Name);
		securityElement.AddAttribute("version", "1");
		securityElement.AddAttribute("Key", publickey.ToString());
		securityElement.AddAttribute("Name", name);
		securityElement.AddAttribute("Version", version.ToString());
		return securityElement.ToString();
	}
}
