using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Tilemaps;

[NativeHeader("Modules/Grid/Public/GridMarshalling.h")]
[NativeType(Header = "Modules/Tilemap/Public/Tilemap.h")]
[NativeHeader("Modules/Grid/Public/Grid.h")]
[NativeHeader("Runtime/Graphics/SpriteFrame.h")]
[NativeHeader("Modules/Tilemap/Public/TilemapMarshalling.h")]
[RequireComponent(typeof(Transform))]
[NativeHeader("Modules/Tilemap/Public/TilemapTile.h")]
public sealed class Tilemap : GridLayout
{
	[NativeMethod(Name = "RefreshTileAsset")]
	public void RefreshTile(Vector3Int position)
	{
		RefreshTile_Injected(ref position);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void RefreshTile_Injected(ref Vector3Int position);
}
