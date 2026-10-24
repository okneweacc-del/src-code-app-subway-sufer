using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

public sealed class TouchScreenKeyboard
{
	public enum Status
	{
		Visible = 0,
		Done = 1,
		Canceled = 2,
		LostFocus = 3
	}

	[NonSerialized]
	internal IntPtr m_Ptr;

	public static bool isSupported
	{
		get
		{
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
			case RuntimePlatform.Android:
			case RuntimePlatform.TizenPlayer:
			case RuntimePlatform.tvOS:
			case RuntimePlatform.Switch:
				return true;
			case RuntimePlatform.MetroPlayerX86:
			case RuntimePlatform.MetroPlayerX64:
			case RuntimePlatform.MetroPlayerARM:
				return false;
			default:
				return false;
			}
		}
	}

	public extern string text
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public static extern bool hideInput
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern bool active
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[Obsolete("property done is deprecated, use status instead")]
	public extern bool done
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	[Obsolete("property wasCanceled is deprecated, use status instead.")]
	public extern bool wasCanceled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern Status status
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern int characterLimit
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern bool canGetSelection
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern bool canSetSelection
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public RangeInt selection
	{
		get
		{
			RangeInt result = default;
			GetSelectionInternal(out result.start, out result.length);
			return result;
		}
		set
		{
			SetSelectionInternal(value.start, value.length);
		}
	}

	public static extern bool visible
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public TouchScreenKeyboard(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert, string textPlaceholder, int characterLimit)
	{
		TouchScreenKeyboard_InternalConstructorHelperArguments arguments = new TouchScreenKeyboard_InternalConstructorHelperArguments
		{
			keyboardType = Convert.ToUInt32(keyboardType),
			autocorrection = Convert.ToUInt32(autocorrection),
			multiline = Convert.ToUInt32(multiline),
			secure = Convert.ToUInt32(secure),
			alert = Convert.ToUInt32(alert),
			characterLimit = characterLimit
		};
		TouchScreenKeyboard_InternalConstructorHelper(ref arguments, text, textPlaceholder);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern void Destroy();

	~TouchScreenKeyboard()
	{
		Destroy();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void TouchScreenKeyboard_InternalConstructorHelper(ref TouchScreenKeyboard_InternalConstructorHelperArguments arguments, string text, string textPlaceholder);

	[ExcludeFromDocs]
	public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert, string textPlaceholder)
	{
		int num = 0;
		return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, num);
	}

	[ExcludeFromDocs]
	public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure)
	{
		int num = 0;
		string textPlaceholder = "";
		bool alert = false;
		return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, num);
	}

	public static TouchScreenKeyboard Open(string text, [DefaultValue("TouchScreenKeyboardType.Default")] TouchScreenKeyboardType keyboardType, [DefaultValue("true")] bool autocorrection, [DefaultValue("false")] bool multiline, [DefaultValue("false")] bool secure, [DefaultValue("false")] bool alert, [DefaultValue("\"\"")] string textPlaceholder, [DefaultValue("0")] int characterLimit)
	{
		return new TouchScreenKeyboard(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, characterLimit);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void GetSelectionInternal(out int start, out int length);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void SetSelectionInternal(int start, int length);
}
