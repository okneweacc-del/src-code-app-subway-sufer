using System.Collections;

namespace System.Text.RegularExpressions;

[Serializable]
public class GroupCollection : ICollection, IEnumerable
{
	private Group[] list;

	private int gap;

	public int Count => list.Length;

	public bool IsSynchronized => false;

	public Group this[int i]
	{
		get
		{
			if (i >= gap)
			{
				Match match = (Match)list[0];
				i = ((match != Match.Empty) ? match.Regex.GetGroupIndex(i) : (-1));
			}
			return (i >= 0) ? list[i] : Group.Fail;
		}
	}

	public object SyncRoot => list;

	internal GroupCollection(int n, int gap)
	{
		list = new Group[n];
		this.gap = gap;
	}

	internal void SetValue(Group g, int i)
	{
		list[i] = g;
	}

	public void CopyTo(Array array, int index)
	{
		list.CopyTo(array, index);
	}

	public IEnumerator GetEnumerator()
	{
		return list.GetEnumerator();
	}
}
