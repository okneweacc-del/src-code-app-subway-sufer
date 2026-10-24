using System.Collections.Generic;

namespace Kiloo.Common.UI;

public class ListAdapter<T> : AbstractListAdapter
{
	public List<T> dataList { get; protected set; }

	public ListAdapter(List<T> list)
	{
		dataList = list;
	}

	public virtual T GetItem(int index)
	{
		return dataList[index];
	}

	public virtual void AddItem(T element)
	{
		dataList.Add(element);
	}

	public virtual void RemoveItem(T element)
	{
		if (dataList.Count > 0)
		{
			dataList.Remove(element);
		}
	}

	public virtual void RemoveItemAt(int index)
	{
		if (dataList.Count > 0)
		{
			dataList.RemoveAt(index);
		}
	}

	public virtual void Clear()
	{
		dataList.Clear();
	}

	public virtual void Insert(int index, T element)
	{
		dataList.Insert(index, element);
	}

	public virtual void AddMany(IEnumerable<T> list)
	{
		dataList.AddRange(list);
	}

	public override int GetCount()
	{
		return dataList.Count;
	}
}
