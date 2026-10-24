using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

[ComVisible(true)]
[ComDefaultInterface(typeof(_SignatureHelper))]
[ClassInterface(ClassInterfaceType.None)]
public sealed class SignatureHelper : _SignatureHelper
{
	internal enum SignatureHelperType
	{
		HELPER_FIELD = 0,
		HELPER_LOCAL = 1,
		HELPER_METHOD = 2,
		HELPER_PROPERTY = 3
	}

	private ModuleBuilder module;

	private Type[] arguments;

	private SignatureHelperType type;

	private Type returnType;

	private CallingConventions callConv;

	private CallingConvention unmanagedCallConv;

	private Type[][] modreqs;

	private Type[][] modopts;

	internal SignatureHelper(ModuleBuilder module, SignatureHelperType type)
	{
		this.type = type;
		this.module = module;
	}

	public static SignatureHelper GetLocalVarSigHelper(Module mod)
	{
		if (mod != null && !(mod is ModuleBuilder))
		{
			throw new ArgumentException("ModuleBuilder is expected");
		}
		return new SignatureHelper((ModuleBuilder)mod, SignatureHelperType.HELPER_LOCAL);
	}

	private static int AppendArray(ref Type[] array, Type t)
	{
		if (array != null)
		{
			Type[] array2 = new Type[array.Length + 1];
			Array.Copy(array, array2, array.Length);
			array2[array.Length] = t;
			array = array2;
			return array.Length - 1;
		}
		array = new Type[1];
		array[0] = t;
		return 0;
	}

	public void AddArgument(Type clsArgument)
	{
		if (clsArgument == null)
		{
			throw new ArgumentNullException("clsArgument");
		}
		AppendArray(ref arguments, clsArgument);
	}

	private static bool CompareOK(Type[][] one, Type[][] two)
	{
		if (one == null)
		{
			if (two == null)
			{
				return true;
			}
			return false;
		}
		if (two == null)
		{
			return false;
		}
		if (one.Length != two.Length)
		{
			return false;
		}
		for (int i = 0; i < one.Length; i++)
		{
			Type[] array = one[i];
			Type[] array2 = two[i];
			if (array == null)
			{
				if (array2 == null)
				{
					continue;
				}
			}
			else if (array2 == null)
			{
				return false;
			}
			if (array.Length != array2.Length)
			{
				return false;
			}
			for (int j = 0; j < array.Length; j++)
			{
				Type type = array[j];
				Type type2 = array2[j];
				if (type == null)
				{
					if (type2 != null)
					{
						return false;
					}
					continue;
				}
				if (type2 == null)
				{
					return false;
				}
				if (!type.Equals(type2))
				{
					return false;
				}
			}
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is SignatureHelper signatureHelper))
		{
			return false;
		}
		if (signatureHelper.module != module || signatureHelper.returnType != returnType || signatureHelper.callConv != callConv || signatureHelper.unmanagedCallConv != unmanagedCallConv)
		{
			return false;
		}
		if (arguments != null)
		{
			if (signatureHelper.arguments == null)
			{
				return false;
			}
			if (arguments.Length != signatureHelper.arguments.Length)
			{
				return false;
			}
			for (int i = 0; i < arguments.Length; i++)
			{
				if (!signatureHelper.arguments[i].Equals(arguments[i]))
				{
					return false;
				}
			}
		}
		else if (signatureHelper.arguments != null)
		{
			return false;
		}
		return CompareOK(signatureHelper.modreqs, modreqs) && CompareOK(signatureHelper.modopts, modopts);
	}

	public override int GetHashCode()
	{
		return 0;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern byte[] get_signature_local();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern byte[] get_signature_field();

	public byte[] GetSignature()
	{
		return type switch
		{
			SignatureHelperType.HELPER_LOCAL => get_signature_local(), 
			SignatureHelperType.HELPER_FIELD => get_signature_field(), 
			_ => throw new NotImplementedException(), 
		};
	}

	public override string ToString()
	{
		return "SignatureHelper";
	}

	internal static SignatureHelper GetMethodSigHelper(Module mod, CallingConventions callingConvention, CallingConvention unmanagedCallingConvention, Type returnType, Type[] parameters)
	{
		if (mod != null && !(mod is ModuleBuilder))
		{
			throw new ArgumentException("ModuleBuilder is expected");
		}
		if (returnType == null)
		{
			returnType = typeof(void);
		}
		if (returnType.IsUserType)
		{
			throw new NotSupportedException("User defined subclasses of System.Type are not yet supported.");
		}
		if (parameters != null)
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].IsUserType)
				{
					throw new NotSupportedException("User defined subclasses of System.Type are not yet supported.");
				}
			}
		}
		SignatureHelper signatureHelper = new SignatureHelper((ModuleBuilder)mod, SignatureHelperType.HELPER_METHOD);
		signatureHelper.returnType = returnType;
		signatureHelper.callConv = callingConvention;
		signatureHelper.unmanagedCallConv = unmanagedCallingConvention;
		if (parameters != null)
		{
			signatureHelper.arguments = new Type[parameters.Length];
			for (int j = 0; j < parameters.Length; j++)
			{
				signatureHelper.arguments[j] = parameters[j];
			}
		}
		return signatureHelper;
	}
}
