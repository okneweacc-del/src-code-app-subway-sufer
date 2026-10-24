using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions;

public sealed class NewExpression : Expression
{
	private ReadOnlyCollection<Expression> arguments;

	private ReadOnlyCollection<MemberInfo> members;

	public ReadOnlyCollection<Expression> Arguments => arguments;

	public ReadOnlyCollection<MemberInfo> Members => members;
}
