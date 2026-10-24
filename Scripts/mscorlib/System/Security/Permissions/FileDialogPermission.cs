using System.Runtime.InteropServices;

namespace System.Security.Permissions;

[Serializable]
[ComVisible(true)]
public sealed class FileDialogPermission : CodeAccessPermission, IBuiltInPermission, IUnrestrictedPermission
{
	private FileDialogPermissionAccess _access;

	public FileDialogPermissionAccess Access
	{
		set
		{
			if (!Enum.IsDefined(typeof(FileDialogPermissionAccess), value))
			{
				string message = string.Format(Locale.GetText("Invalid enum {0}"), value);
				throw new ArgumentException(message, "FileDialogPermissionAccess");
			}
			_access = value;
		}
	}

	public FileDialogPermission(PermissionState state)
	{
		if (CodeAccessPermission.CheckPermissionState(state, allowUnrestricted: true) == PermissionState.Unrestricted)
		{
			_access = FileDialogPermissionAccess.OpenSave;
		}
		else
		{
			_access = FileDialogPermissionAccess.None;
		}
	}

	public FileDialogPermission(FileDialogPermissionAccess access)
	{
		Access = access;
	}

	public override void FromXml(SecurityElement esd)
	{
		CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
		if (CodeAccessPermission.IsUnrestricted(esd))
		{
			_access = FileDialogPermissionAccess.OpenSave;
			return;
		}
		string text = esd.Attribute("Access");
		if (text == null)
		{
			_access = FileDialogPermissionAccess.None;
		}
		else
		{
			_access = (FileDialogPermissionAccess)(int)Enum.Parse(typeof(FileDialogPermissionAccess), text);
		}
	}

	public override bool IsSubsetOf(IPermission target)
	{
		FileDialogPermission fileDialogPermission = Cast(target);
		if (fileDialogPermission == null)
		{
			return false;
		}
		return (_access & fileDialogPermission._access) == _access;
	}

	public override SecurityElement ToXml()
	{
		SecurityElement securityElement = Element(1);
		switch (_access)
		{
		case FileDialogPermissionAccess.Open:
			securityElement.AddAttribute("Access", "Open");
			break;
		case FileDialogPermissionAccess.Save:
			securityElement.AddAttribute("Access", "Save");
			break;
		case FileDialogPermissionAccess.OpenSave:
			securityElement.AddAttribute("Unrestricted", "true");
			break;
		}
		return securityElement;
	}

	private FileDialogPermission Cast(IPermission target)
	{
		if (target == null)
		{
			return null;
		}
		FileDialogPermission fileDialogPermission = target as FileDialogPermission;
		if (fileDialogPermission == null)
		{
			CodeAccessPermission.ThrowInvalidPermission(target, typeof(FileDialogPermission));
		}
		return fileDialogPermission;
	}
}
