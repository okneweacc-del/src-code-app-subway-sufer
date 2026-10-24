using System;
using UnityEngine;

namespace SYBO.Subway.Inputs;

public class InputEventArgs : EventArgs
{
	private InputType _inputType;

	private float _timeStamp;

	private KeyCode _keyCode;

	public InputType InputType => _inputType;

	public float TimeStamp => _timeStamp;

	public KeyCode KeyCode => _keyCode;

	public InputEventArgs(InputType inputType)
	{
		_inputType = inputType;
		_keyCode = KeyCode.None;
		_timeStamp = Time.timeSinceLevelLoad;
	}

	public InputEventArgs(InputType inputType, KeyCode keyCode)
	{
		_inputType = inputType;
		_keyCode = keyCode;
		_timeStamp = Time.timeSinceLevelLoad;
	}

	public override string ToString()
	{
		return $"[InputEventArgs: InputType={InputType}, TimeStamp={TimeStamp}, KeyCode={KeyCode}]";
	}
}
