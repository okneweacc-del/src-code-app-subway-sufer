using JetBrains.Annotations;
using UnityEngine;

namespace UI.Screens;

public class GameOverTabHelper : MonoBehaviour
{
	[SerializeField]
	private GameObject _leaderboardTabActive;

	[SerializeField]
	private GameObject _leaderboardTabInactive;

	[SerializeField]
	private UILabel _leaderboardTabLabel;

	[SerializeField]
	private GameObject _toprunTabActive;

	[SerializeField]
	private GameObject _toprunTabInactive;

	[SerializeField]
	private UILabel _toprunTabLabel;

	[SerializeField]
	private UISprite _toprunTabFlag;

	private const int TOPRUN_LABEL_PIXEL_OFFSET_X = 14;

	private const int TOPRUN_LABEL_PIXEL_OFFSET_Y = -2;

	private void Awake()
	{
		LeaderboardTapped();
	}

	private void OnEnable()
	{
		RefreshFlag();
	}

	public void RefreshFlag()
	{
		_leaderboardTabLabel.text = Strings.Get("HIGH_SCORES_TITLE");
		_toprunTabLabel.text = Strings.Get("TOPRUN_TITLE");
		_toprunTabFlag.spriteName = CountryManager.instance.GetCurrentTournamentCountry().flag;
		UIAnchor component = _toprunTabLabel.GetComponent<UIAnchor>();
		if (_toprunTabFlag.spriteName == string.Empty)
		{
			component.pixelOffset = new Vector2(0f, -2f);
		}
		else
		{
			component.pixelOffset = new Vector2(14f, -2f);
		}
	}

	[UsedImplicitly]
	public void LeaderboardTapped()
	{
		RefreshFlag();
		_leaderboardTabActive.SetActive(value: true);
		_leaderboardTabInactive.SetActive(value: false);
		SetLabelActive(_leaderboardTabLabel, isActive: true);
		_toprunTabActive.SetActive(value: false);
		_toprunTabInactive.SetActive(value: true);
		SetLabelActive(_toprunTabLabel, isActive: false);
	}

	[UsedImplicitly]
	public void TopRunTapped()
	{
		RefreshFlag();
		_leaderboardTabActive.SetActive(value: false);
		_leaderboardTabInactive.SetActive(value: true);
		SetLabelActive(_leaderboardTabLabel, isActive: false);
		_toprunTabActive.SetActive(value: true);
		_toprunTabInactive.SetActive(value: false);
		SetLabelActive(_toprunTabLabel, isActive: true);
	}

	private void SetLabelActive(UILabel label, bool isActive)
	{
		if (isActive)
		{
			label.color = new Color32(28, 103, 156, byte.MaxValue);
			label.effectColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}
		else
		{
			label.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			label.effectColor = new Color32(0, 0, 0, byte.MaxValue);
		}
	}
}
