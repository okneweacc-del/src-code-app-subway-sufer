using System.Runtime.InteropServices;

namespace System;

[Serializable]
public struct Nullable<T>(T value) where T : struct
{
	internal T value = value;

	internal bool has_value = true;

	public bool HasValue => has_value;

	public T Value
	{
		get
		{
			if (!has_value)
			{
				throw new InvalidOperationException("Nullable object must have a value.");
			}
			return value;
		}
	}

	public override bool Equals(object other)
	{
		if (other == null)
		{
			return !has_value;
		}
		if (!(other is T?))
		{
			return false;
		}
		return Equals((T?)other);
	}

	private bool Equals(T? other)
	{
		if (other.has_value != has_value)
		{
			return false;
		}
		if (!has_value)
		{
			return true;
		}
		return other.value.Equals(value);
	}

	public override int GetHashCode()
	{
		if (!has_value)
		{
			return 0;
		}
		return value.GetHashCode();
	}

	public T GetValueOrDefault()
	{
		return (!has_value) ? default(T) : value;
	}

	public T GetValueOrDefault(T defaultValue)
	{
		return (!has_value) ? defaultValue : value;
	}

	public override string ToString()
	{
		if (has_value)
		{
			return value.ToString();
		}
		return string.Empty;
	}

	private static object Box(T? o)
	{
		if (!o.has_value)
		{
			return null;
		}
		return o.value;
	}

	private static T? Unbox(object o)
	{
		if (o == null)
		{
			return null;
		}
		return (T)o;
	}

	public static implicit operator T?(T value)
	{
		return value;
	}

	public static explicit operator T(T? value)
	{
		return value.Value;
	}
}
[ComVisible(true)]
public static class Nullable
{
	public static Type GetUnderlyingType(Type nullableType)
	{
		if (nullableType == null)
		{
			throw new ArgumentNullException("nullableType");
		}
		if (nullableType.IsGenericType && nullableType.GetGenericTypeDefinition() == typeof(Nullable<>))
		{
			return nullableType.GetGenericArguments()[0];
		}
		return null;
	}
}
