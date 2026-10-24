using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Security.Permissions;

[Serializable]
[ComVisible(true)]
public sealed class StrongNameIdentityPermission : CodeAccessPermission, IBuiltInPermission
{
	private struct SNIP
	{
		public StrongNamePublicKeyBlob PublicKey;

		public string Name;

		public Version AssemblyVersion;

		internal SNIP(StrongNamePublicKeyBlob pk, string name, Version version)
		{
			PublicKey = pk;
			Name = name;
			AssemblyVersion = version;
		}

		internal bool IsNameSubsetOf(string target)
		{
			if (Name == null)
			{
				return target == null;
			}
			if (target == null)
			{
				return true;
			}
			int num = Name.LastIndexOf('*');
			switch (num)
			{
			case 0:
				return true;
			case -1:
				num = Name.Length;
				break;
			}
			return string.Compare(Name, 0, target, 0, num, ignoreCase: true, CultureInfo.InvariantCulture) == 0;
		}

		internal bool IsSubsetOf(SNIP target)
		{
			if (PublicKey != null && PublicKey.Equals(target.PublicKey))
			{
				return true;
			}
			if (!IsNameSubsetOf(target.Name))
			{
				return false;
			}
			if (AssemblyVersion != null && !AssemblyVersion.Equals(target.AssemblyVersion))
			{
				return false;
			}
			if (PublicKey == null)
			{
				return target.PublicKey == null;
			}
			return false;
		}
	}

	private static Version defaultVersion = new Version(0, 0);

	private PermissionState _state;

	private ArrayList _list;

	public string Name
	{
		get
		{
			if (_list.Count > 1)
			{
				throw new NotSupportedException();
			}
			return ((SNIP)_list[0]).Name;
		}
	}

	public StrongNamePublicKeyBlob PublicKey
	{
		get
		{
			if (_list.Count > 1)
			{
				throw new NotSupportedException();
			}
			return ((SNIP)_list[0]).PublicKey;
		}
	}

	public Version Version
	{
		get
		{
			if (_list.Count > 1)
			{
				throw new NotSupportedException();
			}
			return ((SNIP)_list[0]).AssemblyVersion;
		}
	}

	public StrongNameIdentityPermission(StrongNamePublicKeyBlob blob, string name, Version version)
	{
		if (blob == null)
		{
			throw new ArgumentNullException("blob");
		}
		if (name != null && name.Length == 0)
		{
			throw new ArgumentException("name");
		}
		_state = PermissionState.None;
		_list = new ArrayList();
		_list.Add(new SNIP(blob, name, version));
	}

	public override void FromXml(SecurityElement e)
	{
		CodeAccessPermission.CheckSecurityElement(e, "e", 1, 1);
		_list.Clear();
		if (e.Children != null && e.Children.Count > 0)
		{
			foreach (SecurityElement child in e.Children)
			{
				_list.Add(FromSecurityElement(child));
			}
			return;
		}
		_list.Add(FromSecurityElement(e));
	}

	private SNIP FromSecurityElement(SecurityElement se)
	{
		string name = se.Attribute("Name");
		StrongNamePublicKeyBlob pk = StrongNamePublicKeyBlob.FromString(se.Attribute("PublicKeyBlob"));
		string text = se.Attribute("AssemblyVersion");
		Version version = ((text != null) ? new Version(text) : null);
		return new SNIP(pk, name, version);
	}

	public override bool IsSubsetOf(IPermission target)
	{
		StrongNameIdentityPermission strongNameIdentityPermission = Cast(target);
		if (strongNameIdentityPermission == null)
		{
			return IsEmpty();
		}
		if (IsEmpty())
		{
			return true;
		}
		if (IsUnrestricted())
		{
			return strongNameIdentityPermission.IsUnrestricted();
		}
		if (strongNameIdentityPermission.IsUnrestricted())
		{
			return true;
		}
		foreach (SNIP item in _list)
		{
			foreach (SNIP item2 in strongNameIdentityPermission._list)
			{
				if (!item.IsSubsetOf(item2))
				{
					return false;
				}
			}
		}
		return true;
	}

	public override SecurityElement ToXml()
	{
		SecurityElement securityElement = Element(1);
		if (_list.Count > 1)
		{
			foreach (SNIP item in _list)
			{
				SecurityElement securityElement2 = new SecurityElement("StrongName");
				ToSecurityElement(securityElement2, item);
				securityElement.AddChild(securityElement2);
			}
		}
		else if (_list.Count == 1)
		{
			SNIP snip2 = (SNIP)_list[0];
			if (!IsEmpty(snip2))
			{
				ToSecurityElement(securityElement, snip2);
			}
		}
		return securityElement;
	}

	private void ToSecurityElement(SecurityElement se, SNIP snip)
	{
		if (snip.PublicKey != null)
		{
			se.AddAttribute("PublicKeyBlob", snip.PublicKey.ToString());
		}
		if (snip.Name != null)
		{
			se.AddAttribute("Name", snip.Name);
		}
		if (snip.AssemblyVersion != null)
		{
			se.AddAttribute("AssemblyVersion", snip.AssemblyVersion.ToString());
		}
	}

	private bool IsUnrestricted()
	{
		return _state == PermissionState.Unrestricted;
	}

	private bool IsEmpty(SNIP snip)
	{
		if (PublicKey != null)
		{
			return false;
		}
		if (Name != null && Name.Length > 0)
		{
			return false;
		}
		return Version == null || defaultVersion.Equals(Version);
	}

	private bool IsEmpty()
	{
		if (IsUnrestricted() || _list.Count > 1)
		{
			return false;
		}
		if (PublicKey != null)
		{
			return false;
		}
		if (Name != null && Name.Length > 0)
		{
			return false;
		}
		return Version == null || defaultVersion.Equals(Version);
	}

	private StrongNameIdentityPermission Cast(IPermission target)
	{
		if (target == null)
		{
			return null;
		}
		StrongNameIdentityPermission strongNameIdentityPermission = target as StrongNameIdentityPermission;
		if (strongNameIdentityPermission == null)
		{
			CodeAccessPermission.ThrowInvalidPermission(target, typeof(StrongNameIdentityPermission));
		}
		return strongNameIdentityPermission;
	}
}
