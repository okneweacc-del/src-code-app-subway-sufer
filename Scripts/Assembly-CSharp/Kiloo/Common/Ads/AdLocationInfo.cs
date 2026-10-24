namespace Kiloo.Common.Ads;

public class AdLocationInfo
{
	public readonly string id;

	public readonly AdType adType;

	public readonly bool isPoolLocation;

	public AdLocationInfo(string id, AdType adType, bool isPoolLocation)
	{
		this.id = id;
		this.adType = adType;
		this.isPoolLocation = isPoolLocation;
	}
}
