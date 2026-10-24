using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEngine;

[RequireComponent(typeof(RectTransform))]
[NativeClass("UI::Canvas")]
public sealed class Canvas : Behaviour
{
	public delegate void WillRenderCanvases();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static WillRenderCanvases willRenderCanvases__BackingField;

	public extern RenderMode renderMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern bool isRootCanvas
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern Camera worldCamera
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern float scaleFactor
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern float referencePixelsPerUnit
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern bool pixelPerfect
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern int renderOrder
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern bool overrideSorting
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern int sortingOrder
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern int targetDisplay
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern int sortingLayerID
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern Canvas rootCanvas
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static event WillRenderCanvases willRenderCanvases
	{
		add
		{
			WillRenderCanvases willRenderCanvases = willRenderCanvases__BackingField;
			WillRenderCanvases willRenderCanvases2;
			do
			{
				willRenderCanvases2 = willRenderCanvases;
				willRenderCanvases = Interlocked.CompareExchange(ref willRenderCanvases__BackingField, (WillRenderCanvases)Delegate.Combine(willRenderCanvases2, value), willRenderCanvases);
			}
			while ((object)willRenderCanvases != willRenderCanvases2);
		}
		remove
		{
			WillRenderCanvases willRenderCanvases = willRenderCanvases__BackingField;
			WillRenderCanvases willRenderCanvases2;
			do
			{
				willRenderCanvases2 = willRenderCanvases;
				willRenderCanvases = Interlocked.CompareExchange(ref willRenderCanvases__BackingField, (WillRenderCanvases)Delegate.Remove(willRenderCanvases2, value), willRenderCanvases);
			}
			while ((object)willRenderCanvases != willRenderCanvases2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern Material GetDefaultCanvasMaterial();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern Material GetETC1SupportedCanvasMaterial();

	[RequiredByNativeCode]
	private static void SendWillRenderCanvases()
	{
		if (willRenderCanvases__BackingField != null)
		{
			willRenderCanvases__BackingField();
		}
	}

	public static void ForceUpdateCanvases()
	{
		SendWillRenderCanvases();
	}
}
