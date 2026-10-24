using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Security;

internal struct SecurityFrame
{
	private AppDomain _domain = null;

	private MethodInfo _method = null;

	private PermissionSet _assert = null;

	private PermissionSet _deny = null;

	private PermissionSet _permitonly = null;

	public Assembly Assembly => _method.ReflectedType.Assembly;

	public AppDomain Domain => _domain;

	public MethodInfo Method => _method;

	public PermissionSet Assert => _assert;

	public PermissionSet Deny => _deny;

	public PermissionSet PermitOnly => _permitonly;

	internal SecurityFrame(RuntimeSecurityFrame frame)
	{
		InitFromRuntimeFrame(frame);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Array _GetSecurityStack(int skip);

	internal void InitFromRuntimeFrame(RuntimeSecurityFrame frame)
	{
		_domain = frame.domain;
		_method = frame.method;
		if (frame.assert.size > 0)
		{
			_assert = SecurityManager.Decode(frame.assert.blob, frame.assert.size);
		}
		if (frame.deny.size > 0)
		{
			_deny = SecurityManager.Decode(frame.deny.blob, frame.deny.size);
		}
		if (frame.permitonly.size > 0)
		{
			_permitonly = SecurityManager.Decode(frame.permitonly.blob, frame.permitonly.size);
		}
	}

	public bool Equals(SecurityFrame sf)
	{
		if (!object.ReferenceEquals(_domain, sf.Domain))
		{
			return false;
		}
		if (Assembly.ToString() != sf.Assembly.ToString())
		{
			return false;
		}
		if (Method.ToString() != sf.Method.ToString())
		{
			return false;
		}
		if (_assert != null && !_assert.Equals(sf.Assert))
		{
			return false;
		}
		if (_deny != null && !_deny.Equals(sf.Deny))
		{
			return false;
		}
		if (_permitonly != null && !_permitonly.Equals(sf.PermitOnly))
		{
			return false;
		}
		return true;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("Frame: {0}{1}", _method, Environment.NewLine);
		stringBuilder.AppendFormat("\tAppDomain: {0}{1}", Domain, Environment.NewLine);
		stringBuilder.AppendFormat("\tAssembly: {0}{1}", Assembly, Environment.NewLine);
		if (_assert != null)
		{
			stringBuilder.AppendFormat("\tAssert: {0}{1}", _assert, Environment.NewLine);
		}
		if (_deny != null)
		{
			stringBuilder.AppendFormat("\tDeny: {0}{1}", _deny, Environment.NewLine);
		}
		if (_permitonly != null)
		{
			stringBuilder.AppendFormat("\tPermitOnly: {0}{1}", _permitonly, Environment.NewLine);
		}
		return stringBuilder.ToString();
	}

	public static ArrayList GetStack(int skipFrames)
	{
		Array array = _GetSecurityStack(skipFrames + 2);
		ArrayList arrayList = new ArrayList();
		for (int i = 0; i < array.Length; i++)
		{
			object value = array.GetValue(i);
			if (value == null)
			{
				break;
			}
			arrayList.Add(new SecurityFrame((RuntimeSecurityFrame)value));
		}
		return arrayList;
	}
}
