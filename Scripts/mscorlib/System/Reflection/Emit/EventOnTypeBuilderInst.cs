namespace System.Reflection.Emit;

internal class EventOnTypeBuilderInst : EventInfo
{
	private MonoGenericClass instantiation;

	private EventBuilder evt;

	public override EventAttributes Attributes => evt.attrs;

	public override Type DeclaringType => instantiation;

	public override string Name => evt.name;

	public override Type ReflectedType => instantiation;

	internal EventOnTypeBuilderInst(MonoGenericClass instantiation, EventBuilder evt)
	{
		this.instantiation = instantiation;
		this.evt = evt;
	}

	public override MethodInfo GetAddMethod(bool nonPublic)
	{
		if (evt.add_method == null || (!nonPublic && !evt.add_method.IsPublic))
		{
			return null;
		}
		return TypeBuilder.GetMethod(instantiation, evt.add_method);
	}

	public override MethodInfo GetRemoveMethod(bool nonPublic)
	{
		if (evt.remove_method == null || (!nonPublic && !evt.remove_method.IsPublic))
		{
			return null;
		}
		return TypeBuilder.GetMethod(instantiation, evt.remove_method);
	}

	public override bool IsDefined(Type attributeType, bool inherit)
	{
		throw new NotSupportedException();
	}

	public override object[] GetCustomAttributes(bool inherit)
	{
		throw new NotSupportedException();
	}

	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		throw new NotSupportedException();
	}
}
