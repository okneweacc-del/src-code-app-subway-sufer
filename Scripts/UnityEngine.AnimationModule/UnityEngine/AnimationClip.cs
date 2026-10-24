using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Animation/ScriptBindings/AnimationClip.bindings.h")]
[NativeType("Runtime/Animation/AnimationClip.h")]
public sealed class AnimationClip : Motion
{
	[NativeProperty("Length", false, TargetType.Function)]
	public extern float length
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[NativeProperty("SampleRate", false, TargetType.Function)]
	public extern float frameRate
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[NativeProperty("WrapMode", false, TargetType.Function)]
	public extern WrapMode wrapMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public AnimationClip()
	{
		Internal_CreateAnimationClip(this);
	}

	public void AddEvent(AnimationEvent evt)
	{
		if (evt == null)
		{
			throw new ArgumentNullException("evt");
		}
		AddEventInternal(evt);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern void AddEventInternal(object evt);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("AnimationClipBindings::Internal_CreateAnimationClip")]
	private static extern void Internal_CreateAnimationClip([Writable] AnimationClip self);

	public void SampleAnimation(GameObject go, float time)
	{
		SampleAnimation(go, this, time, wrapMode);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	[NativeHeader("Runtime/Animation/AnimationUtility.h")]
	internal static extern void SampleAnimation([NotNull] GameObject go, [NotNull] AnimationClip clip, float inTime, WrapMode wrapMode);
}
