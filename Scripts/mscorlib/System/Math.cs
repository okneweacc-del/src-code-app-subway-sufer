using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

namespace System;

public static class Math
{
	public static double Abs(double value)
	{
		return (!(value < 0.0)) ? value : (0.0 - value);
	}

	public static float Abs(float value)
	{
		return (!(value < 0f)) ? value : (0f - value);
	}

	public static int Abs(int value)
	{
		if (value == int.MinValue)
		{
			throw new OverflowException(Locale.GetText("Value is too small."));
		}
		return (value >= 0) ? value : (-value);
	}

	public static long Abs(long value)
	{
		if (value == long.MinValue)
		{
			throw new OverflowException(Locale.GetText("Value is too small."));
		}
		return (value >= 0) ? value : (-value);
	}

	public static double Ceiling(double a)
	{
		double num = Floor(a);
		if (num != a)
		{
			num++;
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern double Floor(double d);

	public static double Log(double a, double newBase)
	{
		double num = Log(a) / Log(newBase);
		return (num != 0.0) ? num : 0.0;
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	public static byte Max(byte val1, byte val2)
	{
		return (val1 <= val2) ? val2 : val1;
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	public static float Max(float val1, float val2)
	{
		if (float.IsNaN(val1) || float.IsNaN(val2))
		{
			return float.NaN;
		}
		return (!(val1 > val2)) ? val2 : val1;
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	public static int Max(int val1, int val2)
	{
		return (val1 <= val2) ? val2 : val1;
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	public static float Min(float val1, float val2)
	{
		if (float.IsNaN(val1) || float.IsNaN(val2))
		{
			return float.NaN;
		}
		return (!(val1 < val2)) ? val2 : val1;
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	public static int Min(int val1, int val2)
	{
		return (val1 >= val2) ? val2 : val1;
	}

	public static decimal Round(decimal d)
	{
		decimal num = decimal.Floor(d);
		decimal num2 = d - num;
		if ((num2 == 0.5m && 2.0m * (num / 2.0m - decimal.Floor(num / 2.0m)) != 0m) || num2 > 0.5m)
		{
			++num;
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern double Round(double a);

	public static double Round(double value, int digits)
	{
		switch (digits)
		{
		default:
			throw new ArgumentOutOfRangeException(Locale.GetText("Value is too small or too big."));
		case 0:
			return Round(value);
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
		case 11:
		case 12:
		case 13:
		case 14:
		case 15:
			return Round2(value, digits, away_from_zero: false);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern double Round2(double value, int digits, bool away_from_zero);

	public static double Round(double value, MidpointRounding mode)
	{
		if (mode != MidpointRounding.ToEven && mode != MidpointRounding.AwayFromZero)
		{
			throw new ArgumentException(string.Concat("The value '", mode, "' is not valid for this usage of the type MidpointRounding."), "mode");
		}
		if (mode == MidpointRounding.ToEven)
		{
			return Round(value);
		}
		if (value > 0.0)
		{
			return Floor(value + 0.5);
		}
		return Ceiling(value - 0.5);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern double Sin(double a);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern double Cos(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern double Tan(double a);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern double Acos(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern double Atan(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern double Atan2(double y, double x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern double Log(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern double Pow(double x, double y);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	public static extern double Sqrt(double d);
}
