using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeClass("TextRendering::Font")]
public sealed class Font : Object
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public delegate void FontTextureRebuildCallback();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<Font> textureRebuilt__BackingField;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private FontTextureRebuildCallback m_FontTextureRebuildCallback;

	public extern Material material
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern string[] fontNames
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern bool dynamic
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern int fontSize
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static event Action<Font> textureRebuilt
	{
		add
		{
			Action<Font> action = textureRebuilt__BackingField;
			Action<Font> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref textureRebuilt__BackingField, (Action<Font>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<Font> action = textureRebuilt__BackingField;
			Action<Font> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref textureRebuilt__BackingField, (Action<Font>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern bool HasCharacter(char c);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void RequestCharactersInTexture(string characters, [UnityEngine.Internal.DefaultValue("0")] int size, [UnityEngine.Internal.DefaultValue("FontStyle.Normal")] FontStyle style);

	[RequiredByNativeCode]
	private static void InvokeTextureRebuilt_Internal(Font font)
	{
		textureRebuilt__BackingField?.Invoke(font);
		if (font.m_FontTextureRebuildCallback != null)
		{
			font.m_FontTextureRebuildCallback();
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern bool GetCharacterInfo(char ch, out CharacterInfo info, [UnityEngine.Internal.DefaultValue("0")] int size, [UnityEngine.Internal.DefaultValue("FontStyle.Normal")] FontStyle style);
}
