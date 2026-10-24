using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/Mesh/SkinnedMeshRenderer.h")]
public class SkinnedMeshRenderer : Renderer
{
	[NativeProperty("Mesh")]
	public extern Mesh sharedMesh
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern float GetBlendShapeWeight(int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void SetBlendShapeWeight(int index, float value);
}
