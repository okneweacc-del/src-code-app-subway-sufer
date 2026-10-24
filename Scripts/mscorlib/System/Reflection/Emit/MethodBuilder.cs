using System.Diagnostics.SymbolStore;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Reflection.Emit;

[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(_MethodBuilder))]
[ComVisible(true)]
public sealed class MethodBuilder : MethodInfo, _MethodBuilder
{
	private RuntimeMethodHandle mhandle;

	private Type rtype;

	internal Type[] parameters;

	private MethodAttributes attrs;

	private MethodImplAttributes iattrs;

	private string name;

	private int table_idx;

	private byte[] code;

	private ILGenerator ilgen;

	private TypeBuilder type;

	internal ParameterBuilder[] pinfo;

	private CustomAttributeBuilder[] cattrs;

	private MethodInfo override_method;

	private string pi_dll;

	private string pi_entry;

	private CharSet charset;

	private uint extra_flags;

	private CallingConvention native_cc;

	private CallingConventions call_conv;

	private bool init_locals = true;

	private IntPtr generic_container;

	internal GenericTypeParameterBuilder[] generic_params;

	private Type[] returnModReq;

	private Type[] returnModOpt;

	private Type[][] paramModReq;

	private Type[][] paramModOpt;

	private RefEmitPermissionSet[] permissions;

	public override bool ContainsGenericParameters
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public override RuntimeMethodHandle MethodHandle
	{
		get
		{
			throw NotSupported();
		}
	}

	public override Type ReturnType => rtype;

	public override Type ReflectedType => type;

	public override Type DeclaringType => type;

	public override string Name => name;

	public override MethodAttributes Attributes => attrs;

	public override CallingConventions CallingConvention => call_conv;

	public override bool IsGenericMethodDefinition => generic_params != null;

	public override bool IsGenericMethod => generic_params != null;

	public override Module Module => base.Module;

	internal MethodBuilder(TypeBuilder tb, string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] returnModReq, Type[] returnModOpt, Type[] parameterTypes, Type[][] paramModReq, Type[][] paramModOpt)
	{
		this.name = name;
		attrs = attributes;
		call_conv = callingConvention;
		rtype = returnType;
		this.returnModReq = returnModReq;
		this.returnModOpt = returnModOpt;
		this.paramModReq = paramModReq;
		this.paramModOpt = paramModOpt;
		if ((attributes & MethodAttributes.Static) == 0)
		{
			call_conv |= CallingConventions.HasThis;
		}
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
		table_idx = get_next_table_index(this, 6, inc: true);
		((ModuleBuilder)tb.Module).RegisterToken(this, GetToken().Token);
	}

	internal MethodBuilder(TypeBuilder tb, string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] returnModReq, Type[] returnModOpt, Type[] parameterTypes, Type[][] paramModReq, Type[][] paramModOpt, string dllName, string entryName, CallingConvention nativeCConv, CharSet nativeCharset)
		: this(tb, name, attributes, callingConvention, returnType, returnModReq, returnModOpt, parameterTypes, paramModReq, paramModOpt)
	{
		pi_dll = dllName;
		pi_entry = entryName;
		native_cc = nativeCConv;
		charset = nativeCharset;
	}

	public MethodToken GetToken()
	{
		return new MethodToken(0x6000000 | table_idx);
	}

	public override MethodInfo GetBaseDefinition()
	{
		return this;
	}

	public override ParameterInfo[] GetParameters()
	{
		if (!type.is_created)
		{
			throw NotSupported();
		}
		if (parameters == null)
		{
			return null;
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
		throw NotSupported();
	}

	public override bool IsDefined(Type attributeType, bool inherit)
	{
		throw NotSupported();
	}

	public override object[] GetCustomAttributes(bool inherit)
	{
		if (type.is_created)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, inherit);
		}
		throw NotSupported();
	}

	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		if (type.is_created)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
		}
		throw NotSupported();
	}

	public ILGenerator GetILGenerator()
	{
		return GetILGenerator(64);
	}

	public ILGenerator GetILGenerator(int size)
	{
		if ((iattrs & MethodImplAttributes.CodeTypeMask) != MethodImplAttributes.IL || (iattrs & MethodImplAttributes.ManagedMask) != MethodImplAttributes.IL)
		{
			throw new InvalidOperationException("Method body should not exist.");
		}
		if (ilgen != null)
		{
			return ilgen;
		}
		ilgen = new ILGenerator(type.Module, ((ModuleBuilder)type.Module).GetTokenGenerator(), size);
		return ilgen;
	}

	internal void check_override()
	{
		if (override_method != null && override_method.IsVirtual && !IsVirtual)
		{
			throw new TypeLoadException($"Method '{name}' override '{override_method}' but it is not virtual");
		}
	}

	internal void fixup()
	{
		if ((attrs & (MethodAttributes.Abstract | MethodAttributes.PinvokeImpl)) == 0 && (iattrs & (MethodImplAttributes)0x1003) == 0 && (ilgen == null || ILGenerator.Mono_GetCurrentOffset(ilgen) == 0) && (code == null || code.Length == 0))
		{
			throw new InvalidOperationException($"Method '{DeclaringType.FullName}.{Name}' does not have a method body.");
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

	public override string ToString()
	{
		return "MethodBuilder [" + type.Name + "::" + name + "]";
	}

	[MonoTODO]
	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return name.GetHashCode();
	}

	internal override int get_next_table_index(object obj, int table, bool inc)
	{
		return type.get_next_table_index(obj, table, inc);
	}

	internal void set_override(MethodInfo mdecl)
	{
		override_method = mdecl;
	}

	private Exception NotSupported()
	{
		return new NotSupportedException("The invoked member is not supported in a dynamic module.");
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public override extern MethodInfo MakeGenericMethod(params Type[] typeArguments);

	public override Type[] GetGenericArguments()
	{
		if (generic_params == null)
		{
			return Type.EmptyTypes;
		}
		Type[] array = new Type[generic_params.Length];
		for (int i = 0; i < generic_params.Length; i++)
		{
			array[i] = generic_params[i];
		}
		return array;
	}
}
