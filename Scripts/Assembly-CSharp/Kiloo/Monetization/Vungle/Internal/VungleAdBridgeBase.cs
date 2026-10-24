using System;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using Kiloo.Monetization.AdBridgeCommon;

namespace Kiloo.Monetization.Vungle.Internal;

public abstract class VungleAdBridgeBase : AdBridgeBase
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private VungleInitialized onInitialized__BackingField;

	protected override bool isLoggingEnabled => false;

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

	[UsedImplicitly]
	private void CallbackOnInitialized(string result)
	{
		Log("CallbackOnInitialized: " + result);
		OnInitialized(bool.Parse(result));
	}

	private void OnInitialized(bool success)
	{
		if (onInitialized__BackingField != null)
		{
			onInitialized__BackingField(success);
		}
	}
}
