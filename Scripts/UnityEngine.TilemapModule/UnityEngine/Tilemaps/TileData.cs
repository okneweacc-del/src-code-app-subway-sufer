using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Tilemaps;

[RequiredByNativeCode]
[NativeType(Header = "Modules/Tilemap/TilemapScripting.h")]
public struct TileData
{
	private Sprite m_Sprite;

	private Color m_Color;

	private Matrix4x4 m_Transform;

	private GameObject m_GameObject;

	private TileFlags m_Flags;

	private Tile.ColliderType m_ColliderType;

	public Sprite sprite
	{
		set
		{
			m_Sprite = value;
		}
	}

	public Color color
	{
		set
		{
			m_Color = value;
		}
	}

	public Matrix4x4 transform
	{
		set
		{
			m_Transform = value;
		}
	}

	public GameObject gameObject
	{
		set
		{
			m_GameObject = value;
		}
	}

	public TileFlags flags
	{
		set
		{
			m_Flags = value;
		}
	}

	public Tile.ColliderType colliderType
	{
		set
		{
			m_ColliderType = value;
		}
	}
}
