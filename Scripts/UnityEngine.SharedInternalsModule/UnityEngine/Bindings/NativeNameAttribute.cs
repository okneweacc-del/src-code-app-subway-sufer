using System;

namespace UnityEngine.Bindings;

[VisibleToOtherModules]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
internal class NativeNameAttribute : Attribute, IBindingsNameProviderAttribute, IBindingsAttribute
{
	public string Name { set; }

	public NativeNameAttribute(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (name == "")
		{
			throw new ArgumentException("name cannot be empty", "name");
		}
		Name = name;
	}
}
