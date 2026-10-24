using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Events;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.SceneManagement;

[RequiredByNativeCode]
public class SceneManager
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static UnityAction<Scene, LoadSceneMode> sceneLoaded__BackingField;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static UnityAction<Scene> sceneUnloaded;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private static UnityAction<Scene, Scene> activeSceneChanged;

	public static extern int sceneCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static event UnityAction<Scene, LoadSceneMode> sceneLoaded
	{
		add
		{
			UnityAction<Scene, LoadSceneMode> unityAction = sceneLoaded__BackingField;
			UnityAction<Scene, LoadSceneMode> unityAction2;
			do
			{
				unityAction2 = unityAction;
				unityAction = Interlocked.CompareExchange(ref sceneLoaded__BackingField, (UnityAction<Scene, LoadSceneMode>)Delegate.Combine(unityAction2, value), unityAction);
			}
			while ((object)unityAction != unityAction2);
		}
		remove
		{
			UnityAction<Scene, LoadSceneMode> unityAction = sceneLoaded__BackingField;
			UnityAction<Scene, LoadSceneMode> unityAction2;
			do
			{
				unityAction2 = unityAction;
				unityAction = Interlocked.CompareExchange(ref sceneLoaded__BackingField, (UnityAction<Scene, LoadSceneMode>)Delegate.Remove(unityAction2, value), unityAction);
			}
			while ((object)unityAction != unityAction2);
		}
	}

	public static Scene GetActiveScene()
	{
		INTERNAL_CALL_GetActiveScene(out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetActiveScene(out Scene value);

	public static AsyncOperation LoadSceneAsync(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
	{
		return LoadSceneAsyncNameIndexInternal(sceneName, -1, mode == LoadSceneMode.Additive, mustCompleteNextFrame: false);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern AsyncOperation LoadSceneAsyncNameIndexInternal(string sceneName, int sceneBuildIndex, bool isAdditive, bool mustCompleteNextFrame);

	public static void MoveGameObjectToScene(GameObject go, Scene scene)
	{
		INTERNAL_CALL_MoveGameObjectToScene(go, ref scene);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_MoveGameObjectToScene(GameObject go, ref Scene scene);

	[RequiredByNativeCode]
	private static void Internal_SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (sceneLoaded__BackingField != null)
		{
			sceneLoaded__BackingField(scene, mode);
		}
	}

	[RequiredByNativeCode]
	private static void Internal_SceneUnloaded(Scene scene)
	{
		if (sceneUnloaded != null)
		{
			sceneUnloaded(scene);
		}
	}

	[RequiredByNativeCode]
	private static void Internal_ActiveSceneChanged(Scene previousActiveScene, Scene newActiveScene)
	{
		if (activeSceneChanged != null)
		{
			activeSceneChanged(previousActiveScene, newActiveScene);
		}
	}
}
