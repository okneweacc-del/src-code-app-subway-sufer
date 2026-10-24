using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine;

[ExcludeFromPreset]
public sealed class GameObject : Object
{
	public extern Transform transform
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern int layer
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern bool activeSelf
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern bool activeInHierarchy
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern string tag
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public GameObject gameObject => this;

	public GameObject(string name)
	{
		Internal_CreateGameObject(this, name);
	}

	public GameObject()
	{
		Internal_CreateGameObject(this, null);
	}

	public GameObject(string name, params Type[] components)
	{
		Internal_CreateGameObject(this, name);
		foreach (Type componentType in components)
		{
			AddComponent(componentType);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	public extern Component GetComponent(Type type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern void GetComponentFastPath(Type type, IntPtr oneFurtherThanResultValue);

	[SecuritySafeCritical]
	public unsafe T GetComponent<T>()
	{
		CastHelper<T> castHelper = default;
		GetComponentFastPath(typeof(T), new IntPtr(&castHelper.onePointerFurtherThanT));
		return castHelper.t;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	[GeneratedByOldBindingsGenerator]
	public extern Component GetComponentInChildren(Type type, bool includeInactive);

	[ExcludeFromDocs]
	public T GetComponentInChildren<T>()
	{
		bool includeInactive = false;
		return GetComponentInChildren<T>(includeInactive);
	}

	public T GetComponentInChildren<T>([DefaultValue("false")] bool includeInactive)
	{
		return (T)(object)GetComponentInChildren(typeof(T), includeInactive);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	[GeneratedByOldBindingsGenerator]
	public extern Component GetComponentInParent(Type type);

	public T GetComponentInParent<T>()
	{
		return (T)(object)GetComponentInParent(typeof(T));
	}

	public T[] GetComponents<T>()
	{
		return (T[])GetComponentsInternal(typeof(T), useSearchTypeAsArrayReturnType: true, recursive: false, includeInactive: true, reverse: false, null);
	}

	public void GetComponents<T>(List<T> results)
	{
		GetComponentsInternal(typeof(T), useSearchTypeAsArrayReturnType: false, recursive: false, includeInactive: true, reverse: false, results);
	}

	public Component[] GetComponentsInChildren(Type type, [DefaultValue("false")] bool includeInactive)
	{
		return (Component[])GetComponentsInternal(type, useSearchTypeAsArrayReturnType: false, recursive: true, includeInactive, reverse: false, null);
	}

	public T[] GetComponentsInChildren<T>(bool includeInactive)
	{
		return (T[])GetComponentsInternal(typeof(T), useSearchTypeAsArrayReturnType: true, recursive: true, includeInactive, reverse: false, null);
	}

	public void GetComponentsInChildren<T>(bool includeInactive, List<T> results)
	{
		GetComponentsInternal(typeof(T), useSearchTypeAsArrayReturnType: true, recursive: true, includeInactive, reverse: false, results);
	}

	public T[] GetComponentsInChildren<T>()
	{
		return GetComponentsInChildren<T>(includeInactive: false);
	}

	public void GetComponentsInChildren<T>(List<T> results)
	{
		GetComponentsInChildren(includeInactive: false, results);
	}

	public void GetComponentsInParent<T>(bool includeInactive, List<T> results)
	{
		GetComponentsInternal(typeof(T), useSearchTypeAsArrayReturnType: true, recursive: true, includeInactive, reverse: true, results);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern Array GetComponentsInternal(Type type, bool useSearchTypeAsArrayReturnType, bool recursive, bool includeInactive, bool reverse, object resultList);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetActive(bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern bool CompareTag(string tag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SendMessage(string methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);

	public void SendMessage(string methodName, SendMessageOptions options)
	{
		SendMessage(methodName, null, options);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void BroadcastMessage(string methodName, [DefaultValue("null")] object parameter, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);

	[ExcludeFromDocs]
	public void BroadcastMessage(string methodName)
	{
		SendMessageOptions options = SendMessageOptions.RequireReceiver;
		object parameter = null;
		BroadcastMessage(methodName, parameter, options);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern Component Internal_AddComponentWithType(Type componentType);

	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	public Component AddComponent(Type componentType)
	{
		return Internal_AddComponentWithType(componentType);
	}

	public T AddComponent<T>() where T : Component
	{
		return AddComponent(typeof(T)) as T;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_CreateGameObject([Writable] GameObject mono, string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern GameObject Find(string name);
}
