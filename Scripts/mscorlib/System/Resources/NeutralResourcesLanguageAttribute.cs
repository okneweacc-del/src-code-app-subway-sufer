using System.Runtime.InteropServices;

namespace System.Resources;

[AttributeUsage(AttributeTargets.Assembly)]
[ComVisible(true)]
public sealed class NeutralResourcesLanguageAttribute : Attribute
{
	private string culture;

	public NeutralResourcesLanguageAttribute(string cultureName)
	{
		if (cultureName == null)
		{
			throw new ArgumentNullException("culture is null");
		}
		culture = cultureName;
	}
}
