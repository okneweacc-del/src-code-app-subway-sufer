using System.Collections.Generic;
using Data.Save;

namespace Info;

public static class Upgrades
{
	private const int SPAWNRATE_FOR_CHARACTER_LETTERS = 25;

	private const int SPAWNRATE_TO_FORCE_A_SPAWNING = 1500000000;

	private const int SPAWNRATE_FOR_LETTERS = 1500000000;

	private const int SPAWNRATE_FOR_MYSTERYPOWERUP = 23;

	public const int SCORE_BOOSTER_MULTIPLIER_TIER1 = 5;

	public const int SCORE_BOOSTER_MULTIPLIER_TIER2 = 6;

	public const int SCORE_BOOSTER_MULTIPLIER_TIER3 = 7;

	public const int CHARACTER_DAY_MULTIPLIER = 5;

	public const int PLUS_MULTIPLIER_MYSTERY_POWERUP = 10;

	private const float HEADSTART_TIER_ONE_SPEED = 400f;

	private const float HEADSTART_TIER_ONE_LAND_SPEED = 0.01f;

	private const float HEADSTART_TIER_ONE_DURATION = 6f;

	public static readonly HeadstartTier[] HEADSTART_TIERS = new HeadstartTier[3]
	{
		new HeadstartTier
		{
			speed = 400f,
			landSpeed = 0.01f,
			duration = 6f
		},
		new HeadstartTier
		{
			speed = 550f,
			landSpeed = 0.5f,
			duration = 8f
		},
		new HeadstartTier
		{
			speed = 700f,
			landSpeed = 1f,
			duration = 10f
		}
	};

	public static UpgradeType[] consumableUpgradeTypes = new UpgradeType[5]
	{
		UpgradeType.hoverboard,
		UpgradeType.mysterybox,
		UpgradeType.tokenmysterybox,
		UpgradeType.scorebooster,
		UpgradeType.headstart2000
	};

	public static UpgradeType[] permanentUpgradeTypes = new UpgradeType[4]
	{
		UpgradeType.jetpack,
		UpgradeType.supersneakers,
		UpgradeType.coinmagnet,
		UpgradeType.doubleMultiplier
	};

