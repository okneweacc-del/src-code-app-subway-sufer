using System;

namespace SYBO.Components.Utilities;

public class ClassWrapper<T> : IEquatable<ClassWrapper<T>> where T : IEquatable<T>
{
	public T Value;

	public bool Equals(ClassWrapper<T> other)
	{
		ref T value = ref Value;
		T value2 = other.Value;
		return value.Equals(value2);
	}

	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}
}
