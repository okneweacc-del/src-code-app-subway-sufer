using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal sealed class GUIClip
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static void Internal_Push(Rect screenRect, Vector2 scrollOffset, Vector2 renderOffset, bool resetOffset)
	{
		INTERNAL_CALL_Internal_Push(ref screenRect, ref scrollOffset, ref renderOffset, resetOffset);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_Push(ref Rect screenRect, ref Vector2 scrollOffset, ref Vector2 renderOffset, bool resetOffset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static extern void Internal_Pop();

	internal static void Push(Rect screenRect, Vector2 scrollOffset, Vector2 renderOffset, bool resetOffset)
	{
		Internal_Push(screenRect, scrollOffset, renderOffset, resetOffset);
	}

	internal static void Pop()
	{
		Internal_Pop();
	}
}
