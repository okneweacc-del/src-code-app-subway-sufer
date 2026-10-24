using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout((LayoutKind)2)]
[UsedByNativeCode]
public struct Color32(byte r, byte g, byte b, byte a)
{
	[FieldOffset(0)]
	private int rgba = 0;

	[FieldOffset(0)]
	public byte r = r;

	[FieldOffset(1)]
	public byte g = g;

	[FieldOffset(2)]
	public byte b = b;

	[FieldOffset(3)]
	public byte a = a;

	public static implicit operator Color32(Color c)
	{
		return new Color32((byte)(Mathf.Clamp01(c.r) * 255f), (byte)(Mathf.Clamp01(c.g) * 255f), (byte)(Mathf.Clamp01(c.b) * 255f), (byte)(Mathf.Clamp01(c.a) * 255f));
	}

	public static implicit operator Color(Color32 c)
	{
		return new Color((float)(int)c.r / 255f, (float)(int)c.g / 255f, (float)(int)c.b / 255f, (float)(int)c.a / 255f);
	}

	public override string ToString()
	{
		return UnityString.Format("RGBA({0}, {1}, {2}, {3})", r, g, b, a);
	}
}
