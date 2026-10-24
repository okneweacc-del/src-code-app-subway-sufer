using System.Runtime.InteropServices;

namespace System.Security.Principal;

[Serializable]
[ComVisible(true)]
public class WindowsPrincipal : IPrincipal
{
	private WindowsIdentity _identity;

	public WindowsPrincipal(WindowsIdentity ntIdentity)
	{
		if (ntIdentity == null)
		{
			throw new ArgumentNullException("ntIdentity");
		}
		_identity = ntIdentity;
	}
}
