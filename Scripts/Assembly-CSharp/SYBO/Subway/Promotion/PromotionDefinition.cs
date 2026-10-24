using System;
using Kiloo;
using Kiloo.Common.Utility;
using UnityEngine;

namespace SYBO.Subway.Promotion;

public abstract class PromotionDefinition : ScriptableObject
{
	[SerializeField]
	private bool _isEnabled;

	[SerializeField]
	private string _promotionId = "promotion_id";

	[Header("Popup")]
	[SerializeField]
	private Texture2D _stickerTexture;

	[SerializeField]
	private Color _titleTextColor = new Color(3f, 59f, 113f, 255f);

	[SerializeField]
	private Color _subtitleTextColor;

	[SerializeField]
	private Color _timerBgColor = GlobalColorsGenerated.WEEKLYHUNT_TIMER_BG;

	[SerializeField]
	private Color _timerTextColor = GlobalColorsGenerated.WEEKLYHUNT_TIMER_TEXT;

	[Header("Front Screen")]
	[SerializeField]
	private GameObject _buttonPrefab;

	public bool IsActive
	{
		get
		{
			long num = CDateTime.Now.ToUnixTime();
			bool flag = num < StartTimeUnix || num >= ExpireTimeUnix;
			return _isEnabled && !flag && IsActiveCheck;
		}
	}

	protected virtual bool IsActiveCheck => true;

	public bool IsEnabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			_isEnabled = value;
		}
	}

	public abstract string PopupId { get; }

	public abstract string TitleId { get; }

	public abstract string SubtitleId { get; }

	public abstract DateTime StartTime { get; }

	public abstract DateTime ExpireTime { get; }

	public string PromotionId => _promotionId;

	public Texture2D StickerTexture => _stickerTexture;

	public Color TitleTextColor => _titleTextColor;

	public Color SubtitleTextColor => _subtitleTextColor;

	public Color TimerBgColor => _timerBgColor;

	public Color TimerTextColor => _timerTextColor;

	public GameObject ButtonPrefab => _buttonPrefab;

	public long StartTimeUnix => StartTime.ToUnixTime();

	public long ExpireTimeUnix => ExpireTime.ToUnixTime();

	public TimeSpan TimeLeft
	{
		get
		{
			long expireTimeUnix = ExpireTimeUnix;
			long num = CDateTime.Now.ToUnixTime();
			return TimeSpan.FromSeconds(expireTimeUnix - num);
		}
	}

	public abstract string GetPurchaseSuccessFlurryEventValue();

	public abstract string GetPurchaseFailureFlurryEventValue();

	public abstract bool ShouldTrackPopupCloseFlurryEvent();
}
