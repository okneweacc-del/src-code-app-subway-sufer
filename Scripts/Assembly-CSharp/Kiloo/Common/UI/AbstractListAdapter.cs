using System;

namespace Kiloo.Common.UI;

public abstract class AbstractListAdapter
{
	public Action<int> onNotifyDataSetChanged;

	public abstract int GetCount();

	public virtual Container GetContainer(Container container, int dataIndex, bool isMeasurePass = false)
	{
		throw new NotImplementedException();
	}

	public virtual int GetModelType(int index)
	{
		return 0;
	}

	public virtual long GetItemId(int position)
	{
		return position;
	}

	public virtual bool HasStableIds()
	{
		return false;
	}

	public void NotifyDataSetChanged(int selection = 0)
	{
		onNotifyDataSetChanged?.Invoke(selection);
	}
}
