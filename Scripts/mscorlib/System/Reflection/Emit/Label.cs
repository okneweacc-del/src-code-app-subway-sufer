using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

[Serializable]
[ComVisible(true)]
public struct Label
{
	internal int label;

	internal Label(int val)
	{
		label = val;
	}

	public override bool Equals(object obj)
	{
		bool flag = obj is Label;
		if (flag)
		{
			Label label = (Label)obj;
			flag = this.label == label.label;
		}
		return flag;
	}

	public override int GetHashCode()
	{
		return label.GetHashCode();
	}
}
