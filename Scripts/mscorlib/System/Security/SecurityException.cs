using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;

namespace System.Security;

[Serializable]
[ComVisible(true)]
public class SecurityException : SystemException
{
	private string permissionState;

	private Type permissionType;

	private string _granted;

	private string _refused;

	private object _demanded;

	private IPermission _firstperm;

	private MethodInfo _method;

	private Evidence _evidence;

	[ComVisible(false)]
	public object Demanded => _demanded;

	public IPermission FirstPermissionThatFailed => _firstperm;

	public string PermissionState => permissionState;

	public Type PermissionType => permissionType;

	public string GrantedSet => _granted;

	public string RefusedSet => _refused;

	public SecurityException()
		: base(Locale.GetText("A security error has been detected."))
	{
		base.HResult = -2146233078;
	}

	public SecurityException(string message)
		: base(message)
	{
		base.HResult = -2146233078;
	}

	protected SecurityException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		base.HResult = -2146233078;
		SerializationInfoEnumerator enumerator = info.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Name == "PermissionState")
			{
				permissionState = (string)enumerator.Value;
				break;
			}
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		try
		{
			info.AddValue("PermissionState", PermissionState);
		}
		catch (SecurityException)
		{
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(base.ToString());
		try
		{
			if (permissionType != null)
			{
				stringBuilder.AppendFormat("{0}Type: {1}", Environment.NewLine, PermissionType);
			}
			if (_method != null)
			{
				string text = _method.ToString();
				int startIndex = text.IndexOf(" ") + 1;
				stringBuilder.AppendFormat("{0}Method: {1} {2}.{3}", Environment.NewLine, _method.ReturnType.Name, _method.ReflectedType, text.Substring(startIndex));
			}
			if (permissionState != null)
			{
				stringBuilder.AppendFormat("{0}State: {1}", Environment.NewLine, PermissionState);
			}
			if (_granted != null && _granted.Length > 0)
			{
				stringBuilder.AppendFormat("{0}Granted: {1}", Environment.NewLine, GrantedSet);
			}
			if (_refused != null && _refused.Length > 0)
			{
				stringBuilder.AppendFormat("{0}Refused: {1}", Environment.NewLine, RefusedSet);
			}
			if (_demanded != null)
			{
				stringBuilder.AppendFormat("{0}Demanded: {1}", Environment.NewLine, Demanded);
			}
			if (_firstperm != null)
			{
				stringBuilder.AppendFormat("{0}Failed Permission: {1}", Environment.NewLine, FirstPermissionThatFailed);
			}
			if (_evidence != null)
			{
				stringBuilder.AppendFormat("{0}Evidences:", Environment.NewLine);
				foreach (object item in _evidence)
				{
					if (!(item is Hash))
					{
						stringBuilder.AppendFormat("{0}\t{1}", Environment.NewLine, item);
					}
				}
			}
		}
		catch (SecurityException)
		{
		}
		return stringBuilder.ToString();
	}
}
