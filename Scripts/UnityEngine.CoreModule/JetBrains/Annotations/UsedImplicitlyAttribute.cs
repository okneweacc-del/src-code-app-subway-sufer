using System;

namespace JetBrains.Annotations;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
public sealed class UsedImplicitlyAttribute : Attribute
{
	private ImplicitUseKindFlags UseKindFlags { set; }

	private ImplicitUseTargetFlags TargetFlags { set; }

	public UsedImplicitlyAttribute()
		: this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
	{
	}

	public UsedImplicitlyAttribute(ImplicitUseTargetFlags targetFlags)
		: this(ImplicitUseKindFlags.Default, targetFlags)
	{
	}

	public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
	{
		UseKindFlags = useKindFlags;
		TargetFlags = targetFlags;
	}
}
