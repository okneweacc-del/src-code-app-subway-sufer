namespace System.Security;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
[MonoTODO("Only supported by the runtime when CoreCLR is enabled")]
public sealed class SecurityCriticalAttribute : Attribute
{
	private SecurityCriticalScope _scope;

	public SecurityCriticalAttribute()
	{
		_scope = SecurityCriticalScope.Explicit;
	}
}
