using System.Collections.Generic;
using Kiloo.Common.Ads;

namespace AdHandling.AdConstants.Internal;

public class AdConstantsIOSRestricted : IAdConstants
{
	private readonly Dictionary<AdLocationInfo, string> _adColonyCacheIDs = new Dictionary<AdLocationInfo, string>
	{
		{
			AdLocations.POOL1_VIDEOS,
			"vzf05a3ad001a84571a9"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"vzad983e598f7a437eb3"
		}
	};

	private readonly Dictionary<AdLocationInfo, string> _tapjoyCacheIDs = new Dictionary<AdLocationInfo, string>();

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
			AdLocations.REVIVAL_VIDEOS,
			"revive_videos"
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
			AdLocations.HOVERBOARD_FREE_VIDEOS,
			"hoverboard_free_videos"
		},
		{
			AdLocations.HOVERBOARD_POPUP_VIDEOS,
			"hoverboard_popup_videos"
		},
		{
			AdLocations.HOVERBOARD_RUN_VIDEOS,
			"hoverboard_run_videos"
		},
		{
			AdLocations.SKIP_MISSION_PAUSE_VIDEOS,
			"skip_missions_pause_videos"
		},
		{
			AdLocations.SKIP_MISSION_TOUR_VIDEOS,
			"skip_missions_tour_videos"
		},
		{
			AdLocations.GAMEOVER_DOUBLE_COINS_VIDEOS,
			"double_coins_videos"
		},
		{
			AdLocations.FREE_COINS_FREE_VIDEOS,
			"free_coins_free_videos"
		},
		{
			AdLocations.FREE_COINS_POPUP_VIDEOS,
			"free_coins_popup_videos"
		},
		{
			AdLocations.HEADSTART_FREE_VIDEOS,
			"headstart_free_videos"
		},
		{
			AdLocations.HEADSTART_RUN_VIDEOS,
			"headstart_run_videos"
		},
		{
			AdLocations.SCOREBOOSTER_FREE_VIDEOS,
			"scorebooster_free_videos"
		},
		{
			AdLocations.SCOREBOOSTER_RUN_VIDEOS,
			"scorebooster_run_videos"
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
			"1511951482886"
		},
		{
			AdLocations.EARN_COINS_VIDEOS,
			"1511186263058"
		},
		{
			AdLocations.DOUBLE_UP_VIDEOS,
			"1513627357718"
		},
		{
			AdLocations.WORDY_WEEKEND_VIDEOS,
			"1512202810490"
		},
		{
			AdLocations.REVIVAL_VIDEOS,
			"1533078252373"
		},
		{
			AdLocations.HOME_INTERSTITIALS,
			"1510898185842"
		},
		{
			AdLocations.GAME_OVER_INTERSTITIALS,
			"1511622234379"
		},
		{
			AdLocations.HOVERBOARD_FREE_VIDEOS,
			"1533646929155"
		},
		{
			AdLocations.HOVERBOARD_POPUP_VIDEOS,
			"1533130071342"
		},
		{
			AdLocations.HOVERBOARD_RUN_VIDEOS,
			"1533766655181"
		},
		{
			AdLocations.SKIP_MISSION_PAUSE_VIDEOS,
			"1533841697609"
		},
		{
			AdLocations.SKIP_MISSION_TOUR_VIDEOS,
			"1532998702976"
		},
		{
			AdLocations.GAMEOVER_DOUBLE_COINS_VIDEOS,
			"1533329106108"
		},
		{
			AdLocations.FREE_COINS_FREE_VIDEOS,
			"1533549977327"
		},
		{
			AdLocations.FREE_COINS_POPUP_VIDEOS,
			"1533306166949"
		},
		{
			AdLocations.HEADSTART_FREE_VIDEOS,
			"1532915692850"
		},
		{
			AdLocations.HEADSTART_RUN_VIDEOS,
			"1532270159821"
		},
		{
			AdLocations.SCOREBOOSTER_FREE_VIDEOS,
			"1533018932454"
		},
		{
			AdLocations.SCOREBOOSTER_RUN_VIDEOS,
			"1530967590885"
		}
	};

	private readonly Dictionary<AdLocationInfo, string> _vungleCacheIDs = new Dictionary<AdLocationInfo, string>
	{
		{
			AdLocations.POOL1_VIDEOS,
			"SUBWAY_SURFERS_IOS_COPPA_VIDEOS_POOL1_VIDEOS-9123470"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"SUBWAY_SURFERS_IOS_COPPA_VIDEOS_POOL2_VIDEOS-2278361"
		},
		{
			AdLocations.POOL3_VIDEOS,
			"SUBWAY_SURFERS_IOS_COPPA_VIDEOS_POOL3_VIDEOS-1936631"
		},
		{
			AdLocations.HOME_VIDEOS,
			"SUBWAYS44839"
		},
		{
			AdLocations.EARN_COINS_VIDEOS,
			"SUBWAYS94646"
		},
		{
			AdLocations.DOUBLE_UP_VIDEOS,
			"SUBWAYS51754"
		},
		{
			AdLocations.WORDY_WEEKEND_VIDEOS,
			"SUBWAYS52191"
		},
		{
			AdLocations.HOVERBOARD_FREE_VIDEOS,
			"SUBWAY_SURFERS_IOS_COPPA_VIDEOS_HOVERBOARD_FREE_VIDEOS-0035816"
		},
		{
			AdLocations.HOVERBOARD_POPUP_VIDEOS,
			"SUBWAY_SURFERS_IOS_COPPA_VIDEOS_HOVERBOARD_POPUP_VIDEOS-9560770"
		},
		{
			AdLocations.HOVERBOARD_RUN_VIDEOS,
			"SUBWAY_SURFERS_IOS_COPPA_VIDEOS_HOVERBOARD_RUN_VIDEOS-6953002"
		},
		{
			AdLocations.SKIP_MISSION_PAUSE_VIDEOS,
			"SUBWAY_SURFERS_IOS_COPPA_VIDEOS_SKIP_MISSIONS_PAUSE_VIDEOS-1777006"
		},
		{
			AdLocations.SKIP_MISSION_TOUR_VIDEOS,
			"SUBWAY_SURFERS_IOS_COPPA_VIDEOS_SKIP_MISSIONS_TOUR_VIDEOS-7539776"
		},
		{
			AdLocations.REVIVAL_VIDEOS,
			"SUBWAY_SURFERS_IOS_COPPA_VIDEOS_REVIVE_VIDEOS-8039706"
		},
		{
			AdLocations.GAMEOVER_DOUBLE_COINS_VIDEOS,
			"SUBWAY_SURFERS_IOS_COPPA_VIDEOS_DOUBLE_COINS_VIDEOS-5422623"
		},
		{
			AdLocations.FREE_COINS_FREE_VIDEOS,
			"SUBWAY_SURFERS_IOS_COPPA_VIDEOS_FREE_COINS_FREE_VIDEOS-5482812"
		},
		{
			AdLocations.FREE_COINS_POPUP_VIDEOS,
			"SUBWAY_SURFERS_IOS_COPPA_VIDEOS_FREE_COINS_POPUP_VIDEOS-6841867"
		},
		{
			AdLocations.HEADSTART_FREE_VIDEOS,
			"SUBWAY_SURFERS_IOS_COPPA_VIDEOS_HEADSTART_FREE_VIDEOS-1755477"
		},
		{
			AdLocations.HEADSTART_RUN_VIDEOS,
			"SUBWAY_SURFERS_IOS_COPPA_VIDEOS_HEADSTART_RUN_VIDEOS-8473409"
		},
		{
			AdLocations.SCOREBOOSTER_FREE_VIDEOS,
			"SUBWAY_SURFERS_IOS_COPPA_VIDEOS_SCOREBOOSTER_FREE_VIDEOS-5147264"
		},
		{
			AdLocations.SCOREBOOSTER_RUN_VIDEOS,
			"SUBWAY_SURFERS_IOS_COPPA_VIDEOS_SCOREBOOSTER_RUN_VIDEOS-9924084"
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
			AdLocations.REVIVAL_VIDEOS,
			"revive_videos"
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
			AdLocations.HOVERBOARD_FREE_VIDEOS,
			"hoverboard_free_videos"
		},
		{
			AdLocations.HOVERBOARD_POPUP_VIDEOS,
			"hoverboard_popup_videos"
		},
		{
			AdLocations.HOVERBOARD_RUN_VIDEOS,
			"hoverboard_run_videos"
		},
		{
			AdLocations.SKIP_MISSION_PAUSE_VIDEOS,
			"skip_missions_pause_videos"
		},
		{
			AdLocations.SKIP_MISSION_TOUR_VIDEOS,
			"skip_missions_tour_videos"
		},
		{
			AdLocations.GAMEOVER_DOUBLE_COINS_VIDEOS,
			"double_coins_videos"
		},
		{
			AdLocations.FREE_COINS_FREE_VIDEOS,
			"free_coins_free_videos"
		},
		{
			AdLocations.FREE_COINS_POPUP_VIDEOS,
			"free_coins_popup_videos"
		},
		{
			AdLocations.HEADSTART_FREE_VIDEOS,
			"headstart_free_videos"
		},
		{
			AdLocations.HEADSTART_RUN_VIDEOS,
			"headstart_run_videos"
		},
		{
			AdLocations.SCOREBOOSTER_FREE_VIDEOS,
			"scorebooster_free_videos"
		},
		{
			AdLocations.SCOREBOOSTER_RUN_VIDEOS,
			"scorebooster_run_videos"
		}
	};

	private readonly Dictionary<AdLocationInfo, string> _facebookCacheIDs = new Dictionary<AdLocationInfo, string>();

	private readonly Dictionary<AdLocationInfo, string> _mobvistaCacheIDs = new Dictionary<AdLocationInfo, string>
	{
		{
			AdLocations.POOL1_VIDEOS,
			"53590"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"53591"
		},
		{
			AdLocations.POOL3_VIDEOS,
			"53592"
		},
		{
			AdLocations.HOME_VIDEOS,
			"34164"
		},
		{
			AdLocations.EARN_COINS_VIDEOS,
			"34166"
		},
		{
			AdLocations.DOUBLE_UP_VIDEOS,
			"34165"
		},
		{
			AdLocations.WORDY_WEEKEND_VIDEOS,
			"34167"
		},
		{
			AdLocations.REVIVAL_VIDEOS,
			"49038"
		},
		{
			AdLocations.HOVERBOARD_FREE_VIDEOS,
			"49546"
		},
		{
			AdLocations.HOVERBOARD_POPUP_VIDEOS,
			"49545"
		},
		{
			AdLocations.HOVERBOARD_RUN_VIDEOS,
			"50445"
		},
		{
			AdLocations.SKIP_MISSION_PAUSE_VIDEOS,
			"50415"
		},
		{
			AdLocations.SKIP_MISSION_TOUR_VIDEOS,
			"50414"
		},
		{
			AdLocations.GAMEOVER_DOUBLE_COINS_VIDEOS,
			"50437"
		},
		{
			AdLocations.FREE_COINS_FREE_VIDEOS,
			"50244"
		},
		{
			AdLocations.FREE_COINS_POPUP_VIDEOS,
			"50243"
		},
		{
			AdLocations.HEADSTART_FREE_VIDEOS,
			"50964"
		},
		{
			AdLocations.HEADSTART_RUN_VIDEOS,
			"50963"
		},
		{
			AdLocations.SCOREBOOSTER_FREE_VIDEOS,
			"50966"
		},
		{
			AdLocations.SCOREBOOSTER_RUN_VIDEOS,
			"50965"
		}
	};

	public bool isRestricted => true;

	public string AdColonyAppId => "app5e9882cd3d6d44d6b3";

	public Dictionary<AdLocationInfo, string> AdColonyCacheIDs => _adColonyCacheIDs;

	public string TapJoyAppId => string.Empty;

	public string TapJoyAppKey => string.Empty;

	public Dictionary<AdLocationInfo, string> TapJoyCacheIDs => _tapjoyCacheIDs;

	public string ChartboostAppId => "5922afa5f6cd4555d6ac141e";

	public string ChartboostAppSignature => "e2ef6a4792d5fd86f5dd4796b451f6d1b5bf2e27";

	public Dictionary<AdLocationInfo, string> ChartboostCacheIDs => _chartboostCacheIDs;

	public string InmobiAccountId => "6c7ea8566bfd4b2c82269e3647046624";

	public Dictionary<AdLocationInfo, string> InmobiCacheIDs => _inmobiCacheIDs;

	public string VungleAppId => "591c3a6b1950241f4a000366";

	public Dictionary<AdLocationInfo, string> VungleCacheIDs => _vungleCacheIDs;

	public string SupersonicAppId => "499b4845";

	public Dictionary<AdLocationInfo, string> SupersonicCacheIDs => _supersonicCacheIDs;

	public Dictionary<AdLocationInfo, string> FacebookCacheIDs => _facebookCacheIDs;

	public Dictionary<AdLocationInfo, string> MobVistaCacheIDs => _mobvistaCacheIDs;

	public string MobVistaAppId => "101367";

	public string MobVistaAppSdkKey => "f51fde5b5c666de31358328df8b3141a";
}
