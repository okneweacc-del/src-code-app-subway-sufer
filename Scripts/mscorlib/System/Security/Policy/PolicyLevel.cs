using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Mono.Xml;

namespace System.Security.Policy;

[Serializable]
[ComVisible(true)]
public sealed class PolicyLevel
{
	private string label;

	private CodeGroup root_code_group;

	private ArrayList full_trust_assemblies;

	private ArrayList named_permission_sets;

	private string _location;

	private PolicyLevelType _type;

	private Hashtable fullNames;

	private SecurityElement xml;

	internal PolicyLevel(string label, PolicyLevelType type)
	{
		this.label = label;
		_type = type;
		full_trust_assemblies = new ArrayList();
		named_permission_sets = new ArrayList();
	}

	internal void LoadFromFile(string filename)
	{
		try
		{
			if (!File.Exists(filename))
			{
				string text = filename + ".default";
				if (File.Exists(text))
				{
					File.Copy(text, filename);
				}
			}
			if (File.Exists(filename))
			{
				using (StreamReader streamReader = File.OpenText(filename))
				{
					xml = FromString(streamReader.ReadToEnd());
				}
				try
				{
					SecurityManager.ResolvingPolicyLevel = this;
					FromXml(xml);
					return;
				}
				finally
				{
					SecurityManager.ResolvingPolicyLevel = this;
				}
			}
			CreateDefaultFullTrustAssemblies();
			CreateDefaultNamedPermissionSets();
			CreateDefaultLevel(_type);
			Save();
		}
		catch
		{
		}
		finally
		{
			_location = filename;
		}
	}

	private SecurityElement FromString(string xml)
	{
		SecurityParser securityParser = new SecurityParser();
		securityParser.LoadXml(xml);
		SecurityElement securityElement = securityParser.ToXml();
		if (securityElement.Tag != "configuration")
		{
			throw new ArgumentException(Locale.GetText("missing <configuration> root element"));
		}
		SecurityElement securityElement2 = (SecurityElement)securityElement.Children[0];
		if (securityElement2.Tag != "mscorlib")
		{
			throw new ArgumentException(Locale.GetText("missing <mscorlib> tag"));
		}
		SecurityElement securityElement3 = (SecurityElement)securityElement2.Children[0];
		if (securityElement3.Tag != "security")
		{
			throw new ArgumentException(Locale.GetText("missing <security> tag"));
		}
		SecurityElement securityElement4 = (SecurityElement)securityElement3.Children[0];
		if (securityElement4.Tag != "policy")
		{
			throw new ArgumentException(Locale.GetText("missing <policy> tag"));
		}
		return (SecurityElement)securityElement4.Children[0];
	}

	public void FromXml(SecurityElement e)
	{
		if (e == null)
		{
			throw new ArgumentNullException("e");
		}
		SecurityElement securityElement = e.SearchForChildByTag("SecurityClasses");
		if (securityElement != null && securityElement.Children != null && securityElement.Children.Count > 0)
		{
			fullNames = new Hashtable(securityElement.Children.Count);
			foreach (SecurityElement child in securityElement.Children)
			{
				fullNames.Add(child.Attributes["Name"], child.Attributes["Description"]);
			}
		}
		SecurityElement securityElement3 = e.SearchForChildByTag("FullTrustAssemblies");
		if (securityElement3 != null && securityElement3.Children != null && securityElement3.Children.Count > 0)
		{
			full_trust_assemblies.Clear();
			foreach (SecurityElement child2 in securityElement3.Children)
			{
				if (child2.Tag != "IMembershipCondition")
				{
					throw new ArgumentException(Locale.GetText("Invalid XML"));
				}
				string text = child2.Attribute("class");
				if (text.IndexOf("StrongNameMembershipCondition") < 0)
				{
					throw new ArgumentException(Locale.GetText("Invalid XML - must be StrongNameMembershipCondition"));
				}
				full_trust_assemblies.Add(new StrongNameMembershipCondition(child2));
			}
		}
		SecurityElement securityElement5 = e.SearchForChildByTag("CodeGroup");
		if (securityElement5 != null && securityElement5.Children != null && securityElement5.Children.Count > 0)
		{
			root_code_group = CodeGroup.CreateFromXml(securityElement5, this);
			SecurityElement securityElement6 = e.SearchForChildByTag("NamedPermissionSets");
			if (securityElement6 == null || securityElement6.Children == null || securityElement6.Children.Count <= 0)
			{
				return;
			}
			named_permission_sets.Clear();
			{
				foreach (SecurityElement child3 in securityElement6.Children)
				{
					NamedPermissionSet namedPermissionSet = new NamedPermissionSet();
					namedPermissionSet.Resolver = this;
					namedPermissionSet.FromXml(child3);
					named_permission_sets.Add(namedPermissionSet);
				}
				return;
			}
		}
		throw new ArgumentException(Locale.GetText("Missing Root CodeGroup"));
	}

