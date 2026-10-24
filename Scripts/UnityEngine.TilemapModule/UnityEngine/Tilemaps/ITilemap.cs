using UnityEngine.Scripting;

namespace UnityEngine.Tilemaps;

[RequiredByNativeCode]
public class ITilemap
{
	internal static ITilemap s_Instance;

	internal Tilemap m_Tilemap;

	internal ITilemap()
	{
	}

	public void RefreshTile(Vector3Int position)
	{
		m_Tilemap.RefreshTile(position);
	}

	[RequiredByNativeCode]
	private static ITilemap CreateInstance()
	{
		s_Instance = new ITilemap();
		return s_Instance;
	}
}
