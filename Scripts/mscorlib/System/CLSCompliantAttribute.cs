using System.Runtime.InteropServices;

namespace System;

[Serializable]
[AttributeUsage(AttributeTargets.All)]
[ComVisible(true)]
public sealed class CLSCompliantAttribute : Attribute
{
	private bool is_compliant;

	public CLSCompliantAttribute(bool isCompliant)
	{
		is_compliant = isCompliant;
	}
}
