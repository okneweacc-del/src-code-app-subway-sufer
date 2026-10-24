using System;

namespace UnityEngine.Bindings;

[VisibleToOtherModules]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property)]
internal class StaticAccessorAttribute : Attribute, IBindingsAttribute
{
	public string Name { set; }

	public StaticAccessorType Type { set; }

	[VisibleToOtherModules]
	internal StaticAccessorAttribute(string name)
	{
		Name = name;
	}

	public StaticAccessorAttribute(string name, StaticAccessorType type)
	{
		Name = name;
		Type = type;
	}
}
