using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Shaders/Material.h")]
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
public class Material : Object
{
	public extern string[] shaderKeywords
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern Shader shader
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public Color color
	{
		get
		{
			return GetColor("_Color");
		}
		set
		{
			SetColor("_Color", value);
		}
	}

	public Texture mainTexture
	{
		get
		{
			return GetTexture("_MainTex");
		}
		set
		{
			SetTexture("_MainTex", value);
		}
	}

	public Vector2 mainTextureOffset
	{
		get
		{
			return GetTextureOffset("_MainTex");
		}
		set
		{
			SetTextureOffset("_MainTex", value);
		}
	}

	public Vector2 mainTextureScale
	{
		set
		{
			SetTextureScale("_MainTex", value);
		}
	}

	public extern int renderQueue
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetActualRenderQueue")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("SetCustomRenderQueue")]
		set;
	}

	public Material(Shader shader)
	{
		CreateWithShader(this, shader);
	}

	public Material(Material source)
	{
		CreateWithMaterial(this, source);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void CopyPropertiesFromMaterial(Material mat);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("MaterialScripting::CreateWithShader")]
	private static extern void CreateWithShader([Writable] Material self, [NotNull] Shader shader);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("MaterialScripting::CreateWithMaterial")]
	private static extern void CreateWithMaterial([Writable] Material self, [NotNull] Material source);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("HasPropertyFromScript")]
	public extern bool HasProperty(int name);

	public bool HasProperty(string name)
	{
		return HasProperty(Shader.PropertyToID(name));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void EnableKeyword(string keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void DisableKeyword(string keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetFloatFromScript")]
	private extern void SetFloatImpl(int name, float value);

	[NativeName("SetColorFromScript")]
	private void SetColorImpl(int name, Color value)
	{
		SetColorImpl_Injected(name, ref value);
	}

	[NativeName("SetMatrixFromScript")]
	private void SetMatrixImpl(int name, Matrix4x4 value)
	{
		SetMatrixImpl_Injected(name, ref value);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetTextureFromScript")]
	private extern void SetTextureImpl(int name, Texture value);

	[NativeName("GetColorFromScript")]
	private Color GetColorImpl(int name)
	{
		GetColorImpl_Injected(name, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetTextureFromScript")]
	private extern Texture GetTextureImpl(int name);

	[NativeName("GetTextureScaleAndOffsetFromScript")]
	private Vector4 GetTextureScaleAndOffsetImpl(int name)
	{
		GetTextureScaleAndOffsetImpl_Injected(name, out var ret);
		return ret;
	}

	[NativeName("SetTextureOffsetFromScript")]
	private void SetTextureOffsetImpl(int name, Vector2 offset)
	{
		SetTextureOffsetImpl_Injected(name, ref offset);
	}

	[NativeName("SetTextureScaleFromScript")]
	private void SetTextureScaleImpl(int name, Vector2 scale)
	{
		SetTextureScaleImpl_Injected(name, ref scale);
	}

	public void SetFloat(string name, float value)
	{
		SetFloatImpl(Shader.PropertyToID(name), value);
	}

	public void SetInt(string name, int value)
	{
		SetFloatImpl(Shader.PropertyToID(name), value);
	}

	public void SetColor(string name, Color value)
	{
		SetColorImpl(Shader.PropertyToID(name), value);
	}

	public void SetVector(string name, Vector4 value)
	{
		SetColorImpl(Shader.PropertyToID(name), value);
	}

	public void SetVector(int name, Vector4 value)
	{
		SetColorImpl(name, value);
	}

	public void SetMatrix(string name, Matrix4x4 value)
	{
		SetMatrixImpl(Shader.PropertyToID(name), value);
	}

	public void SetTexture(string name, Texture value)
	{
		SetTextureImpl(Shader.PropertyToID(name), value);
	}

	public Color GetColor(string name)
	{
		return GetColorImpl(Shader.PropertyToID(name));
	}

	public Texture GetTexture(string name)
	{
		return GetTextureImpl(Shader.PropertyToID(name));
	}

	public void SetTextureOffset(string name, Vector2 value)
	{
		SetTextureOffsetImpl(Shader.PropertyToID(name), value);
	}

	public void SetTextureScale(string name, Vector2 value)
	{
		SetTextureScaleImpl(Shader.PropertyToID(name), value);
	}

	public Vector2 GetTextureOffset(string name)
	{
		return GetTextureOffset(Shader.PropertyToID(name));
	}

	public Vector2 GetTextureOffset(int name)
	{
		Vector4 textureScaleAndOffsetImpl = GetTextureScaleAndOffsetImpl(name);
		return new Vector2(textureScaleAndOffsetImpl.z, textureScaleAndOffsetImpl.w);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetColorImpl_Injected(int name, ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetMatrixImpl_Injected(int name, ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetColorImpl_Injected(int name, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetTextureScaleAndOffsetImpl_Injected(int name, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetTextureOffsetImpl_Injected(int name, ref Vector2 offset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetTextureScaleImpl_Injected(int name, ref Vector2 scale);
}
