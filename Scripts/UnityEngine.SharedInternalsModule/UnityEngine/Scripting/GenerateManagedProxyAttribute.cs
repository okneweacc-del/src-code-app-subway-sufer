using System;
using UnityEngine.Bindings;

namespace UnityEngine.Scripting;

[VisibleToOtherModules]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
internal class GenerateManagedProxyAttribute : Attribute
{
	public string NativeType { set; }

	public GenerateManagedProxyAttribute()
	{
	}

	public GenerateManagedProxyAttribute(string nativeType)
	{
		NativeType = nativeType;
	}
}
