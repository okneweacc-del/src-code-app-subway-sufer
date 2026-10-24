using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices;

[Serializable]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
[ComVisible(true)]
public sealed class DecimalConstantAttribute : Attribute
{
	private byte scale;

	private bool sign;

	private int hi;

	private int mid;

	private int low;

	[CLSCompliant(false)]
	public DecimalConstantAttribute(byte scale, byte sign, uint hi, uint mid, uint low)
	{
		this.scale = scale;
		this.sign = Convert.ToBoolean(sign);
		this.hi = (int)hi;
		this.mid = (int)mid;
		this.low = (int)low;
	}
}
