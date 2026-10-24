using System.Runtime.InteropServices;

namespace System.Security.Policy;

[Serializable]
[ComVisible(true)]
public sealed class PermissionRequestEvidence : IBuiltInEvidence
{
	private PermissionSet requested;

	private PermissionSet optional;

	private PermissionSet denied;

	public PermissionRequestEvidence(PermissionSet request, PermissionSet optional, PermissionSet denied)
	{
		if (request != null)
		{
			requested = new PermissionSet(request);
		}
		if (optional != null)
		{
			this.optional = new PermissionSet(optional);
		}
		if (denied != null)
		{
			this.denied = new PermissionSet(denied);
		}
	}

	public override string ToString()
	{
		SecurityElement securityElement = new SecurityElement("System.Security.Policy.PermissionRequestEvidence");
		securityElement.AddAttribute("version", "1");
		if (requested != null)
		{
			SecurityElement securityElement2 = new SecurityElement("Request");
			securityElement2.AddChild(requested.ToXml());
			securityElement.AddChild(securityElement2);
		}
		if (optional != null)
		{
			SecurityElement securityElement3 = new SecurityElement("Optional");
			securityElement3.AddChild(optional.ToXml());
			securityElement.AddChild(securityElement3);
		}
		if (denied != null)
		{
			SecurityElement securityElement4 = new SecurityElement("Denied");
			securityElement4.AddChild(denied.ToXml());
			securityElement.AddChild(securityElement4);
		}
		return securityElement.ToString();
	}
}
