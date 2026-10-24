using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Ads;
using Kiloo.Monetization.AdBridgeCommon;
using Kiloo.Monetization.Vungle.Internal;
using UnityEngine;

namespace Kiloo.Monetization.Vungle;

public class VungleAdBridgeWrapper : AdBridgeWrapper<VungleAdBridgeBase>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private VungleInitialized onInitialized__BackingField;

	public event VungleInitialized onInitialized
	{
		add
		{
			VungleInitialized vungleInitialized = onInitialized__BackingField;
			VungleInitialized vungleInitialized2;
			do
			{
				vungleInitialized2 = vungleInitialized;
				vungleInitialized = Interlocked.CompareExchange(ref onInitialized__BackingField, (VungleInitialized)Delegate.Combine(vungleInitialized2, value), vungleInitialized);
			}
			while ((object)vungleInitialized != vungleInitialized2);
		}
		remove
		{
			VungleInitialized vungleInitialized = onInitialized__BackingField;
			VungleInitialized vungleInitialized2;
			do
			{
				vungleInitialized2 = vungleInitialized;
				vungleInitialized = Interlocked.CompareExchange(ref onInitialized__BackingField, (VungleInitialized)Delegate.Remove(vungleInitialized2, value), vungleInitialized);
			}
			while ((object)vungleInitialized != vungleInitialized2);
		}
	}

	public VungleAdBridgeWrapper(AbstractAdProvider adProvider)
		: base(adProvider)
	{
	}

	protected override void SubscribeToEvents()
	{
		base.SubscribeToEvents();
		_adBridge.onInitialized += onInitialized__BackingField;
	}

	protected override VungleAdBridgeBase CreateAdBridge(AbstractAdProvider adProvider)
	{
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			return VungleiOS.CreatePlugin(adProvider);
		case RuntimePlatform.Android:
			return VungleAndroid.CreatePlugin(adProvider);
		default:
			SDebug.LogError("There's no platform bridge for the current platform: " + Application.platform);
			return null;
		}
	}
}
