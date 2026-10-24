namespace System.Globalization;

internal class CCMath
{
	public static int div(int x, int y)
	{
		return (int)Math.Floor((double)x / (double)y);
	}

	public static int mod(int x, int y)
	{
		return x - y * div(x, y);
	}

	public static int div_mod(out int remainder, int x, int y)
	{
		int num = div(x, y);
		remainder = x - y * num;
		return num;
	}
}
