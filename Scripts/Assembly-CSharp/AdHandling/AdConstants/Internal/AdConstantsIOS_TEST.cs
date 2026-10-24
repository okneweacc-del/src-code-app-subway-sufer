using System.Collections.Generic;
using Kiloo.Common.Ads;

namespace AdHandling.AdConstants.Internal;

public class AdConstantsIOS_TEST : IAdConstants
{
	private readonly Dictionary<AdLocationInfo, string> _adColonyCacheIDs = new Dictionary<AdLocationInfo, string>
	{
		{
			AdLocations.POOL1_VIDEOS,
			"vz1b7fa5d4092a4f59a4"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"vz0a1d7438d43348b297"
		}
	};

	private readonly Dictionary<AdLocationInfo, string> _tapjoyCacheIDs = new Dictionary<AdLocationInfo, string>
	{
		{
			AdLocations.POOL1_VIDEOS,
			"pool1_videos"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"pool2_videos"
		},
		{
			AdLocations.POOL3_VIDEOS,
			"pool3_videos"
		},
		{
			AdLocations.HOME_VIDEOS,
			"home_videos"
		},
		{
			AdLocations.EARN_COINS_VIDEOS,
			"earn_coins_videos"
		},
		{
			AdLocations.DOUBLE_UP_VIDEOS,
			"double_up_videos"
		},
		{
			AdLocations.WORDY_WEEKEND_VIDEOS,
			"wordy_weekend_videos"
		},
		{
			AdLocations.FREE_COINS_FREE_VIDEOS,
			"free_coins_free_videos"
		}
	};

	private readonly Dictionary<AdLocationInfo, string> _chartboostCacheIDs = new Dictionary<AdLocationInfo, string>
	{
		{
			AdLocations.POOL1_VIDEOS,
			"pool1_videos"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"pool2_videos"
		},
		{
			AdLocations.POOL3_VIDEOS,
			"pool3_videos"
		},
		{
			AdLocations.HOME_VIDEOS,
			"home_videos"
		},
		{
			AdLocations.EARN_COINS_VIDEOS,
			"earn_coins_videos"
		},
		{
			AdLocations.DOUBLE_UP_VIDEOS,
			"double_up_videos"
		},
		{
			AdLocations.WORDY_WEEKEND_VIDEOS,
			"wordy_weekend_videos"
		},
		{
			AdLocations.HOME_INTERSTITIALS,
			"home_interstitials"
		},
		{
			AdLocations.GAME_OVER_INTERSTITIALS,
			"game_over_interstitials"
		},
		{
			AdLocations.FREE_COINS_FREE_VIDEOS,
			"free_coins_free_videos"
		}
	};

	private readonly Dictionary<AdLocationInfo, string> _inmobiCacheIDs = new Dictionary<AdLocationInfo, string>
	{
		{
			AdLocations.POOL1_VIDEOS,
			"1536452057849"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"1536708164374"
		},
		{
			AdLocations.POOL3_VIDEOS,
			"1535445232751"
		},
		{
			AdLocations.HOME_VIDEOS,
			"1484554142365"
		},
		{
			AdLocations.EARN_COINS_VIDEOS,
			"1483719495219"
		},
		{
			AdLocations.DOUBLE_UP_VIDEOS,
			"1482781250472"
		},
		{
			AdLocations.WORDY_WEEKEND_VIDEOS,
			string.Empty
		},
		{
			AdLocations.HOME_INTERSTITIALS,
			"1483524202742"
		},
		{
			AdLocations.GAME_OVER_INTERSTITIALS,
			"1484907783158"
		},
		{
			AdLocations.FREE_COINS_FREE_VIDEOS,
			"x"
		}
	};

