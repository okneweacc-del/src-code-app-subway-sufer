using System;

namespace UnityEngine.Bindings;

[VisibleToOtherModules]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
internal class NativeThrowsAttribute : Attribute, IBindingsThrowsProviderAttribute, IBindingsAttribute
{
	public bool ThrowsException { set; }

	public NativeThrowsAttribute()
	{
		ThrowsException = true;
	}
}
