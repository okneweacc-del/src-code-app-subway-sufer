using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[RequireComponent(typeof(Transform))]
public sealed class AudioListener : AudioBehaviour
{
	internal AudioListenerExtension spatializerExtension;

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern int GetNumExtensionProperties();

	internal PropertyName ReadExtensionName(int listenerIndex)
	{
		INTERNAL_CALL_ReadExtensionName(this, listenerIndex, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ReadExtensionName(AudioListener self, int listenerIndex, out PropertyName value);

	internal PropertyName ReadExtensionPropertyName(int listenerIndex)
	{
		INTERNAL_CALL_ReadExtensionPropertyName(this, listenerIndex, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ReadExtensionPropertyName(AudioListener self, int listenerIndex, out PropertyName value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern float ReadExtensionPropertyValue(int listenerIndex);

	internal void ClearExtensionProperties(PropertyName extensionName)
	{
		INTERNAL_CALL_ClearExtensionProperties(this, ref extensionName);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ClearExtensionProperties(AudioListener self, ref PropertyName extensionName);

	internal AudioListenerExtension AddExtension(Type extensionType)
	{
		if (spatializerExtension == null)
		{
			spatializerExtension = ScriptableObject.CreateInstance(extensionType) as AudioListenerExtension;
		}
		return spatializerExtension;
	}
}
