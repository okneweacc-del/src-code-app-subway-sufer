using System.Collections.ObjectModel;

namespace System.Linq.Expressions;

public sealed class MemberMemberBinding : MemberBinding
{
	private ReadOnlyCollection<MemberBinding> bindings;

	public ReadOnlyCollection<MemberBinding> Bindings => bindings;
}
