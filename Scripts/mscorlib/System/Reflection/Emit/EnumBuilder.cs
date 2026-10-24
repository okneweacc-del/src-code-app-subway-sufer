using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

[ClassInterface(ClassInterfaceType.None)]
[ComVisible(true)]
[ComDefaultInterface(typeof(_EnumBuilder))]
public sealed class EnumBuilder : Type, _EnumBuilder
{
	private TypeBuilder _tb;

	private FieldBuilder _underlyingField;

	private Type _underlyingType;

	public override Assembly Assembly => _tb.Assembly;

	public override string AssemblyQualifiedName => _tb.AssemblyQualifiedName;

	public override Type BaseType => _tb.BaseType;

	public override Type DeclaringType => _tb.DeclaringType;

	public override string FullName => _tb.FullName;

	public override Guid GUID => _tb.GUID;

	public override Module Module => _tb.Module;

	public override string Name => _tb.Name;

	public override string Namespace => _tb.Namespace;

	public override Type ReflectedType => _tb.ReflectedType;

	public override RuntimeTypeHandle TypeHandle => _tb.TypeHandle;

	public override Type UnderlyingSystemType => _underlyingType;

	internal EnumBuilder(ModuleBuilder mb, string name, TypeAttributes visibility, Type underlyingType)
	{
		_tb = new TypeBuilder(mb, name, visibility | TypeAttributes.Sealed, typeof(Enum), null, PackingSize.Unspecified, 0, null);
		_underlyingType = underlyingType;
		_underlyingField = _tb.DefineField("value__", underlyingType, FieldAttributes.Private | FieldAttributes.SpecialName | FieldAttributes.RTSpecialName);
		setup_enum_type(_tb);
	}

	internal TypeBuilder GetTypeBuilder()
	{
		return _tb;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void setup_enum_type(Type t);

	protected override TypeAttributes GetAttributeFlagsImpl()
	{
		return _tb.attrs;
	}

	protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		return _tb.GetConstructor(bindingAttr, binder, callConvention, types, modifiers);
	}

	[ComVisible(true)]
	public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
	{
		return _tb.GetConstructors(bindingAttr);
	}

	public override object[] GetCustomAttributes(bool inherit)
	{
		return _tb.GetCustomAttributes(inherit);
	}

	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		return _tb.GetCustomAttributes(attributeType, inherit);
	}

	public override Type GetElementType()
	{
		return _tb.GetElementType();
	}

	public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
	{
		return _tb.GetEvent(name, bindingAttr);
	}

	public override EventInfo[] GetEvents(BindingFlags bindingAttr)
	{
		return _tb.GetEvents(bindingAttr);
	}

	public override FieldInfo GetField(string name, BindingFlags bindingAttr)
	{
		return _tb.GetField(name, bindingAttr);
	}

	public override FieldInfo[] GetFields(BindingFlags bindingAttr)
	{
		return _tb.GetFields(bindingAttr);
	}

	public override Type GetInterface(string name, bool ignoreCase)
	{
		return _tb.GetInterface(name, ignoreCase);
	}

	public override Type[] GetInterfaces()
	{
		return _tb.GetInterfaces();
	}

	public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
	{
		return _tb.GetMember(name, type, bindingAttr);
	}

	protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		if (types == null)
		{
			return _tb.GetMethod(name, bindingAttr);
		}
		return _tb.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
	}

	public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
	{
		return _tb.GetMethods(bindingAttr);
	}

	public override Type[] GetNestedTypes(BindingFlags bindingAttr)
	{
		return _tb.GetNestedTypes(bindingAttr);
	}

	public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
	{
		return _tb.GetProperties(bindingAttr);
	}

	protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
	{
		throw CreateNotSupportedException();
	}

	protected override bool HasElementTypeImpl()
	{
		return _tb.HasElementType;
	}

	public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
	{
		return _tb.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
	}

	protected override bool IsArrayImpl()
	{
		return false;
	}

	protected override bool IsByRefImpl()
	{
		return false;
	}

	protected override bool IsPointerImpl()
	{
		return false;
	}

	protected override bool IsPrimitiveImpl()
	{
		return false;
	}

	protected override bool IsValueTypeImpl()
	{
		return true;
	}

	public override bool IsDefined(Type attributeType, bool inherit)
	{
		return _tb.IsDefined(attributeType, inherit);
	}

	public override Type MakeArrayType()
	{
		return new ArrayType(this, 0);
	}

	public override Type MakeArrayType(int rank)
	{
		if (rank < 1)
		{
			throw new IndexOutOfRangeException();
		}
		return new ArrayType(this, rank);
	}

	public override Type MakeByRefType()
	{
		return new ByRefType(this);
	}

	public override Type MakePointerType()
	{
		return new PointerType(this);
	}

	private Exception CreateNotSupportedException()
	{
		return new NotSupportedException("The invoked member is not supported in a dynamic module.");
	}
}
