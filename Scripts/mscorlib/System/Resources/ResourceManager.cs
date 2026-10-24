using System.Collections;
using System.Runtime.InteropServices;

namespace System.Resources;

[Serializable]
[ComVisible(true)]
public class ResourceManager
{
	private static Hashtable ResourceCache = new Hashtable();

	private static Hashtable NonExistent = Hashtable.Synchronized(new Hashtable());

	public static readonly int HeaderVersionNumber = 1;

	public static readonly int MagicNumber = -1091581234;

	private Type resourceSetType = typeof(RuntimeResourceSet);

	protected ResourceManager()
	{
	}
}
