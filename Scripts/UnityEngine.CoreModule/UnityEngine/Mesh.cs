using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/Mesh/MeshScriptBindings.h")]
public sealed class Mesh : Object
{
	internal enum InternalShaderChannel
	{
		Vertex = 0,
		Normal = 1,
		Tangent = 2,
		Color = 3,
		TexCoord0 = 4,
		TexCoord1 = 5,
		TexCoord2 = 6,
		TexCoord3 = 7,
		TexCoord4 = 8,
		TexCoord5 = 9,
		TexCoord6 = 10,
		TexCoord7 = 11
	}

	internal enum InternalVertexChannelType
	{
		Float = 0,
		Color = 2
	}

	public extern int blendShapeCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(Name = "GetBlendShapeChannelCount")]
		get;
	}

	internal extern bool canAccess
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("CanAccessFromScript")]
		get;
	}

	public extern int vertexCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("GetVertexCount")]
		get;
	}

	public extern int subMeshCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(Name = "GetSubMeshCount")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction(Name = "MeshScripting::SetSubMeshCount", HasExplicitThis = true)]
		set;
	}

	public Vector3[] vertices
	{
		get
		{
			return GetAllocArrayFromChannel<Vector3>(InternalShaderChannel.Vertex);
		}
		set
		{
			SetArrayForChannel(InternalShaderChannel.Vertex, value);
		}
	}

	public Vector3[] normals
	{
		get
		{
			return GetAllocArrayFromChannel<Vector3>(InternalShaderChannel.Normal);
		}
		set
		{
			SetArrayForChannel(InternalShaderChannel.Normal, value);
		}
	}

	public Vector4[] tangents => GetAllocArrayFromChannel<Vector4>(InternalShaderChannel.Tangent);

	public Vector2[] uv
	{
		get
		{
			return GetAllocArrayFromChannel<Vector2>(InternalShaderChannel.TexCoord0);
		}
		set
		{
			SetArrayForChannel(InternalShaderChannel.TexCoord0, value);
		}
	}

	public Vector2[] uv2
	{
		get
		{
			return GetAllocArrayFromChannel<Vector2>(InternalShaderChannel.TexCoord1);
		}
		set
		{
			SetArrayForChannel(InternalShaderChannel.TexCoord1, value);
		}
	}

	public Vector2[] uv3 => GetAllocArrayFromChannel<Vector2>(InternalShaderChannel.TexCoord2);

	public Vector2[] uv4 => GetAllocArrayFromChannel<Vector2>(InternalShaderChannel.TexCoord3);

	public Color32[] colors32
	{
		get
		{
			return GetAllocArrayFromChannel<Color32>(InternalShaderChannel.Color, InternalVertexChannelType.Color, 1);
		}
		set
		{
			SetArrayForChannel(InternalShaderChannel.Color, InternalVertexChannelType.Color, 1, value);
		}
	}

	public int[] triangles
	{
		get
		{
			if (canAccess)
			{
				return GetTrianglesImpl(-1, applyBaseVertex: true);
			}
			PrintErrorCantAccessIndices();
			return new int[0];
		}
		set
		{
			if (canAccess)
			{
				SetTrianglesImpl(-1, value, NoAllocHelpers.SafeLength(value), calculateBounds: true, 0);
			}
			else
			{
				PrintErrorCantAccessIndices();
			}
		}
	}

	public Mesh()
	{
		Internal_Create(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("MeshScripting::CreateMesh")]
	private static extern void Internal_Create([Writable] Mesh mono);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::GetTriangles", HasExplicitThis = true)]
	private extern int[] GetTrianglesImpl(int submesh, bool applyBaseVertex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::GetIndices", HasExplicitThis = true)]
	private extern int[] GetIndicesImpl(int submesh, bool applyBaseVertex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "SetMeshIndicesFromScript", HasExplicitThis = true)]
	private extern void SetIndicesImpl(int submesh, MeshTopology topology, Array indices, int arraySize, bool calculateBounds, int baseVertex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::PrintErrorCantAccessChannel", HasExplicitThis = true)]
	private extern void PrintErrorCantAccessChannel(InternalShaderChannel ch);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::HasChannel", HasExplicitThis = true)]
	internal extern bool HasChannel(InternalShaderChannel ch);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "SetMeshComponentFromArrayFromScript", HasExplicitThis = true)]
	private extern void SetArrayForChannelImpl(InternalShaderChannel channel, InternalVertexChannelType format, int dim, Array values, int arraySize);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AllocExtractMeshComponentFromScript", HasExplicitThis = true)]
	private extern Array GetAllocArrayFromChannelImpl(InternalShaderChannel channel, InternalVertexChannelType format, int dim);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("Clear")]
	private extern void ClearImpl(bool keepVertexLayout);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("RecalculateBounds")]
	private extern void RecalculateBoundsImpl();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("MarkDynamic")]
	private extern void MarkDynamicImpl();

	internal InternalShaderChannel GetUVChannel(int uvIndex)
	{
		if (uvIndex < 0 || uvIndex > 3)
		{
			throw new ArgumentException("GetUVChannel called for bad uvIndex", "uvIndex");
		}
		return (InternalShaderChannel)(4 + uvIndex);
	}

	internal static int DefaultDimensionForChannel(InternalShaderChannel channel)
	{
		switch (channel)
		{
		case InternalShaderChannel.Vertex:
		case InternalShaderChannel.Normal:
			return 3;
		case InternalShaderChannel.TexCoord0:
		case InternalShaderChannel.TexCoord1:
		case InternalShaderChannel.TexCoord2:
		case InternalShaderChannel.TexCoord3:
			return 2;
		default:
			if (channel == InternalShaderChannel.Tangent || channel == InternalShaderChannel.Color)
			{
				return 4;
			}
			throw new ArgumentException("DefaultDimensionForChannel called for bad channel", "channel");
		}
	}

	private T[] GetAllocArrayFromChannel<T>(InternalShaderChannel channel, InternalVertexChannelType format, int dim)
	{
		if (canAccess)
		{
			if (HasChannel(channel))
			{
				return (T[])GetAllocArrayFromChannelImpl(channel, format, dim);
			}
		}
		else
		{
			PrintErrorCantAccessChannel(channel);
		}
		return new T[0];
	}

	private T[] GetAllocArrayFromChannel<T>(InternalShaderChannel channel)
	{
		return GetAllocArrayFromChannel<T>(channel, InternalVertexChannelType.Float, DefaultDimensionForChannel(channel));
	}

	private void SetSizedArrayForChannel(InternalShaderChannel channel, InternalVertexChannelType format, int dim, Array values, int valuesCount)
	{
		if (canAccess)
		{
			SetArrayForChannelImpl(channel, format, dim, values, valuesCount);
		}
		else
		{
			PrintErrorCantAccessChannel(channel);
		}
	}

	private void SetArrayForChannel<T>(InternalShaderChannel channel, InternalVertexChannelType format, int dim, T[] values)
	{
		SetSizedArrayForChannel(channel, format, dim, values, NoAllocHelpers.SafeLength(values));
	}

	private void SetArrayForChannel<T>(InternalShaderChannel channel, T[] values)
	{
		SetSizedArrayForChannel(channel, InternalVertexChannelType.Float, DefaultDimensionForChannel(channel), values, NoAllocHelpers.SafeLength(values));
	}

	private void SetListForChannel<T>(InternalShaderChannel channel, InternalVertexChannelType format, int dim, List<T> values)
	{
		SetSizedArrayForChannel(channel, format, dim, NoAllocHelpers.ExtractArrayFromList(values), NoAllocHelpers.SafeLength(values));
	}

	private void SetListForChannel<T>(InternalShaderChannel channel, List<T> values)
	{
		SetSizedArrayForChannel(channel, InternalVertexChannelType.Float, DefaultDimensionForChannel(channel), NoAllocHelpers.ExtractArrayFromList(values), NoAllocHelpers.SafeLength(values));
	}

	public void SetVertices(List<Vector3> inVertices)
	{
		SetListForChannel(InternalShaderChannel.Vertex, inVertices);
	}

	public void SetNormals(List<Vector3> inNormals)
	{
		SetListForChannel(InternalShaderChannel.Normal, inNormals);
	}

	public void SetTangents(List<Vector4> inTangents)
	{
		SetListForChannel(InternalShaderChannel.Tangent, inTangents);
	}

	public void SetColors(List<Color> inColors)
	{
		SetListForChannel(InternalShaderChannel.Color, inColors);
	}

	public void SetColors(List<Color32> inColors)
	{
		SetListForChannel(InternalShaderChannel.Color, InternalVertexChannelType.Color, 1, inColors);
	}

	private void SetUvsImpl<T>(int uvIndex, int dim, List<T> uvs)
	{
		if (uvIndex < 0 || uvIndex > 3)
		{
			Debug.LogError("The uv index is invalid (must be in [0..3]");
		}
		else
		{
			SetListForChannel(GetUVChannel(uvIndex), InternalVertexChannelType.Float, dim, uvs);
		}
	}

	public void SetUVs(int channel, List<Vector2> uvs)
	{
		SetUvsImpl(channel, 2, uvs);
	}

	public void SetUVs(int channel, List<Vector4> uvs)
	{
		SetUvsImpl(channel, 4, uvs);
	}

	private void PrintErrorCantAccessIndices()
	{
		Debug.LogError($"Not allowed to access triangles/indices on mesh '{base.name}' (isReadable is false; Read/Write must be enabled in import settings)");
	}

	private bool CheckCanAccessSubmesh(int submesh, bool errorAboutTriangles)
	{
		if (!canAccess)
		{
			PrintErrorCantAccessIndices();
			return false;
		}
		if (submesh < 0 || submesh >= subMeshCount)
		{
			Debug.LogError(string.Format("Failed getting {0}. Submesh index is out of bounds.", (!errorAboutTriangles) ? "indices" : "triangles"), this);
			return false;
		}
		return true;
	}

	private bool CheckCanAccessSubmeshTriangles(int submesh)
	{
		return CheckCanAccessSubmesh(submesh, errorAboutTriangles: true);
	}

	private bool CheckCanAccessSubmeshIndices(int submesh)
	{
		return CheckCanAccessSubmesh(submesh, errorAboutTriangles: false);
	}

	public int[] GetTriangles(int submesh)
	{
		return GetTriangles(submesh, applyBaseVertex: true);
	}

	public int[] GetTriangles(int submesh, [DefaultValue("true")] bool applyBaseVertex)
	{
		return (!CheckCanAccessSubmeshTriangles(submesh)) ? new int[0] : GetTrianglesImpl(submesh, applyBaseVertex);
	}

	public int[] GetIndices(int submesh)
	{
		return GetIndices(submesh, applyBaseVertex: true);
	}

	public int[] GetIndices(int submesh, [DefaultValue("true")] bool applyBaseVertex)
	{
		return (!CheckCanAccessSubmeshIndices(submesh)) ? new int[0] : GetIndicesImpl(submesh, applyBaseVertex);
	}

	private void SetTrianglesImpl(int submesh, Array triangles, int arraySize, bool calculateBounds, int baseVertex)
	{
		SetIndicesImpl(submesh, MeshTopology.Triangles, triangles, arraySize, calculateBounds, baseVertex);
	}

	public void SetTriangles(int[] triangles, int submesh)
	{
		SetTriangles(triangles, submesh, calculateBounds: true, 0);
	}

	public void SetTriangles(int[] triangles, int submesh, [DefaultValue("true")] bool calculateBounds, [DefaultValue("0")] int baseVertex)
	{
		if (CheckCanAccessSubmeshTriangles(submesh))
		{
			SetTrianglesImpl(submesh, triangles, NoAllocHelpers.SafeLength(triangles), calculateBounds, baseVertex);
		}
	}

	public void SetTriangles(List<int> triangles, int submesh)
	{
		SetTriangles(triangles, submesh, calculateBounds: true, 0);
	}

	public void SetTriangles(List<int> triangles, int submesh, [DefaultValue("true")] bool calculateBounds, [DefaultValue("0")] int baseVertex)
	{
		if (CheckCanAccessSubmeshTriangles(submesh))
		{
			SetTrianglesImpl(submesh, NoAllocHelpers.ExtractArrayFromList(triangles), NoAllocHelpers.SafeLength(triangles), calculateBounds, baseVertex);
		}
	}

	public void Clear()
	{
		ClearImpl(keepVertexLayout: true);
	}

	public void RecalculateBounds()
	{
		if (canAccess)
		{
			RecalculateBoundsImpl();
		}
		else
		{
			Debug.LogError($"Not allowed to call RecalculateBounds() on mesh '{base.name}'");
		}
	}

	public void MarkDynamic()
	{
		if (canAccess)
		{
			MarkDynamicImpl();
		}
	}
}
