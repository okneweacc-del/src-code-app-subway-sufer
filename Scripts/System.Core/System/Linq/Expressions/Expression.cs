namespace System.Linq.Expressions;

public sealed class Expression<TDelegate> : LambdaExpression
{
}
public abstract class Expression
{
	private ExpressionType node_type;

	private Type type;

	public ExpressionType NodeType => node_type;

	public Type Type => type;

	public override string ToString()
	{
		return ExpressionPrinter.ToString(this);
	}
}