	public NamedPermissionSet GetNamedPermissionSet(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		foreach (NamedPermissionSet named_permission_set in named_permission_sets)
		{
			if (named_permission_set.Name == name)
			{
				return (NamedPermissionSet)named_permission_set.Copy();
			}
		}
		return null;
	}

	public PolicyStatement Resolve(Evidence evidence)
	{
		if (evidence == null)
		{
			throw new ArgumentNullException("evidence");
		}
		PolicyStatement policyStatement = root_code_group.Resolve(evidence);
		return (policyStatement == null) ? PolicyStatement.Empty() : policyStatement;
	}

	public SecurityElement ToXml()
	{
		Hashtable hashtable = new Hashtable();
		if (full_trust_assemblies.Count > 0 && !hashtable.Contains("StrongNameMembershipCondition"))
		{
			hashtable.Add("StrongNameMembershipCondition", typeof(StrongNameMembershipCondition).FullName);
		}
		SecurityElement securityElement = new SecurityElement("NamedPermissionSets");
		foreach (NamedPermissionSet named_permission_set in named_permission_sets)
		{
			SecurityElement securityElement2 = named_permission_set.ToXml();
			object key = securityElement2.Attributes["class"];
			if (!hashtable.Contains(key))
			{
				hashtable.Add(key, named_permission_set.GetType().FullName);
			}
			securityElement.AddChild(securityElement2);
		}
		SecurityElement securityElement3 = new SecurityElement("FullTrustAssemblies");
		foreach (StrongNameMembershipCondition full_trust_assembly in full_trust_assemblies)
		{
			securityElement3.AddChild(full_trust_assembly.ToXml(this));
		}
		SecurityElement securityElement4 = new SecurityElement("SecurityClasses");
		if (hashtable.Count > 0)
		{
			foreach (DictionaryEntry item in hashtable)
			{
				SecurityElement securityElement5 = new SecurityElement("SecurityClass");
				securityElement5.AddAttribute("Name", (string)item.Key);
				securityElement5.AddAttribute("Description", (string)item.Value);
				securityElement4.AddChild(securityElement5);
			}
		}
		SecurityElement securityElement6 = new SecurityElement(typeof(PolicyLevel).Name);
		securityElement6.AddAttribute("version", "1");
		securityElement6.AddChild(securityElement4);
		securityElement6.AddChild(securityElement);
		if (root_code_group != null)
		{
			securityElement6.AddChild(root_code_group.ToXml(this));
		}
		securityElement6.AddChild(securityElement3);
		return securityElement6;
	}

