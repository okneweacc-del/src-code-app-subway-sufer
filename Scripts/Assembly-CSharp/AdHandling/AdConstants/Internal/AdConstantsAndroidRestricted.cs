using System.Collections.Generic;
using Kiloo.Common.Ads;

namespace AdHandling.AdConstants.Internal;

public class AdConstantsAndroidRestricted : IAdConstants
{
	private readonly Dictionary<AdLocationInfo, string> _adColonyCacheIDs = new Dictionary<AdLocationInfo, string>
	{
		{
			AdLocations.POOL1_VIDEOS,
			"vzfaa9e30239bd42c791"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"vzd7a31c6013184a80b6"
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
			AdLocations.HOVERBOARD_RUN_VIDEOS,
			"hoverboard_run_videos"
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
			AdLocations.SKIP_MISSION_TOUR_VIDEOS,
			"skip_missions_tour_videos"
		},
		{
			AdLocations.SKIP_MISSION_PAUSE_VIDEOS,
			"skip_missions_pause_videos"
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
			"1535640878100"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"1536451956722"
		},
		{
			AdLocations.POOL3_VIDEOS,
			"1536743934438"
		},
		{
			AdLocations.HOME_VIDEOS,
			"1507693175403"
		},
		{
			AdLocations.EARN_COINS_VIDEOS,
			"1504073866403"
		},
		{
			AdLocations.DOUBLE_UP_VIDEOS,
			"1503946563250"
		},
		{
			AdLocations.WORDY_WEEKEND_VIDEOS,
			"1504794347985"
		},
		{
			AdLocations.REVIVAL_VIDEOS,
			"1533143736225"
		},
		{
			AdLocations.HOME_INTERSTITIALS,
			"1505637766750"
		},
		{
			AdLocations.GAME_OVER_INTERSTITIALS,
			"1504241684825"
		},
		{
			AdLocations.HOVERBOARD_RUN_VIDEOS,
			"1530342096301"
		},
		{
			AdLocations.HOVERBOARD_FREE_VIDEOS,
			"1532363881452"
		},
		{
			AdLocations.HOVERBOARD_POPUP_VIDEOS,
			"1533896221931"
		},
		{
			AdLocations.SKIP_MISSION_TOUR_VIDEOS,
			"1530411299028"
		},
		{
			AdLocations.SKIP_MISSION_PAUSE_VIDEOS,
			"1531548457112"
		},
		{
			AdLocations.GAMEOVER_DOUBLE_COINS_VIDEOS,
			"1533349746105"
		},
		{
			AdLocations.FREE_COINS_FREE_VIDEOS,
			"1533034360669"
		},
		{
			AdLocations.FREE_COINS_POPUP_VIDEOS,
			"1530148332014"
		},
		{
			AdLocations.HEADSTART_FREE_VIDEOS,
			"1532477695351"
		},
		{
			AdLocations.HEADSTART_RUN_VIDEOS,
			"1533240534984"
		},
		{
			AdLocations.SCOREBOOSTER_FREE_VIDEOS,
			"1532923903242"
		},
		{
			AdLocations.SCOREBOOSTER_RUN_VIDEOS,
			"1533596427519"
		}
	};

