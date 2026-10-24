namespace UnityEngine.EventSystems;

public struct RaycastResult
{
	private GameObject m_GameObject;

	public BaseRaycaster module;

	public float distance;

	public float index;

	public int depth;

	public int sortingLayer;

	public int sortingOrder;

	public Vector3 worldPosition;

	public Vector3 worldNormal;

	public Vector2 screenPosition;

	public GameObject gameObject
	{
		get
		{
			return m_GameObject;
		}
		set
		{
			m_GameObject = value;
		}
	}

	public bool isValid => module != null && gameObject != null;

	public override string ToString()
	{
		if (!isValid)
		{
			return "";
		}
		return string.Concat("Name: ", gameObject, "\nmodule: ", module, "\ndistance: ", distance, "\nindex: ", index, "\ndepth: ", depth, "\nworldNormal: ", worldNormal, "\nworldPosition: ", worldPosition, "\nscreenPosition: ", screenPosition, "\nmodule.sortOrderPriority: ", module.sortOrderPriority, "\nmodule.renderOrderPriority: ", module.renderOrderPriority, "\nsortingLayer: ", sortingLayer, "\nsortingOrder: ", sortingOrder);
	}
}
