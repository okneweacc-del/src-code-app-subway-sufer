using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Collections;

[Serializable]
[DebuggerDisplay("Count={Count}")]
[DebuggerTypeProxy(typeof(CollectionDebuggerView))]
[ComVisible(true)]
public class Stack : IEnumerable, ICloneable, ICollection
{
	private class Enumerator : IEnumerator, ICloneable
	{
		private Stack stack;

		private int modCount;

		private int current;

		public virtual object Current
		{
			get
			{
				if (modCount != stack.modCount || current == -2 || current == -1 || current > stack.count)
				{
					throw new InvalidOperationException();
				}
				return stack.contents[current];
			}
		}

		internal Enumerator(Stack s)
		{
			stack = s;
			modCount = s.modCount;
			current = -2;
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public virtual bool MoveNext()
		{
			if (modCount != stack.modCount)
			{
				throw new InvalidOperationException();
			}
			switch (current)
			{
			case -2:
				current = stack.current;
				return current != -1;
			case -1:
				return false;
			default:
				current--;
				return current != -1;
			}
		}

		public virtual void Reset()
		{
			if (modCount != stack.modCount)
			{
				throw new InvalidOperationException();
			}
			current = -2;
		}
	}

	private object[] contents;

	private int current = -1;

	private int count;

	private int capacity;

	private int modCount;

	public virtual int Count => count;

	public virtual bool IsSynchronized => false;

	public virtual object SyncRoot => this;

	public Stack()
	{
		contents = new object[16];
		capacity = 16;
	}

	public Stack(ICollection col)
		: this(col?.Count ?? 16)
	{
		if (col == null)
		{
			throw new ArgumentNullException("col");
		}
		foreach (object item in col)
		{
			Push(item);
		}
	}

	public Stack(int initialCapacity)
	{
		if (initialCapacity < 0)
		{
			throw new ArgumentOutOfRangeException("initialCapacity");
		}
		capacity = initialCapacity;
		contents = new object[capacity];
	}

	private void Resize(int ncapacity)
	{
		ncapacity = Math.Max(ncapacity, 16);
		object[] destinationArray = new object[ncapacity];
		Array.Copy(contents, destinationArray, count);
		capacity = ncapacity;
		contents = destinationArray;
	}

	public virtual void Clear()
	{
		modCount++;
		for (int i = 0; i < count; i++)
		{
			contents[i] = null;
		}
		count = 0;
		current = -1;
	}

	public virtual object Clone()
	{
		Stack stack = new Stack(contents);
		stack.current = current;
		stack.count = count;
		return stack;
	}

	public virtual void CopyTo(Array array, int index)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (array.Rank > 1 || (array.Length > 0 && index >= array.Length) || count > array.Length - index)
		{
			throw new ArgumentException();
		}
		for (int num = current; num != -1; num--)
		{
			array.SetValue(contents[num], count - (num + 1) + index);
		}
	}

	public virtual IEnumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	public virtual object Peek()
	{
		if (current == -1)
		{
			throw new InvalidOperationException();
		}
		return contents[current];
	}

	public virtual object Pop()
	{
		if (current == -1)
		{
			throw new InvalidOperationException();
		}
		modCount++;
		object result = contents[current];
		contents[current] = null;
		count--;
		current--;
		if (count <= capacity / 4 && count > 16)
		{
			Resize(capacity / 2);
		}
		return result;
	}

	public virtual void Push(object obj)
	{
		modCount++;
		if (capacity == count)
		{
			Resize(capacity * 2);
		}
		count++;
		current++;
		contents[current] = obj;
	}
}
