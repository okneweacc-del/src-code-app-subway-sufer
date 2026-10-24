using System.Reflection;

namespace System.Linq.Expressions;

public sealed class MemberExpression : Expression
{
	private Expression expression;

	private MemberInfo member;

	public Expression Expression => expression;

	public MemberInfo Member => member;
}
