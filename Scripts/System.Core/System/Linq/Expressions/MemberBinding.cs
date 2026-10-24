using System.Reflection;

namespace System.Linq.Expressions;

public abstract class MemberBinding
{
	private MemberBindingType binding_type;

	private MemberInfo member;

	public MemberBindingType BindingType => binding_type;

	public MemberInfo Member => member;

	public override string ToString()
	{
		return ExpressionPrinter.ToString(this);
	}
}
