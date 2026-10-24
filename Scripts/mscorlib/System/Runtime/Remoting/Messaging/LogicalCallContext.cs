using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Runtime.Remoting.Messaging;

[Serializable]
[ComVisible(true)]
public sealed class LogicalCallContext : ICloneable, ISerializable
{
	private Hashtable _data;

	private CallContextRemotingData _remotingData = new CallContextRemotingData();

	public bool HasInfo => _data != null && _data.Count > 0;

	internal Hashtable Datastore => _data;

	internal LogicalCallContext()
	{
	}

	internal LogicalCallContext(SerializationInfo info, StreamingContext context)
	{
		foreach (SerializationEntry item in info)
		{
			if (item.Name == "__RemotingData")
			{
				_remotingData = (CallContextRemotingData)item.Value;
			}
			else
			{
				SetData(item.Name, item.Value);
			}
		}
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("__RemotingData", _remotingData);
		if (_data == null)
		{
			return;
		}
		foreach (DictionaryEntry datum in _data)
		{
			info.AddValue((string)datum.Key, datum.Value);
		}
	}

	public void SetData(string name, object data)
	{
		if (_data == null)
		{
			_data = new Hashtable();
		}
		_data[name] = data;
	}

	public object Clone()
	{
		LogicalCallContext logicalCallContext = new LogicalCallContext();
		logicalCallContext._remotingData = (CallContextRemotingData)_remotingData.Clone();
		if (_data != null)
		{
			logicalCallContext._data = new Hashtable();
			foreach (DictionaryEntry datum in _data)
			{
				logicalCallContext._data[datum.Key] = datum.Value;
			}
		}
		return logicalCallContext;
	}
}
