namespace System.Collections.Generic;

[Serializable]
internal sealed class GenericEqualityComparer<T> : EqualityComparer<T> where T : IEquatable<T>
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
