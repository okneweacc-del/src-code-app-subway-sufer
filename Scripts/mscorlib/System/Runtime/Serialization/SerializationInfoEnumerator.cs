using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization;

[ComVisible(true)]
public sealed class SerializationInfoEnumerator : IEnumerator
{
	private IEnumerator enumerator;

	object IEnumerator.Current => enumerator.Current;

	public SerializationEntry Current => (SerializationEntry)enumerator.Current;

	public string Name => Current.Name;

	public Type ObjectType => Current.ObjectType;

	public object Value => Current.Value;

	internal SerializationInfoEnumerator(ArrayList list)
	{
		enumerator = list.GetEnumerator();
	}

	public bool MoveNext()
	{
		return enumerator.MoveNext();
	}

	public void Reset()
	{
		enumerator.Reset();
	}
}
