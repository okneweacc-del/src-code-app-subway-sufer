using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/Renderer.h")]
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[RequireComponent(typeof(Transform))]
public class Renderer : Component
{
	public Bounds bounds
	{
		[FreeFunction(Name = "RendererScripting::GetBounds", HasExplicitThis = true)]
		get
		{
			get_bounds_Injected(out var ret);
			return ret;
		}
	}

	public extern bool enabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern ShadowCastingMode shadowCastingMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern bool receiveShadows
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern string sortingLayerName
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern int sortingLayerID
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public extern int sortingOrder
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public Material[] materials
	{
		get
		{
			return GetMaterialArray();
		}
		set
		{
			SetMaterialArray(value);
		}
	}

	public Material material
	{
		get
		{
			return GetMaterial();
		}
		set
		{
			SetMaterial(value);
		}
	}

	public Material sharedMaterial => GetSharedMaterial();

	public Material[] sharedMaterials
	{
		get
		{
			return GetSharedMaterialArray();
		}
		set
		{
			SetMaterialArray(value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::GetMaterial", HasExplicitThis = true)]
	private extern Material GetMaterial();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::GetSharedMaterial", HasExplicitThis = true)]
	private extern Material GetSharedMaterial();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::SetMaterial", HasExplicitThis = true)]
	private extern void SetMaterial(Material m);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::GetMaterialArray", HasExplicitThis = true)]
	private extern Material[] GetMaterialArray();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::GetSharedMaterialArray", HasExplicitThis = true)]
	private extern Material[] GetSharedMaterialArray();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::SetMaterialArray", HasExplicitThis = true)]
	private extern void SetMaterialArray([NotNull] Material[] m);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::SetPropertyBlock", HasExplicitThis = true)]
	internal extern void Internal_SetPropertyBlock(MaterialPropertyBlock properties);

	public void SetPropertyBlock(MaterialPropertyBlock properties)
	{
		Internal_SetPropertyBlock(properties);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_bounds_Injected(out Bounds ret);
}
