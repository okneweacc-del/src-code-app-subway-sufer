using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

[ComVisible(true)]
public sealed class DynamicMethod : MethodInfo
{
	private RuntimeMethodHandle mhandle;

	private string name;

	private Type returnType;

	private Type[] parameters;

	private MethodAttributes attributes;

	private CallingConventions callingConvention;

	private Module module;

	private bool skipVisibility;

	private bool init_locals;

	private ILGenerator ilgen;

	private int nrefs;

	private object[] refs;

	private IntPtr referenced_by;

	private Type owner;

	private Delegate deleg;

	private MonoMethod method;

	private ParameterBuilder[] pinfo;

	internal bool creating;

	public override MethodAttributes Attributes => attributes;

	public override CallingConventions CallingConvention => callingConvention;

	public override Type DeclaringType => null;

	public override RuntimeMethodHandle MethodHandle => mhandle;

	public override Module Module => module;

	public override string Name => name;

	public override Type ReflectedType => null;

	public override Type ReturnType => returnType;

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void create_dynamic_method(DynamicMethod m);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void destroy_dynamic_method(DynamicMethod m);

	private void CreateDynMethod()
	{
		if (!(mhandle.Value == IntPtr.Zero))
		{
			return;
		}
		if (ilgen == null || ILGenerator.Mono_GetCurrentOffset(ilgen) == 0)
		{
			throw new InvalidOperationException("Method '" + name + "' does not have a method body.");
		}
		ilgen.label_fixup();
		try
		{
			creating = true;
			if (refs != null)
			{
				for (int i = 0; i < refs.Length; i++)
				{
					if (refs[i] is DynamicMethod)
					{
						DynamicMethod dynamicMethod = (DynamicMethod)refs[i];
						if (!dynamicMethod.creating)
						{
							dynamicMethod.CreateDynMethod();
						}
					}
				}
			}
		}
		finally
		{
			creating = false;
		}
		create_dynamic_method(this);
	}

	~DynamicMethod()
	{
		destroy_dynamic_method(this);
	}

	public override MethodInfo GetBaseDefinition()
	{
		return this;
	}

	[MonoTODO("Not implemented")]
	public override object[] GetCustomAttributes(bool inherit)
	{
		throw new NotImplementedException();
	}

	[MonoTODO("Not implemented")]
	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		throw new NotImplementedException();
	}

	public override ParameterInfo[] GetParameters()
	{
		if (parameters == null)
		{
			return new ParameterInfo[0];
		}
		ParameterInfo[] array = new ParameterInfo[parameters.Length];
		for (int i = 0; i < parameters.Length; i++)
		{
			array[i] = new ParameterInfo((pinfo != null) ? pinfo[i + 1] : null, parameters[i], this, i + 1);
		}
		return array;
	}

	public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
	{
		try
		{
			CreateDynMethod();
			if (method == null)
			{
				method = new MonoMethod(mhandle);
			}
			return method.Invoke(obj, parameters);
		}
		catch (MethodAccessException inner)
		{
			throw new TargetInvocationException("Method cannot be invoked.", inner);
		}
	}

	[MonoTODO("Not implemented")]
	public override bool IsDefined(Type attributeType, bool inherit)
	{
		throw new NotImplementedException();
	}

	public override string ToString()
	{
		string text = string.Empty;
		ParameterInfo[] array = GetParameters();
		for (int i = 0; i < array.Length; i++)
		{
			if (i > 0)
			{
				text += ", ";
			}
			text += array[i].ParameterType.Name;
		}
		return ReturnType.Name + " " + Name + "(" + text + ")";
	}
}
