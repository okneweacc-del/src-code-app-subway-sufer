using System.Collections.Generic;
using System.Linq;
using Data.Save;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using NaughtyAttributes;
using SYBO.Subway;
using UI.Generic.Layout;
using UI.Screens.FrontScreenInternal;
using UnityEngine;

namespace UI;

public class TopBar : MonoBehaviour
{
	private enum AnimationDirection
	{
		None = 0,
		In = 1,
		Out = 2
	}

	public class TopBarPayload
	{
		public int width = 320;

		public int height = 26;
	}

	[SerializeField]
	private UISprite _background;

	[SerializeField]
	private UISprite _settingsIcon;

	[SerializeField]
	private UIWidget _coinboxFunds;

	[SerializeField]
	private UIWidget _coinboxFundsBackground;

	[SerializeField]
	private List<UISprite> _buttonBackgrounds;

	[SerializeField]
	private List<UISprite> _buttonShadows;

	[SerializeField]
	private UILabel _scoreLabel;

	[SerializeField]
	private UILabel _hoverboardsLabel;

	[SerializeField]
	private UILabel _keysLabel;

	[SerializeField]
	private UILabel _coinsLabel;

	[SerializeField]
	private Layout _leftLayout;

	[SerializeField]
	private Layout _centerLayout;

	[SerializeField]
	private Layout _rightLayout;

	[SerializeField]
	private TutorialIconAnimation _hoverboardTutorial;

	[SerializeField]
	private Collider[] _clickAreasLockedDuringTutorial;

	private static int _topBarOffset;

	private AnimationDirection _currentAnimationDirection;

	private TopBarPayload _payload;

	private const float SHOWN_POSITION = 0f;

	private float _hiddenPosition;

	private const int FRAME_DELAYS_BEFORE_ANIMATION = 2;

	private int _currentFrameDelays;

	private float _currentFactor = 1f;

	[SerializeField]
	private float _animationDuration = 0.25f;

	private bool _interactionAllowed = true;

	private bool _highScoreLocked;

	private bool _coinsLocked;

	private bool _keysLocked;

	private bool _hoverboardsLocked;

	private static bool showHoverboardTutorial => OnboardingSave.shouldShowHoverboardPopup && !OnboardingSave.hasShownHoverboardPopup && (!Game.IsInstantiated || !Game.Instance.IsPaused);

	private float deltaFactor => Mathf.Min(Time.unscaledDeltaTime, 0.075f) * (1f / _animationDuration);

	public static int GetTopBarOffset()
	{
		return _topBarOffset;
	}

	public void Init(TopBarPayload payload)
	{
		_payload = payload;
		_topBarOffset = -(payload.height + 2);
		_hiddenPosition = _payload.height + 20;
		base.transform.SetLocalPosY(_hiddenPosition);
		SetupBasic();
		_hoverboardTutorial.InitAnimation(GuidelineButton.GuidelineButtonType.Hoverboard.ToString());
		SetHighScore(PlayerSave.highScore, align: false);
		SetHoverboards(InventorySave.GetAmount(UpgradeType.hoverboard), align: false);
		SetCoins(InventorySave.coins, align: false);
		SetKeys(InventorySave.keys, align: false);
		InventorySave.coinsChanged += CoinsChanged;
		InventorySave.keysChanged += KeysChanged;
		PlayerSave.highScoreChanged += HighScoreChanged;
		InventorySave.powerupsChanged += PowerupsChanged;
		UIScreenController.OnChangedScreen += ChangedScreen;
		DetermineHoverboardTutorialVisibility();
		Align();
	}

	public void OnDestroy()
	{
		InventorySave.coinsChanged -= CoinsChanged;
		InventorySave.keysChanged -= KeysChanged;
		PlayerSave.highScoreChanged -= HighScoreChanged;
		InventorySave.powerupsChanged -= PowerupsChanged;
		UIScreenController.OnChangedScreen -= ChangedScreen;
	}

	private void HighScoreChanged()
	{
		if (!_highScoreLocked)
		{
			SetHighScore(PlayerSave.highScore);
		}
	}

	private void CoinsChanged()
	{
		if (!_coinsLocked)
		{
			SetCoins(InventorySave.coins);
		}
	}

	private void KeysChanged()
	{
		if (!_keysLocked)
		{
			SetKeys(InventorySave.keys);
		}
	}

	private void PowerupsChanged(UpgradeType powerup, int amount)
	{
		if (powerup == UpgradeType.hoverboard)
		{
			HoverboardsChanged();
		}
	}

