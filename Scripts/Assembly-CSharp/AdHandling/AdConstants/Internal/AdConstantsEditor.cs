using System.Collections.Generic;
using Kiloo.Common.Ads;

namespace AdHandling.AdConstants.Internal;

public class AdConstantsEditor : IAdConstants
{
	private readonly Dictionary<AdLocationInfo, string> _editorCacheIDs = new Dictionary<AdLocationInfo, string>
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
			AdLocations.REVIVAL_VIDEOS,
			"revival_videos"
		},
		{
			AdLocations.BOARD_BUTTON_VIDEOS,
			"board_button_videos"
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
			"double_coins_video"
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

	public bool isRestricted => false;

	public string AdColonyAppId => string.Empty;

	public Dictionary<AdLocationInfo, string> AdColonyCacheIDs => _editorCacheIDs;

	public string TapJoyAppId => string.Empty;

	public string TapJoyAppKey => string.Empty;

	public Dictionary<AdLocationInfo, string> TapJoyCacheIDs => _editorCacheIDs;

	public string ChartboostAppId => string.Empty;

	public string ChartboostAppSignature => string.Empty;

	public Dictionary<AdLocationInfo, string> ChartboostCacheIDs => _editorCacheIDs;

	public string InmobiAccountId => string.Empty;

	public Dictionary<AdLocationInfo, string> InmobiCacheIDs => _editorCacheIDs;

	public string VungleAppId => string.Empty;

	public Dictionary<AdLocationInfo, string> VungleCacheIDs => _editorCacheIDs;

	public string SupersonicAppId => string.Empty;

	public Dictionary<AdLocationInfo, string> SupersonicCacheIDs => _editorCacheIDs;

	public Dictionary<AdLocationInfo, string> FacebookCacheIDs => _editorCacheIDs;

	public string MobVistaAppId => string.Empty;

	public string MobVistaAppSdkKey => string.Empty;

	public Dictionary<AdLocationInfo, string> MobVistaCacheIDs => _editorCacheIDs;
}
