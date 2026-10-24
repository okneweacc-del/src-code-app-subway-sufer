using System.Runtime.InteropServices;

namespace System.Collections;

[ComVisible(true)]
public interface IDictionary : IEnumerable, ICollection
{
	object this[object key] { get; set; }

	ICollection Keys { get; }

	void Add(object key, object value);

	bool Contains(object key);

	new IDictionaryEnumerator GetEnumerator();

	void Remove(object key);
}
