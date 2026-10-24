using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;

namespace System.Security;

[ComVisible(true)]
public static class SecurityManager
{
	private static object _lockObject;

	private static ArrayList _hierarchy;

	private static Hashtable _declsecCache;

	private static PolicyLevel _level;

	private static SecurityPermission _execution;

	public static extern bool CheckExecutionRights
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[Obsolete("The security manager cannot be turned off on MS runtime")]
	public static extern bool SecurityEnabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	private static IEnumerator Hierarchy
	{
		get
		{
			lock (_lockObject)
			{
				if (_hierarchy == null)
				{
					InitializePolicyHierarchy();
				}
			}
			return _hierarchy.GetEnumerator();
		}
	}

	internal static PolicyLevel ResolvingPolicyLevel
	{
		get
		{
			return _level;
		}
		set
		{
			_level = value;
		}
	}

	static SecurityManager()
	{
		_execution = new SecurityPermission(SecurityPermissionFlag.Execution);
		_lockObject = new object();
	}

	public static PermissionSet ResolvePolicy(Evidence evidence)
	{
		if (evidence == null)
		{
			return new PermissionSet(PermissionState.None);
		}
		PermissionSet ps = null;
		IEnumerator hierarchy = Hierarchy;
		while (hierarchy.MoveNext())
		{
			PolicyLevel pl = (PolicyLevel)hierarchy.Current;
			if (ResolvePolicyLevel(ref ps, pl, evidence))
			{
				break;
			}
		}
		ResolveIdentityPermissions(ps, evidence);
		return ps;
	}

	public static PermissionSet ResolvePolicy(Evidence evidence, PermissionSet reqdPset, PermissionSet optPset, PermissionSet denyPset, out PermissionSet denied)
	{
		PermissionSet permissionSet = ResolvePolicy(evidence);
		if (reqdPset != null && !reqdPset.IsSubsetOf(permissionSet))
		{
			throw new PolicyException(Locale.GetText("Policy doesn't grant the minimal permissions required to execute the assembly."));
		}
		if (CheckExecutionRights)
		{
			bool flag = false;
			if (permissionSet != null)
			{
				if (permissionSet.IsUnrestricted())
				{
					flag = true;
				}
				else
				{
					IPermission permission = permissionSet.GetPermission(typeof(SecurityPermission));
					flag = _execution.IsSubsetOf(permission);
				}
			}
			if (!flag)
			{
				throw new PolicyException(Locale.GetText("Policy doesn't grant the right to execute the assembly."));
			}
		}
		denied = denyPset;
		return permissionSet;
	}

	private static void InitializePolicyHierarchy()
	{
		string directoryName = Path.GetDirectoryName(Environment.GetMachineConfigPath());
		string path = Path.Combine(Environment.InternalGetFolderPath(Environment.SpecialFolder.ApplicationData), "mono");
		PolicyLevel policyLevel = (_level = new PolicyLevel("Enterprise", PolicyLevelType.Enterprise));
		policyLevel.LoadFromFile(Path.Combine(directoryName, "enterprisesec.config"));
		PolicyLevel policyLevel2 = (_level = new PolicyLevel("Machine", PolicyLevelType.Machine));
		policyLevel2.LoadFromFile(Path.Combine(directoryName, "security.config"));
		PolicyLevel policyLevel3 = (_level = new PolicyLevel("User", PolicyLevelType.User));
		policyLevel3.LoadFromFile(Path.Combine(path, "security.config"));
		ArrayList arrayList = new ArrayList();
		arrayList.Add(policyLevel);
		arrayList.Add(policyLevel2);
		arrayList.Add(policyLevel3);
		_hierarchy = ArrayList.Synchronized(arrayList);
		_level = null;
	}

	internal static bool ResolvePolicyLevel(ref PermissionSet ps, PolicyLevel pl, Evidence evidence)
	{
		PolicyStatement policyStatement = pl.Resolve(evidence);
		if (policyStatement != null)
		{
			if (ps == null)
			{
				ps = policyStatement.PermissionSet;
			}
			else
			{
				ps = ps.Intersect(policyStatement.PermissionSet);
				if (ps == null)
				{
					ps = new PermissionSet(PermissionState.None);
				}
			}
			if ((policyStatement.Attributes & PolicyStatementAttribute.LevelFinal) == PolicyStatementAttribute.LevelFinal)
			{
				return true;
			}
		}
		return false;
	}

	internal static void ResolveIdentityPermissions(PermissionSet ps, Evidence evidence)
	{
		if (ps.IsUnrestricted())
		{
			return;
		}
		IEnumerator hostEnumerator = evidence.GetHostEnumerator();
		while (hostEnumerator.MoveNext())
		{
			if (hostEnumerator.Current is IIdentityPermissionFactory identityPermissionFactory)
			{
				IPermission perm = identityPermissionFactory.CreateIdentityPermission(evidence);
				ps.AddPermission(perm);
			}
		}
	}

	internal static PermissionSet Decode(IntPtr permissions, int length)
	{
		PermissionSet permissionSet = null;
		lock (_lockObject)
		{
			if (_declsecCache == null)
			{
				_declsecCache = new Hashtable();
			}
			object key = (int)permissions;
			permissionSet = (PermissionSet)_declsecCache[key];
			if (permissionSet == null)
			{
				byte[] array = new byte[length];
				Marshal.Copy(permissions, array, 0, length);
				permissionSet = Decode(array);
				permissionSet.DeclarativeSecurity = true;
				_declsecCache.Add(key, permissionSet);
			}
		}
		return permissionSet;
	}

	internal static PermissionSet Decode(byte[] encodedPermissions)
	{
		if (encodedPermissions == null || encodedPermissions.Length < 1)
		{
			throw new SecurityException("Invalid metadata format.");
		}
		switch (encodedPermissions[0])
		{
		case 60:
		{
			string xml = Encoding.Unicode.GetString(encodedPermissions);
			return new PermissionSet(xml);
		}
		case 46:
			return PermissionSet.CreateFromBinaryFormat(encodedPermissions);
		default:
			throw new SecurityException(Locale.GetText("Unknown metadata format."));
		}
	}
}
