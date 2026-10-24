using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Collections.Generic;

[Serializable]
[ComVisible(false)]
public class LinkedList<T> : IEnumerable<T>, ICollection, IDeserializationCallback, IEnumerable, ICollection<T>, ISerializable
{
	[Serializable]
	public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
	{
		private LinkedList<T> list;

		private LinkedListNode<T> current;

		private int index;

		private uint version;

		object IEnumerator.Current => Current;

		public T Current
		{
			get
			{
				if (list == null)
				{
					throw new ObjectDisposedException(null);
				}
				if (current == null)
				{
					throw new InvalidOperationException();
				}
				return current.Value;
			}
		}

		internal Enumerator(LinkedList<T> parent)
		{
			list = parent;
			current = null;
			index = -1;
			version = parent.version;
		}

		void IEnumerator.Reset()
		{
			if (list == null)
			{
				throw new ObjectDisposedException(null);
			}
			if (version != list.version)
			{
				throw new InvalidOperationException("list modified");
			}
			current = null;
			index = -1;
		}

		public bool MoveNext()
		{
			if (list == null)
			{
				throw new ObjectDisposedException(null);
			}
			if (version != list.version)
			{
				throw new InvalidOperationException("list modified");
			}
			if (current == null)
			{
				current = list.first;
			}
			else
			{
				current = current.forward;
				if (current == list.first)
				{
					current = null;
				}
			}
			if (current == null)
			{
				index = -1;
				return false;
			}
			index++;
			return true;
		}

		public void Dispose()
		{
			if (list == null)
			{
				throw new ObjectDisposedException(null);
			}
			current = null;
			list = null;
		}
	}

	private const string DataArrayKey = "DataArray";

	private const string VersionKey = "version";

	private uint count;

	private uint version;

	private object syncRoot;

	internal LinkedListNode<T> first;

	internal SerializationInfo si;

	bool ICollection<T>.IsReadOnly => false;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => syncRoot;

	public int Count => (int)count;

	public LinkedListNode<T> First => first;

	public LinkedListNode<T> Last => (first == null) ? null : first.back;

	public LinkedList()
	{
		syncRoot = new object();
		first = null;
		count = (version = 0u);
	}

	public LinkedList(IEnumerable<T> collection)
		: this()
	{
		foreach (T item in collection)
		{
			AddLast(item);
		}
	}

	protected LinkedList(SerializationInfo info, StreamingContext context)
		: this()
	{
		si = info;
		syncRoot = new object();
	}

	void ICollection<T>.Add(T value)
	{
		AddLast(value);
	}

	void ICollection.CopyTo(Array array, int index)
	{
		if (!(array is T[] array2))
		{
			throw new ArgumentException("array");
		}
		CopyTo(array2, index);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private void VerifyReferencedNode(LinkedListNode<T> node)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		if (node.List != this)
		{
			throw new InvalidOperationException();
		}
	}

	private static void VerifyBlankNode(LinkedListNode<T> newNode)
	{
		if (newNode == null)
		{
			throw new ArgumentNullException("newNode");
		}
		if (newNode.List != null)
		{
			throw new InvalidOperationException();
		}
	}

	public LinkedListNode<T> AddLast(T value)
	{
		LinkedListNode<T> result = ((first != null) ? new LinkedListNode<T>(this, value, first.back, first) : (first = new LinkedListNode<T>(this, value)));
		count++;
		version++;
		return result;
	}

	public void AddLast(LinkedListNode<T> node)
	{
		VerifyBlankNode(node);
		if (first == null)
		{
			node.SelfReference(this);
			first = node;
		}
		else
		{
			node.InsertBetween(first.back, first, this);
		}
		count++;
		version++;
	}

	public void Clear()
	{
		while (first != null)
		{
			RemoveLast();
		}
	}

	public bool Contains(T value)
	{
		LinkedListNode<T> forward = first;
		if (forward == null)
		{
			return false;
		}
		do
		{
			if (value.Equals(forward.Value))
			{
				return true;
			}
			forward = forward.forward;
		}
		while (forward != first);
		return false;
	}

	public void CopyTo(T[] array, int index)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if ((uint)index < (uint)array.GetLowerBound(0))
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (array.Rank != 1)
		{
			throw new ArgumentException("array", "Array is multidimensional");
		}
		if (array.Length - index + array.GetLowerBound(0) < count)
		{
			throw new ArgumentException("number of items exceeds capacity");
		}
		LinkedListNode<T> forward = first;
		if (first != null)
		{
			do
			{
				array[index] = forward.Value;
				index++;
				forward = forward.forward;
			}
			while (forward != first);
		}
	}

	public LinkedListNode<T> Find(T value)
	{
		LinkedListNode<T> forward = first;
		if (forward == null)
		{
			return null;
		}
		do
		{
			if ((value == null && forward.Value == null) || (value != null && value.Equals(forward.Value)))
			{
				return forward;
			}
			forward = forward.forward;
		}
		while (forward != first);
		return null;
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		T[] array = new T[count];
		CopyTo(array, 0);
		info.AddValue("DataArray", array, typeof(T[]));
		info.AddValue("version", version);
	}

	public virtual void OnDeserialization(object sender)
	{
		if (si == null)
		{
			return;
		}
		T[] array = (T[])si.GetValue("DataArray", typeof(T[]));
		if (array != null)
		{
			T[] array2 = array;
			foreach (T value in array2)
			{
				AddLast(value);
			}
		}
		version = si.GetUInt32("version");
		si = null;
	}

	public bool Remove(T value)
	{
		LinkedListNode<T> linkedListNode = Find(value);
		if (linkedListNode == null)
		{
			return false;
		}
		Remove(linkedListNode);
		return true;
	}

	public void Remove(LinkedListNode<T> node)
	{
		VerifyReferencedNode(node);
		count--;
		if (count == 0)
		{
			first = null;
		}
		if (node == first)
		{
			first = first.forward;
		}
		version++;
		node.Detach();
	}

	public void RemoveFirst()
	{
		if (first != null)
		{
			Remove(first);
		}
	}

	public void RemoveLast()
	{
		if (first != null)
		{
			Remove(first.back);
		}
	}
}
