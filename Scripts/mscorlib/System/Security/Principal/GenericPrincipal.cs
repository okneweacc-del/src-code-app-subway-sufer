using System.Runtime.InteropServices;

namespace System.Security.Principal;

[Serializable]
[ComVisible(true)]
public class GenericPrincipal : IPrincipal
{
	private IIdentity m_identity;

	private string[] m_roles;

	public GenericPrincipal(IIdentity identity, string[] roles)
	{
		if (identity == null)
		{
			throw new ArgumentNullException("identity");
		}
		m_identity = identity;
		if (roles != null)
		{
			m_roles = new string[roles.Length];
			for (int i = 0; i < roles.Length; i++)
			{
				m_roles[i] = roles[i];
			}
		}
	}
}
