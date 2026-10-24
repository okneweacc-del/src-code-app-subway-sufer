using System;

namespace UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public sealed class ColorUsageAttribute : PropertyAttribute
{
	public readonly bool showAlpha = true;

	public readonly bool hdr = false;

	[Obsolete("This field is no longer used for anything.")]
	public readonly float minBrightness = 0f;

	[Obsolete("This field is no longer used for anything.")]
	public readonly float maxBrightness = 8f;

	[Obsolete("This field is no longer used for anything.")]
	public readonly float minExposureValue = 0.125f;

	[Obsolete("This field is no longer used for anything.")]
	public readonly float maxExposureValue = 3f;

	public ColorUsageAttribute(bool showAlpha)
	{
		this.showAlpha = showAlpha;
	}
}
