using System.Collections.Generic;
using Kiloo.Common.LocalizationManager;

namespace Info.Localization;

public class LocalizedOnlineSettings
{
	private readonly string _defaultLocale;

	private readonly string _combinedLocale;

	private readonly OnlineSettingsData _data;

	public string loadScreenTitle => FetchOrDefault(_data.loadScreenTitle);

	public UserTip[] userTips => FetchOrDefault(_data.userTips);

	public string latestVersionChangelog => FetchOrDefault(_data.latestVersionChangelog);

	public LocalizedOnlineSettings(OnlineSettingsData data)
	{
		_combinedLocale = Strings.language.locale.combined;
		_data = data;
		_defaultLocale = LocaleData.instance.defaultLocale.combined;
	}

	public string getLinkLocalizedTitle(string Id)
	{
		return FetchOrDefault(_data.socialLinkOverrides[Id].title);
	}

	public string getLinkLocalizedDesc(string Id)
	{
		return FetchOrDefault(_data.socialLinkOverrides[Id].desc);
	}

	public string getLinkLocalizediOSDesc(string Id)
	{
		return FetchOrDefault(_data.socialLinkOverrides[Id].iosDesc);
	}

	private T FetchOrDefault<T>(IDictionary<string, T> localizedFieldValue)
	{
		T value = default;
		if (localizedFieldValue == null)
		{
			return value;
		}
		if (!localizedFieldValue.TryGetValue(_combinedLocale, out value))
		{
			localizedFieldValue.TryGetValue(_defaultLocale, out value);
		}
		return value;
	}
}
