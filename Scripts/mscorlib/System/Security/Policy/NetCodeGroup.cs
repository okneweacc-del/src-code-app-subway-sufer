using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy;

[Serializable]
[ComVisible(true)]
public sealed class NetCodeGroup : CodeGroup
{
	public static readonly string AbsentOriginScheme = string.Empty;

	public static readonly string AnyOtherOriginScheme = "*";

	private Hashtable _rules = new Hashtable();

	private int _hashcode;

	public NetCodeGroup(IMembershipCondition membershipCondition)
		: base(membershipCondition, null)
	{
	}

	internal NetCodeGroup(SecurityElement e, PolicyLevel level)
		: base(e, level)
	{
	}

	public override CodeGroup Copy()
	{
		NetCodeGroup netCodeGroup = new NetCodeGroup(base.MembershipCondition);
		netCodeGroup.Name = base.Name;
		netCodeGroup.Description = base.Description;
		netCodeGroup.PolicyStatement = base.PolicyStatement;
		foreach (CodeGroup child in base.Children)
		{
			netCodeGroup.AddChild(child.Copy());
		}
		return netCodeGroup;
	}

	private bool Equals(CodeConnectAccess[] rules1, CodeConnectAccess[] rules2)
	{
		for (int i = 0; i < rules1.Length; i++)
		{
			bool flag = false;
			for (int j = 0; j < rules2.Length; j++)
			{
				if (rules1[i].Equals(rules2[j]))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	public override bool Equals(object o)
	{
		if (!base.Equals(o))
		{
			return false;
		}
		if (!(o is NetCodeGroup netCodeGroup))
		{
			return false;
		}
		foreach (DictionaryEntry rule in _rules)
		{
			bool flag = false;
			CodeConnectAccess[] array = (CodeConnectAccess[])netCodeGroup._rules[rule.Key];
			if (!((array == null) ? (rule.Value == null) : Equals((CodeConnectAccess[])rule.Value, array)))
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		if (_hashcode == 0)
		{
			_hashcode = base.GetHashCode();
			foreach (DictionaryEntry rule in _rules)
			{
				CodeConnectAccess[] array = (CodeConnectAccess[])rule.Value;
				if (array != null)
				{
					CodeConnectAccess[] array2 = array;
					foreach (CodeConnectAccess codeConnectAccess in array2)
					{
						_hashcode ^= codeConnectAccess.GetHashCode();
					}
				}
			}
		}
		return _hashcode;
	}

	public override PolicyStatement Resolve(Evidence evidence)
	{
		if (evidence == null)
		{
			throw new ArgumentNullException("evidence");
		}
		if (!base.MembershipCondition.Check(evidence))
		{
			return null;
		}
		PermissionSet permissionSet = null;
		permissionSet = ((base.PolicyStatement != null) ? base.PolicyStatement.PermissionSet.Copy() : new PermissionSet(PermissionState.None));
		if (base.Children.Count > 0)
		{
			foreach (CodeGroup child in base.Children)
			{
				PolicyStatement policyStatement = child.Resolve(evidence);
				if (policyStatement != null)
				{
					permissionSet = permissionSet.Union(policyStatement.PermissionSet);
				}
			}
		}
		PolicyStatement policyStatement2 = base.PolicyStatement.Copy();
		policyStatement2.PermissionSet = permissionSet;
		return policyStatement2;
	}

	[MonoTODO("(2.0) Add new stuff (CodeConnectAccess) into XML")]
	protected override void CreateXml(SecurityElement element, PolicyLevel level)
	{
		base.CreateXml(element, level);
	}

	[MonoTODO("(2.0) Parse new stuff (CodeConnectAccess) from XML")]
	protected override void ParseXml(SecurityElement e, PolicyLevel level)
	{
		base.ParseXml(e, level);
	}
}
