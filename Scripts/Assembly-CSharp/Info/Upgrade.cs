namespace Info;

public class Upgrade
{
	public string name;

	public string nameTwoLines;

	public string description;

	public string mysteryBoxDescription;

	public int numberOfTiers;

	public float[] durations;

	public float speed;

	public float landSpeed;

	public int spawnProbability;

	public int[] pricesRaw;

	public PurchaseData.UnlockType priceType = PurchaseData.UnlockType.Coins;

	public string iconName;

	public int weight;

	public string GetName()
	{
		if (!string.IsNullOrEmpty(Strings.Get(nameTwoLines)))
		{
			return nameTwoLines;
		}
		return name;
	}

	public int GetPrice(int tier)
	{
		if (pricesRaw == null)
		{
			return -1;
		}
		return pricesRaw[tier];
	}
}
