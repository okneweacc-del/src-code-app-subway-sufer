using System.Collections.Generic;

namespace System.Linq;

internal class OrderedSequence<TElement, TKey> : OrderedEnumerable<TElement>
{
	private OrderedEnumerable<TElement> parent;

	private Func<TElement, TKey> selector;

	private IComparer<TKey> comparer;

	private SortDirection direction;

	internal OrderedSequence(IEnumerable<TElement> source, Func<TElement, TKey> key_selector, IComparer<TKey> comparer, SortDirection direction)
		: base(source)
	{
		selector = key_selector;
		this.comparer = comparer ?? Comparer<TKey>.Default;
		this.direction = direction;
	}

	public override SortContext<TElement> CreateContext(SortContext<TElement> current)
	{
		SortContext<TElement> sortContext = new SortSequenceContext<TElement, TKey>(selector, comparer, direction, current);
		if (parent != null)
		{
			return parent.CreateContext(sortContext);
		}
		return sortContext;
	}

	protected override IEnumerable<TElement> Sort(IEnumerable<TElement> source)
	{
		return QuickSort<TElement>.Sort(source, CreateContext(null));
	}
}
