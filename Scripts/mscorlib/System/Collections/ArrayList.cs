using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Collections;

[Serializable]
[ComVisible(true)]
[DebuggerDisplay("Count={Count}")]
[DebuggerTypeProxy(typeof(CollectionDebuggerView))]
public class ArrayList : IEnumerable, ICloneable, ICollection, IList
{
	private sealed class SimpleEnumerator : IEnumerator, ICloneable
	{
		private ArrayList list;

		private int index;

		private int version;

		private object currentElement;

		private static object endFlag = new object();

		public object Current
		{
			get
			{
				if (currentElement == endFlag)
				{
					if (index == -1)
					{
						throw new InvalidOperationException("Enumerator not started");
					}
					throw new InvalidOperationException("Enumerator ended");
				}
				return currentElement;
			}
		}

		public SimpleEnumerator(ArrayList list)
		{
			this.list = list;
			index = -1;
			version = list._version;
			currentElement = endFlag;
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public bool MoveNext()
		{
			if (version != list._version)
			{
				throw new InvalidOperationException("List has changed.");
			}
			if (++index < list.Count)
			{
				currentElement = list[index];
				return true;
			}
			currentElement = endFlag;
			return false;
		}

		public void Reset()
		{
			if (version != list._version)
			{
				throw new InvalidOperationException("List has changed.");
			}
			currentElement = endFlag;
			index = -1;
		}
	}

	[Serializable]
	private class ArrayListWrapper : ArrayList
	{
		protected ArrayList m_InnerArrayList;

		public override object this[int index]
		{
			get
			{
				return m_InnerArrayList[index];
			}
			set
			{
				m_InnerArrayList[index] = value;
			}
		}

		public override int Count => m_InnerArrayList.Count;

		public override int Capacity
		{
			get
			{
				return m_InnerArrayList.Capacity;
			}
			set
			{
				m_InnerArrayList.Capacity = value;
			}
		}

		public override bool IsFixedSize => m_InnerArrayList.IsFixedSize;

		public override bool IsReadOnly => m_InnerArrayList.IsReadOnly;

		public override bool IsSynchronized => m_InnerArrayList.IsSynchronized;

		public override object SyncRoot => m_InnerArrayList.SyncRoot;

		public ArrayListWrapper(ArrayList innerArrayList)
		{
			m_InnerArrayList = innerArrayList;
		}

		public override int Add(object value)
		{
			return m_InnerArrayList.Add(value);
		}

		public override void Clear()
		{
			m_InnerArrayList.Clear();
		}

		public override bool Contains(object value)
		{
			return m_InnerArrayList.Contains(value);
		}

		public override int IndexOf(object value)
		{
			return m_InnerArrayList.IndexOf(value);
		}

		public override int IndexOf(object value, int startIndex)
		{
			return m_InnerArrayList.IndexOf(value, startIndex);
		}

		public override int IndexOf(object value, int startIndex, int count)
		{
			return m_InnerArrayList.IndexOf(value, startIndex, count);
		}

		public override void Insert(int index, object value)
		{
			m_InnerArrayList.Insert(index, value);
		}

		public override void InsertRange(int index, ICollection c)
		{
			m_InnerArrayList.InsertRange(index, c);
		}

		public override void Remove(object value)
		{
			m_InnerArrayList.Remove(value);
		}

		public override void RemoveAt(int index)
		{
			m_InnerArrayList.RemoveAt(index);
		}

		public override void CopyTo(Array array)
		{
			m_InnerArrayList.CopyTo(array);
		}

		public override void CopyTo(Array array, int index)
		{
			m_InnerArrayList.CopyTo(array, index);
		}

		public override void CopyTo(int index, Array array, int arrayIndex, int count)
		{
			m_InnerArrayList.CopyTo(index, array, arrayIndex, count);
		}

		public override IEnumerator GetEnumerator()
		{
			return m_InnerArrayList.GetEnumerator();
		}

		public override void AddRange(ICollection c)
		{
			m_InnerArrayList.AddRange(c);
		}

		public override object Clone()
		{
			return m_InnerArrayList.Clone();
		}

		public override void Sort()
		{
			m_InnerArrayList.Sort();
		}

		public override void Sort(IComparer comparer)
		{
			m_InnerArrayList.Sort(comparer);
		}

		public override object[] ToArray()
		{
			return m_InnerArrayList.ToArray();
		}

		public override Array ToArray(Type elementType)
		{
			return m_InnerArrayList.ToArray(elementType);
		}
	}

