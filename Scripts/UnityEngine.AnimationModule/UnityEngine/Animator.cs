using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
[NativeHeader("Runtime/Animation/ScriptBindings/AnimatorControllerParameter.bindings.h")]
[NativeHeader("Runtime/Animation/ScriptBindings/Animator.bindings.h")]
[NativeHeader("Runtime/Animation/Animator.h")]
public class Animator : Behaviour
{
	public extern float speed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern bool hasBoundPlayables
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("HasBoundPlayables")]
		get;
	}

	public bool GetBool(int id)
	{
		return GetBoolID(id);
	}

	public void SetBool(int id, bool value)
	{
		SetBoolID(id, value);
	}

	public void SetTrigger(string name)
	{
		SetTriggerString(name);
	}

	public void ResetTrigger(string name)
	{
		ResetTriggerString(name);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetAnimatorStateInfo(int layerIndex, StateInfoIndex stateInfoIndex, out AnimatorStateInfo info);

	public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex)
	{
		GetAnimatorStateInfo(layerIndex, StateInfoIndex.CurrentState, out var info);
		return info;
	}

	public void Play(string stateName)
	{
		float normalizedTime = float.NegativeInfinity;
		int layer = -1;
		Play(stateName, layer, normalizedTime);
	}

	public void Play(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
	{
		Play(StringToHash(stateName), layer, normalizedTime);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::Play", HasExplicitThis = true)]
	public extern void Play(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime);

	public void Play(int stateNameHash)
	{
		float normalizedTime = float.NegativeInfinity;
		int layer = -1;
		Play(stateNameHash, layer, normalizedTime);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void StopPlayback();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "ScriptingStringToCRC32", IsThreadSafe = true)]
	public static extern int StringToHash(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::SetBoolID", HasExplicitThis = true)]
	private extern void SetBoolID(int id, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::GetBoolID", HasExplicitThis = true)]
	private extern bool GetBoolID(int id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::SetTriggerString", HasExplicitThis = true)]
	private extern void SetTriggerString(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::ResetTriggerString", HasExplicitThis = true)]
	private extern void ResetTriggerString(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("UpdateWithDelta")]
	public extern void Update(float deltaTime);
}
