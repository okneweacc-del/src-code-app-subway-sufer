using System.Runtime.InteropServices;

namespace System.Reflection;

[Serializable]
[DefaultMember("Item")]
[ComVisible(true)]
public struct ParameterModifier
{
	private bool[] _byref;
}
