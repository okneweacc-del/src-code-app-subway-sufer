using System.Runtime.InteropServices;

namespace System.Runtime.Hosting;

[Serializable]
[ComVisible(true)]
public sealed class ActivationArguments
{
	private ActivationContext _context;

	private ApplicationIdentity _identity;

	public ActivationArguments(ActivationContext activationData)
	{
		if (activationData == null)
		{
			throw new ArgumentNullException("activationData");
		}
		_context = activationData;
		_identity = activationData.Identity;
	}
}
