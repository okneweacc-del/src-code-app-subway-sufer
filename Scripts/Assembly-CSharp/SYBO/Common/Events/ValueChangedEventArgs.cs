using System;

namespace SYBO.Common.Events;

public class ValueChangedEventArgs : EventArgs
{
	private float _newValue;

	private float _oldValue;

	private float _deltaValue;

	public float NewValue => _newValue;

	public float OldValue => _oldValue;

	public float DeltaValue => _deltaValue;

	public ValueChangedEventArgs(float newValue, float oldValue, float deltaValue)
	{
		_newValue = newValue;
		_oldValue = oldValue;
		_deltaValue = deltaValue;
	}
}
