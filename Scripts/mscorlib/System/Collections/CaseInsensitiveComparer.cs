using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Collections;

[Serializable]
[ComVisible(true)]
public class CaseInsensitiveComparer : IComparer
{
	private static CaseInsensitiveComparer defaultComparer = new CaseInsensitiveComparer();

	private static CaseInsensitiveComparer defaultInvariantComparer = new CaseInsensitiveComparer(invariant: true);

	private CultureInfo culture;

	public static CaseInsensitiveComparer Default => defaultComparer;

	public static CaseInsensitiveComparer DefaultInvariant => defaultInvariantComparer;

	public CaseInsensitiveComparer()
	{
		culture = CultureInfo.CurrentCulture;
	}

	private CaseInsensitiveComparer(bool invariant)
	{
	}

	public int Compare(object a, object b)
	{
		string text = a as string;
		string text2 = b as string;
		if (text != null && text2 != null)
		{
			if (culture != null)
			{
				return culture.CompareInfo.Compare(text, text2, CompareOptions.IgnoreCase);
			}
			return CultureInfo.InvariantCulture.CompareInfo.Compare(text, text2, CompareOptions.IgnoreCase);
		}
		return Comparer.Default.Compare(a, b);
	}
}
