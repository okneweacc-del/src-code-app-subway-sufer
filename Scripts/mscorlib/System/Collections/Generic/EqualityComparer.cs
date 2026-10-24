namespace System.Collections.Generic;

[Serializable]
public abstract class EqualityComparer<T> : IEqualityComparer<T>, IEqualityComparer
{
	[Serializable]
	private sealed class DefaultComparer : EqualityComparer<T>
	{
		public override int GetHashCode(T obj)
		{
			return obj?.GetHashCode() ?? 0;
		}

		public override bool Equals(T x, T y)
		{
			return x?.Equals(y) ?? (y == null);
		}
	}

	private static readonly EqualityComparer<T> _default;

	public static EqualityComparer<T> Default => _default;

	static EqualityComparer()
	{
		if (typeof(IEquatable<T>).IsAssignableFrom(typeof(T)))
		{
			_default = (EqualityComparer<T>)Activator.CreateInstance(typeof(GenericEqualityComparer<>).MakeGenericType(typeof(T)));
		}
		else
		{
			_default = new DefaultComparer();
		}
	}

	int IEqualityComparer.GetHashCode(object obj)
	{
		return GetHashCode((T)obj);
	}

	bool IEqualityComparer.Equals(object x, object y)
	{
		return Equals((T)x, (T)y);
	}

	public abstract int GetHashCode(T obj);

	public abstract bool Equals(T x, T y);
}
