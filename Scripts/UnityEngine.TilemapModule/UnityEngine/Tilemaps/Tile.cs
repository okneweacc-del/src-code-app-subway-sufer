using System;
using UnityEngine.Scripting;

namespace UnityEngine.Tilemaps;

[Serializable]
[RequiredByNativeCode]
public class Tile : TileBase
{
	public enum ColliderType
	{
		None = 0,
		Sprite = 1,
		Grid = 2
	}

	[SerializeField]
	private Sprite m_Sprite;

	[SerializeField]
	private Color m_Color = Color.white;

	[SerializeField]
	private Matrix4x4 m_Transform = Matrix4x4.identity;

	[SerializeField]
	private GameObject m_InstancedGameObject;

	[SerializeField]
	private TileFlags m_Flags = TileFlags.LockColor;

	[SerializeField]
	private ColliderType m_ColliderType = ColliderType.Sprite;

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
	{
		tileData.sprite = m_Sprite;
		tileData.color = m_Color;
		tileData.transform = m_Transform;
		tileData.gameObject = m_InstancedGameObject;
		tileData.flags = m_Flags;
		tileData.colliderType = m_ColliderType;
	}
}
