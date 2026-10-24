using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout((LayoutKind)0)]
[RequiredByNativeCode]
[NativeClass(null)]
[NativeHeader("Runtime/Mono/MonoBehaviour.h")]
public class ScriptableObject : Object
{
	public ScriptableObject()
	{
		CreateScriptableObject(this);
	}

	public static ScriptableObject CreateInstance(Type type)
	{
		return CreateScriptableObjectInstanceFromType(type);
	}

	public static T CreateInstance<T>() where T : ScriptableObject
	{
		return (T)CreateInstance(typeof(T));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	private static extern void CreateScriptableObject([Writable] ScriptableObject self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Scripting::CreateScriptableObjectWithType")]
	private static extern ScriptableObject CreateScriptableObjectInstanceFromType(Type type);
}
