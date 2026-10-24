using System.Collections.ObjectModel;

namespace System.Linq.Expressions;

public sealed class InvocationExpression : Expression
{
	private Expression expression;

	private ReadOnlyCollection<Expression> arguments;

	public Expression Expression => expression;

	public ReadOnlyCollection<Expression> Arguments => arguments;
}
