using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Modules/Terrain/Public/TerrainDataScriptingInterface.h")]
[NativeHeader("TerrainScriptingClasses.h")]
public sealed class TerrainData : Object
{
	private enum BoundaryValueType
	{
		MaxHeightmapRes = 0,
		MinDetailResPerPatch = 1,
		MaxDetailResPerPatch = 2,
		MaxDetailPatchCount = 3,
		MinAlphamapRes = 4,
		MaxAlphamapRes = 5,
		MinBaseMapRes = 6,
		MaxBaseMapRes = 7
	}

	private static readonly int k_MaximumResolution = GetBoundaryValue(BoundaryValueType.MaxHeightmapRes);

	private static readonly int k_MinimumDetailResolutionPerPatch = GetBoundaryValue(BoundaryValueType.MinDetailResPerPatch);

	private static readonly int k_MaximumDetailResolutionPerPatch = GetBoundaryValue(BoundaryValueType.MaxDetailResPerPatch);

	private static readonly int k_MaximumDetailPatchCount = GetBoundaryValue(BoundaryValueType.MaxDetailPatchCount);

	private static readonly int k_MinimumAlphamapResolution = GetBoundaryValue(BoundaryValueType.MinAlphamapRes);

	private static readonly int k_MaximumAlphamapResolution = GetBoundaryValue(BoundaryValueType.MaxAlphamapRes);

	private static readonly int k_MinimumBaseMapResolution = GetBoundaryValue(BoundaryValueType.MinBaseMapRes);

	private static readonly int k_MaximumBaseMapResolution = GetBoundaryValue(BoundaryValueType.MaxBaseMapRes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("TerrainDataScriptingInterface", StaticAccessorType.DoubleColon)]
	[ThreadSafe]
	private static extern int GetBoundaryValue(BoundaryValueType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetSplatDatabase().GetAlphamapResolution")]
	[RequiredByNativeCode]
	internal extern float GetAlphamapResolutionInternal();
}
