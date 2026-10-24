using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Shaders/Shader.h")]
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[NativeHeader("Runtime/Shaders/ComputeShader.h")]
[NativeHeader("Runtime/Shaders/ShaderNameRegistry.h")]
[NativeHeader("Runtime/Shaders/GpuPrograms/ShaderVariantCollection.h")]
[NativeHeader("Runtime/Misc/ResourceManager.h")]
public sealed class Shader : Object
{
	private Shader()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern int PropertyToID(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetScriptMapper().FindShader")]
	public static extern Shader Find(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::SetGlobalFloat")]
	private static extern void SetGlobalFloatImpl(int name, float value);

	[FreeFunction("ShaderScripting::SetGlobalVector")]
	private static void SetGlobalVectorImpl(int name, Vector4 value)
	{
		SetGlobalVectorImpl_Injected(name, ref value);
	}

	public static void SetGlobalFloat(string name, float value)
	{
		SetGlobalFloatImpl(PropertyToID(name), value);
	}

	public static void SetGlobalInt(string name, int value)
	{
		SetGlobalFloatImpl(PropertyToID(name), value);
	}

	public static void SetGlobalVector(string name, Vector4 value)
	{
		SetGlobalVectorImpl(PropertyToID(name), value);
	}

	public static void SetGlobalColor(string name, Color value)
	{
		SetGlobalVectorImpl(PropertyToID(name), value);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGlobalVectorImpl_Injected(int name, ref Vector4 value);
}
