using System;
using Kiloo.Common.Extensions;

namespace Cloud.Internal;

public static class CloudSaveDecorator
{
	public static string GetName(CloudSaveComparisonElement id)
	{
		return id switch
		{
			CloudSaveComparisonElement.Multiplier => Strings.Get("CLOUDSAVE_ADVANCED_MULTIPLIER"), 
			CloudSaveComparisonElement.Highscore => Strings.Get("CLOUDSAVE_ADVANCED_HIGHSCORE"), 
			CloudSaveComparisonElement.Keys => Strings.Get("CLOUDSAVE_ADVANCED_KEYS"), 
			CloudSaveComparisonElement.Upgrades => Strings.Get("CLOUDSAVE_ADVANCED_UPGRADES"), 
			CloudSaveComparisonElement.Awards => Strings.Get("CLOUDSAVE_ADVANCED_AWARDS"), 
			CloudSaveComparisonElement.Characters => Strings.Get("CLOUDSAVE_ADVANCED_CHARACTERS"), 
			CloudSaveComparisonElement.Boards => Strings.Get("CLOUDSAVE_ADVANCED_BOARDS"), 
			CloudSaveComparisonElement.Outfits => Strings.Get("CLOUDSAVE_ADVANCED_OUTFITS"), 
			CloudSaveComparisonElement.BoardThemes => Strings.Get("CLOUDSAVE_ADVANCED_BOARD_VARIATIONS"), 
			CloudSaveComparisonElement.LastModifiedTime => Strings.Get("CLOUDSAVE_ADVANCED_SAVED_LAST"), 
			CloudSaveComparisonElement.DeviceSource => Strings.Get("CLOUDSAVE_ADVANCED_SAVED_DEVICE"), 
			_ => string.Empty, 
		};
	}

	public static string GetIconName(CloudSaveComparisonElement id)
	{
		return id switch
		{
			CloudSaveComparisonElement.Multiplier => "ic_multiplier", 
			CloudSaveComparisonElement.Highscore => "ic_highscore", 
			CloudSaveComparisonElement.Keys => "ic_keys", 
			CloudSaveComparisonElement.Upgrades => "ic_upgrades", 
			CloudSaveComparisonElement.Awards => "ic_awards", 
			CloudSaveComparisonElement.Characters => "ic_character", 
			CloudSaveComparisonElement.Boards => "ic_boards", 
			CloudSaveComparisonElement.Outfits => "ic_character_skins", 
			CloudSaveComparisonElement.BoardThemes => "ic_boards_flavour", 
			CloudSaveComparisonElement.LastModifiedTime => "ic_saved", 
			_ => string.Empty, 
		};
	}

	public static string FormatDateTime(DateTime dt)
	{
		if (dt == DateTime.MinValue)
		{
			return "-";
		}
		return string.Format(Strings.Get("CLOUDSAVE_STATS_SAVED_DAYS_AGO"), (DateTime.UtcNow - dt).NicifyTimeString());
	}

	public static string FormatValue(CloudSaveComparisonElement id, float value)
	{
		switch (id)
		{
		case CloudSaveComparisonElement.Multiplier:
			return "x{0:N0}".Format(value);
		case CloudSaveComparisonElement.Highscore:
			return "{0:N0}".Format(value);
		case CloudSaveComparisonElement.Keys:
			if (Math.Abs(value) < 0.1f)
			{
				return "-";
			}
			return "{0:N0}".Format(value);
		case CloudSaveComparisonElement.Upgrades:
			if (Math.Abs(value) < 0.1f)
			{
				return "-";
			}
			return "{0:N0}".Format(value);
		case CloudSaveComparisonElement.Awards:
			if (Math.Abs(value) < 0.1f)
			{
				return "-";
			}
			return "{0:N0} %".Format(value);
		case CloudSaveComparisonElement.Characters:
			return "{0:N0}".Format(value);
		case CloudSaveComparisonElement.Boards:
			if (Math.Abs(value) < 0.1f)
			{
				return "-";
			}
			return "{0:N0}".Format(value);
		case CloudSaveComparisonElement.Outfits:
			if (Math.Abs(value) < 0.1f)
			{
				return "-";
			}
			return "{0:N0}".Format(value);
		case CloudSaveComparisonElement.BoardThemes:
			if (Math.Abs(value) < 0.1f)
			{
				return "-";
			}
			return "{0:N0}".Format(value);
		default:
			return string.Empty;
		}
	}

	public static string FormatValueWithTitle(CloudSaveComparisonElement id, float value)
	{
		return id switch
		{
			CloudSaveComparisonElement.Multiplier => "x{0:N0} {1}".Format(value, GetName(id)), 
			CloudSaveComparisonElement.Highscore => "{0:N0}".Format(value), 
			CloudSaveComparisonElement.Keys => "{0:N0} {1}".Format(value, GetName(id)), 
			CloudSaveComparisonElement.Upgrades => "{0:N0} {1}".Format(value, GetName(id)), 
			CloudSaveComparisonElement.Awards => "{0:N0} % {1}".Format(value, GetName(id)), 
			CloudSaveComparisonElement.Characters => "{0:N0} {1}".Format(value, GetName(id)), 
			CloudSaveComparisonElement.Boards => "{0:N0} {1}".Format(value, GetName(id)), 
			CloudSaveComparisonElement.Outfits => "{0:N0} {1}".Format(value, GetName(id)), 
			CloudSaveComparisonElement.BoardThemes => "{0:N0} {1}".Format(value, GetName(id)), 
			_ => string.Empty, 
		};
	}
}
