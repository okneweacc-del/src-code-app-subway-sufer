using System.Collections.Generic;
using Kiloo.Common.Ads;

namespace AdHandling.AdConstants.Internal;

public class AdConstantsKindle : IAdConstants
{
	private readonly Dictionary<AdLocationInfo, string> _adColonyCacheIDs = new Dictionary<AdLocationInfo, string>
	{
		{
			AdLocations.POOL1_VIDEOS,
			"vzfd751e8859984929af"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"vz999c3ffdb945400182"
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

	private readonly Dictionary<AdLocationInfo, string> _inmobiCacheIDs = new Dictionary<AdLocationInfo, string>();

	private readonly Dictionary<AdLocationInfo, string> _vungleCacheIDs = new Dictionary<AdLocationInfo, string>
	{
		{
			AdLocations.POOL1_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_POOL1_VIDEOS-2493543"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_POOL2_VIDEOS-6175492"
		},
		{
			AdLocations.POOL3_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_POOL3_VIDEOS-5822855"
		},
		{
			AdLocations.HOME_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_HOME_VIDEOS-5741016"
		},
		{
			AdLocations.EARN_COINS_VIDEOS,
			"SUBSUBWAY_SURFERS_KINDLE_VIDEOS_EARN_COINS_VIDEOS-9880690WAYS71777"
		},
		{
			AdLocations.DOUBLE_UP_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_DOUBLE_UP_VIDEOS-2208727"
		},
		{
			AdLocations.WORDY_WEEKEND_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_WORDY_WEEKEND_VIDEOS-3556617"
		},
		{
			AdLocations.HOVERBOARD_FREE_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_HOVERBOARD_FREE_VIDEOS-1925495"
		},
		{
			AdLocations.HOVERBOARD_POPUP_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_HOVERBOARD_POPUP_VIDEOS-7851222"
		},
		{
			AdLocations.HOVERBOARD_RUN_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_HOVERBOARD_RUN_VIDEOS-8870356"
		},
		{
			AdLocations.SKIP_MISSION_PAUSE_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_SKIP_MISSIONS_PAUSE_VIDEOS-3450690"
		},
		{
			AdLocations.SKIP_MISSION_TOUR_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_SKIP_MISSIONS_TOUR_VIDEOS-7084311"
		},
		{
			AdLocations.REVIVAL_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_REVIVE_VIDEOS-1503225"
		},
		{
			AdLocations.GAMEOVER_DOUBLE_COINS_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_DOUBLE_COINS_VIDEOS-9493499"
		},
		{
			AdLocations.FREE_COINS_FREE_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_FREE_COINS_FREE_VIDEOS-8292407"
		},
		{
			AdLocations.FREE_COINS_POPUP_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_FREE_COINS_POPUP_VIDEOS-7590827"
		},
		{
			AdLocations.HEADSTART_FREE_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_HEADSTART_FREE_VIDEOS-0540191"
		},
		{
			AdLocations.HEADSTART_RUN_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_HEADSTART_RUN_VIDEOS-2889878"
		},
		{
			AdLocations.SCOREBOOSTER_FREE_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_SCOREBOOSTER_FREE_VIDEOS-0470254"
		},
		{
			AdLocations.SCOREBOOSTER_RUN_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_VIDEOS_SCOREBOOSTER_RUN_VIDEOS-7864411"
		}
	};

	private readonly Dictionary<AdLocationInfo, string> _supersonicCacheIDs = new Dictionary<AdLocationInfo, string>();

	private readonly Dictionary<AdLocationInfo, string> _facebookCacheIDs = new Dictionary<AdLocationInfo, string>();

	public bool isRestricted => false;

	public string AdColonyAppId => "appdcc8fa2442444dfbad";

	public Dictionary<AdLocationInfo, string> AdColonyCacheIDs => _adColonyCacheIDs;

	public string TapJoyAppId => string.Empty;

	public string TapJoyAppKey => string.Empty;

	public Dictionary<AdLocationInfo, string> TapJoyCacheIDs => _tapjoyCacheIDs;

	public string ChartboostAppId => "5b1a3203bd37ad0aaef02d69";

	public string ChartboostAppSignature => "245cb01954dbefe6b7f160ae14a948188bbb38fa";

	public Dictionary<AdLocationInfo, string> ChartboostCacheIDs => _chartboostCacheIDs;

	public string InmobiAccountId => "6c7ea8566bfd4b2c82269e3647046624";

	public Dictionary<AdLocationInfo, string> InmobiCacheIDs => _inmobiCacheIDs;

	public string VungleAppId => "5b07dbce3b5f67265c7e5f76";

	public Dictionary<AdLocationInfo, string> VungleCacheIDs => _vungleCacheIDs;

	public string SupersonicAppId => string.Empty;

	public Dictionary<AdLocationInfo, string> SupersonicCacheIDs => _supersonicCacheIDs;

	public Dictionary<AdLocationInfo, string> FacebookCacheIDs => _facebookCacheIDs;

	public Dictionary<AdLocationInfo, string> MobVistaCacheIDs => new Dictionary<AdLocationInfo, string>();

	public string MobVistaAppId => string.Empty;

	public string MobVistaAppSdkKey => string.Empty;
}
