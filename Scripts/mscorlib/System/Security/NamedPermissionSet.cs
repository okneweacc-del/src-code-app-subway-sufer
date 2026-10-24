using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security;

[Serializable]
[ComVisible(true)]
public sealed class NamedPermissionSet : PermissionSet
{
	private string name;

	private string description;

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			if (value == null || value == string.Empty)
			{
				throw new ArgumentException(Locale.GetText("invalid name"));
			}
			name = value;
		}
	}

	internal NamedPermissionSet()
	{
	}

	public NamedPermissionSet(string name, PermissionState state)
		: base(state)
	{
		Name = name;
	}

	public NamedPermissionSet(NamedPermissionSet permSet)
		: base(permSet)
	{
		name = permSet.name;
		description = permSet.description;
	}

	public override PermissionSet Copy()
	{
		return new NamedPermissionSet(this);
	}

	public override void FromXml(SecurityElement et)
	{
		base.FromXml(et);
		name = et.Attribute("Name");
		description = et.Attribute("Description");
		if (description == null)
		{
			description = string.Empty;
		}
	}

	public override SecurityElement ToXml()
	{
		SecurityElement securityElement = base.ToXml();
		if (name != null)
		{
			securityElement.AddAttribute("Name", name);
		}
		if (description != null)
		{
			securityElement.AddAttribute("Description", description);
		}
		return securityElement;
	}

	[ComVisible(false)]
	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is NamedPermissionSet namedPermissionSet))
		{
			return false;
		}
		return name == namedPermissionSet.Name && base.Equals(obj);
	}

	[ComVisible(false)]
	public override int GetHashCode()
	{
		int num = base.GetHashCode();
		if (name != null)
		{
			num ^= name.GetHashCode();
		}
		return num;
	}
}
