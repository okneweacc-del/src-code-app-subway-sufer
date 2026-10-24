using System.Collections;
using System.Collections.Generic;

namespace System.Linq;

internal abstract class OrderedEnumerable<TElement> : IEnumerable, IEnumerable<TElement>, IOrderedEnumerable<TElement>
{
	private IEnumerable<TElement> source;

	protected OrderedEnumerable(IEnumerable<TElement> source)
	{
		this.source = source;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public IEnumerator<TElement> GetEnumerator()
	{
		return Sort(source).GetEnumerator();
	}

	public abstract SortContext<TElement> CreateContext(SortContext<TElement> current);

	protected abstract IEnumerable<TElement> Sort(IEnumerable<TElement> source);
}
