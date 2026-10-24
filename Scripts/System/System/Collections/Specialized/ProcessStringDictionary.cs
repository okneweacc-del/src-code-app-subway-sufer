using System.Reflection;

namespace System.Collections.Specialized;

[DefaultMember("Item")]
internal class ProcessStringDictionary : StringDictionary, IEnumerable
{
	private Hashtable table;

	public override int Count => table.Count;

	public override ICollection Keys => table.Keys;

	public override ICollection Values => table.Values;

	public ProcessStringDictionary()
	{
		IHashCodeProvider hcp = null;
		IComparer comparer = null;
		int platform = (int)Environment.OSVersion.Platform;
		if (platform != 4 && platform != 128)
		{
			hcp = CaseInsensitiveHashCodeProvider.DefaultInvariant;
			comparer = CaseInsensitiveComparer.DefaultInvariant;
		}
		table = new Hashtable(hcp, comparer);
	}

	public override void Add(string key, string value)
	{
		table.Add(key, value);
	}

	public override IEnumerator GetEnumerator()
	{
		return table.GetEnumerator();
	}
}
