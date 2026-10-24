using System.Runtime.InteropServices;

namespace System.Security.Permissions;

[Serializable]
[ComVisible(true)]
public abstract class IsolatedStoragePermission : CodeAccessPermission, IUnrestrictedPermission
{
	internal long m_userQuota;

	internal long m_machineQuota;

	internal long m_expirationDays;

	internal bool m_permanentData;

	internal IsolatedStorageContainment m_allowed;

	public long UserQuota
	{
		set
		{
			m_userQuota = value;
		}
	}

	public IsolatedStorageContainment UsageAllowed
	{
		set
		{
			if (!Enum.IsDefined(typeof(IsolatedStorageContainment), value))
			{
				string message = string.Format(Locale.GetText("Invalid enum {0}"), value);
				throw new ArgumentException(message, "IsolatedStorageContainment");
			}
			m_allowed = value;
			if (m_allowed == IsolatedStorageContainment.UnrestrictedIsolatedStorage)
			{
				m_userQuota = long.MaxValue;
				m_machineQuota = long.MaxValue;
				m_expirationDays = long.MaxValue;
				m_permanentData = true;
			}
		}
	}

	protected IsolatedStoragePermission(PermissionState state)
	{
		if (CodeAccessPermission.CheckPermissionState(state, allowUnrestricted: true) == PermissionState.Unrestricted)
		{
			UsageAllowed = IsolatedStorageContainment.UnrestrictedIsolatedStorage;
		}
	}

	public bool IsUnrestricted()
	{
		return IsolatedStorageContainment.UnrestrictedIsolatedStorage == m_allowed;
	}

	public override SecurityElement ToXml()
	{
		SecurityElement securityElement = Element(1);
		if (m_allowed == IsolatedStorageContainment.UnrestrictedIsolatedStorage)
		{
			securityElement.AddAttribute("Unrestricted", "true");
		}
		else
		{
			securityElement.AddAttribute("Allowed", m_allowed.ToString());
			if (m_userQuota > 0)
			{
				securityElement.AddAttribute("UserQuota", m_userQuota.ToString());
			}
		}
		return securityElement;
	}

	public override void FromXml(SecurityElement esd)
	{
		CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
		m_userQuota = 0L;
		m_machineQuota = 0L;
		m_expirationDays = 0L;
		m_permanentData = false;
		m_allowed = IsolatedStorageContainment.None;
		if (CodeAccessPermission.IsUnrestricted(esd))
		{
			UsageAllowed = IsolatedStorageContainment.UnrestrictedIsolatedStorage;
			return;
		}
		string text = esd.Attribute("Allowed");
		if (text != null)
		{
			UsageAllowed = (IsolatedStorageContainment)(int)Enum.Parse(typeof(IsolatedStorageContainment), text);
		}
		text = esd.Attribute("UserQuota");
		if (text != null)
		{
			long.Parse(text, tryParse: true, out m_userQuota, out var _);
		}
	}

	internal bool IsEmpty()
	{
		return m_userQuota == 0L && m_allowed == IsolatedStorageContainment.None;
	}
}
