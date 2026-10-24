using System;
using SYBO.Common.Exceptions;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Subway.Inputs;

[PrefabPath("Prefabs/Managers/InputManager")]
public class InputManager : SingletonMonoBehaviour<InputManager>
{
	[SerializeField]
	private Game.SwipeInfo _swipeInfo;

	private bool _isEnabled = true;

	private InputEventArgs _previousInputEventArgs;

	private InputEventArgs _currentInputEventArgs;

	private Swipe _currentSwipe;

	private float _lastTapTime = float.MinValue;

	private Camera __gameCamera;

	private Camera __menuCamera;

	private int _lastInputFrame;

	public bool IsEnabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			if (_isEnabled != value)
			{
				_isEnabled = value;
			}
		}
	}

	public InputEventArgs PreviousInputEventArgs => _previousInputEventArgs;

	public InputEventArgs CurrentInputEventArgs => _currentInputEventArgs;

	public bool WasButtonClicked { get; set; }

	private Camera _gameCamera
	{
		get
		{
			if (__gameCamera == null)
			{
				__gameCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
			}
			return __gameCamera;
		}
	}

	private Camera _menuCamera
	{
		get
		{
			if (__menuCamera == null)
			{
				__menuCamera = GameObject.Find("1normal 2D Camera").GetComponent<Camera>();
			}
			return __menuCamera;
		}
	}

	public event EventHandler<InputEventArgs> OnInputEvent;

	protected void Start()
	{
		_currentInputEventArgs = new InputEventArgs(InputType.None, KeyCode.None);
		_previousInputEventArgs = new InputEventArgs(InputType.None, KeyCode.None);
	}

	protected void Update()
	{
		if (!_isEnabled || (Game.Instance.IsPaused && !Game.Instance.IsInGame.Value))
		{
			_currentSwipe = null;
			return;
		}
		HandleInput();
		HandleControls();
	}

	private void ProcessKeyCode(KeyCode keyCode)
	{
		if ((_previousInputEventArgs.InputType == InputType.KeyDownEnter || _previousInputEventArgs.InputType == InputType.KeyDownStay) && _previousInputEventArgs.KeyCode == keyCode && Input.GetKey(keyCode))
		{
			RaiseOnInputEvent(new InputEventArgs(InputType.KeyDownStay, keyCode));
		}
		if (Input.GetKeyDown(keyCode))
		{
			RaiseOnInputEvent(new InputEventArgs(InputType.KeyDownEnter, keyCode));
		}
		if (Input.GetKeyUp(keyCode))
		{
			RaiseOnInputEvent(new InputEventArgs(InputType.KeyDownExit, keyCode));
		}
	}

	private void HandleControls()
	{
		if (Time.frameCount == _lastInputFrame)
		{
			return;
		}
		_lastInputFrame = Time.frameCount;
		if (Input.touchCount <= 0)
		{
			return;
		}
		Touch touch = Input.touches[0];
		if (touch.phase == TouchPhase.Began)
		{
			_currentSwipe = new Swipe();
			_currentSwipe.start = touch.position;
			_currentSwipe.startTime = Time.time;
		}
		if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && _currentSwipe != null)
		{
			_currentSwipe.endTime = Time.time;
			_currentSwipe.end = touch.position;
			SwipeDir swipeDir = AnalyzeSwipe(_currentSwipe);
			if (swipeDir != SwipeDir.None)
			{
				ConvertSwipeToInputEventAndRaise(_currentSwipe, swipeDir);
				_currentSwipe = null;
			}
		}
		if (touch.phase == TouchPhase.Ended && _currentSwipe != null)
		{
			_currentSwipe.endTime = Time.time;
			_currentSwipe.end = touch.position;
			if (AnalyzeSwipe(_currentSwipe) == SwipeDir.None)
			{
				HandleTap();
			}
		}
	}

	private SwipeDir AnalyzeSwipe(Swipe nextSwipe)
	{
		Camera camera = _gameCamera;
		if (camera == null)
		{
			camera = _menuCamera;
		}
		SwipeDir result = SwipeDir.None;
		if (camera != null)
		{
			Vector3 b = camera.ScreenToWorldPoint(new Vector3(nextSwipe.start.x, nextSwipe.start.y, 2f));
			if (Vector3.Distance(camera.ScreenToWorldPoint(new Vector3(nextSwipe.end.x, nextSwipe.end.y, 2f)), b) < _swipeInfo.distanceMin)
			{
				result = SwipeDir.None;
			}
			else
			{
				Vector3 lhs = nextSwipe.end - nextSwipe.start;
				float num = 0f;
				float num2 = Vector3.Dot(lhs, Vector3.up);
				if (num2 > num)
				{
					num = num2;
					result = SwipeDir.Up;
				}
				num2 = Vector3.Dot(lhs, Vector3.down);
				if (num2 > num)
				{
					num = num2;
					result = SwipeDir.Down;
				}
				num2 = Vector3.Dot(lhs, Vector3.left);
				if (num2 > num)
				{
					num = num2;
					result = SwipeDir.Left;
				}
				num2 = Vector3.Dot(lhs, Vector3.right);
				if (num2 > num)
				{
					result = SwipeDir.Right;
				}
			}
		}
		return result;
	}

	private bool HandleTap()
	{
		bool result = false;
		if (Time.time < _lastTapTime + _swipeInfo.doubleTapDuration)
		{
			if (!WasButtonClicked)
			{
				RaiseOnInputEvent(new InputEventArgs(InputType.FingerDoubleTap));
				result = true;
			}
		}
		else if (!WasButtonClicked)
		{
			RaiseOnInputEvent(new InputEventArgs(InputType.FingerTap));
		}
		WasButtonClicked = false;
		_lastTapTime = Time.time;
		return result;
	}

	private void ConvertSwipeToInputEventAndRaise(Swipe swipe, SwipeDir swipeDir)
	{
		InputType inputType = InputType.None;
		switch (swipeDir)
		{
		case SwipeDir.Up:
			inputType = InputType.SwipeUp;
			break;
		case SwipeDir.Down:
			inputType = InputType.SwipeDown;
			break;
		case SwipeDir.Left:
			inputType = InputType.SwipeLeft;
			break;
		case SwipeDir.Right:
			inputType = InputType.SwipeRight;
			break;
		default:
			SwitchDefaultException.Throw(swipeDir.ToString());
			break;
		}
		RaiseOnInputEvent(new InputEventArgs(inputType));
	}

	protected override void SubclassInitialize()
	{
		HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
	}

	public void SendInputEvent(InputEventArgs inputEventArgs)
	{
		RaiseOnInputEvent(inputEventArgs);
	}

	private void RaiseOnInputEvent(InputEventArgs inputEventArgs)
	{
		_currentInputEventArgs = inputEventArgs;
		if (OnInputEvent != null)
		{
			OnInputEvent(this, _currentInputEventArgs);
		}
		_previousInputEventArgs = _currentInputEventArgs;
	}

	private void HandleInput()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
		{
			RaiseOnInputEvent(new InputEventArgs(InputType.SwipeUp));
		}
		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
		{
			RaiseOnInputEvent(new InputEventArgs(InputType.SwipeDown));
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
		{
			RaiseOnInputEvent(new InputEventArgs(InputType.SwipeLeft));
		}
		if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
		{
			RaiseOnInputEvent(new InputEventArgs(InputType.SwipeRight));
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			RaiseOnInputEvent(new InputEventArgs(InputType.FingerDoubleTap));
		}
		if (Input.GetKeyDown(KeyCode.Q) && Game.Instance != null && Game.Instance.pogoJumpsLeft == 0 && Game.Instance.Character.IsGrounded.Value)
		{
			Game.Instance.PickupPogostick();
		}
	}
}