	private readonly Dictionary<AdLocationInfo, string> _vungleCacheIDs = new Dictionary<AdLocationInfo, string>
	{
		{
			AdLocations.POOL1_VIDEOS,
			"pool1_videos"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"pool2_videos"
		},
		{
			AdLocations.POOL3_VIDEOS,
			"pool3_videos"
		},
		{
			AdLocations.HOME_VIDEOS,
			"home_videos"
		},
		{
			AdLocations.EARN_COINS_VIDEOS,
			"earn_coins_videos"
		},
		{
			AdLocations.DOUBLE_UP_VIDEOS,
			"double_up_videos"
		},
		{
			AdLocations.WORDY_WEEKEND_VIDEOS,
			"wordy_weekend_videos"
		},
		{
			AdLocations.FREE_COINS_FREE_VIDEOS,
			"free_coins_free_videos"
		}
	};

	private readonly Dictionary<AdLocationInfo, string> _supersonicCacheIDs = new Dictionary<AdLocationInfo, string>
	{
		{
			AdLocations.POOL1_VIDEOS,
			"pool1_videos"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"pool2_videos"
		},
		{
			AdLocations.POOL3_VIDEOS,
			"pool3_videos"
		},
		{
			AdLocations.HOME_VIDEOS,
			"home_videos"
		},
		{
			AdLocations.EARN_COINS_VIDEOS,
			"earn_coins_videos"
		},
		{
			AdLocations.DOUBLE_UP_VIDEOS,
			"double_up_videos"
		},
		{
			AdLocations.WORDY_WEEKEND_VIDEOS,
			"wordy_weekend_videos"
		},
		{
			AdLocations.HOME_INTERSTITIALS,
			"home_interstitials"
		},
		{
			AdLocations.GAME_OVER_INTERSTITIALS,
			"game_over_interstitials"
		},
		{
			AdLocations.FREE_COINS_FREE_VIDEOS,
			"free_coins_free_videos"
		}
	};

	private readonly Dictionary<AdLocationInfo, string> _facebookCacheIDs = new Dictionary<AdLocationInfo, string>();

	private readonly Dictionary<AdLocationInfo, string> _mobvistaCacheIDs = new Dictionary<AdLocationInfo, string>
	{
		{
			AdLocations.POOL1_VIDEOS,
			"53598"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"53599"
		},
		{
			AdLocations.POOL3_VIDEOS,
			"53600"
		},
		{
			AdLocations.HOME_VIDEOS,
			"21319"
		}
	};

	public bool isRestricted => false;

	public string AdColonyAppId => "appe16ddc2239b94013abd6b2";

	public Dictionary<AdLocationInfo, string> AdColonyCacheIDs => _adColonyCacheIDs;

	public string TapJoyAppId => "feb4000a-c9ee-4388-941f-e0391e462ec5";

	public string TapJoyAppKey => "_rQACsnuQ4iUH-A5HkYuxQEBElEQtlcZKs1W2D9sa6g1grIoAafYJ9EJffsN";

	public Dictionary<AdLocationInfo, string> TapJoyCacheIDs => _tapjoyCacheIDs;

	public string ChartboostAppId => "5024e3582284bb9a5a00003d";

	public string ChartboostAppSignature => "73a9d4bbe6eee00ee592f4aa73604c78fddd717e";

	public Dictionary<AdLocationInfo, string> ChartboostCacheIDs => _chartboostCacheIDs;

	public string InmobiAccountId => "65f49e71333d405686b652d839b89af9";

	public Dictionary<AdLocationInfo, string> InmobiCacheIDs => _inmobiCacheIDs;

	public string VungleAppId => "586f90feee0dcc35280004c3";

	public Dictionary<AdLocationInfo, string> VungleCacheIDs => _vungleCacheIDs;

	public string SupersonicAppId => "4cbeec05";

	public Dictionary<AdLocationInfo, string> SupersonicCacheIDs => _supersonicCacheIDs;

	public Dictionary<AdLocationInfo, string> FacebookCacheIDs => _facebookCacheIDs;

	public Dictionary<AdLocationInfo, string> MobVistaCacheIDs => _mobvistaCacheIDs;

	public string MobVistaAppId => "92763";

	public string MobVistaAppSdkKey => "936dcbdd57fe235fd7cf61c2e93da3c4";
}
