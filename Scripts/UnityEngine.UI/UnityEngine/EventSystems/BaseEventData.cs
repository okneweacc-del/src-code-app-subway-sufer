namespace UnityEngine.EventSystems;

public class BaseEventData : AbstractEventData
{
	private readonly EventSystem m_EventSystem;

	public GameObject selectedObject
	{
		set
		{
			m_EventSystem.SetSelectedGameObject(value, this);
		}
	}

	public BaseEventData(EventSystem eventSystem)
	{
		m_EventSystem = eventSystem;
	}
}
