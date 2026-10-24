using System.Runtime.InteropServices;

namespace System.Security.Permissions;

[Serializable]
[ComVisible(true)]
public sealed class IsolatedStorageFilePermission : IsolatedStoragePermission, IBuiltInPermission
{
	public IsolatedStorageFilePermission(PermissionState state)
		: base(state)
	{
	}

	public override bool IsSubsetOf(IPermission target)
	{
		IsolatedStorageFilePermission isolatedStorageFilePermission = Cast(target);
		if (isolatedStorageFilePermission == null)
		{
			return IsEmpty();
		}
		if (isolatedStorageFilePermission.IsUnrestricted())
		{
			return true;
		}
		if (m_userQuota > isolatedStorageFilePermission.m_userQuota)
		{
			return false;
		}
		if (m_machineQuota > isolatedStorageFilePermission.m_machineQuota)
		{
			return false;
		}
		if (m_expirationDays > isolatedStorageFilePermission.m_expirationDays)
		{
			return false;
		}
		if (m_permanentData != isolatedStorageFilePermission.m_permanentData)
		{
			return false;
		}
		if (m_allowed > isolatedStorageFilePermission.m_allowed)
		{
			return false;
		}
		return true;
	}

	[MonoTODO("(2.0) new override - something must have been added ???")]
	[ComVisible(false)]
	public override SecurityElement ToXml()
	{
		return base.ToXml();
	}

	private IsolatedStorageFilePermission Cast(IPermission target)
	{
		if (target == null)
		{
			return null;
		}
		IsolatedStorageFilePermission isolatedStorageFilePermission = target as IsolatedStorageFilePermission;
		if (isolatedStorageFilePermission == null)
		{
			CodeAccessPermission.ThrowInvalidPermission(target, typeof(IsolatedStorageFilePermission));
		}
		return isolatedStorageFilePermission;
	}
}
