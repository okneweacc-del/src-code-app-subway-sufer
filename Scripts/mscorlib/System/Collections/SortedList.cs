using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Collections;

[Serializable]
[DebuggerDisplay("Count={Count}")]
[ComVisible(true)]
public class SortedList : IEnumerable, ICloneable, ICollection, IDictionary
{
	[Serializable]
	internal struct Slot
	{
		internal object key;

		internal object value;
	}

	private enum EnumeratorMode
	{
		KEY_MODE = 0,
		VALUE_MODE = 1,
		ENTRY_MODE = 2
	}

	private sealed class Enumerator : IEnumerator, ICloneable, IDictionaryEnumerator
	{
		private SortedList host;

		private int stamp;

		private int pos;

		private int size;

		private EnumeratorMode mode;

		private object currentKey;

		private object currentValue;

		private bool invalid;

		private static readonly string xstr = "SortedList.Enumerator: snapshot out of sync.";

		public DictionaryEntry Entry
		{
			get
			{
				if (invalid || pos >= size || pos == -1)
				{
					throw new InvalidOperationException(xstr);
				}
				return new DictionaryEntry(currentKey, currentValue);
			}
		}

		public object Key
		{
			get
			{
				if (invalid || pos >= size || pos == -1)
				{
					throw new InvalidOperationException(xstr);
				}
				return currentKey;
			}
		}

		public object Value
		{
			get
			{
				if (invalid || pos >= size || pos == -1)
				{
					throw new InvalidOperationException(xstr);
				}
				return currentValue;
			}
		}

		public object Current
		{
			get
			{
				if (invalid || pos >= size || pos == -1)
				{
					throw new InvalidOperationException(xstr);
				}
				return mode switch
				{
					EnumeratorMode.KEY_MODE => currentKey, 
					EnumeratorMode.VALUE_MODE => currentValue, 
					EnumeratorMode.ENTRY_MODE => Entry, 
					_ => throw new NotSupportedException(string.Concat(mode, " is not a supported mode.")), 
				};
			}
		}

		public Enumerator(SortedList host, EnumeratorMode mode)
		{
			this.host = host;
			stamp = host.modificationCount;
			size = host.Count;
			this.mode = mode;
			Reset();
		}

		public void Reset()
		{
			if (host.modificationCount != stamp || invalid)
			{
				throw new InvalidOperationException(xstr);
			}
			pos = -1;
			currentKey = null;
			currentValue = null;
		}

		public bool MoveNext()
		{
			if (host.modificationCount != stamp || invalid)
			{
				throw new InvalidOperationException(xstr);
			}
			Slot[] table = host.table;
			if (++pos < size)
			{
				Slot slot = table[pos];
				currentKey = slot.key;
				currentValue = slot.value;
				return true;
			}
			currentKey = null;
			currentValue = null;
			return false;
		}

		public object Clone()
		{
			Enumerator enumerator = new Enumerator(host, mode);
			enumerator.stamp = stamp;
			enumerator.pos = pos;
			enumerator.size = size;
			enumerator.currentKey = currentKey;
			enumerator.currentValue = currentValue;
			enumerator.invalid = invalid;
			return enumerator;
		}
	}

	[Serializable]
	private class ListKeys : IEnumerable, ICollection, IList
	{
		private SortedList host;

		public virtual int Count => host.Count;

		public virtual bool IsSynchronized => host.IsSynchronized;

		public virtual object SyncRoot => host.SyncRoot;

		public virtual bool IsFixedSize => true;

		public virtual bool IsReadOnly => true;

		public virtual object this[int index]
		{
			get
			{
				return host.GetKey(index);
			}
			set
			{
				throw new NotSupportedException("attempt to modify a key");
			}
		}

		public ListKeys(SortedList host)
		{
			if (host == null)
			{
				throw new ArgumentNullException();
			}
			this.host = host;
		}

		public virtual void CopyTo(Array array, int arrayIndex)
		{
			host.CopyToArray(array, arrayIndex, EnumeratorMode.KEY_MODE);
		}

