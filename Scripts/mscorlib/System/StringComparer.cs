using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System;

[Serializable]
[ComVisible(true)]
public abstract class StringComparer : IComparer<string>, IEqualityComparer<string>, IComparer, IEqualityComparer
{
	private static StringComparer invariantCultureIgnoreCase = new CultureAwareComparer(CultureInfo.InvariantCulture, ignore_case: true);

	private static StringComparer invariantCulture = new CultureAwareComparer(CultureInfo.InvariantCulture, ignore_case: false);

	private static StringComparer ordinalIgnoreCase = new OrdinalComparer(ignoreCase: true);

	private static StringComparer ordinal = new OrdinalComparer(ignoreCase: false);

	public static StringComparer InvariantCulture => invariantCulture;

	public static StringComparer InvariantCultureIgnoreCase => invariantCultureIgnoreCase;

	public static StringComparer OrdinalIgnoreCase => ordinalIgnoreCase;

	public int Compare(object x, object y)
	{
		if (x == y)
		{
			return 0;
		}
		if (x == null)
		{
			return -1;
		}
		if (y == null)
		{
			return 1;
		}
		if (x is string x2 && y is string y2)
		{
			return Compare(x2, y2);
		}
		if (!(x is IComparable comparable))
		{
			throw new ArgumentException();
		}
		return comparable.CompareTo(y);
	}

	public new bool Equals(object x, object y)
	{
		if (x == y)
		{
			return true;
		}
		if (x == null || y == null)
		{
			return false;
		}
		if (x is string x2 && y is string y2)
		{
			return Equals(x2, y2);
		}
		return x.Equals(y);
	}

	public int GetHashCode(object obj)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		return (obj is string obj2) ? GetHashCode(obj2) : obj.GetHashCode();
	}

	public abstract int Compare(string x, string y);

	public abstract bool Equals(string x, string y);

	public abstract int GetHashCode(string obj);
}