	private readonly Dictionary<AdLocationInfo, string> _vungleCacheIDs = new Dictionary<AdLocationInfo, string>
	{
		{
			AdLocations.POOL1_VIDEOS,
			"SUBWAY_SURFERS_ANDROID_COPPA_VIDEOS_POOL1_VIDEOS-2448089"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"SUBWAY_SURFERS_ANDROID_COPPA_VIDEOS_POOL2_VIDEOS-3044169"
		},
		{
			AdLocations.POOL3_VIDEOS,
			"SUBWAY_SURFERS_ANDROID_COPPA_VIDEOS_POOL3_VIDEOS-7241273"
		},
		{
			AdLocations.HOME_VIDEOS,
			"SUBWAYS91739"
		},
		{
			AdLocations.EARN_COINS_VIDEOS,
			"SUBWAYS73071"
		},
		{
			AdLocations.DOUBLE_UP_VIDEOS,
			"SUBWAYS17890"
		},
		{
			AdLocations.WORDY_WEEKEND_VIDEOS,
			"SUBWAYS77142"
		},
		{
			AdLocations.HOVERBOARD_RUN_VIDEOS,
			"SUBWAY_SURFERS_ANDROID_COPPA_VIDEOS_HOVERBOARD_RUN_VIDEOS-6241823"
		},
		{
			AdLocations.HOVERBOARD_FREE_VIDEOS,
			"SUBWAY_SURFERS_ANDROID_COPPA_VIDEOS_HOVERBOARD_FREE_VIDEOS-8440201"
		},
		{
			AdLocations.HOVERBOARD_POPUP_VIDEOS,
			"SUBWAY_SURFERS_ANDROID_COPPA_VIDEOS_HOVERBOARD_POPUP_VIDEOS-6824297"
		},
		{
			AdLocations.SKIP_MISSION_TOUR_VIDEOS,
			"SUBWAY_SURFERS_ANDROID_COPPA_SKIP_MISSIONS_TOUR_VIDEOS-9750344"
		},
		{
			AdLocations.SKIP_MISSION_PAUSE_VIDEOS,
			"SUBWAY_SURFERS_ANDROID_COPPA_SKIP_MISSIONS_PAUSE_VIDEOS-5736726"
		},
		{
			AdLocations.REVIVAL_VIDEOS,
			"SUBWAY_SURFERS_ANDROID_COPPA_VIDEOS_REVIVE_VIDEOS-6338409"
		},
		{
			AdLocations.GAMEOVER_DOUBLE_COINS_VIDEOS,
			"SUBWAY_SURFERS_ANDROID_COPPA_VIDEOS_DOUBLE_COINS_VIDEOS-3654409"
		},
		{
			AdLocations.FREE_COINS_FREE_VIDEOS,
			"SUBWAY_SURFERS_ANDROID_COPPA_VIDEOS_FREE_COINS_FREE_VIDEOS-1061595"
		},
		{
			AdLocations.FREE_COINS_POPUP_VIDEOS,
			"SUBWAY_SURFERS_ANDROID_COPPA_VIDEOS_FREE_COINS_POPUP_VIDEOS-4824030"
		},
		{
			AdLocations.HEADSTART_FREE_VIDEOS,
			"SUBWAY_SURFERS_ANDROID_COPPA_VIDEOS_HEADSTART_FREE_VIDEOS-3299691"
		},
		{
			AdLocations.HEADSTART_RUN_VIDEOS,
			"SUBWAY_SURFERS_ANDROID_COPPA_VIDEOS_HEADSTART_RUN_VIDEOS-5536349"
		},
		{
			AdLocations.SCOREBOOSTER_FREE_VIDEOS,
			"SUBWAY_SURFERS_ANDROID_COPPA_VIDEOS_SCOREBOOSTER_FREE_VIDEOS-9311563"
		},
		{
			AdLocations.SCOREBOOSTER_RUN_VIDEOS,
			"SUBWAY_SURFERS_ANDROID_COPPA_VIDEOS_SCOREBOOSTER_RUN_VIDEOS-9169297"
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
			AdLocations.HOVERBOARD_RUN_VIDEOS,
			"hoverboard_run_videos"
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
			AdLocations.SKIP_MISSION_TOUR_VIDEOS,
			"skip_missions_tour_videos"
		},
		{
			AdLocations.SKIP_MISSION_PAUSE_VIDEOS,
			"skip_missions_pause_videos"
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
			"53595"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"53596"
		},
		{
			AdLocations.POOL3_VIDEOS,
			"53597"
		},
		{
			AdLocations.HOME_VIDEOS,
			"34174"
		},
		{
			AdLocations.EARN_COINS_VIDEOS,
			"34175"
		},
		{
			AdLocations.DOUBLE_UP_VIDEOS,
			"34176"
		},
		{
			AdLocations.WORDY_WEEKEND_VIDEOS,
			"34177"
		},
		{
			AdLocations.HOVERBOARD_RUN_VIDEOS,
			"50446"
		},
		{
			AdLocations.HOVERBOARD_FREE_VIDEOS,
			"49548"
		},
		{
			AdLocations.HOVERBOARD_POPUP_VIDEOS,
			"49547"
		},
		{
			AdLocations.SKIP_MISSION_TOUR_VIDEOS,
			"50416"
		},
		{
			AdLocations.SKIP_MISSION_PAUSE_VIDEOS,
			"50417"
		},
		{
			AdLocations.REVIVAL_VIDEOS,
			"49039"
		},
		{
			AdLocations.GAMEOVER_DOUBLE_COINS_VIDEOS,
			"50438"
		},
		{
			AdLocations.FREE_COINS_FREE_VIDEOS,
			"50246"
		},
		{
			AdLocations.FREE_COINS_POPUP_VIDEOS,
			"50245"
		},
		{
			AdLocations.HEADSTART_FREE_VIDEOS,
			"50968"
		},
		{
			AdLocations.HEADSTART_RUN_VIDEOS,
			"50967"
		},
		{
			AdLocations.SCOREBOOSTER_FREE_VIDEOS,
			"50970"
		},
		{
			AdLocations.SCOREBOOSTER_RUN_VIDEOS,
			"50969"
		}
	};

	public bool isRestricted => true;

	public string AdColonyAppId => "appf8a3f6e919564ed7b7";

	public Dictionary<AdLocationInfo, string> AdColonyCacheIDs => _adColonyCacheIDs;

	public string TapJoyAppId => string.Empty;

	public string TapJoyAppKey => string.Empty;

	public Dictionary<AdLocationInfo, string> TapJoyCacheIDs => _tapjoyCacheIDs;

	public string ChartboostAppId => "5922afa3f6cd45741492fe26";

	public string ChartboostAppSignature => "47feec269d093a00d6af9e1e86cb4781ba7ee12a";

	public Dictionary<AdLocationInfo, string> ChartboostCacheIDs => _chartboostCacheIDs;

	public string InmobiAccountId => "6c7ea8566bfd4b2c82269e3647046624";

	public Dictionary<AdLocationInfo, string> InmobiCacheIDs => _inmobiCacheIDs;

	public string VungleAppId => "591c39924753e6384a0002b3";

	public Dictionary<AdLocationInfo, string> VungleCacheIDs => _vungleCacheIDs;

	public string SupersonicAppId => "4a25f10d";

	public Dictionary<AdLocationInfo, string> SupersonicCacheIDs => _supersonicCacheIDs;

	public Dictionary<AdLocationInfo, string> FacebookCacheIDs => _facebookCacheIDs;

	public Dictionary<AdLocationInfo, string> MobVistaCacheIDs => _mobvistaCacheIDs;

	public string MobVistaAppId => "101366";

	public string MobVistaAppSdkKey => "f51fde5b5c666de31358328df8b3141a";

	public string MobVistaEventName => "Subway iOS home_videos";
}
