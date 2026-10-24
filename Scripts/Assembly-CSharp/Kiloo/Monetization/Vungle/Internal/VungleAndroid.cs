using Kiloo.Common.Ads;
using Kiloo.Monetization.AdBridgeCommon;
using UnityEngine;

namespace Kiloo.Monetization.Vungle.Internal;

public class VungleAndroid : VungleAdBridgeBase
{
	private const string CALLBACK_RECEIVER_NAME = "VungleAndroidReceiver";

	private const string JAVA_BRIDGE_CLASS_PATH = "com.kiloo.vungleplugin.VungleAdBridgeEventListener";

	protected override RuntimePlatform platform => RuntimePlatform.Android;

	protected override IPlatformEventInvoker CreateAdBridgeEventInvoker()
	{
		return new AndroidEventInvoker("com.kiloo.vungleplugin.VungleAdBridgeEventListener");
	}

	protected override void InitializeAdBridge(object[] initializeParams)
	{
		_platformEventInvoker.OnInitialize(initializeParams);
	}

	public static VungleAndroid CreatePlugin(AbstractAdProvider adProvider)
	{
		return AdBridgeBase.CreatePlugin<VungleAndroid>("VungleAndroidReceiver", adProvider);
	}
}
