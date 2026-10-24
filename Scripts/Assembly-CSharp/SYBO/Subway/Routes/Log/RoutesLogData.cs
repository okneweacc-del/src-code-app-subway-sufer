using System;
using UnityEngine;

namespace SYBO.Subway.Routes.Log;

[Serializable]
public class RoutesLogData
{
	private float _timeStamp;

	private RoutesLogType _routesLogType;

	private object[] _args;

	protected string _message = string.Empty;

	public float TimeStamp => _timeStamp;

	public RoutesLogType RoutesLogType => _routesLogType;

	public object[] Args => _args;

	public string Message => _message;

	public RoutesLogData(RoutesLogType pickupLogType, object[] args)
	{
		_routesLogType = pickupLogType;
		_args = args;
		_timeStamp = Time.timeSinceLevelLoad;
		_message = $"[RoutesLogData (RoutesLogType={_routesLogType},TimeStamp={_timeStamp})]";
	}

	public override string ToString()
	{
		return _message;
	}
}
