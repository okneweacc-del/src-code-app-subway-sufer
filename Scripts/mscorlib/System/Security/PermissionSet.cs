using System.Security.Permissions;
using System.Security.Policy;

namespace System.Security;

[Serializable]
public class PermissionSet
{
	internal PolicyLevel Resolver { set; }

	internal bool DeclarativeSecurity { set; }

	public PermissionSet()
	{
	}

	internal PermissionSet(string xml)
	{
	}

	public PermissionSet(PermissionState state)
	{
	}

	public PermissionSet(PermissionSet permSet)
	{
	}

	public IPermission AddPermission(IPermission perm)
	{
		return perm;
	}

	public virtual PermissionSet Copy()
	{
		return new PermissionSet(this);
	}

	public virtual IPermission GetPermission(Type permClass)
	{
		return null;
	}

	public virtual PermissionSet Intersect(PermissionSet other)
	{
		return other;
	}

	public virtual void FromXml(SecurityElement et)
	{
	}

	public virtual SecurityElement ToXml()
	{
		return null;
	}

	public virtual bool IsSubsetOf(PermissionSet target)
	{
		return true;
	}

	internal void SetReadOnly(bool value)
	{
	}

	public bool IsUnrestricted()
	{
		return true;
	}

	public PermissionSet Union(PermissionSet other)
	{
		return new PermissionSet();
	}

	internal static PermissionSet CreateFromBinaryFormat(byte[] data)
	{
		return new PermissionSet();
	}
}
