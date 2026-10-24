namespace System.Linq.Expressions;

public sealed class MemberAssignment : MemberBinding
{
	private Expression expression;

	public Expression Expression => expression;
}
