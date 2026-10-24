using System;

namespace JetBrains.Annotations;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class ContractAnnotationAttribute : Attribute
{
	private string Contract { set; }

	private bool ForceFullStates { set; }

	public ContractAnnotationAttribute([NotNull] string contract)
		: this(contract, forceFullStates: false)
	{
	}

	public ContractAnnotationAttribute([NotNull] string contract, bool forceFullStates)
	{
		Contract = contract;
		ForceFullStates = forceFullStates;
	}
}
