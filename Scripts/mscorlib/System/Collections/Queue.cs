using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Collections;

[Serializable]
[DebuggerTypeProxy(typeof(CollectionDebuggerView))]
[ComVisible(true)]
[DebuggerDisplay("Count={Count}")]
public class Queue : IEnumerable, ICloneable, ICollection
{
	[Serializable]
	private class QueueEnumerator : IEnumerator, ICloneable
	{
		private Queue queue;

		private int _version;

		private int current;

		public virtual object Current
		{
			get
			{
				if (_version != queue._version || current < 0 || current >= queue._size)
				{
					throw new InvalidOperationException();
				}
				return queue._array[(queue._head + current) % queue._array.Length];
			}
		}

		internal QueueEnumerator(Queue q)
		{
			queue = q;
			_version = q._version;
			current = -1;
		}

		public object Clone()
		{
			QueueEnumerator queueEnumerator = new QueueEnumerator(queue);
			queueEnumerator._version = _version;
			queueEnumerator.current = current;
			return queueEnumerator;
		}

		public virtual bool MoveNext()
		{
			if (_version != queue._version)
			{
				throw new InvalidOperationException();
			}
			if (current >= queue._size - 1)
			{
				current = int.MaxValue;
				return false;
			}
			current++;
			return true;
		}

		public virtual void Reset()
		{
			if (_version != queue._version)
			{
				throw new InvalidOperationException();
			}
			current = -1;
		}
	}

	private object[] _array;

	private int _head;

	private int _size;

	private int _tail;

	private int _growFactor;

	private int _version;

	public virtual int Count => _size;

	public virtual bool IsSynchronized => false;

	public virtual object SyncRoot => this;

	public Queue()
		: this(32, 2f)
	{
	}

	public Queue(int capacity)
		: this(capacity, 2f)
	{
	}

	public Queue(ICollection col)
		: this(col?.Count ?? 32)
	{
		if (col == null)
		{
			throw new ArgumentNullException("col");
		}
		foreach (object item in col)
		{
			Enqueue(item);
		}
	}

	public Queue(int capacity, float growFactor)
	{
		if (capacity < 0)
		{
			throw new ArgumentOutOfRangeException("capacity", "Needs a non-negative number");
		}
		if (!(growFactor >= 1f) || !(growFactor <= 10f))
		{
			throw new ArgumentOutOfRangeException("growFactor", "Queue growth factor must be between 1.0 and 10.0, inclusive");
		}
		_array = new object[capacity];
		_growFactor = (int)(growFactor * 100f);
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
		if (array.Rank > 1 || (index != 0 && index >= array.Length) || _size > array.Length - index)
		{
			throw new ArgumentException();
		}
		int num = _array.Length;
		int num2 = num - _head;
		Array.Copy(_array, _head, array, index, Math.Min(_size, num2));
		if (_size > num2)
		{
			Array.Copy(_array, 0, array, index + num2, _size - num2);
		}
	}

	public virtual IEnumerator GetEnumerator()
	{
		return new QueueEnumerator(this);
	}

	public virtual object Clone()
	{
		Queue queue = new Queue(_array.Length);
		queue._growFactor = _growFactor;
		Array.Copy(_array, 0, queue._array, 0, _array.Length);
		queue._head = _head;
		queue._size = _size;
		queue._tail = _tail;
		return queue;
	}

	public virtual void Clear()
	{
		_version++;
		_head = 0;
		_size = 0;
		_tail = 0;
		for (int num = _array.Length - 1; num >= 0; num--)
		{
			_array[num] = null;
		}
	}

	public virtual object Dequeue()
	{
		_version++;
		if (_size < 1)
		{
			throw new InvalidOperationException();
		}
		object result = _array[_head];
		_array[_head] = null;
		_head = (_head + 1) % _array.Length;
		_size--;
		return result;
	}

	public virtual void Enqueue(object obj)
	{
		_version++;
		if (_size == _array.Length)
		{
			grow();
		}
		_array[_tail] = obj;
		_tail = (_tail + 1) % _array.Length;
		_size++;
	}

	public virtual object Peek()
	{
		if (_size < 1)
		{
			throw new InvalidOperationException();
		}
		return _array[_head];
	}

	public virtual object[] ToArray()
	{
		object[] array = new object[_size];
		CopyTo(array, 0);
		return array;
	}

	private void grow()
	{
		int num = _array.Length * _growFactor / 100;
		if (num < _array.Length + 1)
		{
			num = _array.Length + 1;
		}
		object[] array = new object[num];
		CopyTo(array, 0);
		_array = array;
		_head = 0;
		_tail = _head + _size;
	}
}
