using System.Diagnostics.SymbolStore;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Reflection.Emit;

[ClassInterface(ClassInterfaceType.None)]
[ComVisible(true)]
[ComDefaultInterface(typeof(_ConstructorBuilder))]
public sealed class ConstructorBuilder : ConstructorInfo, _ConstructorBuilder
{
	private RuntimeMethodHandle mhandle;

	private ILGenerator ilgen;

	internal Type[] parameters;

	private MethodAttributes attrs;

	private MethodImplAttributes iattrs;

	private int table_idx;

	private CallingConventions call_conv;

	private TypeBuilder type;

	internal ParameterBuilder[] pinfo;

	private CustomAttributeBuilder[] cattrs;

	private bool init_locals = true;

	private Type[][] paramModReq;

	private Type[][] paramModOpt;

	private RefEmitPermissionSet[] permissions;

	[MonoTODO]
	public override CallingConventions CallingConvention => call_conv;

	internal TypeBuilder TypeBuilder => type;

	public override RuntimeMethodHandle MethodHandle
	{
		get
		{
			throw not_supported();
		}
	}

	public override MethodAttributes Attributes => attrs;

	public override Type ReflectedType => type;

	public override Type DeclaringType => type;

	public override string Name => ((attrs & MethodAttributes.Static) == 0) ? ConstructorInfo.ConstructorName : ConstructorInfo.TypeConstructorName;

	public override Module Module => base.Module;

	private bool IsCompilerContext
	{
		get
		{
			ModuleBuilder moduleBuilder = (ModuleBuilder)TypeBuilder.Module;
			AssemblyBuilder assemblyBuilder = (AssemblyBuilder)moduleBuilder.Assembly;
			return assemblyBuilder.IsCompilerContext;
		}
	}

	internal ConstructorBuilder(TypeBuilder tb, MethodAttributes attributes, CallingConventions callingConvention, Type[] parameterTypes, Type[][] paramModReq, Type[][] paramModOpt)
	{
		attrs = attributes | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
		call_conv = callingConvention;
		if (parameterTypes != null)
		{
			for (int i = 0; i < parameterTypes.Length; i++)
			{
				if (parameterTypes[i] == null)
				{
					throw new ArgumentException("Elements of the parameterTypes array cannot be null", "parameterTypes");
				}
			}
			parameters = new Type[parameterTypes.Length];
			Array.Copy(parameterTypes, parameters, parameterTypes.Length);
		}
		type = tb;
		this.paramModReq = paramModReq;
		this.paramModOpt = paramModOpt;
		table_idx = get_next_table_index(this, 6, inc: true);
		((ModuleBuilder)tb.Module).RegisterToken(this, GetToken().Token);
	}

	public override ParameterInfo[] GetParameters()
	{
		if (!type.is_created && !IsCompilerContext)
		{
			throw not_created();
		}
		return GetParametersInternal();
	}

	internal ParameterInfo[] GetParametersInternal()
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

	internal override int GetParameterCount()
	{
		if (parameters == null)
		{
			return 0;
		}
		return parameters.Length;
	}

	public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
	{
		throw not_supported();
	}

	public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
	{
		throw not_supported();
	}

	public override bool IsDefined(Type attributeType, bool inherit)
	{
		throw not_supported();
	}

	public override object[] GetCustomAttributes(bool inherit)
	{
		if (type.is_created && IsCompilerContext)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, inherit);
		}
		throw not_supported();
	}

	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		if (type.is_created && IsCompilerContext)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
		}
		throw not_supported();
	}

	public ILGenerator GetILGenerator()
	{
		return GetILGenerator(64);
	}

	public ILGenerator GetILGenerator(int streamSize)
	{
		if (ilgen != null)
		{
			return ilgen;
		}
		ilgen = new ILGenerator(type.Module, ((ModuleBuilder)type.Module).GetTokenGenerator(), streamSize);
		return ilgen;
	}

	public MethodToken GetToken()
	{
		return new MethodToken(0x6000000 | table_idx);
	}

	public override string ToString()
	{
		return "ConstructorBuilder ['" + type.Name + "']";
	}

	internal void fixup()
	{
		if ((attrs & (MethodAttributes.Abstract | MethodAttributes.PinvokeImpl)) == 0 && (iattrs & (MethodImplAttributes)0x1003) == 0 && (ilgen == null || ILGenerator.Mono_GetCurrentOffset(ilgen) == 0))
		{
			throw new InvalidOperationException("Method '" + Name + "' does not have a method body.");
		}
		if (ilgen != null)
		{
			ilgen.label_fixup();
		}
	}

	internal void GenerateDebugInfo(ISymbolWriter symbolWriter)
	{
		if (ilgen != null && ilgen.HasDebugInfo)
		{
			SymbolToken symbolToken = new SymbolToken(GetToken().Token);
			symbolWriter.OpenMethod(symbolToken);
			symbolWriter.SetSymAttribute(symbolToken, "__name", Encoding.UTF8.GetBytes(Name));
			ilgen.GenerateDebugInfo(symbolWriter);
			symbolWriter.CloseMethod();
		}
	}

	internal override int get_next_table_index(object obj, int table, bool inc)
	{
		return type.get_next_table_index(obj, table, inc);
	}

	private Exception not_supported()
	{
		return new NotSupportedException("The invoked member is not supported in a dynamic module.");
	}

	private Exception not_created()
	{
		return new NotSupportedException("The type is not yet created.");
	}
}
