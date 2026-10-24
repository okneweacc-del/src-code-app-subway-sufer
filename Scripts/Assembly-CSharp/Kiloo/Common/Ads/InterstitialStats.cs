using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Kiloo.Common.Ads;

public class InterstitialStats
{
	public Dictionary<string, string> listVersionForID = new Dictionary<string, string>();

	public int seenThisHour { get; set; }

	public int currentHour { get; set; }

	public int seenThisDay { get; set; }

	public int currentDay { get; set; }

	public bool hasSeenFirstInterstitial { get; set; }

	private int actualCurrentHour => (int)(DateTime.UtcNow - DateTime.MinValue).TotalHours;

	private int actualCurrentDay => (int)(DateTime.UtcNow - DateTime.MinValue).TotalDays;

	[UsedImplicitly]
	public InterstitialStats()
	{
	}

	public bool ReachedLimit(IAdSettingsProvider settings)
	{
		return ReachedHourlyLimit(settings) || ReachedDailyLimit(settings);
	}

	public bool ReachedHourlyLimit(IAdSettingsProvider settings)
	{
		CheckTime();
		return seenThisHour >= settings.GetMaxInterstitialImpressionsPerHour();
	}

	public bool ReachedDailyLimit(IAdSettingsProvider settings)
	{
		CheckTime();
		return seenThisDay >= settings.GetMaxInterstitialImpressionsPerDay();
	}

	public bool HasVersionChangedForList(string waterfallID, string newVersion)
	{
		bool flag = true;
		if (listVersionForID.TryGetValue(waterfallID, out var value))
		{
			flag = value != newVersion;
		}
		if (flag)
		{
			listVersionForID[waterfallID] = newVersion;
		}
		return flag;
	}

	private void CheckTime()
	{
		if (currentDay != actualCurrentDay)
		{
			seenThisDay = 1;
			currentDay = actualCurrentDay;
		}
		if (currentHour != actualCurrentHour)
		{
			seenThisHour = 1;
			currentHour = actualCurrentHour;
		}
	}

	public void IncrementShowCount()
	{
		seenThisHour++;
		seenThisDay++;
		if (!hasSeenFirstInterstitial)
		{
			hasSeenFirstInterstitial = true;
		}
	}
}
