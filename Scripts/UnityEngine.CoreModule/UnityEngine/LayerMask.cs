using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
public struct LayerMask
{
	private int m_Mask;

	public int value
	{
		get
		{
			return m_Mask;
		}
		set
		{
			m_Mask = value;
		}
	}

	public static implicit operator int(LayerMask mask)
	{
		return mask.m_Mask;
	}

	public static implicit operator LayerMask(int intVal)
	{
		LayerMask result = default;
		result.m_Mask = intVal;
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern int NameToLayer(string layerName);
}
