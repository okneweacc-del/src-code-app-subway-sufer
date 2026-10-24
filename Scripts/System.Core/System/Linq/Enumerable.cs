using System.Collections;
using System.Collections.Generic;

namespace System.Linq;

public static class Enumerable
{
	private enum Fallback
	{
		Default = 0,
		Throw = 1
	}

	public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
	{
		Check.SourceAndFunc(source, func);
		TAccumulate val = seed;
		foreach (TSource item in source)
		{
			val = func(val, item);
		}
		return val;
	}

	public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		Check.SourceAndPredicate(source, predicate);
		foreach (TSource item in source)
		{
			if (!predicate(item))
			{
				return false;
			}
		}
		return true;
	}

	public static bool Any<TSource>(this IEnumerable<TSource> source)
	{
		Check.Source(source);
		if (source is ICollection<TSource> collection)
		{
			return collection.Count > 0;
		}
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		return enumerator.MoveNext();
	}

	public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		Check.SourceAndPredicate(source, predicate);
		foreach (TSource item in source)
		{
			if (predicate(item))
			{
				return true;
			}
		}
		return false;
	}

	public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
	{
		Check.Source(source);
		if (source is IEnumerable<TResult> result)
		{
			return result;
		}
		return CreateCastIterator<TResult>(source);
	}

	private static IEnumerable<TResult> CreateCastIterator<TResult>(IEnumerable source)
	{
		foreach (TResult item in source)
		{
			yield return item;
		}
	}

	public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
	{
		if (source is ICollection<TSource> collection)
		{
			return collection.Contains(value);
		}
		return source.Contains(value, null);
	}

	public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
	{
		Check.Source(source);
		if (comparer == null)
		{
			comparer = EqualityComparer<TSource>.Default;
		}
		foreach (TSource item in source)
		{
			if (comparer.Equals(item, value))
			{
				return true;
			}
		}
		return false;
	}

	public static int Count<TSource>(this IEnumerable<TSource> source)
	{
		Check.Source(source);
		if (source is ICollection<TSource> collection)
		{
			return collection.Count;
		}
		int num = 0;
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num++;
		}
		return num;
	}

	public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> selector)
	{
		Check.SourceAndSelector(source, selector);
		int num = 0;
		foreach (TSource item in source)
		{
			if (selector(item))
			{
				num++;
			}
		}
		return num;
	}

	public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
	{
		return source.Distinct(null);
	}

	public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
	{
		Check.Source(source);
		if (comparer == null)
		{
			comparer = EqualityComparer<TSource>.Default;
		}
		return CreateDistinctIterator(source, comparer);
	}

	private static IEnumerable<TSource> CreateDistinctIterator<TSource>(IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
	{
		HashSet<TSource> items = new HashSet<TSource>(comparer);
		foreach (TSource element in source)
		{
			if (!items.Contains(element))
			{
				items.Add(element);
				yield return element;
			}
		}
	}

	private static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index, Fallback fallback)
	{
		long num = 0L;
		foreach (TSource item in source)
		{
			if (index == num++)
			{
				return item;
			}
		}
		if (fallback == Fallback.Throw)
		{
			throw new ArgumentOutOfRangeException();
		}
		return default;
	}

	public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index)
	{
		Check.Source(source);
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		if (source is IList<TSource> list)
		{
			return list[index];
		}
		return source.ElementAt(index, Fallback.Throw);
	}

	private static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Fallback fallback)
	{
		foreach (TSource item in source)
		{
			if (predicate(item))
			{
				return item;
			}
		}
		if (fallback == Fallback.Throw)
		{
			throw new InvalidOperationException();
		}
		return default;
	}

	public static TSource First<TSource>(this IEnumerable<TSource> source)
	{
		Check.Source(source);
		if (source is IList<TSource> list)
		{
			if (list.Count != 0)
			{
				return list[0];
			}
			throw new InvalidOperationException();
		}
		using (IEnumerator<TSource> enumerator = source.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		throw new InvalidOperationException();
	}

	public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		Check.SourceAndPredicate(source, predicate);
		return source.First(predicate, Fallback.Throw);
	}

	public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
	{
		Check.Source(source);
		using (IEnumerator<TSource> enumerator = source.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		return default;
	}

	public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		Check.SourceAndPredicate(source, predicate);
		return source.First(predicate, Fallback.Default);
	}

	public static TSource Last<TSource>(this IEnumerable<TSource> source)
	{
		Check.Source(source);
		if (source is ICollection<TSource> { Count: 0 })
		{
			throw new InvalidOperationException();
		}
		if (source is IList<TSource> list)
		{
			return list[list.Count - 1];
		}
		bool flag = true;
		TSource result = default;
		foreach (TSource item in source)
		{
			result = item;
			flag = false;
		}
		if (!flag)
		{
			return result;
		}
		throw new InvalidOperationException();
	}

	private static U Iterate<T, U>(IEnumerable<T> source, U initValue, Func<T, U, U> selector)
	{
		bool flag = true;
		foreach (T item in source)
		{
			initValue = selector(item, initValue);
			flag = false;
		}
		if (flag)
		{
			throw new InvalidOperationException();
		}
		return initValue;
	}

	public static int Min(this IEnumerable<int> source)
	{
		Check.Source(source);
		return Iterate(source, int.MaxValue, (int a, int b) => Math.Min(a, b));
	}

	public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
	{
		Check.Source(source);
		return CreateOfTypeIterator<TResult>(source);
	}

	private static IEnumerable<TResult> CreateOfTypeIterator<TResult>(IEnumerable source)
	{
		foreach (object element in source)
		{
			if (element is TResult)
			{
				yield return (TResult)element;
			}
		}
	}

	public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		return source.OrderBy(keySelector, null);
	}

	public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
	{
		Check.SourceAndKeySelector(source, keySelector);
		return new OrderedSequence<TSource, TKey>(source, keySelector, comparer, SortDirection.Ascending);
	}

	public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		return source.OrderByDescending(keySelector, null);
	}

	public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
	{
		Check.SourceAndKeySelector(source, keySelector);
		return new OrderedSequence<TSource, TKey>(source, keySelector, comparer, SortDirection.Descending);
	}

	public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
	{
		Check.Source(source);
		return CreateReverseIterator(source);
	}

	private static IEnumerable<TSource> CreateReverseIterator<TSource>(IEnumerable<TSource> source)
	{
		IList<TSource> list = source as IList<TSource>;
		if (list == null)
		{
			list = new List<TSource>(source);
		}
		for (int i = list.Count - 1; i >= 0; i--)
		{
			yield return list[i];
		}
	}

	public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
	{
		Check.SourceAndSelector(source, selector);
		return CreateSelectIterator(source, selector);
	}

	private static IEnumerable<TResult> CreateSelectIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector)
	{
		foreach (TSource element in source)
		{
			yield return selector(element);
		}
	}

	private static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Fallback fallback)
	{
		bool flag = false;
		TSource result = default;
		foreach (TSource item in source)
		{
			if (predicate(item))
			{
				if (flag)
				{
					throw new InvalidOperationException();
				}
				flag = true;
				result = item;
			}
		}
		if (!flag && fallback == Fallback.Throw)
		{
			throw new InvalidOperationException();
		}
		return result;
	}

	public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		Check.SourceAndPredicate(source, predicate);
		return source.Single(predicate, Fallback.Default);
	}

	public static int Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
	{
		Check.SourceAndSelector(source, selector);
		int num = 0;
		foreach (TSource item in source)
		{
			num = checked(num + selector(item));
		}
		return num;
	}

	public static float Sum(this IEnumerable<float> source)
	{
		Check.Source(source);
		return source.Sum((float a, float b) => a + b);
	}

	private static TR Sum<TA, TR>(this IEnumerable<TA> source, Func<TR, TA, TR> selector)
	{
		TR val = default;
		long num = 0L;
		foreach (TA item in source)
		{
			val = selector(val, item);
			num++;
		}
		return val;
	}

	public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
	{
		Check.Source(source);
		return CreateTakeIterator(source, count);
	}

	private static IEnumerable<TSource> CreateTakeIterator<TSource>(IEnumerable<TSource> source, int count)
	{
		if (count <= 0)
		{
			yield break;
		}
		int counter = 0;
		foreach (TSource item in source)
		{
			yield return item;
			int num;
			counter = (num = counter + 1);
			if (num == count)
			{
				break;
			}
		}
	}

	public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
	{
		Check.Source(source);
		if (source is ICollection<TSource> collection)
		{
			TSource[] array = new TSource[collection.Count];
			collection.CopyTo(array, 0);
			return array;
		}
		return new List<TSource>(source).ToArray();
	}

	public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
	{
		return source.ToDictionary(keySelector, elementSelector, null);
	}

	public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
	{
		Check.SourceAndKeyElementSelectors(source, keySelector, elementSelector);
		if (comparer == null)
		{
			comparer = EqualityComparer<TKey>.Default;
		}
		Dictionary<TKey, TElement> dictionary = new Dictionary<TKey, TElement>(comparer);
		foreach (TSource item in source)
		{
			dictionary.Add(keySelector(item), elementSelector(item));
		}
		return dictionary;
	}

	public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
	{
		Check.Source(source);
		return new List<TSource>(source);
	}

	public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
	{
		return first.SequenceEqual(second, null);
	}

	public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
	{
		Check.FirstAndSecond(first, second);
		if (comparer == null)
		{
			comparer = EqualityComparer<TSource>.Default;
		}
		using IEnumerator<TSource> enumerator = first.GetEnumerator();
		using IEnumerator<TSource> enumerator2 = second.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (!enumerator2.MoveNext())
			{
				return false;
			}
			if (!comparer.Equals(enumerator.Current, enumerator2.Current))
			{
				return false;
			}
		}
		return !enumerator2.MoveNext();
	}

	public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		Check.SourceAndPredicate(source, predicate);
		return CreateWhereIterator(source, predicate);
	}

	private static IEnumerable<TSource> CreateWhereIterator<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		foreach (TSource element in source)
		{
			if (predicate(element))
			{
				yield return element;
			}
		}
	}
}