	[Serializable]
	private sealed class SynchronizedArrayListWrapper : ArrayListWrapper
	{
		private object m_SyncRoot;

		public override object this[int index]
		{
			get
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList[index];
				}
			}
			set
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList[index] = value;
				}
			}
		}

		public override int Count
		{
			get
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.Count;
				}
			}
		}

		public override int Capacity
		{
			get
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.Capacity;
				}
			}
			set
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList.Capacity = value;
				}
			}
		}

		public override bool IsFixedSize
		{
			get
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.IsFixedSize;
				}
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.IsReadOnly;
				}
			}
		}

		public override bool IsSynchronized => true;

		public override object SyncRoot => m_SyncRoot;

		internal SynchronizedArrayListWrapper(ArrayList innerArrayList)
			: base(innerArrayList)
		{
			m_SyncRoot = innerArrayList.SyncRoot;
		}

		public override int Add(object value)
		{
			lock (m_SyncRoot)
			{
				return m_InnerArrayList.Add(value);
			}
		}

		public override void Clear()
		{
			lock (m_SyncRoot)
			{
				m_InnerArrayList.Clear();
			}
		}

		public override bool Contains(object value)
		{
			lock (m_SyncRoot)
			{
				return m_InnerArrayList.Contains(value);
			}
		}

		public override int IndexOf(object value)
		{
			lock (m_SyncRoot)
			{
				return m_InnerArrayList.IndexOf(value);
			}
		}

		public override int IndexOf(object value, int startIndex)
		{
			lock (m_SyncRoot)
			{
				return m_InnerArrayList.IndexOf(value, startIndex);
			}
		}

		public override int IndexOf(object value, int startIndex, int count)
		{
			lock (m_SyncRoot)
			{
				return m_InnerArrayList.IndexOf(value, startIndex, count);
			}
		}

		public override void Insert(int index, object value)
		{
			lock (m_SyncRoot)
			{
				m_InnerArrayList.Insert(index, value);
			}
		}

		public override void InsertRange(int index, ICollection c)
		{
			lock (m_SyncRoot)
			{
				m_InnerArrayList.InsertRange(index, c);
			}
		}

		public override void Remove(object value)
		{
			lock (m_SyncRoot)
			{
				m_InnerArrayList.Remove(value);
			}
		}

		public override void RemoveAt(int index)
		{
			lock (m_SyncRoot)
			{
				m_InnerArrayList.RemoveAt(index);
			}
		}

		public override void CopyTo(Array array)
		{
			lock (m_SyncRoot)
			{
				m_InnerArrayList.CopyTo(array);
			}
		}

		public override void CopyTo(Array array, int index)
		{
			lock (m_SyncRoot)
			{
				m_InnerArrayList.CopyTo(array, index);
			}
		}

		public override void CopyTo(int index, Array array, int arrayIndex, int count)
		{
			lock (m_SyncRoot)
			{
				m_InnerArrayList.CopyTo(index, array, arrayIndex, count);
			}
		}

		public override IEnumerator GetEnumerator()
		{
			lock (m_SyncRoot)
			{
				return m_InnerArrayList.GetEnumerator();
			}
		}

		public override void AddRange(ICollection c)
		{
			lock (m_SyncRoot)
			{
				m_InnerArrayList.AddRange(c);
			}
		}

		public override object Clone()
		{
			lock (m_SyncRoot)
			{
				return m_InnerArrayList.Clone();
			}
		}

		public override void Sort()
		{
			lock (m_SyncRoot)
			{
				m_InnerArrayList.Sort();
			}
		}

		public override void Sort(IComparer comparer)
		{
			lock (m_SyncRoot)
			{
				m_InnerArrayList.Sort(comparer);
			}
		}

		public override object[] ToArray()
		{
			lock (m_SyncRoot)
			{
				return m_InnerArrayList.ToArray();
			}
		}

		public override Array ToArray(Type elementType)
		{
			lock (m_SyncRoot)
			{
				return m_InnerArrayList.ToArray(elementType);
			}
		}
	}

	[Serializable]
	private class FixedSizeArrayListWrapper : ArrayListWrapper
	{
		protected virtual string ErrorMessage => "Can't add or remove from a fixed-size list.";

		public override int Capacity
		{
			get
			{
				return base.Capacity;
			}
			set
			{
				throw new NotSupportedException(ErrorMessage);
			}
		}

		public override bool IsFixedSize => true;

		public FixedSizeArrayListWrapper(ArrayList innerList)
			: base(innerList)
		{
		}

		public override int Add(object value)
		{
			throw new NotSupportedException(ErrorMessage);
		}

		public override void AddRange(ICollection c)
		{
			throw new NotSupportedException(ErrorMessage);
		}

		public override void Clear()
		{
			throw new NotSupportedException(ErrorMessage);
		}

		public override void Insert(int index, object value)
		{
			throw new NotSupportedException(ErrorMessage);
		}

		public override void InsertRange(int index, ICollection c)
		{
			throw new NotSupportedException(ErrorMessage);
		}

		public override void Remove(object value)
		{
			throw new NotSupportedException(ErrorMessage);
		}

		public override void RemoveAt(int index)
		{
			throw new NotSupportedException(ErrorMessage);
		}
	}

	[Serializable]
	private sealed class ReadOnlyArrayListWrapper : FixedSizeArrayListWrapper
	{
		protected override string ErrorMessage => "Can't modify a readonly list.";

		public override bool IsReadOnly => true;

		public override object this[int index]
		{
			get
			{
				return m_InnerArrayList[index];
			}
			set
			{
				throw new NotSupportedException(ErrorMessage);
			}
		}

		public ReadOnlyArrayListWrapper(ArrayList innerArrayList)
			: base(innerArrayList)
		{
		}

		public override void Sort()
		{
			throw new NotSupportedException(ErrorMessage);
		}

		public override void Sort(IComparer comparer)
		{
			throw new NotSupportedException(ErrorMessage);
		}
	}

	private const int DefaultInitialCapacity = 4;

	private int _size;

	private object[] _items;

	private int _version;

	private static readonly object[] EmptyArray = new object[0];

	public virtual object this[int index]
	{
		get
		{
			if (index < 0 || index >= _size)
			{
				ThrowNewArgumentOutOfRangeException("index", index, "Index is less than 0 or more than or equal to the list count.");
			}
			return _items[index];
		}
		set
		{
			if (index < 0 || index >= _size)
			{
				ThrowNewArgumentOutOfRangeException("index", index, "Index is less than 0 or more than or equal to the list count.");
			}
			_items[index] = value;
			_version++;
		}
	}

	public virtual int Count => _size;

	public virtual int Capacity
	{
		get
		{
			return _items.Length;
		}
		set
		{
			if (value < _size)
			{
				ThrowNewArgumentOutOfRangeException("Capacity", value, "Must be more than count.");
			}
			object[] array = new object[value];
			Array.Copy(_items, 0, array, 0, _size);
			_items = array;
		}
	}

	public virtual bool IsFixedSize => false;

	public virtual bool IsReadOnly => false;

	public virtual bool IsSynchronized => false;

	public virtual object SyncRoot => this;

	public ArrayList()
	{
		_items = EmptyArray;
	}

	public ArrayList(ICollection c)
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		if (c is Array { Rank: not 1 })
		{
			throw new RankException();
		}
		_items = new object[c.Count];
		AddRange(c);
	}

	public ArrayList(int capacity)
	{
		if (capacity < 0)
		{
			ThrowNewArgumentOutOfRangeException("capacity", capacity, "The initial capacity can't be smaller than zero.");
		}
		if (capacity == 0)
		{
			capacity = 4;
		}
		_items = new object[capacity];
	}

	private ArrayList(object[] array, int index, int count)
	{
		if (count == 0)
		{
			_items = new object[4];
		}
		else
		{
			_items = new object[count];
		}
		Array.Copy(array, index, _items, 0, count);
		_size = count;
	}

	private void EnsureCapacity(int count)
	{
		if (count > _items.Length)
		{
			int num = _items.Length << 1;
			if (num == 0)
			{
				num = 4;
			}
			while (num < count)
			{
				num <<= 1;
			}
			object[] array = new object[num];
			Array.Copy(_items, 0, array, 0, _items.Length);
			_items = array;
		}
	}

	private void Shift(int index, int count)
	{
		if (count > 0)
		{
			if (_size + count > _items.Length)
			{
				int num;
				for (num = ((_items.Length <= 0) ? 1 : (_items.Length << 1)); num < _size + count; num <<= 1)
				{
				}
				object[] array = new object[num];
				Array.Copy(_items, 0, array, 0, index);
				Array.Copy(_items, index, array, index + count, _size - index);
				_items = array;
			}
			else
			{
				Array.Copy(_items, index, _items, index + count, _size - index);
			}
		}
		else if (count < 0)
		{
			int num2 = index - count;
			Array.Copy(_items, num2, _items, index, _size - num2);
			Array.Clear(_items, _size + count, -count);
		}
	}

	public virtual int Add(object value)
	{
		if (_items.Length <= _size)
		{
			EnsureCapacity(_size + 1);
		}
		_items[_size] = value;
		_version++;
		return _size++;
	}

	public virtual void Clear()
	{
		Array.Clear(_items, 0, _size);
		_size = 0;
		_version++;
	}

	public virtual bool Contains(object item)
	{
		return IndexOf(item, 0, _size) > -1;
	}

	public virtual int IndexOf(object value)
	{
		return IndexOf(value, 0);
	}

	public virtual int IndexOf(object value, int startIndex)
	{
		return IndexOf(value, startIndex, _size - startIndex);
	}

	public virtual int IndexOf(object value, int startIndex, int count)
	{
		if (startIndex < 0 || startIndex > _size)
		{
			ThrowNewArgumentOutOfRangeException("startIndex", startIndex, "Does not specify valid index.");
		}
		if (count < 0)
		{
			ThrowNewArgumentOutOfRangeException("count", count, "Can't be less than 0.");
		}
		if (startIndex > _size - count)
		{
			throw new ArgumentOutOfRangeException("count", "Start index and count do not specify a valid range.");
		}
		return Array.IndexOf(_items, value, startIndex, count);
	}

	public virtual void Insert(int index, object value)
	{
		if (index < 0 || index > _size)
		{
			ThrowNewArgumentOutOfRangeException("index", index, "Index must be >= 0 and <= Count.");
		}
		Shift(index, 1);
		_items[index] = value;
		_size++;
		_version++;
	}

	public virtual void InsertRange(int index, ICollection c)
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		if (index < 0 || index > _size)
		{
			ThrowNewArgumentOutOfRangeException("index", index, "Index must be >= 0 and <= Count.");
		}
		int count = c.Count;
		if (_items.Length < _size + count)
		{
			EnsureCapacity(_size + count);
		}
		if (index < _size)
		{
			Array.Copy(_items, index, _items, index + count, _size - index);
		}
		if (this == c.SyncRoot)
		{
			Array.Copy(_items, 0, _items, index, index);
			Array.Copy(_items, index + count, _items, index << 1, _size - index);
		}
		else
		{
			c.CopyTo(_items, index);
		}
		_size += c.Count;
		_version++;
	}

	public virtual void Remove(object obj)
	{
		int num = IndexOf(obj);
		if (num > -1)
		{
			RemoveAt(num);
		}
		_version++;
	}

	public virtual void RemoveAt(int index)
	{
		if (index < 0 || index >= _size)
		{
			ThrowNewArgumentOutOfRangeException("index", index, "Less than 0 or more than list count.");
		}
		Shift(index, -1);
		_size--;
		_version++;
	}

	public virtual void CopyTo(Array array)
	{
		Array.Copy(_items, array, _size);
	}

	public virtual void CopyTo(Array array, int arrayIndex)
	{
		CopyTo(0, array, arrayIndex, _size);
	}

	public virtual void CopyTo(int index, Array array, int arrayIndex, int count)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (array.Rank != 1)
		{
			throw new ArgumentException("Must have only 1 dimensions.", "array");
		}
		Array.Copy(_items, index, array, arrayIndex, count);
	}

	public virtual IEnumerator GetEnumerator()
	{
		return new SimpleEnumerator(this);
	}

	public virtual void AddRange(ICollection c)
	{
		InsertRange(_size, c);
	}

	public virtual void Sort()
	{
		Array.Sort(_items, 0, _size);
		_version++;
	}

	public virtual void Sort(IComparer comparer)
	{
		Array.Sort(_items, 0, _size, comparer);
	}

	public virtual object[] ToArray()
	{
		object[] array = new object[_size];
		CopyTo(array);
		return array;
	}

	public virtual Array ToArray(Type type)
	{
		Array array = Array.CreateInstance(type, _size);
		CopyTo(array);
		return array;
	}

	public virtual object Clone()
	{
		return new ArrayList(_items, 0, _size);
	}

	internal static void ThrowNewArgumentOutOfRangeException(string name, object actual, string message)
	{
		throw new ArgumentOutOfRangeException(name, actual, message);
	}

	public static ArrayList Synchronized(ArrayList list)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		if (list.IsSynchronized)
		{
			return list;
		}
		return new SynchronizedArrayListWrapper(list);
	}

	public static ArrayList ReadOnly(ArrayList list)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		if (list.IsReadOnly)
		{
			return list;
		}
		return new ReadOnlyArrayListWrapper(list);
	}
}
