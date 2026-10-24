using System.Runtime.InteropServices;

namespace System.Security.Permissions;

[Serializable]
[ComVisible(true)]
public sealed class SecurityPermission : CodeAccessPermission, IBuiltInPermission, IUnrestrictedPermission
{
	private SecurityPermissionFlag flags;

	public SecurityPermissionFlag Flags
	{
		set
		{
			if ((value & SecurityPermissionFlag.AllFlags) != value)
			{
				string message = string.Format(Locale.GetText("Invalid flags {0}"), value);
				throw new ArgumentException(message, "SecurityPermissionFlag");
			}
			flags = value;
		}
	}

	public SecurityPermission(SecurityPermissionFlag flag)
	{
		Flags = flag;
	}

	public bool IsUnrestricted()
	{
		return flags == SecurityPermissionFlag.AllFlags;
	}

	public override bool IsSubsetOf(IPermission target)
	{
		SecurityPermission securityPermission = Cast(target);
		if (securityPermission == null)
		{
			return IsEmpty();
		}
		if (securityPermission.IsUnrestricted())
		{
			return true;
		}
		if (IsUnrestricted())
		{
			return false;
		}
		return (flags & ~securityPermission.flags) == 0;
	}

	public override void FromXml(SecurityElement esd)
	{
		CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
		if (CodeAccessPermission.IsUnrestricted(esd))
		{
			flags = SecurityPermissionFlag.AllFlags;
			return;
		}
		string text = esd.Attribute("Flags");
		if (text == null)
		{
			flags = SecurityPermissionFlag.NoFlags;
		}
		else
		{
			flags = (SecurityPermissionFlag)(int)Enum.Parse(typeof(SecurityPermissionFlag), text);
		}
	}

	public override SecurityElement ToXml()
	{
		SecurityElement securityElement = Element(1);
		if (IsUnrestricted())
		{
			securityElement.AddAttribute("Unrestricted", "true");
		}
		else
		{
			securityElement.AddAttribute("Flags", flags.ToString());
		}
		return securityElement;
	}

	private bool IsEmpty()
	{
		return flags == SecurityPermissionFlag.NoFlags;
	}

	private SecurityPermission Cast(IPermission target)
	{
		if (target == null)
		{
			return null;
		}
		SecurityPermission securityPermission = target as SecurityPermission;
		if (securityPermission == null)
		{
			CodeAccessPermission.ThrowInvalidPermission(target, typeof(SecurityPermission));
		}
		return securityPermission;
	}
}
