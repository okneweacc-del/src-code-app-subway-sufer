using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

public struct SortingLayer
{
	private int m_Id;

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern int GetLayerValueFromID(int id);
}
