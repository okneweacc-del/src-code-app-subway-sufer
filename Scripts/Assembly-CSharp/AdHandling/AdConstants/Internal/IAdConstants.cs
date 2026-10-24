using System.Collections.Generic;
using Kiloo.Common.Ads;

namespace AdHandling.AdConstants.Internal;

public interface IAdConstants
{
	bool isRestricted { get; }

	string AdColonyAppId { get; }

	Dictionary<AdLocationInfo, string> AdColonyCacheIDs { get; }

	string TapJoyAppId { get; }

	string TapJoyAppKey { get; }

	Dictionary<AdLocationInfo, string> TapJoyCacheIDs { get; }

	string ChartboostAppId { get; }

	string ChartboostAppSignature { get; }

	Dictionary<AdLocationInfo, string> ChartboostCacheIDs { get; }

	string InmobiAccountId { get; }

	Dictionary<AdLocationInfo, string> InmobiCacheIDs { get; }

	string VungleAppId { get; }

	Dictionary<AdLocationInfo, string> VungleCacheIDs { get; }

	string SupersonicAppId { get; }

	Dictionary<AdLocationInfo, string> SupersonicCacheIDs { get; }

	Dictionary<AdLocationInfo, string> FacebookCacheIDs { get; }

	Dictionary<AdLocationInfo, string> MobVistaCacheIDs { get; }

	string MobVistaAppId { get; }

	string MobVistaAppSdkKey { get; }
}
