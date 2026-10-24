using Kiloo.Common.Ads;
using Kiloo.Monetization.AdBridgeCommon;
using UnityEngine;

namespace Kiloo.Monetization.Vungle.Internal;

public class VungleiOS : VungleAdBridgeBase
{
	private const string CALLBACK_RECEIVER_NAME = "VungleIOSReceiver";

	protected override RuntimePlatform platform => RuntimePlatform.IPhonePlayer;

	protected override IPlatformEventInvoker CreateAdBridgeEventInvoker()
	{
		return new IosEventInvoker(_providerId);
	}

	public static VungleiOS CreatePlugin(AbstractAdProvider adProvider)
	{
		return AdBridgeBase.CreatePlugin<VungleiOS>("VungleIOSReceiver", adProvider);
	}
}