		public virtual int Add(object value)
		{
			throw new NotSupportedException("IList::Add not supported");
		}

		public virtual void Clear()
		{
			throw new NotSupportedException("IList::Clear not supported");
		}

		public virtual bool Contains(object key)
		{
			return host.Contains(key);
		}

		public virtual int IndexOf(object key)
		{
			return host.IndexOfKey(key);
		}

		public virtual void Insert(int index, object value)
		{
			throw new NotSupportedException("IList::Insert not supported");
		}

		public virtual void Remove(object value)
		{
			throw new NotSupportedException("IList::Remove not supported");
		}

		public virtual void RemoveAt(int index)
		{
			throw new NotSupportedException("IList::RemoveAt not supported");
		}

		public virtual IEnumerator GetEnumerator()
		{
			return new Enumerator(host, EnumeratorMode.KEY_MODE);
		}
	}

	[Serializable]
	private class ListValues : IEnumerable, ICollection, IList
	{
		private SortedList host;

		public virtual int Count => host.Count;

		public virtual bool IsSynchronized => host.IsSynchronized;

		public virtual object SyncRoot => host.SyncRoot;

		public virtual bool IsFixedSize => true;

		public virtual bool IsReadOnly => true;

		public virtual object this[int index]
		{
			get
			{
				return host.GetByIndex(index);
			}
			set
			{
				throw new NotSupportedException("This operation is not supported on GetValueList return");
			}
		}

		public ListValues(SortedList host)
		{
			if (host == null)
			{
				throw new ArgumentNullException();
			}
			this.host = host;
		}

		public virtual void CopyTo(Array array, int arrayIndex)
		{
			host.CopyToArray(array, arrayIndex, EnumeratorMode.VALUE_MODE);
		}

		public virtual int Add(object value)
		{
			throw new NotSupportedException("IList::Add not supported");
		}

		public virtual void Clear()
		{
			throw new NotSupportedException("IList::Clear not supported");
		}

		public virtual bool Contains(object value)
		{
			return host.ContainsValue(value);
		}

		public virtual int IndexOf(object value)
		{
			return host.IndexOfValue(value);
		}

		public virtual void Insert(int index, object value)
		{
			throw new NotSupportedException("IList::Insert not supported");
		}

		public virtual void Remove(object value)
		{
			throw new NotSupportedException("IList::Remove not supported");
		}

		public virtual void RemoveAt(int index)
		{
			throw new NotSupportedException("IList::RemoveAt not supported");
		}

		public virtual IEnumerator GetEnumerator()
		{
			return new Enumerator(host, EnumeratorMode.VALUE_MODE);
		}
	}

	private static readonly int INITIAL_SIZE = 16;

	private int inUse;

	private int modificationCount;

	private Slot[] table;

	private IComparer comparer;

	private int defaultCapacity;

	public virtual int Count => inUse;

	public virtual bool IsSynchronized => false;

	public virtual object SyncRoot => this;

	public virtual bool IsFixedSize => false;

	public virtual bool IsReadOnly => false;

	public virtual ICollection Keys => new ListKeys(this);

	public virtual object this[object key]
	{
		get
		{
			if (key == null)
			{
				throw new ArgumentNullException();
			}
			return GetImpl(key);
		}
		set
		{
			if (key == null)
			{
				throw new ArgumentNullException();
			}
			if (IsReadOnly)
			{
				throw new NotSupportedException("SortedList is Read Only.");
			}
			if (Find(key) < 0 && IsFixedSize)
			{
				throw new NotSupportedException("Key not found and SortedList is fixed size.");
			}
			PutImpl(key, value, overwrite: true);
		}
	}

	public virtual int Capacity
	{
		get
		{
			return table.Length;
		}
		set
		{
			int num = table.Length;
			if (inUse > value)
			{
				throw new ArgumentOutOfRangeException("capacity too small");
			}
			if (value == 0)
			{
				Slot[] destinationArray = new Slot[defaultCapacity];
				Array.Copy(table, destinationArray, inUse);
				table = destinationArray;
			}
			else if (value > inUse)
			{
				Slot[] destinationArray2 = new Slot[value];
				Array.Copy(table, destinationArray2, inUse);
				table = destinationArray2;
			}
			else if (value > num)
			{
				Slot[] destinationArray3 = new Slot[value];
				Array.Copy(table, destinationArray3, num);
				table = destinationArray3;
			}
		}
	}

