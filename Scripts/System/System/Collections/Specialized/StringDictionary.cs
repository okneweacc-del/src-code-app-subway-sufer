using System.Globalization;
using System.Reflection;

namespace System.Collections.Specialized;

[Serializable]
[DefaultMember("Item")]
public class StringDictionary : IEnumerable
{
	private Hashtable contents;

	public virtual int Count => contents.Count;

	public virtual ICollection Keys => contents.Keys;

	public virtual ICollection Values => contents.Values;

	public StringDictionary()
	{
		contents = new Hashtable();
	}

	public virtual void Add(string key, string value)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		contents.Add(key.ToLower(CultureInfo.InvariantCulture), value);
	}

	public virtual IEnumerator GetEnumerator()
	{
		return contents.GetEnumerator();
	}
}
