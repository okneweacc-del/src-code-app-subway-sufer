using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using AdHandling.AdConstants.Internal;
using Info;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using UnityEngine;

namespace AdHandling.AdConstants;

public class AdConstants : IAdConstants
{
	private IAdConstants _constants;

	private static AdConstants _instance;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action constantsChanged__BackingField;

	public static AdConstants instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new AdConstants();
			}
			return _instance;
		}
	}

	public static string FlurryApiKey
	{
		get
		{
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
				{
					return "GDGAT7PY98MAEQQIWC68";
				}
				return "CIJUE322XIHTDWNV519J";
			case RuntimePlatform.Android:
				if (DeviceUtility.isKindleDevice)
				{
					if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
					{
						return "XJ5NMRG7R5WGC4BZC6XQ";
					}
					return "D5BDSJTZB2JX75Y2T4SN";
				}
				if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
				{
					return "XJ5NMRG7R5WGC4BZC6XQ";
				}
				return "YR898G65YFPWNMQ6X5H5";
			default:
				return string.Empty;
			}
		}
	}

	public bool isRestricted => _constants.isRestricted;

	public string AdColonyAppId => _constants.AdColonyAppId;

	public Dictionary<AdLocationInfo, string> AdColonyCacheIDs => _constants.AdColonyCacheIDs;

	public string TapJoyAppId => _constants.TapJoyAppId;

	public string TapJoyAppKey => _constants.TapJoyAppKey;

	public Dictionary<AdLocationInfo, string> TapJoyCacheIDs => _constants.TapJoyCacheIDs;

	public string ChartboostAppId => _constants.ChartboostAppId;

	public string ChartboostAppSignature => _constants.ChartboostAppSignature;

	public Dictionary<AdLocationInfo, string> ChartboostCacheIDs => _constants.ChartboostCacheIDs;

	public string InmobiAccountId => _constants.InmobiAccountId;

	public Dictionary<AdLocationInfo, string> InmobiCacheIDs => _constants.InmobiCacheIDs;

	public string VungleAppId => _constants.VungleAppId;

	public Dictionary<AdLocationInfo, string> VungleCacheIDs => _constants.VungleCacheIDs;

	public string SupersonicAppId => _constants.SupersonicAppId;

	public Dictionary<AdLocationInfo, string> SupersonicCacheIDs => _constants.SupersonicCacheIDs;

	public Dictionary<AdLocationInfo, string> FacebookCacheIDs => _constants.FacebookCacheIDs;

	public Dictionary<AdLocationInfo, string> MobVistaCacheIDs => _constants.MobVistaCacheIDs;

	public string MobVistaAppId => _constants.MobVistaAppId;

	public string MobVistaAppSdkKey => _constants.MobVistaAppSdkKey;

	public static event Action constantsChanged
	{
		add
		{
			Action action = constantsChanged__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref constantsChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = constantsChanged__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref constantsChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private AdConstants()
	{
		SetPlatformDependentConstants();
		AgeRestriction.restrictionChanged += SetPlatformDependentConstants;
	}

	private void SetPlatformDependentConstants()
	{
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
			{
				_constants = new AdConstantsIOS_TEST();
			}
			else if (AgeRestriction.isRestricted)
			{
				_constants = new AdConstantsIOSRestricted();
			}
			else
			{
				_constants = new AdConstantsIOS();
			}
			break;
		case RuntimePlatform.Android:
			if (DeviceUtility.isKindleDevice)
			{
				if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
				{
					_constants = new AdConstantsKindle_TEST();
				}
				else if (AgeRestriction.isRestricted)
				{
					_constants = new AdConstantsKindleRestricted();
				}
				else
				{
					_constants = new AdConstantsKindle();
				}
			}
			else if (Globals.DEBUG_USE_TEST_AD_CONSTANTS)
			{
				_constants = new AdConstantsAndroid_TEST();
			}
			else if (AgeRestriction.isRestricted)
			{
				_constants = new AdConstantsAndroidRestricted();
			}
			else
			{
				_constants = new AdConstantsAndroid();
			}
			break;
		default:
			if (AgeRestriction.isRestricted)
			{
				_constants = new AdConstantsEditorRestricted();
			}
			else
			{
				_constants = new AdConstantsEditor();
			}
			break;
		}
		constantsChanged__BackingField.SafeInvoke();
	}
}
