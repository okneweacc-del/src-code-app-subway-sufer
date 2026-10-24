using System.Runtime.InteropServices;

namespace System.Collections.Generic;

[Serializable]
[ComVisible(false)]
public class Stack<T> : ICollection, IEnumerable, IEnumerable<T>
{
	[Serializable]
	public struct Enumerator : IEnumerator, IDisposable, IEnumerator<T>
	{
		private Stack<T> parent;

		private int idx;

		private int _version;

		object IEnumerator.Current => Current;

		public T Current
		{
			get
			{
				if (idx < 0)
				{
					throw new InvalidOperationException();
				}
				return parent._array[idx];
			}
		}

		internal Enumerator(Stack<T> t)
		{
			parent = t;
			idx = -2;
			_version = t._version;
		}

		void IEnumerator.Reset()
		{
			if (_version != parent._version)
			{
				throw new InvalidOperationException();
			}
			idx = -2;
		}

		public void Dispose()
		{
			idx = -2;
		}

		public bool MoveNext()
		{
			if (_version != parent._version)
			{
				throw new InvalidOperationException();
			}
			if (idx == -2)
			{
				idx = parent._size;
			}
			return idx != -1 && --idx != -1;
		}
	}

	private const int INITIAL_SIZE = 16;

	private T[] _array;

	private int _size;

	private int _version;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	public int Count => _size;

	void ICollection.CopyTo(Array dest, int idx)
	{
		try
		{
			if (_array != null)
			{
				_array.CopyTo(dest, idx);
				Array.Reverse(dest, idx, _size);
			}
		}
		catch (ArrayTypeMismatchException)
		{
			throw new ArgumentException();
		}
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Clear()
	{
		if (_array != null)
		{
			Array.Clear(_array, 0, _array.Length);
		}
		_size = 0;
		_version++;
	}

	public bool Contains(T t)
	{
		return _array != null && Array.IndexOf(_array, t, 0, _size) != -1;
	}

	public T Peek()
	{
		if (_size == 0)
		{
			throw new InvalidOperationException();
		}
		return _array[_size - 1];
	}

	public T Pop()
	{
		if (_size == 0)
		{
			throw new InvalidOperationException();
		}
		_version++;
		T result = _array[--_size];
		_array[_size] = default;
		return result;
	}

	public void Push(T t)
	{
		if (_array == null || _size == _array.Length)
		{
			Array.Resize(ref _array, (_size != 0) ? (2 * _size) : 16);
		}
		_version++;
		_array[_size++] = t;
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}
}
