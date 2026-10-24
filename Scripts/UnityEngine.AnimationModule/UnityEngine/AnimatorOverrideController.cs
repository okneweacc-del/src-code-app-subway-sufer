using System.Reflection;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
[NativeHeader("Runtime/Animation/ScriptBindings/Animation.bindings.h")]
[DefaultMember("Item")]
[NativeHeader("Runtime/Animation/AnimatorOverrideController.h")]
public class AnimatorOverrideController : RuntimeAnimatorController
{
	internal delegate void OnOverrideControllerDirtyCallback();

	internal OnOverrideControllerDirtyCallback OnOverrideControllerDirty;

	[NativeConditional("UNITY_EDITOR")]
	[RequiredByNativeCode]
	internal static void OnInvalidateOverrideController(AnimatorOverrideController controller)
	{
		if (controller.OnOverrideControllerDirty != null)
		{
			controller.OnOverrideControllerDirty();
		}
	}
}