	private void HoverboardsChanged()
	{
		if (!_hoverboardsLocked)
		{
			SetHoverboards(InventorySave.GetAmount(UpgradeType.hoverboard));
		}
	}

	public void SetHighScoreLock(bool value)
	{
		_highScoreLocked = value;
		if (!_highScoreLocked)
		{
			SetHighScore(PlayerSave.highScore);
		}
	}

	public void SetCoinsLock(bool value)
	{
		_coinsLocked = value;
		if (!_coinsLocked)
		{
			SetCoins(InventorySave.coins);
		}
	}

	public void SetKeysLock(bool value)
	{
		_keysLocked = value;
		if (!_keysLocked)
		{
			SetKeys(InventorySave.keys);
		}
	}

	public void SetHoverboardLock(bool value)
	{
		_hoverboardsLocked = value;
		if (!_hoverboardsLocked)
		{
			SetHoverboards(InventorySave.GetAmount(UpgradeType.hoverboard));
		}
	}

	public void SetHighScore(int newScore, bool align = true)
	{
		_scoreLabel.text = newScore.FormatSI();
		if (align)
		{
			Align();
		}
	}

	public void SetCoins(int coins, bool align = true)
	{
		_coinsLabel.text = coins.FormatSI();
		if (align)
		{
			Align();
		}
	}

	public void UpdateCoinBoxLayout()
	{
		Layout[] componentsInChildren = _rightLayout.GetComponentsInChildren<Layout>();
		foreach (Layout layout in componentsInChildren)
		{
			layout.Align(once: true);
		}
		_coinboxFunds.ResetAndUpdateAnchors();
		_coinboxFundsBackground.ResetAndUpdateAnchors();
	}

	public void SetKeys(int keys, bool align = true)
	{
		_keysLabel.text = keys.FormatSI();
		if (align)
		{
			Align();
		}
	}

	public void SetHoverboards(int hoverboards, bool align = true)
	{
		_hoverboardsLabel.text = hoverboards.FormatSI();
		if (align)
		{
			Align();
		}
	}

	public void ShowTopBar(bool show)
	{
		_rightLayout.gameObject.SetActive(OnboardingSave.isTutorialCompleted);
		_centerLayout.gameObject.SetActive(OnboardingSave.isTutorialCompleted);
		Collider[] clickAreasLockedDuringTutorial = _clickAreasLockedDuringTutorial;
		foreach (Collider collider in clickAreasLockedDuringTutorial)
		{
			collider.enabled = OnboardingSave.isTutorialCompleted;
		}
		_currentAnimationDirection = (show ? AnimationDirection.In : AnimationDirection.Out);
		Align();
		_currentFrameDelays = 2;
	}

	private void Update()
	{
		if (_currentFrameDelays > 0)
		{
			_currentFrameDelays--;
		}
		else
		{
			UpdateAnimation();
		}
		_interactionAllowed = _currentAnimationDirection == AnimationDirection.None;
	}

	private void UpdateAnimation()
	{
		switch (_currentAnimationDirection)
		{
		case AnimationDirection.In:
		{
			_currentFactor = Mathf.Clamp01(_currentFactor - deltaFactor);
			float t = Easings.CubicEaseIn(_currentFactor);
			float posY = Mathf.Lerp(0f, _hiddenPosition, t);
			base.transform.SetLocalPosY(posY);
			if (_currentFactor <= 0f)
			{
				_currentAnimationDirection = AnimationDirection.None;
			}
			break;
		}
		case AnimationDirection.Out:
		{
			_currentFactor = Mathf.Clamp01(_currentFactor + deltaFactor);
			float t = Easings.CubicEaseIn(_currentFactor);
			float posY = Mathf.Lerp(0f, _hiddenPosition, t);
			base.transform.SetLocalPosY(posY);
			if (_currentFactor >= 1f)
			{
				_currentAnimationDirection = AnimationDirection.None;
			}
			break;
		}
		}
	}

	[UsedImplicitly]
	public void SettingsClicked()
	{
		if (_interactionAllowed)
		{
			CloseOtherPopups("SettingsPopup");
			ShowOrClosePopup("SettingsPopup");
		}
	}

	[UsedImplicitly]
	public void ScoreClicked()
	{
		if (_interactionAllowed)
		{
			CloseOtherPopups("LeaderboardPopup");
			ShowOrClosePopup("LeaderboardPopup");
		}
	}

