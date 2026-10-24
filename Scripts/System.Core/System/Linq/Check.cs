namespace System.Linq;

internal static class Check
{
	public static void Source(object source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
	}

	public static void SourceAndFunc(object source, object func)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (func == null)
		{
			throw new ArgumentNullException("func");
		}
	}

	public static void SourceAndSelector(object source, object selector)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (selector == null)
		{
			throw new ArgumentNullException("selector");
		}
	}

	public static void SourceAndPredicate(object source, object predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
	}

	public static void FirstAndSecond(object first, object second)
	{
		if (first == null)
		{
			throw new ArgumentNullException("first");
		}
		if (second == null)
		{
			throw new ArgumentNullException("second");
		}
	}

	public static void SourceAndKeySelector(object source, object keySelector)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (keySelector == null)
		{
			throw new ArgumentNullException("keySelector");
		}
	}

	public static void SourceAndKeyElementSelectors(object source, object keySelector, object elementSelector)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (keySelector == null)
		{
			throw new ArgumentNullException("keySelector");
		}
		if (elementSelector == null)
		{
			throw new ArgumentNullException("elementSelector");
		}
	}
}
