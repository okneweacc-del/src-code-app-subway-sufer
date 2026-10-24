using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices;

[Serializable]
[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Method)]
public class CompilationRelaxationsAttribute : Attribute
{
	private int relax;

	public CompilationRelaxationsAttribute(CompilationRelaxations relaxations)
	{
		relax = (int)relaxations;
	}
}
