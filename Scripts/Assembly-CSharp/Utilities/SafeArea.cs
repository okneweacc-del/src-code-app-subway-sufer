using Kiloo.Common.Extensions;
using UnityEngine;

namespace Utilities;

public static class SafeArea
{
	private const int PADDING_MINIMIZATION_TOP = 4;

	private const int PADDING_MINIMIZATION_BOTTOM = 11;

	private static int _offsetTop;

	private static int _offsetBottom;

	private static bool _isRestricted;

	private static bool _hasCheckedOnce;

	public static int OFFSET_TOP
	{
		get
		{
			VerifyHasAssignedOffsets();
			return _offsetTop;
		}
	}

	public static int OFFSET_BOTTOM
	{
		get
		{
			VerifyHasAssignedOffsets();
			return _offsetBottom;
		}
	}

	public static bool IsRestricted => CheckForSafeAreaRestrictions();

	private static bool CheckForSafeAreaRestrictions()
	{
		if (!_hasCheckedOnce)
		{
			_isRestricted = false;
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
			{
				if (Screen.height == 2436 && Screen.width == 1125)
				{
					SetOffsets(out _isRestricted);
				}
				else if (Screen.height == 2688 && Screen.width == 1242)
				{
					SetOffsets(out _isRestricted);
				}
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				SetOffsets(out _isRestricted);
			}
			_hasCheckedOnce = true;
		}
		return _isRestricted;
	}

	private static void SetOffsets(out bool restrictionsWereDetected)
	{
		float num = IOSViewController.GetTopInsetInPoints();
		float num2 = IOSViewController.GetBottomInsetInPoints();
		if (num > 0f || num2 > 0f)
		{
			restrictionsWereDetected = true;
			float num3 = IOSViewController.GetDeviceHeightInPoints();
			float num4 = num / num3;
			float num5 = num2 / num3;
			float num6 = UIScreenController.Instance.root.manualHeight;
			_offsetTop = (num6 * num4 - 4f).RoundToInt();
			_offsetBottom = (num6 * num5 - 11f).RoundToInt();
		}
		else
		{
			restrictionsWereDetected = false;
		}
	}

	private static void VerifyHasAssignedOffsets()
	{
		if (!_hasCheckedOnce)
		{
			CheckForSafeAreaRestrictions();
			SDebug.LogError("Attempted to use SafeArea offset without ever checking SafeArea.IsRestricted! Always wrap affected code to ensure no unnecessary handling on unaffected devices.");
		}
	}
}
