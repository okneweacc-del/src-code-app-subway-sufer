using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Collections.ObjectModel;

[Serializable]
[ComVisible(false)]
public class ReadOnlyCollection<T> : IEnumerable, ICollection, IList, ICollection<T>, IList<T>, IEnumerable<T>
{
	private IList<T> list;

	T IList<T>.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	bool ICollection<T>.IsReadOnly => true;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	bool IList.IsFixedSize => true;

	bool IList.IsReadOnly => true;

	object IList.this[int index]
	{
		get
		{
			return list[index];
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public int Count => list.Count;

	public T this[int index] => list[index];

	public ReadOnlyCollection(IList<T> list)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		this.list = list;
	}

	void ICollection<T>.Add(T item)
	{
		throw new NotSupportedException();
	}

	void ICollection<T>.Clear()
	{
		throw new NotSupportedException();
	}

	void IList<T>.Insert(int index, T item)
	{
		throw new NotSupportedException();
	}

	bool ICollection<T>.Remove(T item)
	{
		throw new NotSupportedException();
	}

	void IList<T>.RemoveAt(int index)
	{
		throw new NotSupportedException();
	}

	void ICollection.CopyTo(Array array, int index)
	{
		((ICollection)list).CopyTo(array, index);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)list).GetEnumerator();
	}

	int IList.Add(object value)
	{
		throw new NotSupportedException();
	}

	void IList.Clear()
	{
		throw new NotSupportedException();
	}

	bool IList.Contains(object value)
	{
		if (Collection<T>.IsValidItem(value))
		{
			return list.Contains((T)value);
		}
		return false;
	}

	int IList.IndexOf(object value)
	{
		if (Collection<T>.IsValidItem(value))
		{
			return list.IndexOf((T)value);
		}
		return -1;
	}

	void IList.Insert(int index, object value)
	{
		throw new NotSupportedException();
	}

	void IList.Remove(object value)
	{
		throw new NotSupportedException();
	}

	void IList.RemoveAt(int index)
	{
		throw new NotSupportedException();
	}

	public bool Contains(T value)
	{
		return list.Contains(value);
	}

	public void CopyTo(T[] array, int index)
	{
		list.CopyTo(array, index);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return list.GetEnumerator();
	}

	public int IndexOf(T value)
	{
		return list.IndexOf(value);
	}
}
