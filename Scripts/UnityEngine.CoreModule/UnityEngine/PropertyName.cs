using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
public struct PropertyName
{
	internal int id;

	public PropertyName(string name)
		: this(PropertyNameUtils.PropertyNameFromString(name))
	{
	}

	public PropertyName(PropertyName other)
	{
		id = other.id;
	}

	public PropertyName(int id)
	{
		this.id = id;
	}

	public static bool operator ==(PropertyName lhs, PropertyName rhs)
	{
		return lhs.id == rhs.id;
	}

	public override int GetHashCode()
	{
		return id;
	}

	public override bool Equals(object other)
	{
		return other is PropertyName && this == (PropertyName)other;
	}

	public static implicit operator PropertyName(string name)
	{
		return new PropertyName(name);
	}

	public static implicit operator PropertyName(int id)
	{
		return new PropertyName(id);
	}

	public override string ToString()
	{
		return $"Unknown:{id}";
	}
}
