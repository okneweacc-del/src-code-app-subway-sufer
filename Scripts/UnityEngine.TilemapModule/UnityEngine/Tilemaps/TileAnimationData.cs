using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Tilemaps;

[NativeType(Header = "Modules/Tilemap/TilemapScripting.h")]
[RequiredByNativeCode]
public struct TileAnimationData
{
	private Sprite[] m_AnimatedSprites;

	private float m_AnimationSpeed;

	private float m_AnimationStartTime;
}
