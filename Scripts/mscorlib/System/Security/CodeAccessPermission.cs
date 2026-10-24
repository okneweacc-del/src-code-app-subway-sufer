using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security;

[Serializable]
[ComVisible(true)]
[MonoTODO("CAS support is experimental (and unsupported).")]
public abstract class CodeAccessPermission : IPermission, ISecurityEncodable, IStackWalk
{
	public void Demand()
	{
	}

	[ComVisible(false)]
	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		CodeAccessPermission codeAccessPermission = obj as CodeAccessPermission;
		return IsSubsetOf(codeAccessPermission) && codeAccessPermission.IsSubsetOf(this);
	}

	public abstract void FromXml(SecurityElement elem);

	[ComVisible(false)]
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public abstract bool IsSubsetOf(IPermission target);

	public override string ToString()
	{
		SecurityElement securityElement = ToXml();
		return securityElement.ToString();
	}

	public abstract SecurityElement ToXml();

	internal SecurityElement Element(int version)
	{
		SecurityElement securityElement = new SecurityElement("IPermission");
		Type type = GetType();
		securityElement.AddAttribute("class", type.FullName + ", " + type.Assembly.ToString().Replace('"', '\''));
		securityElement.AddAttribute("version", version.ToString());
		return securityElement;
	}

	internal static PermissionState CheckPermissionState(PermissionState state, bool allowUnrestricted)
	{
		if (state != PermissionState.None && state != PermissionState.Unrestricted)
		{
			string message = string.Format(Locale.GetText("Invalid enum {0}"), state);
			throw new ArgumentException(message, "state");
		}
		return state;
	}

	internal static int CheckSecurityElement(SecurityElement se, string parameterName, int minimumVersion, int maximumVersion)
	{
		if (se == null)
		{
			throw new ArgumentNullException(parameterName);
		}
		if (se.Tag != "IPermission")
		{
			string message = string.Format(Locale.GetText("Invalid tag {0}"), se.Tag);
			throw new ArgumentException(message, parameterName);
		}
		int num = minimumVersion;
		string text = se.Attribute("version");
		if (text != null)
		{
			try
			{
				num = int.Parse(text);
			}
			catch (Exception innerException)
			{
				string text2 = Locale.GetText("Couldn't parse version from '{0}'.");
				text2 = string.Format(text2, text);
				throw new ArgumentException(text2, parameterName, innerException);
			}
		}
		if (num < minimumVersion || num > maximumVersion)
		{
			string text3 = Locale.GetText("Unknown version '{0}', expected versions between ['{1}','{2}'].");
			text3 = string.Format(text3, num, minimumVersion, maximumVersion);
			throw new ArgumentException(text3, parameterName);
		}
		return num;
	}

	internal static bool IsUnrestricted(SecurityElement se)
	{
		string text = se.Attribute("Unrestricted");
		if (text == null)
		{
			return false;
		}
		return string.Compare(text, bool.TrueString, ignoreCase: true, CultureInfo.InvariantCulture) == 0;
	}

	internal static void ThrowInvalidPermission(IPermission target, Type expected)
	{
		string text = Locale.GetText("Invalid permission type '{0}', expected type '{1}'.");
		text = string.Format(text, target.GetType(), expected);
		throw new ArgumentException(text, "target");
	}
}
