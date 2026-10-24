namespace System.Collections.Specialized;

[Serializable]
public class HybridDictionary : ICollection, IDictionary, IEnumerable
{
	private bool caseInsensitive;

	private Hashtable hashtable;

	private ListDictionary list;

	private IDictionary inner
	{
		get
		{
			object result;
			if (list == null)
			{
				IDictionary dictionary = hashtable;
				result = dictionary;
			}
			else
			{
				result = list;
			}
			return (IDictionary)result;
		}
	}

	public int Count => inner.Count;

	public bool IsSynchronized => false;

	public object this[object key]
	{
		get
		{
			return inner[key];
		}
		set
		{
			inner[key] = value;
			if (list != null && Count > 10)
			{
				Switch();
			}
		}
	}

	public ICollection Keys => inner.Keys;

	public object SyncRoot => this;

	public HybridDictionary()
		: this(0, caseInsensitive: false)
	{
	}

	public HybridDictionary(int initialSize, bool caseInsensitive)
	{
		this.caseInsensitive = caseInsensitive;
		IComparer comparer = ((!caseInsensitive) ? null : CaseInsensitiveComparer.DefaultInvariant);
		IHashCodeProvider hcp = ((!caseInsensitive) ? null : CaseInsensitiveHashCodeProvider.DefaultInvariant);
		if (initialSize <= 10)
		{
			list = new ListDictionary(comparer);
		}
		else
		{
			hashtable = new Hashtable(initialSize, hcp, comparer);
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Add(object key, object value)
	{
		inner.Add(key, value);
		if (list != null && Count > 10)
		{
			Switch();
		}
	}

	public bool Contains(object key)
	{
		return inner.Contains(key);
	}

	public void CopyTo(Array array, int index)
	{
		inner.CopyTo(array, index);
	}

	public IDictionaryEnumerator GetEnumerator()
	{
		return inner.GetEnumerator();
	}

	public void Remove(object key)
	{
		inner.Remove(key);
	}

	private void Switch()
	{
		IComparer comparer = ((!caseInsensitive) ? null : CaseInsensitiveComparer.DefaultInvariant);
		IHashCodeProvider hcp = ((!caseInsensitive) ? null : CaseInsensitiveHashCodeProvider.DefaultInvariant);
		hashtable = new Hashtable(list, hcp, comparer);
		list.Clear();
		list = null;
	}
}
