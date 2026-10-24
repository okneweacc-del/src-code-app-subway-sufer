using System.Collections;

namespace System.Runtime.InteropServices;

[ComVisible(true)]
public sealed class ExtensibleClassFactory
{
	private static Hashtable hashtable;

	static ExtensibleClassFactory()
	{
		hashtable = new Hashtable();
	}

	internal static ObjectCreationDelegate GetObjectCreationCallback(Type t)
	{
		return hashtable[t] as ObjectCreationDelegate;
	}
}
