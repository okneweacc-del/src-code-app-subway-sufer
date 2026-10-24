using System;

namespace NaughtyAttributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ShowAssetPreviewAttribute : DrawerAttribute
{
	private int width;

	private int height;

	public int Width => width;

	public int Height => height;

	public ShowAssetPreviewAttribute(int width = 64, int height = 64)
	{
		this.width = width;
		this.height = height;
	}
}
