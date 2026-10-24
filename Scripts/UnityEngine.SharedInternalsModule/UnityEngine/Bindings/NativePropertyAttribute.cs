using System;

namespace UnityEngine.Bindings;

[AttributeUsage(AttributeTargets.Property)]
[VisibleToOtherModules]
internal class NativePropertyAttribute : NativeMethodAttribute
{
	public TargetType TargetType { set; }

	public NativePropertyAttribute()
	{
	}

	public NativePropertyAttribute(string name)
		: base(name)
	{
	}

	public NativePropertyAttribute(string name, bool isFree, TargetType targetType)
		: base(name, isFree)
	{
		TargetType = targetType;
	}
}
