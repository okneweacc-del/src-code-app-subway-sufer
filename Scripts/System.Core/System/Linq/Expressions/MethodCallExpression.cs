using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions;

public sealed class MethodCallExpression : Expression
{
	private Expression obj;

	private MethodInfo method;

	private ReadOnlyCollection<Expression> arguments;

	public Expression Object => obj;

	public MethodInfo Method => method;

	public ReadOnlyCollection<Expression> Arguments => arguments;
}
