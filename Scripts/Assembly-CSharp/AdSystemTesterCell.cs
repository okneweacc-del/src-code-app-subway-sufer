using Kiloo.Common.Ads;
using UnityEngine;

public class AdSystemTesterCell : MonoBehaviour
{
	[SerializeField]
	private UILabel _statusLabel;

	[SerializeField]
	private GameObject _successIcon;

	private AdLocationInfo _adLocation;

	private const string MAIN_TEXT_COLOR = "[004A80FF]";

	private const string SECONDARY_TEXT_COLOR = "[0374A5FF]";

	public void Init(AdLocationInfo adLocation)
	{
		_adLocation = adLocation;
		UpdateStatusLabel(_adLocation);
	}

	public void Cache()
	{
		AdSystemManager.Cache();
	}

	public void Show()
	{
		if (_adLocation.adType == AdType.Video)
		{
			AdSystemManager.Show(_adLocation, GiveDebugReward);
		}
		else
		{
			AdSystemManager.Show(_adLocation);
		}
	}

	private void OnEnable()
	{
		AdSystemManager.onCachedVideo += OnCacheStateChanged;
		AdSystemManager.onCachedInterstitial += OnCacheStateChanged;
		AdSystemManager.onFailedToCacheVideo += OnCacheStateChanged;
		AdSystemManager.onFailedToCacheInterstitial += OnCacheStateChanged;
		AdSystemManager.onShowedVideo += OnCacheStateChanged;
		AdSystemManager.onShowedInterstitial += OnCacheStateChanged;
		AdSystemManager.onFailedToShowVideo += OnCacheStateChanged;
		AdSystemManager.onFailedToShowInterstitial += OnCacheStateChanged;
		AdSystemManager.onDismissedVideo += OnCacheStateChanged;
		AdSystemManager.onDismissedInterstitial += OnCacheStateChanged;
		AdSystemManager.onPriorityListWrapped += OnCacheStateChanged;
		AdSystemManager.onChangedProvider += OnCacheStateChanged;
		UpdateStatusLabel(_adLocation);
	}

	private void OnDisable()
	{
		AdSystemManager.onCachedVideo -= OnCacheStateChanged;
		AdSystemManager.onCachedInterstitial -= OnCacheStateChanged;
		AdSystemManager.onFailedToCacheVideo -= OnCacheStateChanged;
		AdSystemManager.onFailedToCacheInterstitial -= OnCacheStateChanged;
		AdSystemManager.onShowedVideo -= OnCacheStateChanged;
		AdSystemManager.onShowedInterstitial -= OnCacheStateChanged;
		AdSystemManager.onFailedToShowVideo -= OnCacheStateChanged;
		AdSystemManager.onFailedToShowInterstitial -= OnCacheStateChanged;
		AdSystemManager.onDismissedVideo -= OnCacheStateChanged;
		AdSystemManager.onDismissedInterstitial -= OnCacheStateChanged;
		AdSystemManager.onPriorityListWrapped -= OnCacheStateChanged;
		AdSystemManager.onChangedProvider -= OnCacheStateChanged;
	}

	private void OnCacheStateChanged(string adLocationId)
	{
		AdLocationInfo adLocationInfoFromID = AdSystemManager.GetAdLocationInfoFromID(adLocationId);
		OnCacheStateChanged(adLocationInfoFromID, wasAlreadyCached: false);
	}

	private void OnCacheStateChanged(AdLocationInfo adLocationInfo)
	{
		OnCacheStateChanged(adLocationInfo, wasAlreadyCached: false);
	}

	private void OnCacheStateChanged(AdLocationInfo adLocationInfo, bool wasAlreadyCached)
	{
		if (_adLocation == adLocationInfo)
		{
			UpdateStatusLabel(_adLocation);
		}
	}

	private void UpdateStatusLabel(AdLocationInfo adLocation)
	{
		if (adLocation != null)
		{
			_statusLabel.text = $"[c][004A80FF]{adLocation.id}\n[0374A5FF]{GetProviderIDForLocation(adLocation)}";
			_successIcon.SetActive(AdSystemManager.IsCached(adLocation));
		}
	}

	private string GetProviderIDForLocation(AdLocationInfo adLocationInfo)
	{
		string currentProviderIDForLocation = AdSystemManager.GetCurrentProviderIDForLocation(adLocationInfo);
		string text = "none";
		if (currentProviderIDForLocation == text)
		{
			return text;
		}
		if (!string.IsNullOrEmpty(currentProviderIDForLocation))
		{
			text = currentProviderIDForLocation;
		}
		return text;
	}

	private void GiveDebugReward()
	{
		UIScreenController.Instance.QueueErrorMessageSlidein("<Empty Debug Reward>");
	}
}
