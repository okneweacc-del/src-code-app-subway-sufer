using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization;

[ComVisible(true)]
public sealed class SerializationInfo
{
	private Hashtable serialized = new Hashtable();

	private ArrayList values = new ArrayList();

	private string assemblyName;

	private string fullTypeName;

	private IFormatterConverter converter;

	public string AssemblyName => assemblyName;

	public string FullTypeName => fullTypeName;

	public int MemberCount => serialized.Count;

	[CLSCompliant(false)]
	public SerializationInfo(Type type, IFormatterConverter converter)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type", "Null argument");
		}
		if (converter == null)
		{
			throw new ArgumentNullException("converter", "Null argument");
		}
		this.converter = converter;
		assemblyName = type.Assembly.FullName;
		fullTypeName = type.FullName;
	}

	public void AddValue(string name, object value, Type type)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name is null");
		}
		if (type == null)
		{
			throw new ArgumentNullException("type is null");
		}
		if (serialized.ContainsKey(name))
		{
			throw new SerializationException("Value has been serialized already.");
		}
		SerializationEntry serializationEntry = new SerializationEntry(name, type, value);
		serialized.Add(name, serializationEntry);
		values.Add(serializationEntry);
	}

	public object GetValue(string name, Type type)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name is null.");
		}
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (!serialized.ContainsKey(name))
		{
			throw new SerializationException("No element named " + name + " could be found.");
		}
		SerializationEntry serializationEntry = (SerializationEntry)serialized[name];
		if (serializationEntry.Value != null && !type.IsInstanceOfType(serializationEntry.Value))
		{
			return converter.Convert(serializationEntry.Value, type);
		}
		return serializationEntry.Value;
	}

	public void SetType(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type is null.");
		}
		fullTypeName = type.FullName;
		assemblyName = type.Assembly.FullName;
	}

	public SerializationInfoEnumerator GetEnumerator()
	{
		return new SerializationInfoEnumerator(values);
	}

	public void AddValue(string name, short value)
	{
		AddValue(name, value, typeof(short));
	}

	public void AddValue(string name, int value)
	{
		AddValue(name, value, typeof(int));
	}

	public void AddValue(string name, bool value)
	{
		AddValue(name, value, typeof(bool));
	}

	public void AddValue(string name, DateTime value)
	{
		AddValue(name, value, typeof(DateTime));
	}

	public void AddValue(string name, float value)
	{
		AddValue(name, value, typeof(float));
	}

	[CLSCompliant(false)]
	public void AddValue(string name, uint value)
	{
		AddValue(name, value, typeof(uint));
	}

	public void AddValue(string name, long value)
	{
		AddValue(name, value, typeof(long));
	}

	[CLSCompliant(false)]
	public void AddValue(string name, ulong value)
	{
		AddValue(name, value, typeof(ulong));
	}

	public void AddValue(string name, object value)
	{
		if (value == null)
		{
			AddValue(name, value, typeof(object));
		}
		else
		{
			AddValue(name, value, value.GetType());
		}
	}

	public bool GetBoolean(string name)
	{
		object value = GetValue(name, typeof(bool));
		return converter.ToBoolean(value);
	}

	public short GetInt16(string name)
	{
		object value = GetValue(name, typeof(short));
		return converter.ToInt16(value);
	}

	public int GetInt32(string name)
	{
		object value = GetValue(name, typeof(int));
		return converter.ToInt32(value);
	}

	public long GetInt64(string name)
	{
		object value = GetValue(name, typeof(long));
		return converter.ToInt64(value);
	}

	public string GetString(string name)
	{
		object value = GetValue(name, typeof(string));
		if (value == null)
		{
			return null;
		}
		return converter.ToString(value);
	}

	[CLSCompliant(false)]
	public uint GetUInt32(string name)
	{
		object value = GetValue(name, typeof(uint));
		return converter.ToUInt32(value);
	}
}
