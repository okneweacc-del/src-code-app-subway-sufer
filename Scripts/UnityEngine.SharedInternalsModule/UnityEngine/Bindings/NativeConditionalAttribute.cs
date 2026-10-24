using System;

namespace UnityEngine.Bindings;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property)]
[VisibleToOtherModules]
internal class NativeConditionalAttribute : Attribute, IBindingsAttribute
{
	public string Condition { set; }

	public bool Enabled { set; }

	public NativeConditionalAttribute(string condition)
	{
		Condition = condition;
		Enabled = true;
	}
}
