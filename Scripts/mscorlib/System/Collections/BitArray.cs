using System.Runtime.InteropServices;

namespace System.Collections;

[Serializable]
[ComVisible(true)]
public sealed class BitArray : IEnumerable, ICloneable, ICollection
{
	[Serializable]
	private class BitArrayEnumerator : IEnumerator, ICloneable
	{
		private BitArray _bitArray;

		private bool _current;

		private int _index;

		private int _version;

		public object Current
		{
			get
			{
				if (_index == -1)
				{
					throw new InvalidOperationException("Enum not started");
				}
				if (_index >= _bitArray.Count)
				{
					throw new InvalidOperationException("Enum Ended");
				}
				return _current;
			}
		}

		public BitArrayEnumerator(BitArray ba)
		{
			_index = -1;
			_bitArray = ba;
			_version = ba._version;
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public bool MoveNext()
		{
			checkVersion();
			if (_index < _bitArray.Count - 1)
			{
				_current = _bitArray[++_index];
				return true;
			}
			_index = _bitArray.Count;
			return false;
		}

		public void Reset()
		{
			checkVersion();
			_index = -1;
		}

		private void checkVersion()
		{
			if (_version != _bitArray._version)
			{
				throw new InvalidOperationException();
			}
		}
	}

	private int[] m_array;

	private int m_length;

	private int _version;

	public int Count => m_length;

	public bool IsSynchronized => false;

	public bool this[int index]
	{
		get
		{
			return Get(index);
		}
		set
		{
			Set(index, value);
		}
	}

	public int Length => m_length;

	public object SyncRoot => this;

	public BitArray(BitArray bits)
	{
		if (bits == null)
		{
			throw new ArgumentNullException("bits");
		}
		m_length = bits.m_length;
		m_array = new int[(m_length + 31) / 32];
		if (m_array.Length == 1)
		{
			m_array[0] = bits.m_array[0];
		}
		else
		{
			Array.Copy(bits.m_array, m_array, m_array.Length);
		}
	}

	public BitArray(int length)
	{
		if (length < 0)
		{
			throw new ArgumentOutOfRangeException("length");
		}
		m_length = length;
		m_array = new int[(m_length + 31) / 32];
	}

	private byte getByte(int byteIndex)
	{
		int num = byteIndex / 4;
		int num2 = byteIndex % 4 * 8;
		int num3 = m_array[num] & (255 << num2);
		return (byte)((num3 >> num2) & 0xFF);
	}

	public object Clone()
	{
		return new BitArray(this);
	}

	public void CopyTo(Array array, int index)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (array.Rank != 1)
		{
			throw new ArgumentException("array", "Array rank must be 1");
		}
		if (index >= array.Length && m_length > 0)
		{
			throw new ArgumentException("index", "index is greater than array.Length");
		}
		if (array is bool[])
		{
			if (array.Length - index < m_length)
			{
				throw new ArgumentException();
			}
			bool[] array2 = (bool[])array;
			for (int i = 0; i < m_length; i++)
			{
				array2[index + i] = this[i];
			}
		}
		else if (array is byte[])
		{
			int num = (m_length + 7) / 8;
			if (array.Length - index < num)
			{
				throw new ArgumentException();
			}
			byte[] array3 = (byte[])array;
			for (int j = 0; j < num; j++)
			{
				array3[index + j] = getByte(j);
			}
		}
		else
		{
			if (!(array is int[]))
			{
				throw new ArgumentException("array", "Unsupported type");
			}
			Array.Copy(m_array, 0, array, index, (m_length + 31) / 32);
		}
	}

	public bool Get(int index)
	{
		if (index < 0 || index >= m_length)
		{
			throw new ArgumentOutOfRangeException();
		}
		return (m_array[index >> 5] & (1 << index)) != 0;
	}

	public void Set(int index, bool value)
	{
		if (index < 0 || index >= m_length)
		{
			throw new ArgumentOutOfRangeException();
		}
		if (value)
		{
			m_array[index >> 5] |= 1 << (index & 0x1F);
		}
		else
		{
			m_array[index >> 5] &= ~(1 << (index & 0x1F));
		}
		_version++;
	}

	public IEnumerator GetEnumerator()
	{
		return new BitArrayEnumerator(this);
	}
}
