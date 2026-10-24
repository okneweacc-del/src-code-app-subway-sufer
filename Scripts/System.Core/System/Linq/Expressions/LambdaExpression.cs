using System.Collections.ObjectModel;

namespace System.Linq.Expressions;

public class LambdaExpression : Expression
{
	private Expression body;

	private ReadOnlyCollection<ParameterExpression> parameters;

	public Expression Body => body;

	public ReadOnlyCollection<ParameterExpression> Parameters => parameters;
}
