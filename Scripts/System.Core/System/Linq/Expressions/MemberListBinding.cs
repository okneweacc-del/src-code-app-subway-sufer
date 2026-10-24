using System.Collections.ObjectModel;

namespace System.Linq.Expressions;

public sealed class MemberListBinding : MemberBinding
{
	private ReadOnlyCollection<ElementInit> initializers;

	public ReadOnlyCollection<ElementInit> Initializers => initializers;
}