	[UsedImplicitly]
	public void HoverboardsClicked()
	{
		if (_interactionAllowed)
		{
			SellConsumable.activeConsumable = SellableConsumable.hoverboard;
			CloseOtherPopups("TutorialHoverboardsPopup", "ConsumablesPopup");
			if (showHoverboardTutorial)
			{
				UIScreenController.Instance.QueuePopup("TutorialHoverboardsPopup");
				OnboardingSave.hasShownHoverboardPopup = true;
				OnboardingSave.shouldShowHoverboardPopup = false;
				DetermineHoverboardTutorialVisibility();
			}
			else if (UIScreenController.Instance.IsPopupInQueue("TutorialHoverboardsPopup"))
			{
				UIScreenController.Instance.CloseActivePopups("TutorialHoverboardsPopup");
			}
			else
			{
				ShowOrClosePopup("ConsumablesPopup");
			}
		}
	}

	[UsedImplicitly]
	public void CoinboxClicked()
	{
		if (_interactionAllowed)
		{
			CloseOtherPopups("StorePopup");
			ShowOrClosePopup("StorePopup");
		}
	}

	private void ShowOrClosePopup(string popupName)
	{
		if (!UIScreenController.Instance.IsPopupInQueue(popupName))
		{
			UIScreenController.Instance.QueuePopup(popupName);
		}
		else
		{
			UIScreenController.Instance.CloseAllPopups();
		}
	}

	private void CloseOtherPopups(params string[] excludedPopups)
	{
		if (UIScreenController.Instance.isShowingPopup && !UIScreenController.Instance.IsPopupInQueue(excludedPopups))
		{
			UIScreenController.Instance.ignoreLastPopupClosed = true;
			UIScreenController.Instance.CloseAllPopups();
		}
	}

	private void ChangedScreen(string screen)
	{
		if (!UIScreenController.Instance.isShowingPopup)
		{
			DetermineHoverboardTutorialVisibility();
		}
	}

	private void DetermineHoverboardTutorialVisibility()
	{
		_hoverboardTutorial.ShowIcon(showHoverboardTutorial);
	}

	[Button(null)]
	private void Align()
	{
		UIWidget component = GetComponent<UIWidget>();
		component.width = ((_payload == null) ? 320 : _payload.width);
		component.height = ((_payload == null) ? 26 : _payload.height);
		List<Layout> list = (from c in GetComponentsInChildren<Layout>()
			orderby c.transform.GetHierarchyDepth(base.transform) descending
			select c).ToList();
		list.ForEach((Layout l) =>
		{
			l.Align(once: true);
		});
		_coinboxFunds.ResetAndUpdateAnchors();
		_coinboxFundsBackground.ResetAndUpdateAnchors();
		_leftLayout.Align(once: true);
		_rightLayout.Align(once: true);
		_centerLayout.Align(once: true);
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(base.transform, _leftLayout.transform, considerInactive: false);
		Bounds bounds2 = NGUIMath.CalculateRelativeWidgetBounds(base.transform, _rightLayout.transform, considerInactive: false, considerChildren: true, useLabelPrintedSize: true);
		int num = (component.width - 10) / 2;
		float x = bounds.size.x;
		float x2 = bounds2.size.x;
		float posX = (x - x2) / 2f;
		_leftLayout.transform.SetLocalPosX(-num);
		_rightLayout.transform.SetLocalPosX(num);
		_centerLayout.transform.SetLocalPosX(posX);
		(from r in GetComponentsInChildren<UIRect>().ToList()
			orderby r.transform.GetHierarchyDepth() descending
			select r).ToList().ForEach((UIRect r) =>
		{
			r.ResetAndUpdateAnchors();
		});
	}

	[Button(null)]
	private void SetupBasic()
	{
		ColorData colors = ColorData.instance;
		_settingsIcon.color = colors.stroking;
		_background.gradientTop = "#54666d".ToColor();
		_background.gradientBottom = "#617b85".ToColor();
		_buttonBackgrounds.ForEach((UISprite s) =>
		{
			s.gradientTop = colors.rock;
			s.gradientBottom = colors.iron;
		});
		_buttonShadows.ForEach((UISprite s) =>
		{
			s.color = colors.shadow;
		});
		if (!Application.isPlaying)
		{
			base.gameObject.SetActive(value: false);
			base.gameObject.SetActive(value: true);
		}
	}

	[Button("ShowBar")]
	private void Editor_ShowBar()
	{
		ShowTopBar(show: true);
	}

	[Button("HideBar")]
	private void Editor_HideBar()
	{
		ShowTopBar(show: false);
	}
}
