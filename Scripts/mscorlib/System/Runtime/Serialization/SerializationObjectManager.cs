using System.Collections;

namespace System.Runtime.Serialization;

public sealed class SerializationObjectManager
{
	private readonly StreamingContext context;

	private readonly Hashtable seen = new Hashtable();

	private SerializationCallbacks.CallbackHandler callbacks;

	public SerializationObjectManager(StreamingContext context)
	{
		this.context = context;
	}

	public void RegisterObject(object obj)
	{
		if (seen.Contains(obj))
		{
			return;
		}
		SerializationCallbacks sc = SerializationCallbacks.GetSerializationCallbacks(obj.GetType());
		seen[obj] = 1;
		sc.RaiseOnSerializing(obj, context);
		if (sc.HasSerializedCallbacks)
		{
			callbacks = (SerializationCallbacks.CallbackHandler)Delegate.Combine(callbacks, (SerializationCallbacks.CallbackHandler)((StreamingContext ctx) =>
			{
				sc.RaiseOnSerialized(obj, ctx);
			}));
		}
	}

	public void RaiseOnSerializedEvent()
	{
		if (callbacks != null)
		{
			callbacks(context);
		}
	}
}
