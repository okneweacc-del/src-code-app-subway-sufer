using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Collections;

[Serializable]
[ComVisible(true)]
[DebuggerDisplay("{_value}", Name = "[{_key}]")]
public struct DictionaryEntry(object key, object value)
{
	private object _key = key;

	private object _value = value;

	public object Key => _key;

	public object Value => _value;
}
