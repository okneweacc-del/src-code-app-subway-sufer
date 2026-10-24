namespace System;

[Serializable]
public struct ArraySegment<T>
{
	private T[] array;

	private int offset;

	private int count;

	public T[] Array => array;

	public int Offset => offset;

	public int Count => count;

	public override bool Equals(object obj)
	{
		if (obj is ArraySegment<T>)
		{
			return Equals((ArraySegment<T>)obj);
		}
		return false;
	}

	public bool Equals(ArraySegment<T> obj)
	{
		if (array == obj.Array && offset == obj.Offset && count == obj.Count)
		{
			return true;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return array.GetHashCode() ^ offset ^ count;
	}
}