	public SortedList()
		: this(null, INITIAL_SIZE)
	{
	}

	public SortedList(int initialCapacity)
		: this(null, initialCapacity)
	{
	}

	public SortedList(IComparer comparer, int capacity)
	{
		if (capacity < 0)
		{
			throw new ArgumentOutOfRangeException("capacity");
		}
		if (capacity == 0)
		{
			defaultCapacity = 0;
		}
		else
		{
			defaultCapacity = INITIAL_SIZE;
		}
		this.comparer = comparer;
		InitTable(capacity, forceSize: true);
	}

	public SortedList(IComparer comparer)
	{
		this.comparer = comparer;
		InitTable(INITIAL_SIZE, forceSize: true);
	}

	public SortedList(IDictionary d, IComparer comparer)
	{
		if (d == null)
		{
			throw new ArgumentNullException("dictionary");
		}
		InitTable(d.Count, forceSize: true);
		this.comparer = comparer;
		IDictionaryEnumerator enumerator = d.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Add(enumerator.Key, enumerator.Value);
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new Enumerator(this, EnumeratorMode.ENTRY_MODE);
	}

	public virtual void Add(object key, object value)
	{
		PutImpl(key, value, overwrite: false);
	}

	public virtual bool Contains(object key)
	{
		if (key == null)
		{
			throw new ArgumentNullException();
		}
		try
		{
			return Find(key) >= 0;
		}
		catch (Exception)
		{
			throw new InvalidOperationException();
		}
	}

	public virtual IDictionaryEnumerator GetEnumerator()
	{
		return new Enumerator(this, EnumeratorMode.ENTRY_MODE);
	}

	public virtual void Remove(object key)
	{
		int num = IndexOfKey(key);
		if (num >= 0)
		{
			RemoveAt(num);
		}
	}

	public virtual void CopyTo(Array array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException();
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		if (array.Rank > 1)
		{
			throw new ArgumentException("array is multi-dimensional");
		}
		if (arrayIndex >= array.Length)
		{
			throw new ArgumentNullException("arrayIndex is greater than or equal to array.Length");
		}
		if (Count > array.Length - arrayIndex)
		{
			throw new ArgumentNullException("Not enough space in array from arrayIndex to end of array");
		}
		IDictionaryEnumerator enumerator = GetEnumerator();
		int num = arrayIndex;
		while (enumerator.MoveNext())
		{
			array.SetValue(enumerator.Entry, num++);
		}
	}

	public virtual object Clone()
	{
		SortedList sortedList = new SortedList(this, comparer);
		sortedList.modificationCount = modificationCount;
		return sortedList;
	}

	public virtual IList GetValueList()
	{
		return new ListValues(this);
	}

	public virtual void RemoveAt(int index)
	{
		Slot[] array = table;
		int count = Count;
		if (index >= 0 && index < count)
		{
			if (index != count - 1)
			{
				Array.Copy(array, index + 1, array, index, count - 1 - index);
			}
			else
			{
				array[index].key = null;
				array[index].value = null;
			}
			inUse--;
			modificationCount++;
			return;
		}
		throw new ArgumentOutOfRangeException("index out of range");
	}

	public virtual int IndexOfKey(object key)
	{
		if (key == null)
		{
			throw new ArgumentNullException();
		}
		int num = 0;
		try
		{
			num = Find(key);
		}
		catch (Exception)
		{
			throw new InvalidOperationException();
		}
		return num | (num >> 31);
	}

	public virtual int IndexOfValue(object value)
	{
		if (inUse == 0)
		{
			return -1;
		}
		for (int i = 0; i < inUse; i++)
		{
			Slot slot = table[i];
			if (object.Equals(value, slot.value))
			{
				return i;
			}
		}
		return -1;
	}

