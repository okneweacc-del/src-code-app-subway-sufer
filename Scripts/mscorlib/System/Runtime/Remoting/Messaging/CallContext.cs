using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging;

[Serializable]
[ComVisible(true)]
public sealed class CallContext
{
	[ThreadStatic]
	private static Hashtable datastore;

	private static Hashtable Datastore
	{
		get
		{
			Hashtable hashtable = datastore;
			if (hashtable == null)
			{
				return datastore = new Hashtable();
			}
			return hashtable;
		}
	}

	private CallContext()
	{
	}

	public static void SetData(string name, object data)
	{
		Datastore[name] = data;
	}

	internal static LogicalCallContext CreateLogicalCallContext(bool createEmpty)
	{
		LogicalCallContext logicalCallContext = null;
		if (datastore != null)
		{
			foreach (DictionaryEntry item in datastore)
			{
				if (item.Value is ILogicalThreadAffinative)
				{
					if (logicalCallContext == null)
					{
						logicalCallContext = new LogicalCallContext();
					}
					logicalCallContext.SetData((string)item.Key, item.Value);
				}
			}
		}
		if (logicalCallContext == null && createEmpty)
		{
			return new LogicalCallContext();
		}
		return logicalCallContext;
	}

	internal static object SetCurrentCallContext(LogicalCallContext ctx)
	{
		object result = datastore;
		if (ctx != null && ctx.HasInfo)
		{
			datastore = (Hashtable)ctx.Datastore.Clone();
		}
		else
		{
			datastore = null;
		}
		return result;
	}

	internal static void UpdateCurrentCallContext(LogicalCallContext ctx)
	{
		Hashtable hashtable = ctx.Datastore;
		foreach (DictionaryEntry item in hashtable)
		{
			SetData((string)item.Key, item.Value);
		}
	}

	internal static void RestoreCallContext(object oldContext)
	{
		datastore = (Hashtable)oldContext;
	}
}
