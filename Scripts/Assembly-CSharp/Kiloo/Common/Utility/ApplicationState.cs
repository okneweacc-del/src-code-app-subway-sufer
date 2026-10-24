using System;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;

namespace Kiloo.Common.Utility;

public class ApplicationState : MonoBehaviour
{
	private static ApplicationState _instance;

	private static bool _isQuitting;

	private Action _resumed;

	private Action _paused;

	private Action _focused;

	private Action _unfocused;

	private Action _quit;

	private bool _isPaused;

	private bool _hasFocus = true;

	public static bool isPaused
	{
		get
		{
			ApplicationState instance = GetInstance();
			return instance != null && instance._isPaused;
		}
	}

	public static bool hasFocus
	{
		get
		{
			ApplicationState instance = GetInstance();
			return instance != null && instance._hasFocus;
		}
	}

	public static bool isQuitting => GetInstance() == null;

	public static event Action paused
	{
		add
		{
			ApplicationState instance = GetInstance();
			if (instance != null)
			{
				instance._paused = (Action)Delegate.Combine(instance._paused, value);
			}
		}
		remove
		{
			ApplicationState instance = GetInstance();
			if (instance != null)
			{
				instance._paused = (Action)Delegate.Remove(instance._paused, value);
			}
		}
	}

	public static event Action resumed
	{
		add
		{
			ApplicationState instance = GetInstance();
			if (instance != null)
			{
				instance._resumed = (Action)Delegate.Combine(instance._resumed, value);
			}
		}
		remove
		{
			ApplicationState instance = GetInstance();
			if (instance != null)
			{
				instance._resumed = (Action)Delegate.Remove(instance._resumed, value);
			}
		}
	}

	public static event Action focused
	{
		add
		{
			ApplicationState instance = GetInstance();
			if (instance != null)
			{
				instance._focused = (Action)Delegate.Combine(instance._focused, value);
			}
		}
		remove
		{
			ApplicationState instance = GetInstance();
			if (instance != null)
			{
				instance._focused = (Action)Delegate.Remove(instance._focused, value);
			}
		}
	}

	public static event Action unfocused
	{
		add
		{
			ApplicationState instance = GetInstance();
			if (instance != null)
			{
				instance._unfocused = (Action)Delegate.Combine(instance._unfocused, value);
			}
		}
		remove
		{
			ApplicationState instance = GetInstance();
			if (instance != null)
			{
				instance._unfocused = (Action)Delegate.Remove(instance._unfocused, value);
			}
		}
	}

	public static event Action quit
	{
		add
		{
			ApplicationState instance = GetInstance();
			if (instance != null)
			{
				instance._quit = (Action)Delegate.Combine(instance._quit, value);
			}
		}
		remove
		{
			ApplicationState instance = GetInstance();
			if (instance != null)
			{
				instance._quit = (Action)Delegate.Remove(instance._quit, value);
			}
		}
	}

	private static ApplicationState GetInstance()
	{
		if (!Application.isPlaying)
		{
			throw new Exception("Can only be used in play mode");
		}
		if (_instance == null)
		{
			if (Application.isEditor)
			{
				ApplicationState applicationState;
				while ((applicationState = UnityEngine.Object.FindObjectOfType<ApplicationState>()) != null)
				{
					UnityEngine.Object.DestroyImmediate(applicationState.gameObject);
				}
			}
			if (_isQuitting)
			{
				LogWarning("Cannot create instance while quitting");
			}
			else
			{
				_instance = new GameObject(typeof(ApplicationState).Name).AddComponent<ApplicationState>();
				UnityEngine.Object.DontDestroyOnLoad(_instance.gameObject);
				_instance.gameObject.hideFlags = HideFlags.DontSave;
			}
		}
		return _instance;
	}

	[UsedImplicitly]
	private void OnApplicationPause(bool paused)
	{
		_isPaused = paused;
		((!_isPaused) ? _resumed : _paused)?.Invoke();
	}

	[UsedImplicitly]
	private void OnApplicationFocus(bool inFocus)
	{
		_hasFocus = inFocus;
		((!_hasFocus) ? _unfocused : _focused)?.Invoke();
	}

	[UsedImplicitly]
	private void OnApplicationQuit()
	{
		_isQuitting = true;
		_quit?.Invoke();
		if (Application.isEditor)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
	}

	private static void LogWarning(string msg)
	{
		Debug.LogWarning("[ApplicationState] " + msg);
	}

	[Conditional("APPLICATION_STATE_DEBUG_LOGS")]
	private static void Log(string msg)
	{
		Debug.Log("[ApplicationState] " + msg);
	}
}
