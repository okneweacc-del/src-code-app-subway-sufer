using System.Collections.ObjectModel;

namespace System.Linq.Expressions;

public sealed class ListInitExpression : Expression
{
	private NewExpression new_expression;

	private ReadOnlyCollection<ElementInit> initializers;

	public NewExpression NewExpression => new_expression;

	public ReadOnlyCollection<ElementInit> Initializers => initializers;
}
