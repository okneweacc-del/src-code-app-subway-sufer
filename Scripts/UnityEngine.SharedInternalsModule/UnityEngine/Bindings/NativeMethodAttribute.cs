using System;

namespace UnityEngine.Bindings;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
[VisibleToOtherModules]
internal class NativeMethodAttribute : Attribute, IBindingsNameProviderAttribute, IBindingsIsThreadSafeProviderAttribute, IBindingsIsFreeFunctionProviderAttribute, IBindingsThrowsProviderAttribute, IBindingsAttribute
{
	public string Name { set; }

	public bool IsThreadSafe { set; }

	public bool IsFreeFunction { set; }

	public bool ThrowsException { set; }

	public bool HasExplicitThis { set; }

	public NativeMethodAttribute()
	{
	}

	public NativeMethodAttribute(string name)
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

	public NativeMethodAttribute(string name, bool isFreeFunction)
		: this(name)
	{
		IsFreeFunction = isFreeFunction;
	}

	public NativeMethodAttribute(string name, bool isFreeFunction, bool isThreadSafe)
		: this(name, isFreeFunction)
	{
		IsThreadSafe = isThreadSafe;
	}
}
