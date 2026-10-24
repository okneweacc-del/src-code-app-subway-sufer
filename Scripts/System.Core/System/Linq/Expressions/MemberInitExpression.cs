using System.Collections.ObjectModel;

namespace System.Linq.Expressions;

public sealed class MemberInitExpression : Expression
{
	private NewExpression new_expression;

	private ReadOnlyCollection<MemberBinding> bindings;

	public NewExpression NewExpression => new_expression;

	public ReadOnlyCollection<MemberBinding> Bindings => bindings;
}
