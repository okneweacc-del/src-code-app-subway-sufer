using System.Collections;
using System.Reflection;

namespace System.Text.RegularExpressions;

[Serializable]
[DefaultMember("Item")]
public class CaptureCollection : ICollection, IEnumerable
{
	private Capture[] list;

	public int Count => list.Length;

	public bool IsSynchronized => false;

	public object SyncRoot => list;

	internal CaptureCollection(int n)
	{
		list = new Capture[n];
	}

	internal void SetValue(Capture cap, int i)
	{
		list[i] = cap;
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