	internal void Save()
	{
		if (_type == PolicyLevelType.AppDomain)
		{
			throw new PolicyException(Locale.GetText("Can't save AppDomain PolicyLevel"));
		}
		if (_location == null)
		{
			return;
		}
		try
		{
			if (File.Exists(_location))
			{
				File.Copy(_location, _location + ".backup", overwrite: true);
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			using StreamWriter streamWriter = new StreamWriter(_location);
			streamWriter.Write(ToXml().ToString());
			streamWriter.Close();
		}
	}

	internal void CreateDefaultLevel(PolicyLevelType type)
	{
		PolicyStatement policy = new PolicyStatement(DefaultPolicies.FullTrust);
		switch (type)
		{
		case PolicyLevelType.Machine:
		{
			PolicyStatement policy2 = new PolicyStatement(DefaultPolicies.Nothing);
			root_code_group = new UnionCodeGroup(new AllMembershipCondition(), policy2);
			root_code_group.Name = "All_Code";
			UnionCodeGroup unionCodeGroup = new UnionCodeGroup(new ZoneMembershipCondition(SecurityZone.MyComputer), policy);
			unionCodeGroup.Name = "My_Computer_Zone";
			root_code_group.AddChild(unionCodeGroup);
			UnionCodeGroup unionCodeGroup2 = new UnionCodeGroup(new ZoneMembershipCondition(SecurityZone.Intranet), new PolicyStatement(DefaultPolicies.LocalIntranet));
			unionCodeGroup2.Name = "LocalIntranet_Zone";
			root_code_group.AddChild(unionCodeGroup2);
			PolicyStatement policy3 = new PolicyStatement(DefaultPolicies.Internet);
			UnionCodeGroup unionCodeGroup3 = new UnionCodeGroup(new ZoneMembershipCondition(SecurityZone.Internet), policy3);
			unionCodeGroup3.Name = "Internet_Zone";
			root_code_group.AddChild(unionCodeGroup3);
			UnionCodeGroup unionCodeGroup4 = new UnionCodeGroup(new ZoneMembershipCondition(SecurityZone.Untrusted), policy2);
			unionCodeGroup4.Name = "Restricted_Zone";
			root_code_group.AddChild(unionCodeGroup4);
			UnionCodeGroup unionCodeGroup5 = new UnionCodeGroup(new ZoneMembershipCondition(SecurityZone.Trusted), policy3);
			unionCodeGroup5.Name = "Trusted_Zone";
			root_code_group.AddChild(unionCodeGroup5);
			break;
		}
		case PolicyLevelType.User:
		case PolicyLevelType.Enterprise:
		case PolicyLevelType.AppDomain:
			root_code_group = new UnionCodeGroup(new AllMembershipCondition(), policy);
			root_code_group.Name = "All_Code";
			break;
		}
	}

	internal void CreateDefaultFullTrustAssemblies()
	{
		full_trust_assemblies.Clear();
		full_trust_assemblies.Add(DefaultPolicies.FullTrustMembership("mscorlib", DefaultPolicies.Key.Ecma));
		full_trust_assemblies.Add(DefaultPolicies.FullTrustMembership("System", DefaultPolicies.Key.Ecma));
		full_trust_assemblies.Add(DefaultPolicies.FullTrustMembership("System.Data", DefaultPolicies.Key.Ecma));
		full_trust_assemblies.Add(DefaultPolicies.FullTrustMembership("System.DirectoryServices", DefaultPolicies.Key.MsFinal));
		full_trust_assemblies.Add(DefaultPolicies.FullTrustMembership("System.Drawing", DefaultPolicies.Key.MsFinal));
		full_trust_assemblies.Add(DefaultPolicies.FullTrustMembership("System.Messaging", DefaultPolicies.Key.MsFinal));
		full_trust_assemblies.Add(DefaultPolicies.FullTrustMembership("System.ServiceProcess", DefaultPolicies.Key.MsFinal));
	}

	internal void CreateDefaultNamedPermissionSets()
	{
		named_permission_sets.Clear();
		try
		{
			SecurityManager.ResolvingPolicyLevel = this;
			named_permission_sets.Add(DefaultPolicies.LocalIntranet);
			named_permission_sets.Add(DefaultPolicies.Internet);
			named_permission_sets.Add(DefaultPolicies.SkipVerification);
			named_permission_sets.Add(DefaultPolicies.Execution);
			named_permission_sets.Add(DefaultPolicies.Nothing);
			named_permission_sets.Add(DefaultPolicies.Everything);
			named_permission_sets.Add(DefaultPolicies.FullTrust);
		}
		finally
		{
			SecurityManager.ResolvingPolicyLevel = null;
		}
	}

	internal bool IsFullTrustAssembly(Assembly a)
	{
		AssemblyName assemblyName = a.UnprotectedGetName();
		StrongNamePublicKeyBlob blob = new StrongNamePublicKeyBlob(assemblyName.GetPublicKey());
		StrongNameMembershipCondition o = new StrongNameMembershipCondition(blob, assemblyName.Name, assemblyName.Version);
		foreach (StrongNameMembershipCondition full_trust_assembly in full_trust_assemblies)
		{
			if (full_trust_assembly.Equals(o))
			{
				return true;
			}
		}
		return false;
	}
}
