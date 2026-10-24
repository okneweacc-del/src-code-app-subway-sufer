using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions;

public sealed class ElementInit
{
	private MethodInfo add_method;

	private ReadOnlyCollection<Expression> arguments;

	public MethodInfo AddMethod => add_method;

	public ReadOnlyCollection<Expression> Arguments => arguments;

	public override string ToString()
	{
		return ExpressionPrinter.ToString(this);
	}
}
