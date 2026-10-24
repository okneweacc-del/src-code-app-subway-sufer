using System;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
public struct CharacterInfo
{
	public int index;

	[Obsolete("CharacterInfo.uv is deprecated. Use uvBottomLeft, uvBottomRight, uvTopRight or uvTopLeft instead.")]
	public Rect uv;

	[Obsolete("CharacterInfo.vert is deprecated. Use minX, maxX, minY, maxY instead.")]
	public Rect vert;

	[Obsolete("CharacterInfo.width is deprecated. Use advance instead.")]
	public float width;

	public int size;

	public FontStyle style;

	[Obsolete("CharacterInfo.flipped is deprecated. Use uvBottomLeft, uvBottomRight, uvTopRight or uvTopLeft instead, which will be correct regardless of orientation.")]
	public bool flipped;

	public int advance => (int)Math.Round(width, MidpointRounding.AwayFromZero);

	public int minY => (int)(vert.y + vert.height);

	public int maxY => (int)vert.y;

	public int minX => (int)vert.x;

	public int maxX => (int)(vert.x + vert.width);

	internal Vector2 uvBottomLeftUnFlipped => new Vector2(uv.x, uv.y);

	internal Vector2 uvBottomRightUnFlipped => new Vector2(uv.x + uv.width, uv.y);

	internal Vector2 uvTopRightUnFlipped => new Vector2(uv.x + uv.width, uv.y + uv.height);

	internal Vector2 uvTopLeftUnFlipped => new Vector2(uv.x, uv.y + uv.height);

	public Vector2 uvBottomLeft => uvBottomLeftUnFlipped;

	public Vector2 uvBottomRight => (!flipped) ? uvBottomRightUnFlipped : uvTopLeftUnFlipped;

	public Vector2 uvTopRight => uvTopRightUnFlipped;

	public Vector2 uvTopLeft => (!flipped) ? uvTopLeftUnFlipped : uvBottomRightUnFlipped;
}
