using System.Reflection;
using System.Runtime.Serialization;

namespace System.Collections.Specialized;

[Serializable]
public abstract class NameObjectCollectionBase : ICollection, IDeserializationCallback, IEnumerable, ISerializable
{
	internal class _Item
	{
		public string key;

		public object value;

		public _Item(string key, object value)
		{
			this.key = key;
			this.value = value;
		}
	}

	[Serializable]
	internal class _KeysEnumerator : IEnumerator
	{
		private NameObjectCollectionBase m_collection;

		private int m_position;

		public object Current
		{
			get
			{
				if (m_position < m_collection.Count || m_position < 0)
				{
					return m_collection.BaseGetKey(m_position);
				}
				throw new InvalidOperationException();
			}
		}

		internal _KeysEnumerator(NameObjectCollectionBase collection)
		{
			m_collection = collection;
			Reset();
		}

		public bool MoveNext()
		{
			return ++m_position < m_collection.Count;
		}

		public void Reset()
		{
			m_position = -1;
		}
	}

	[Serializable]
	[DefaultMember("Item")]
	public class KeysCollection : ICollection, IEnumerable
	{
		private NameObjectCollectionBase m_collection;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => m_collection;

		public int Count => m_collection.Count;

		internal KeysCollection(NameObjectCollectionBase collection)
		{
			m_collection = collection;
		}

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			ArrayList itemsArray = m_collection.m_ItemsArray;
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			if (array.Length > 0 && arrayIndex >= array.Length)
			{
				throw new ArgumentException("arrayIndex is equal to or greater than array.Length");
			}
			if (arrayIndex + itemsArray.Count > array.Length)
			{
				throw new ArgumentException("Not enough room from arrayIndex to end of array for this KeysCollection");
			}
			if (array != null && array.Rank > 1)
			{
				throw new ArgumentException("array is multidimensional");
			}
			object[] array2 = (object[])array;
			int num = 0;
			while (num < itemsArray.Count)
			{
				array2[arrayIndex] = ((_Item)itemsArray[num]).key;
				num++;
				arrayIndex++;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return new _KeysEnumerator(m_collection);
		}
	}

	private Hashtable m_ItemsContainer;

	private _Item m_NullKeyItem;

	private ArrayList m_ItemsArray;

	private IHashCodeProvider m_hashprovider;

	private IComparer m_comparer;

	private int m_defCapacity;

	private bool m_readonly;

	private SerializationInfo infoCopy;

	private KeysCollection keyscoll;

	private IEqualityComparer equality_comparer;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	public virtual KeysCollection Keys
	{
		get
		{
			if (keyscoll == null)
			{
				keyscoll = new KeysCollection(this);
			}
			return keyscoll;
		}
	}

	public virtual int Count => m_ItemsArray.Count;

	protected bool IsReadOnly => m_readonly;

	protected NameObjectCollectionBase()
	{
		m_readonly = false;
		m_hashprovider = CaseInsensitiveHashCodeProvider.DefaultInvariant;
		m_comparer = CaseInsensitiveComparer.DefaultInvariant;
		m_defCapacity = 0;
		Init();
	}

	protected NameObjectCollectionBase(SerializationInfo info, StreamingContext context)
	{
		infoCopy = info;
	}

	void ICollection.CopyTo(Array array, int index)
	{
		((ICollection)Keys).CopyTo(array, index);
	}

	private void Init()
	{
		if (equality_comparer != null)
		{
			m_ItemsContainer = new Hashtable(m_defCapacity, equality_comparer);
		}
		else
		{
			m_ItemsContainer = new Hashtable(m_defCapacity, m_hashprovider, m_comparer);
		}
		m_ItemsArray = new ArrayList();
		m_NullKeyItem = null;
	}

	public virtual IEnumerator GetEnumerator()
	{
		return new _KeysEnumerator(this);
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		int count = Count;
		string[] array = new string[count];
		object[] array2 = new object[count];
		int num = 0;
		foreach (_Item item in m_ItemsArray)
		{
			array[num] = item.key;
			array2[num] = item.value;
			num++;
		}
		if (equality_comparer != null)
		{
			info.AddValue("KeyComparer", equality_comparer, typeof(IEqualityComparer));
			info.AddValue("Version", 4, typeof(int));
		}
		else
		{
			info.AddValue("HashProvider", m_hashprovider, typeof(IHashCodeProvider));
			info.AddValue("Comparer", m_comparer, typeof(IComparer));
			info.AddValue("Version", 2, typeof(int));
		}
		info.AddValue("ReadOnly", m_readonly);
		info.AddValue("Count", count);
		info.AddValue("Keys", array, typeof(string[]));
		info.AddValue("Values", array2, typeof(object[]));
	}

	public virtual void OnDeserialization(object sender)
	{
		SerializationInfo serializationInfo = infoCopy;
		if (serializationInfo == null)
		{
			return;
		}
		infoCopy = null;
		m_hashprovider = (IHashCodeProvider)serializationInfo.GetValue("HashProvider", typeof(IHashCodeProvider));
		if (m_hashprovider == null)
		{
			equality_comparer = (IEqualityComparer)serializationInfo.GetValue("KeyComparer", typeof(IEqualityComparer));
		}
		else
		{
			m_comparer = (IComparer)serializationInfo.GetValue("Comparer", typeof(IComparer));
			if (m_comparer == null)
			{
				throw new SerializationException("The comparer is null");
			}
		}
		m_readonly = serializationInfo.GetBoolean("ReadOnly");
		string[] array = (string[])serializationInfo.GetValue("Keys", typeof(string[]));
		if (array == null)
		{
			throw new SerializationException("keys is null");
		}
		object[] array2 = (object[])serializationInfo.GetValue("Values", typeof(object[]));
		if (array2 == null)
		{
			throw new SerializationException("values is null");
		}
		Init();
		int num = array.Length;
		for (int i = 0; i < num; i++)
		{
			BaseAdd(array[i], array2[i]);
		}
	}

	protected void BaseAdd(string name, object value)
	{
		if (IsReadOnly)
		{
			throw new NotSupportedException("Collection is read-only");
		}
		_Item item = new _Item(name, value);
		if (name == null)
		{
			if (m_NullKeyItem == null)
			{
				m_NullKeyItem = item;
			}
		}
		else if (m_ItemsContainer[name] == null)
		{
			m_ItemsContainer.Add(name, item);
		}
		m_ItemsArray.Add(item);
	}

	protected object BaseGet(int index)
	{
		return ((_Item)m_ItemsArray[index]).value;
	}

	protected object BaseGet(string name)
	{
		return FindFirstMatchedItem(name)?.value;
	}

	protected string BaseGetKey(int index)
	{
		return ((_Item)m_ItemsArray[index]).key;
	}

	[System.MonoTODO]
	private _Item FindFirstMatchedItem(string name)
	{
		if (name != null)
		{
			return (_Item)m_ItemsContainer[name];
		}
		return m_NullKeyItem;
	}
}