	public virtual bool ContainsKey(object key)
	{
		if (key == null)
		{
			throw new ArgumentNullException();
		}
		try
		{
			return Contains(key);
		}
		catch (Exception)
		{
			throw new InvalidOperationException();
		}
	}

	public virtual bool ContainsValue(object value)
	{
		return IndexOfValue(value) >= 0;
	}

	public virtual object GetByIndex(int index)
	{
		if (index >= 0 && index < Count)
		{
			return table[index].value;
		}
		throw new ArgumentOutOfRangeException("index out of range");
	}

	public virtual object GetKey(int index)
	{
		if (index >= 0 && index < Count)
		{
			return table[index].key;
		}
		throw new ArgumentOutOfRangeException("index out of range");
	}

	private void EnsureCapacity(int n, int free)
	{
		Slot[] array = table;
		Slot[] array2 = null;
		int capacity = Capacity;
		bool flag = free >= 0 && free < Count;
		if (n > capacity)
		{
			array2 = new Slot[n << 1];
		}
		if (array2 != null)
		{
			if (flag)
			{
				int num = free;
				if (num > 0)
				{
					Array.Copy(array, 0, array2, 0, num);
				}
				num = Count - free;
				if (num > 0)
				{
					Array.Copy(array, free, array2, free + 1, num);
				}
			}
			else
			{
				Array.Copy(array, array2, Count);
			}
			table = array2;
		}
		else if (flag)
		{
			Array.Copy(array, free, array, free + 1, Count - free);
		}
	}

	private void PutImpl(object key, object value, bool overwrite)
	{
		if (key == null)
		{
			throw new ArgumentNullException("null key");
		}
		Slot[] array = table;
		int num = -1;
		try
		{
			num = Find(key);
		}
		catch (Exception)
		{
			throw new InvalidOperationException();
		}
		if (num >= 0)
		{
			if (!overwrite)
			{
				string text = Locale.GetText("Key '{0}' already exists in list.", key);
				throw new ArgumentException(text);
			}
			array[num].value = value;
			modificationCount++;
			return;
		}
		num = ~num;
		if (num > Capacity + 1)
		{
			throw new Exception(string.Concat("SortedList::internal error (", key, ", ", value, ") at [", num, "]"));
		}
		EnsureCapacity(Count + 1, num);
		array = table;
		array[num].key = key;
		array[num].value = value;
		inUse++;
		modificationCount++;
	}

	private object GetImpl(object key)
	{
		int num = Find(key);
		if (num >= 0)
		{
			return table[num].value;
		}
		return null;
	}

	private void InitTable(int capacity, bool forceSize)
	{
		if (!forceSize && capacity < defaultCapacity)
		{
			capacity = defaultCapacity;
		}
		table = new Slot[capacity];
		inUse = 0;
		modificationCount = 0;
	}

	private void CopyToArray(Array arr, int i, EnumeratorMode mode)
	{
		if (arr == null)
		{
			throw new ArgumentNullException("arr");
		}
		if (i < 0 || i + Count > arr.Length)
		{
			throw new ArgumentOutOfRangeException("i");
		}
		IEnumerator enumerator = new Enumerator(this, mode);
		while (enumerator.MoveNext())
		{
			arr.SetValue(enumerator.Current, i++);
		}
	}

	private int Find(object key)
	{
		Slot[] array = table;
		int count = Count;
		if (count == 0)
		{
			return -1;
		}
		IComparer obj;
		if (this.comparer == null)
		{
			IComparer comparer = Comparer.Default;
			obj = comparer;
		}
		else
		{
			obj = this.comparer;
		}
		IComparer comparer2 = obj;
		int num = 0;
		int num2 = count - 1;
		while (num <= num2)
		{
			int num3 = num + num2 >> 1;
			int num4 = comparer2.Compare(array[num3].key, key);
			if (num4 == 0)
			{
				return num3;
			}
			if (num4 < 0)
			{
				num = num3 + 1;
			}
			else
			{
				num2 = num3 - 1;
			}
		}
		return ~num;
	}
}
