using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeClass("UI::CanvasGroup")]
public sealed class CanvasGroup : Component, ICanvasRaycastFilter
{
	public extern float alpha
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern bool interactable
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern bool blocksRaycasts
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern bool ignoreParentGroups
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
	{
		return blocksRaycasts;
	}
}
