using System;

namespace JetBrains.Annotations;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class MeansImplicitUseAttribute : Attribute
{
	[UsedImplicitly]
	private ImplicitUseKindFlags UseKindFlags { set; }

	[UsedImplicitly]
	private ImplicitUseTargetFlags TargetFlags { set; }

	public MeansImplicitUseAttribute()
		: this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
	{
	}

	public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
	{
		UseKindFlags = useKindFlags;
		TargetFlags = targetFlags;
	}
}
