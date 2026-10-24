using System.Collections.ObjectModel;

namespace System.Linq.Expressions;

public sealed class NewArrayExpression : Expression
{
	private ReadOnlyCollection<Expression> expressions;

	public ReadOnlyCollection<Expression> Expressions => expressions;
}