	public static readonly Dictionary<UpgradeType, Upgrade> upgrades = new Dictionary<UpgradeType, Upgrade>
	{
		{
			UpgradeType.hoverboard,
			new Upgrade
			{
				name = "HOVERBOARD_TITLE",
				description = "UPGRADES_HOVERBOARD_DESCRIPTION",
				mysteryBoxDescription = "UPGRADES_HOVERBOARD_DESCRIPTION_MYSTERY",
				durations = new float[1] { 30f },
				pricesRaw = new int[1] { 300 },
				iconName = "icon_upgrades_hoverboard"
			}
		},
		{
			UpgradeType.scorebooster,
			new Upgrade
			{
				name = "UPGRADES_SCOREBOOSTER",
				description = "UPGRADES_SCOREBOOSTER_DESCRIPTION",
				mysteryBoxDescription = "UPGRADES_SCOREBOOSTER_DESCRIPTION_MYSTERY",
				pricesRaw = new int[1] { 3000 },
				iconName = "icon_upgrades_scorebooster"
			}
		},
		{
			UpgradeType.headstart2000,
			new Upgrade
			{
				name = "UPGRADES_HEADSTART",
				nameTwoLines = "UPGRADES_HEADSTART",
				mysteryBoxDescription = "UPGRADES_MEGAHEADSTART_DESCRIPTION_MYSTERY",
				durations = new float[1] { 6f },
				description = "UPGRADES_MEGAHEADSTART_DESCRIPTION",
				pricesRaw = new int[1] { 2000 },
				iconName = "icon_upgrades_headstart2000",
				speed = 400f,
				landSpeed = 0.01f
			}
		},
		{
			UpgradeType.mysterybox,
			new Upgrade
			{
				name = "UPGRADES_MYSTERY_BOX",
				description = string.Empty,
				spawnProbability = 18,
				pricesRaw = new int[1] { 500 },
				iconName = "icon_upgrades_mysteryBox"
			}
		},
		{
			UpgradeType.tokenmysterybox,
			new Upgrade
			{
				name = "TOKEN_MYSTERY_BOX_NAME",
				description = string.Empty,
				pricesRaw = new int[1] { 6 },
				priceType = PurchaseData.UnlockType.Keys,
				iconName = "icon_upgrades_tokenBox"
			}
		},
		{
			UpgradeType.jetpack,
			new Upgrade
			{
				name = "UPGRADES_JETPACK",
				description = "UPGRADES_JETPACK_DESCRIPTION",
				numberOfTiers = 7,
				durations = new float[7] { 8f, 9f, 10.5f, 12.5f, 15f, 19f, 25f },
				spawnProbability = 14,
				pricesRaw = new int[7] { 0, 500, 1500, 3000, 10000, 30000, 60000 },
				iconName = "icon_upgrades_jetpack"
			}
		},
		{
			UpgradeType.supersneakers,
			new Upgrade
			{
				name = "UPGRADES_SUPERSNEAKERS",
				description = "UPGRADES_SUPERSNEAKERS_DESCRIPTION",
				numberOfTiers = 7,
				spawnProbability = 26,
				durations = new float[7] { 10f, 11.5f, 13.4f, 15.8f, 19f, 24f, 30f },
				pricesRaw = new int[7] { 0, 500, 1500, 3000, 10000, 30000, 60000 },
				iconName = "icon_upgrades_superSneakers"
			}
		},
		{
			UpgradeType.coinmagnet,
			new Upgrade
			{
				name = "UPGRADES_COIN_MAGNET",
				description = "UPGRADES_COIN_MAGNET_DESCRIPTION",
				numberOfTiers = 7,
				durations = new float[7] { 10f, 11.5f, 13.4f, 15.8f, 19f, 24f, 30f },
				spawnProbability = 29,
				pricesRaw = new int[7] { 0, 500, 1500, 3000, 10000, 30000, 60000 },
				iconName = "icon_upgrades_coinMagnet"
			}
		},
		{
			UpgradeType.doubleMultiplier,
			new Upgrade
			{
				name = "UPGRADES_DOUBLE_MULTIPLIER",
				description = "UPGRADES_DOUBLE_MULTIPLIER_DESCRIPTION",
				numberOfTiers = 7,
				durations = new float[7] { 10f, 11.5f, 13.4f, 15.8f, 19f, 24f, 30f },
				spawnProbability = 29,
				pricesRaw = new int[7] { 0, 500, 1500, 3000, 10000, 30000, 60000 },
				iconName = "icon_upgrades_doubleScore"
			}
		},
		{
			UpgradeType.saveMeToken,
			new Upgrade
			{
				name = "UPGRADES_KEY",
				description = string.Empty,
				spawnProbability = 2,
				pricesRaw = new int[1],
				iconName = "icon_key"
			}
		},
		{
			UpgradeType.letters,
			new Upgrade
			{
				spawnProbability = 1500000000
			}
		},
		{
			UpgradeType.characterLetter,
			new Upgrade
			{
				spawnProbability = 25
			}
		},
		{
			UpgradeType.mysteryPowerup,
			new Upgrade
			{
				name = "UPGRADES_MYSTERY_POWERUP",
				numberOfTiers = 7,
				spawnProbability = 23,
				durations = new float[7] { 10f, 11.5f, 13.4f, 15.8f, 19f, 24f, 30f },
				pricesRaw = new int[7] { 0, 500, 1500, 3000, 10000, 30000, 60000 },
				iconName = "icon_upgrades_mysteryBox"
			}
		},
		{
			UpgradeType.doubleCoinsMysteryPowerup,
			new Upgrade
			{
				name = "MYESTERY_POWERUP_DOUBLE_COINS_DESCRIPTION",
				description = "MYESTERY_POWERUP_DOUBLE_COINS_DESCRIPTION",
				weight = 16,
				durations = new float[1] { 15f },
				pricesRaw = new int[1],
				iconName = "icon_doubleCoins"
			}
		},
		{
			UpgradeType.doubleJump,
			new Upgrade
			{
				name = "MYESTERY_POWERUP_DOUBLE_JUMP_DESCRIPTION",
				description = "MYESTERY_POWERUP_DOUBLE_JUMP_DESCRIPTION",
				weight = 31,
				durations = new float[1] { 15f },
				pricesRaw = new int[1],
				iconName = "icon_double_jump"
			}
		},
		{
			UpgradeType.plusMultiplier,
			new Upgrade
			{
				name = "MYESTERY_POWERUP_PLUS_MULTIPLIER_DESCRIPTION",
				description = "MYESTERY_POWERUP_PLUS_MULTIPLIER_DESCRIPTION",
				weight = 31,
				durations = new float[1] { 15f },
				pricesRaw = new int[1],
				iconName = "icon_plus_10"
			}
		}
	};

	public static float GetUpgradeDuration(UpgradeType type)
	{
		if (!upgrades.ContainsKey(type))
		{
			return 0f;
		}
		if (type == UpgradeType.hoverboard && Hoverboard.Instance.IsARewardFromMysteryPowerup)
		{
			return MysteryPowerup.GetRandomHoverboardDuration();
		}
		return upgrades[type].durations[InventorySave.GetTier(type)];
	}

	public static int CountUpgradeTiers()
	{
		int num = 0;
		num += InventorySave.GetTier(UpgradeType.jetpack);
		num += InventorySave.GetTier(UpgradeType.supersneakers);
		num += InventorySave.GetTier(UpgradeType.coinmagnet);
		return num + InventorySave.GetTier(UpgradeType.doubleMultiplier);
	}
}
