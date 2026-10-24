using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.SceneManagement;

public struct Scene
{
	private int m_Handle;

	internal int handle => m_Handle;

	public string name => GetNameInternal(handle);

	public override int GetHashCode()
	{
		return m_Handle;
	}

	public override bool Equals(object other)
	{
		if (!(other is Scene scene))
		{
			return false;
		}
		return handle == scene.handle;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern string GetNameInternal(int sceneHandle);
}
