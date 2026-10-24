using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics;

public class AccountCreatedEvent : BaseSessionEvent
{
	[SerializeField]
	[SerializedAs("device_id")]
	[SerializedSizeLimit(44, false)]
	protected string _deviceID;

	[SerializeField]
	[SerializedAs("sampling_key")]
	protected float _samplingKey;

	public string DeviceID => _deviceID;

	public float SamplingKey => _samplingKey;

	public override string EventName => "account_created";

	public AccountCreatedEvent()
	{
		_deviceID = BaseSessionEvent.GetDeviceID();
		_samplingKey = AnalyticsEvent.GetSamplingKey();
		_gameVersion = AnalyticsEvent.GetGameVersion();
	}
}
