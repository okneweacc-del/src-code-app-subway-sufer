using SYBO.Subway.Pickups;
using UnityEngine;

namespace SYBO.Subway.Coins;

[RequireComponent(typeof(Coin))]
[RequireComponent(typeof(PickupRotate))]
public class CoinRenderer : MonoBehaviour
{
	[SerializeField]
	private Renderer _renderer;

	[SerializeField]
	private Glow _glow;

	private PickupRotate _pickupRotate;

	private Transform _transform;

	private bool _actualRendering;

	private bool _renderersEnabled;

	private void Awake()
	{
		_pickupRotate = GetComponent<PickupRotate>();
		_transform = base.transform;
	}

	public void UpdateCoinRenderer(float time, float characterZ, bool lowPerformanceLevel)
	{
		if (!_renderersEnabled)
		{
			return;
		}
		if (!_actualRendering)
		{
			if (!lowPerformanceLevel || _transform.position.z <= characterZ + 750f)
			{
				_actualRendering = true;
				_renderer.enabled = true;
				_glow.SetVisible(visible: true);
			}
		}
		else
		{
			_pickupRotate.PhasedRotate(time);
		}
	}

	public void ToggleRenderers(bool enabled)
	{
		_renderer.enabled = false;
		_glow.SetVisible(visible: false);
		_actualRendering = false;
		_renderersEnabled = enabled;
	}
}
