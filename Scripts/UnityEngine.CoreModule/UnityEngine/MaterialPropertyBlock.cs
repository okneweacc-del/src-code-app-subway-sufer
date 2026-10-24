using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Shaders/ComputeShader.h")]
[NativeHeader("Runtime/Math/SphericalHarmonicsL2.h")]
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[NativeHeader("Runtime/Shaders/ShaderPropertySheet.h")]
public sealed class MaterialPropertyBlock
{
	internal IntPtr m_Ptr;

	public MaterialPropertyBlock()
	{
		m_Ptr = CreateImpl();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetFloatFromScript")]
	private extern void SetFloatImpl(int name, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetTextureFromScript")]
	private extern void SetTextureImpl(int name, [NotNull] Texture value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "MaterialPropertyBlockScripting::Create", IsFreeFunction = true)]
	private static extern IntPtr CreateImpl();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "MaterialPropertyBlockScripting::Destroy", IsFreeFunction = true, IsThreadSafe = true)]
	private static extern void DestroyImpl(IntPtr mpb);

	~MaterialPropertyBlock()
	{
		Dispose();
	}

	private void Dispose()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			DestroyImpl(m_Ptr);
			m_Ptr = IntPtr.Zero;
		}
		GC.SuppressFinalize(this);
	}

	public void SetFloat(string name, float value)
	{
		SetFloatImpl(Shader.PropertyToID(name), value);
	}

	public void SetTexture(string name, Texture value)
	{
		SetTextureImpl(Shader.PropertyToID(name), value);
	}
}
