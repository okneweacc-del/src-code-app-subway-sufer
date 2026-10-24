using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace System.Collections.Specialized;

[Serializable]
[DefaultMember("Item")]
public class NameValueCollection : NameObjectCollectionBase
{
	private string[] cachedAllKeys;

	private string[] cachedAll;

	public NameValueCollection()
	{
	}

	protected NameValueCollection(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public virtual void Add(string name, string val)
	{
		if (base.IsReadOnly)
		{
			throw new NotSupportedException("Collection is read-only");
		}
		InvalidateCachedArrays();
		ArrayList arrayList = (ArrayList)BaseGet(name);
		if (arrayList == null)
		{
			arrayList = new ArrayList();
			if (val != null)
			{
				arrayList.Add(val);
			}
			BaseAdd(name, arrayList);
		}
		else if (val != null)
		{
			arrayList.Add(val);
		}
	}

	public virtual string Get(int index)
	{
		ArrayList values = (ArrayList)BaseGet(index);
		return AsSingleString(values);
	}

	private static string AsSingleString(ArrayList values)
	{
		if (values == null)
		{
			return null;
		}
		int count = values.Count;
		switch (count)
		{
		case 0:
			return null;
		case 1:
			return (string)values[0];
		case 2:
			return (string)values[0] + ',' + (string)values[1];
		default:
		{
			int num = count;
			for (int i = 0; i < count; i++)
			{
				num += ((string)values[i]).Length;
			}
			StringBuilder stringBuilder = new StringBuilder((string)values[0], num);
			for (int j = 1; j < count; j++)
			{
				stringBuilder.Append(',');
				stringBuilder.Append(values[j]);
			}
			return stringBuilder.ToString();
		}
		}
	}

	public virtual string GetKey(int index)
	{
		return BaseGetKey(index);
	}

	protected void InvalidateCachedArrays()
	{
		cachedAllKeys = null;
		cachedAll = null;
	}
}
