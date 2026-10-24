using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeClass("UI::CanvasRenderer")]
public sealed class CanvasRenderer : Component
{
	public extern bool hasPopInstruction
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern int materialCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern int popMaterialCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern bool cull
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern int absoluteDepth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern bool hasMoved
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public void SetColor(Color color)
	{
		INTERNAL_CALL_SetColor(this, ref color);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_SetColor(CanvasRenderer self, ref Color color);

	public Color GetColor()
	{
		INTERNAL_CALL_GetColor(this, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetColor(CanvasRenderer self, out Color value);

	public void EnableRectClipping(Rect rect)
	{
		INTERNAL_CALL_EnableRectClipping(this, ref rect);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_EnableRectClipping(CanvasRenderer self, ref Rect rect);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void DisableRectClipping();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetMaterial(Material material, int index);

	public void SetMaterial(Material material, Texture texture)
	{
		materialCount = Math.Max(1, materialCount);
		SetMaterial(material, 0);
		SetTexture(texture);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetPopMaterial(Material material, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetTexture(Texture texture);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetAlphaTexture(Texture texture);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetMesh(Mesh mesh);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Clear();

	public static void SplitUIVertexStreams(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector2> uv0S, List<Vector2> uv1S, List<Vector2> uv2S, List<Vector2> uv3S, List<Vector3> normals, List<Vector4> tangents, List<int> indices)
	{
		SplitUIVertexStreamsInternal(verts, positions, colors, uv0S, uv1S, uv2S, uv3S, normals, tangents);
		SplitIndicesStreamsInternal(verts, indices);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void SplitUIVertexStreamsInternal(object verts, object positions, object colors, object uv0S, object uv1S, object uv2S, object uv3S, object normals, object tangents);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void SplitIndicesStreamsInternal(object verts, object indices);

	public static void CreateUIVertexStream(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector2> uv0S, List<Vector2> uv1S, List<Vector2> uv2S, List<Vector2> uv3S, List<Vector3> normals, List<Vector4> tangents, List<int> indices)
	{
		CreateUIVertexStreamInternal(verts, positions, colors, uv0S, uv1S, uv2S, uv3S, normals, tangents, indices);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void CreateUIVertexStreamInternal(object verts, object positions, object colors, object uv0S, object uv1S, object uv2S, object uv3S, object normals, object tangents, object indices);
}
