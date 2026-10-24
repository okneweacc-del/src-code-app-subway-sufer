using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Collections.ObjectModel;

[Serializable]
[ComVisible(false)]
public class Collection<T> : IEnumerable, ICollection, IList, ICollection<T>, IList<T>, IEnumerable<T>
{
	private IList<T> list;

	private object syncRoot;

	bool ICollection<T>.IsReadOnly => list.IsReadOnly;

	bool ICollection.IsSynchronized => IsSynchronized(list);

	object ICollection.SyncRoot => syncRoot;

	bool IList.IsFixedSize => IsFixedSize(list);

	bool IList.IsReadOnly => list.IsReadOnly;

	object IList.this[int index]
	{
		get
		{
			return list[index];
		}
		set
		{
			SetItem(index, ConvertItem(value));
		}
	}

	public int Count => list.Count;

	public T this[int index]
	{
		get
		{
			return list[index];
		}
		set
		{
			SetItem(index, value);
		}
	}

	public Collection()
	{
		List<T> list = new List<T>();
		IList list2 = list;
		syncRoot = list2.SyncRoot;
		this.list = list;
	}

	void ICollection.CopyTo(Array array, int index)
	{
		((ICollection)list).CopyTo(array, index);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return list.GetEnumerator();
	}

	int IList.Add(object value)
	{
		int count = list.Count;
		InsertItem(count, ConvertItem(value));
		return count;
	}

	bool IList.Contains(object value)
	{
		if (IsValidItem(value))
		{
			return list.Contains((T)value);
		}
		return false;
	}

	int IList.IndexOf(object value)
	{
		if (IsValidItem(value))
		{
			return list.IndexOf((T)value);
		}
		return -1;
	}

	void IList.Insert(int index, object value)
	{
		InsertItem(index, ConvertItem(value));
	}

	void IList.Remove(object value)
	{
		CheckWritable(list);
		int index = IndexOf(ConvertItem(value));
		RemoveItem(index);
	}

	public void Add(T item)
	{
		int count = list.Count;
		InsertItem(count, item);
	}

	public void Clear()
	{
		ClearItems();
	}

	protected virtual void ClearItems()
	{
		list.Clear();
	}

	public bool Contains(T item)
	{
		return list.Contains(item);
	}

	public void CopyTo(T[] array, int index)
	{
		list.CopyTo(array, index);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return list.GetEnumerator();
	}

	public int IndexOf(T item)
	{
		return list.IndexOf(item);
	}

	public void Insert(int index, T item)
	{
		InsertItem(index, item);
	}

	protected virtual void InsertItem(int index, T item)
	{
		list.Insert(index, item);
	}

	public bool Remove(T item)
	{
		int num = IndexOf(item);
		if (num == -1)
		{
			return false;
		}
		RemoveItem(num);
		return true;
	}

	public void RemoveAt(int index)
	{
		RemoveItem(index);
	}

	protected virtual void RemoveItem(int index)
	{
		list.RemoveAt(index);
	}

	protected virtual void SetItem(int index, T item)
	{
		list[index] = item;
	}

	internal static bool IsValidItem(object item)
	{
		return item is T || (item == null && !typeof(T).IsValueType);
	}

	internal static T ConvertItem(object item)
	{
		if (IsValidItem(item))
		{
			return (T)item;
		}
		throw new ArgumentException("item");
	}

	internal static void CheckWritable(IList<T> list)
	{
		if (list.IsReadOnly)
		{
			throw new NotSupportedException();
		}
	}

	internal static bool IsSynchronized(IList<T> list)
	{
		return list is ICollection collection && collection.IsSynchronized;
	}

	internal static bool IsFixedSize(IList<T> list)
	{
		return list is IList list2 && list2.IsFixedSize;
	}
}
