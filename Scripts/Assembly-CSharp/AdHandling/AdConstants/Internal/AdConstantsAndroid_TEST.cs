using System.Collections.Generic;
using Kiloo.Common.Ads;

namespace AdHandling.AdConstants.Internal;

public class AdConstantsAndroid_TEST : IAdConstants
{
	private readonly Dictionary<AdLocationInfo, string> _adColonyCacheIDs = new Dictionary<AdLocationInfo, string>
	{
		{
			AdLocations.POOL1_VIDEOS,
			"pool1_videos"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"pool2_videos"
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
		},
		{
			AdLocations.FREE_COINS_POPUP_VIDEOS,
			"free_coins_popup_videos"
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
		},
		{
			AdLocations.FREE_COINS_POPUP_VIDEOS,
			"free_coins_popup_videos"
		}
	};

	private readonly Dictionary<AdLocationInfo, string> _inmobiCacheIDs = new Dictionary<AdLocationInfo, string>
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
			"1483261275117"
		},
		{
			AdLocations.EARN_COINS_VIDEOS,
			"1484355156940"
		},
		{
			AdLocations.DOUBLE_UP_VIDEOS,
			"1484337271044"
		},
		{
			AdLocations.WORDY_WEEKEND_VIDEOS,
			"1483261275117"
		},
		{
			AdLocations.HOME_INTERSTITIALS,
			"1484122908377"
		},
		{
			AdLocations.GAME_OVER_INTERSTITIALS,
			"1482985195426"
		},
		{
			AdLocations.FREE_COINS_FREE_VIDEOS,
			"x"
		},
		{
			AdLocations.FREE_COINS_POPUP_VIDEOS,
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
		},
		{
			AdLocations.FREE_COINS_POPUP_VIDEOS,
			"free_coins_popup_videos"
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
		},
		{
			AdLocations.FREE_COINS_POPUP_VIDEOS,
			"free_coins_popup_videos"
		}
	};

	private readonly Dictionary<AdLocationInfo, string> _facebookCacheIDs = new Dictionary<AdLocationInfo, string>
	{
		{
			AdLocations.HOME_INTERSTITIALS,
			"254616967963463_1278936912198125"
		},
		{
			AdLocations.GAME_OVER_INTERSTITIALS,
			"254616967963463_1278937068864776"
		}
	};

	private readonly Dictionary<AdLocationInfo, string> _mobvistaCacheIDs = new Dictionary<AdLocationInfo, string>
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
			"21310"
		}
	};

	public bool isRestricted => false;

	public string AdColonyAppId => "app6caa49a0fc224e4e81";

	public Dictionary<AdLocationInfo, string> AdColonyCacheIDs => _adColonyCacheIDs;

	public string TapJoyAppId => "79933fc1-a870-4194-9cca-68e4f571c2a6";

	public string TapJoyAppKey => "DyK3KNZ-RJKC-lSjbZzkowEC1FeZOeZEJGzfQxpf4kT3jgxfDKQb3LDSPfJW";

	public Dictionary<AdLocationInfo, string> TapJoyCacheIDs => _tapjoyCacheIDs;

	public string ChartboostAppId => "583413c2f6cd450e7cddae1b";

	public string ChartboostAppSignature => "cb1e3f6960cf0070e868c049e903232d72b83b83";

	public Dictionary<AdLocationInfo, string> ChartboostCacheIDs => _chartboostCacheIDs;

	public string InmobiAccountId => "65f49e71333d405686b652d839b89af9";

	public Dictionary<AdLocationInfo, string> InmobiCacheIDs => _inmobiCacheIDs;

	public string VungleAppId => "586f5f39ee4ada033e00025b";

	public Dictionary<AdLocationInfo, string> VungleCacheIDs => _vungleCacheIDs;

	public string SupersonicAppId => "4cbeb27d";

	public Dictionary<AdLocationInfo, string> SupersonicCacheIDs => _supersonicCacheIDs;

	public Dictionary<AdLocationInfo, string> FacebookCacheIDs => _facebookCacheIDs;

	public Dictionary<AdLocationInfo, string> MobVistaCacheIDs => _mobvistaCacheIDs;

	public string MobVistaAppId => "92762";

	public string MobVistaAppSdkKey => "936dcbdd57fe235fd7cf61c2e93da3c4";
}
