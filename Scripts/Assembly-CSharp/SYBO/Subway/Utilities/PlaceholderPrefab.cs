using SYBO.Components.Utilities;
using SYBO.Subway.Level;
using UnityEngine;

namespace SYBO.Subway.Utilities;

public class PlaceholderPrefab : MonoBehaviour, IPrefabBehaviour
{
	[SerializeField]
	[HideInInspector]
	private TrackObject[] _trackObjects;

	[SerializeField]
	[HideInInspector]
	private Selector[] _selectors;

	public TrackObject[] TrackObjects => _trackObjects;

	public Selector[] Selectors => _selectors;

	public void Initialize()
	{
		_trackObjects = GetComponentsInChildren<TrackObject>(includeInactive: true);
		_selectors = GetComponentsInChildren<Selector>(includeInactive: true);
	}
}
