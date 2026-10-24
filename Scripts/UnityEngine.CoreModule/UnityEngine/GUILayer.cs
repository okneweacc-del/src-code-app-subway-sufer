using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[RequireComponent(typeof(Camera))]
[Obsolete("This component is part of the legacy UI system and will be removed in a future release.")]
public sealed class GUILayer : Behaviour
{
	public GUIElement HitTest(Vector3 screenPosition)
	{
		return INTERNAL_CALL_HitTest(this, ref screenPosition);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern GUIElement INTERNAL_CALL_HitTest(GUILayer self, ref Vector3 screenPosition);
}
