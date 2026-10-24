using System.Runtime.InteropServices;

namespace System.Security.Permissions;

[Serializable]
[ComVisible(true)]
public sealed class KeyContainerPermission : CodeAccessPermission, IBuiltInPermission, IUnrestrictedPermission
{
	private KeyContainerPermissionFlags _flags;

	public KeyContainerPermission(PermissionState state)
	{
		if (CodeAccessPermission.CheckPermissionState(state, allowUnrestricted: true) == PermissionState.Unrestricted)
		{
			_flags = KeyContainerPermissionFlags.AllFlags;
		}
	}

	[MonoTODO("(2.0) missing support for AccessEntries")]
	public override void FromXml(SecurityElement securityElement)
	{
		CodeAccessPermission.CheckSecurityElement(securityElement, "securityElement", 1, 1);
		if (CodeAccessPermission.IsUnrestricted(securityElement))
		{
			_flags = KeyContainerPermissionFlags.AllFlags;
		}
		else
		{
			_flags = (KeyContainerPermissionFlags)(int)Enum.Parse(typeof(KeyContainerPermissionFlags), securityElement.Attribute("Flags"));
		}
	}

	[MonoTODO("(2.0)")]
	public override bool IsSubsetOf(IPermission target)
	{
		return false;
	}

	public bool IsUnrestricted()
	{
		return _flags == KeyContainerPermissionFlags.AllFlags;
	}

	[MonoTODO("(2.0) missing support for AccessEntries")]
	public override SecurityElement ToXml()
	{
		SecurityElement securityElement = Element(1);
		if (IsUnrestricted())
		{
			securityElement.AddAttribute("Unrestricted", "true");
		}
		return securityElement;
	}
}
