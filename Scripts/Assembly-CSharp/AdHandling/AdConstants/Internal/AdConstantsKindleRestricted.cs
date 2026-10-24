using System.Collections.Generic;
using Kiloo.Common.Ads;

namespace AdHandling.AdConstants.Internal;

public class AdConstantsKindleRestricted : IAdConstants
{
	private readonly Dictionary<AdLocationInfo, string> _adColonyCacheIDs = new Dictionary<AdLocationInfo, string>
	{
		{
			AdLocations.POOL1_VIDEOS,
			"vz7810cf7bb5cb403caa"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"vzff9597780fcb4741a9"
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
			"SUBWAY_SURFERS_KINDLE_COPPA_VIDEOS_POOL1_VIDEOS-2934722"
		},
		{
			AdLocations.POOL2_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_COPPA_VIDEOS_POOL2_VIDEOS-6124258"
		},
		{
			AdLocations.POOL3_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_COPPA_VIDEOS_POOL3_VIDEOS-3614054"
		},
		{
			AdLocations.HOME_VIDEOS,
			"SUBWAYS47364"
		},
		{
			AdLocations.EARN_COINS_VIDEOS,
			"SUBWAYS37026"
		},
		{
			AdLocations.DOUBLE_UP_VIDEOS,
			"SUBWAYS74690"
		},
		{
			AdLocations.WORDY_WEEKEND_VIDEOS,
			"SUBWAYS16717"
		},
		{
			AdLocations.HOVERBOARD_FREE_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_COPPA_VIDEOS_HOVERBOARD_FREE_VIDEOS-6929793"
		},
		{
			AdLocations.HOVERBOARD_POPUP_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_COPPA_VIDEOS_HOVERBOARD_POPUP_VIDEOS-8325440"
		},
		{
			AdLocations.HOVERBOARD_RUN_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_COPPA_VIDEOS_HOVERBOARD_RUN_VIDEOS-0598334"
		},
		{
			AdLocations.SKIP_MISSION_PAUSE_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_COPPA_VIDEOS_SKIP_MISSIONS_PAUSE_VIDEOS-0013205"
		},
		{
			AdLocations.SKIP_MISSION_TOUR_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_COPPA_VIDEOS_SKIP_MISSIONS_TOUR_VIDEOS-6073117"
		},
		{
			AdLocations.REVIVAL_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_COPPA_VIDEOS_REVIVE_VIDEOS-7711945"
		},
		{
			AdLocations.GAMEOVER_DOUBLE_COINS_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_COPPA_VIDEOS_DOUBLE_COINS_VIDEOS-1547872"
		},
		{
			AdLocations.FREE_COINS_FREE_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_COPPA_VIDEOS_FREE_COINS_FREE_VIDEOS-1596219"
		},
		{
			AdLocations.FREE_COINS_POPUP_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_COPPA_VIDEOS_FREE_COINS_POPUP_VIDEOS-5088993"
		},
		{
			AdLocations.HEADSTART_FREE_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_COPPA_VIDEOS_HEADSTART_FREE_VIDEOS-5923269"
		},
		{
			AdLocations.HEADSTART_RUN_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_COPPA_VIDEOS_HEADSTART_RUN_VIDEOS-4058891"
		},
		{
			AdLocations.SCOREBOOSTER_FREE_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_COPPA_VIDEOS_SCOREBOOSTER_FREE_VIDEOS-1978968"
		},
		{
			AdLocations.SCOREBOOSTER_RUN_VIDEOS,
			"SUBWAY_SURFERS_KINDLE_COPPA_VIDEOS_SCOREBOOSTER_RUN_VIDEOS-2571430"
		}
	};

	private readonly Dictionary<AdLocationInfo, string> _supersonicCacheIDs = new Dictionary<AdLocationInfo, string>();

	private readonly Dictionary<AdLocationInfo, string> _facebookCacheIDs = new Dictionary<AdLocationInfo, string>();

	public bool isRestricted => true;

	public string AdColonyAppId => "app60a5688ed75a4ef595";

	public Dictionary<AdLocationInfo, string> AdColonyCacheIDs => _adColonyCacheIDs;

	public string TapJoyAppId => string.Empty;

	public string TapJoyAppKey => string.Empty;

	public Dictionary<AdLocationInfo, string> TapJoyCacheIDs => _tapjoyCacheIDs;

	public string ChartboostAppId => "5922afa143150f6edddd6c23";

	public string ChartboostAppSignature => "bbf5160efc2c49b4fc3e7d22fb9272f5eb3768df";

	public Dictionary<AdLocationInfo, string> ChartboostCacheIDs => _chartboostCacheIDs;

	public string InmobiAccountId => "6c7ea8566bfd4b2c82269e3647046624";

	public Dictionary<AdLocationInfo, string> InmobiCacheIDs => _inmobiCacheIDs;

	public string VungleAppId => "5a3a17ab7f1834bc71006e8a";

	public Dictionary<AdLocationInfo, string> VungleCacheIDs => _vungleCacheIDs;

	public string SupersonicAppId => string.Empty;

	public Dictionary<AdLocationInfo, string> SupersonicCacheIDs => _supersonicCacheIDs;

	public Dictionary<AdLocationInfo, string> FacebookCacheIDs => _facebookCacheIDs;

	public Dictionary<AdLocationInfo, string> MobVistaCacheIDs => new Dictionary<AdLocationInfo, string>();

	public string MobVistaAppId => string.Empty;

	public string MobVistaAppSdkKey => string.Empty;
}
