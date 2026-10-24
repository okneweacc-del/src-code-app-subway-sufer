using System.Runtime.InteropServices;

namespace System.Security.Permissions;

[Serializable]
[ComVisible(true)]
[Obsolete("CAS support is not available with Silverlight applications.")]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class SecurityPermissionAttribute : CodeAccessSecurityAttribute
{
	private SecurityPermissionFlag m_Flags;

	public bool SkipVerification
	{
		set
		{
			if (value)
			{
				m_Flags |= SecurityPermissionFlag.SkipVerification;
			}
			else
			{
				m_Flags &= ~SecurityPermissionFlag.SkipVerification;
			}
		}
	}
}
