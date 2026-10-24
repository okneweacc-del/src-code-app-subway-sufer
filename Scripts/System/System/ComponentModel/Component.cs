using System.Runtime.InteropServices;

namespace System.ComponentModel;

[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
[DesignerCategory("Component")]
public class Component : MarshalByRefObject, IDisposable, IComponent
{
	private EventHandlerList event_handlers;

	private ISite mySite;

	private object disposedEvent = new object();

	protected EventHandlerList Events
	{
		get
		{
			if (event_handlers == null)
			{
				event_handlers = new EventHandlerList();
			}
			return event_handlers;
		}
	}

	public Component()
	{
		event_handlers = null;
	}

	~Component()
	{
		Dispose(release_all: false);
	}

	public void Dispose()
	{
		Dispose(release_all: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool release_all)
	{
		if (release_all)
		{
			if (mySite != null && mySite.Container != null)
			{
				mySite.Container.Remove(this);
			}
			((EventHandler)Events[disposedEvent])?.Invoke(this, EventArgs.Empty);
		}
	}

	public override string ToString()
	{
		if (mySite == null)
		{
			return GetType().ToString();
		}
		return $"{mySite.Name} [{GetType().ToString()}]";
	}
}
