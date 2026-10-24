using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

[Serializable]
[Obsolete("An alternate API is available: Emit the MarshalAs custom attribute instead.")]
[ComVisible(true)]
public sealed class UnmanagedMarshal
{
	private int count;

	private UnmanagedType t;

	private UnmanagedType tbase;

	private string guid;

	private string mcookie;

	private string marshaltype;

	private Type marshaltyperef;

	private int param_num;

	private bool has_size;

	internal MarshalAsAttribute ToMarshalAsAttribute()
	{
		MarshalAsAttribute marshalAsAttribute = new MarshalAsAttribute(t);
		marshalAsAttribute.ArraySubType = tbase;
		marshalAsAttribute.MarshalCookie = mcookie;
		marshalAsAttribute.MarshalType = marshaltype;
		marshalAsAttribute.MarshalTypeRef = marshaltyperef;
		if (count == -1)
		{
			marshalAsAttribute.SizeConst = 0;
		}
		else
		{
			marshalAsAttribute.SizeConst = count;
		}
		if (param_num == -1)
		{
			marshalAsAttribute.SizeParamIndex = 0;
		}
		else
		{
			marshalAsAttribute.SizeParamIndex = (short)param_num;
		}
		return marshalAsAttribute;
	}
}
