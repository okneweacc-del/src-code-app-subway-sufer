namespace System.ComponentModel;

public sealed class EventHandlerList : IDisposable
{
	private System.ComponentModel.ListEntry entries;

	private Delegate null_entry;

	public Delegate this[object key]
	{
		get
		{
			if (key == null)
			{
				return null_entry;
			}
			return FindEntry(key)?.value;
		}
	}

	public void Dispose()
	{
		entries = null;
	}

	private System.ComponentModel.ListEntry FindEntry(object key)
	{
		for (System.ComponentModel.ListEntry next = entries; next != null; next = next.next)
		{
			if (next.key == key)
			{
				return next;
			}
		}
		return null;
	}
}
