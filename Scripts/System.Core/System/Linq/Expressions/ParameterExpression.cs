namespace System.Linq.Expressions;

public sealed class ParameterExpression : Expression
{
	private string name;

	public string Name => name;
}
