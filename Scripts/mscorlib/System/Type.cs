using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System;

[Serializable]
[ComDefaultInterface(typeof(_Type))]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
public abstract class Type : MemberInfo, IReflect, _Type
{
	internal const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

	internal RuntimeTypeHandle _impl;

	public static readonly char Delimiter = '.';

	public static readonly Type[] EmptyTypes = new Type[0];

	public static readonly MemberFilter FilterAttribute = FilterAttribute_impl;

	public static readonly MemberFilter FilterName = FilterName_impl;

	public static readonly MemberFilter FilterNameIgnoreCase = FilterNameIgnoreCase_impl;

	public static readonly object Missing = System.Reflection.Missing.Value;

	public abstract Assembly Assembly { get; }

	public abstract string AssemblyQualifiedName { get; }

	public TypeAttributes Attributes => GetAttributeFlagsImpl();

	public abstract Type BaseType { get; }

	public override Type DeclaringType => null;

	public abstract string FullName { get; }

	public abstract Guid GUID { get; }

	public bool HasElementType => HasElementTypeImpl();

	public bool IsAbstract => (Attributes & TypeAttributes.Abstract) != 0;

	public bool IsArray => IsArrayImpl();

	public bool IsByRef => IsByRefImpl();

	public bool IsClass
	{
		get
		{
			if (IsInterface)
			{
				return false;
			}
			return !IsValueType;
		}
	}

	public bool IsContextful => IsContextfulImpl();

	public bool IsEnum => IsSubclassOf(typeof(Enum));

	public bool IsExplicitLayout => (Attributes & TypeAttributes.LayoutMask) == TypeAttributes.ExplicitLayout;

	public bool IsImport => (Attributes & TypeAttributes.Import) != 0;

	public bool IsInterface => (Attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.ClassSemanticsMask;

	public bool IsMarshalByRef => IsMarshalByRefImpl();

	public bool IsPointer => IsPointerImpl();

	public bool IsPrimitive => IsPrimitiveImpl();

	public bool IsSealed => (Attributes & TypeAttributes.Sealed) != 0;

	public bool IsSerializable
	{
		get
		{
			if ((Attributes & TypeAttributes.Serializable) != TypeAttributes.NotPublic)
			{
				return true;
			}
			Type type = UnderlyingSystemType;
			if (type == null)
			{
				return false;
			}
			if (type.IsSystemType)
			{
				return type_is_subtype_of(type, typeof(Enum), check_interfaces: false) || type_is_subtype_of(type, typeof(Delegate), check_interfaces: false);
			}
			do
			{
				if (type == typeof(Enum) || type == typeof(Delegate))
				{
					return true;
				}
				type = type.BaseType;
			}
			while (type != null);
			return false;
		}
	}

	public bool IsValueType => IsValueTypeImpl();

	public override MemberTypes MemberType => MemberTypes.TypeInfo;

	public abstract override Module Module { get; }

	public abstract string Namespace { get; }

	public override Type ReflectedType => null;

	public virtual RuntimeTypeHandle TypeHandle => default;

	public abstract Type UnderlyingSystemType { get; }

	internal bool IsSystemType => _impl.Value != IntPtr.Zero;

	public virtual bool ContainsGenericParameters => false;

	public virtual extern bool IsGenericTypeDefinition
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public virtual extern bool IsGenericType
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public virtual bool IsGenericParameter => false;

	public bool IsNested => DeclaringType != null;

	public virtual int GenericParameterPosition
	{
		get
		{
			int genericParameterPosition = GetGenericParameterPosition();
			if (genericParameterPosition < 0)
			{
				throw new InvalidOperationException();
			}
			return genericParameterPosition;
		}
	}

	public virtual MethodBase DeclaringMethod => null;

	internal bool IsUserType => _impl.Value == IntPtr.Zero && (GetType().Assembly != typeof(Type).Assembly || GetType() == typeof(TypeDelegator));

	private static bool FilterName_impl(MemberInfo m, object filterCriteria)
	{
		string text = (string)filterCriteria;
		if (text == null || text.Length == 0)
		{
			return false;
		}
		if (text[text.Length - 1] == '*')
		{
			return string.Compare(text, 0, m.Name, 0, text.Length - 1, ignoreCase: false, CultureInfo.InvariantCulture) == 0;
		}
		return text.Equals(m.Name);
	}

	private static bool FilterNameIgnoreCase_impl(MemberInfo m, object filterCriteria)
	{
		string text = (string)filterCriteria;
		if (text == null || text.Length == 0)
		{
			return false;
		}
		if (text[text.Length - 1] == '*')
		{
			return string.Compare(text, 0, m.Name, 0, text.Length - 1, ignoreCase: true, CultureInfo.InvariantCulture) == 0;
		}
		return string.Compare(text, m.Name, ignoreCase: true, CultureInfo.InvariantCulture) == 0;
	}

	private static bool FilterAttribute_impl(MemberInfo m, object filterCriteria)
	{
		int num = ((IConvertible)filterCriteria).ToInt32(null);
		if (m is MethodInfo)
		{
			return ((uint)((MethodInfo)m).Attributes & (uint)num) != 0;
		}
		if (m is FieldInfo)
		{
			return ((uint)((FieldInfo)m).Attributes & (uint)num) != 0;
		}
		if (m is PropertyInfo)
		{
			return ((uint)((PropertyInfo)m).Attributes & (uint)num) != 0;
		}
		if (m is EventInfo)
		{
			return ((uint)((EventInfo)m).Attributes & (uint)num) != 0;
		}
		return false;
	}

	public override bool Equals(object o)
	{
		if (o == null)
		{
			return false;
		}
		if (!(o is Type o2))
		{
			return false;
		}
		return Equals(o2);
	}

	public bool Equals(Type o)
	{
		if (o == null)
		{
			return false;
		}
		return UnderlyingSystemType.EqualsInternal(o.UnderlyingSystemType);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern bool EqualsInternal(Type type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Type internal_from_handle(IntPtr handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Type internal_from_name(string name, bool throwOnError, bool ignoreCase);

	public static Type GetType(string typeName)
	{
		if (typeName == null)
		{
			throw new ArgumentNullException("TypeName");
		}
		return internal_from_name(typeName, throwOnError: false, ignoreCase: false);
	}

	public static Type GetType(string typeName, bool throwOnError)
	{
		if (typeName == null)
		{
			throw new ArgumentNullException("TypeName");
		}
		Type type = internal_from_name(typeName, throwOnError, ignoreCase: false);
		if (throwOnError && type == null)
		{
			throw new TypeLoadException("Error loading '" + typeName + "'");
		}
		return type;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern TypeCode GetTypeCodeInternal(Type type);

	public static TypeCode GetTypeCode(Type type)
	{
		if (type is MonoType)
		{
			return GetTypeCodeInternal(type);
		}
		if (type == null)
		{
			return TypeCode.Empty;
		}
		type = type.UnderlyingSystemType;
		if (!type.IsSystemType)
		{
			return TypeCode.Object;
		}
		return GetTypeCodeInternal(type);
	}

	public static Type GetTypeFromHandle(RuntimeTypeHandle handle)
	{
		if (handle.Value == IntPtr.Zero)
		{
			return null;
		}
		return internal_from_handle(handle.Value);
	}

	public static RuntimeTypeHandle GetTypeHandle(object o)
	{
		if (o == null)
		{
			throw new ArgumentNullException();
		}
		return o.GetType().TypeHandle;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool type_is_subtype_of(Type a, Type b, bool check_interfaces);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool type_is_assignable_from(Type a, Type b);

	public new Type GetType()
	{
		return base.GetType();
	}

	[ComVisible(true)]
	public virtual bool IsSubclassOf(Type c)
	{
		if (c == null || c == this)
		{
			return false;
		}
		if (IsSystemType)
		{
			return c.IsSystemType && type_is_subtype_of(this, c, check_interfaces: false);
		}
		for (Type baseType = BaseType; baseType != null; baseType = baseType.BaseType)
		{
			if (baseType == c)
			{
				return true;
			}
		}
		return false;
	}

	public Type GetInterface(string name)
	{
		return GetInterface(name, ignoreCase: false);
	}

	public abstract Type GetInterface(string name, bool ignoreCase);

	public abstract Type[] GetInterfaces();

	public virtual bool IsAssignableFrom(Type c)
	{
		if (c == null)
		{
			return false;
		}
		if (Equals(c))
		{
			return true;
		}
		if (c is TypeBuilder)
		{
			return ((TypeBuilder)c).IsAssignableTo(this);
		}
		if (!IsSystemType)
		{
			Type underlyingSystemType = UnderlyingSystemType;
			if (!underlyingSystemType.IsSystemType)
			{
				return false;
			}
			return underlyingSystemType.IsAssignableFrom(c);
		}
		if (!c.IsSystemType)
		{
			Type underlyingSystemType2 = c.UnderlyingSystemType;
			if (!underlyingSystemType2.IsSystemType)
			{
				return false;
			}
			return IsAssignableFrom(underlyingSystemType2);
		}
		return type_is_assignable_from(this, c);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public virtual extern bool IsInstanceOfType(object o);

	public virtual int GetArrayRank()
	{
		throw new NotSupportedException();
	}

	public abstract Type GetElementType();

	public abstract EventInfo GetEvent(string name, BindingFlags bindingAttr);

	public abstract EventInfo[] GetEvents(BindingFlags bindingAttr);

	public FieldInfo GetField(string name)
	{
		return GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
	}

	public abstract FieldInfo GetField(string name, BindingFlags bindingAttr);

	public abstract FieldInfo[] GetFields(BindingFlags bindingAttr);

	public override int GetHashCode()
	{
		Type underlyingSystemType = UnderlyingSystemType;
		if (underlyingSystemType != null && underlyingSystemType != this)
		{
			return underlyingSystemType.GetHashCode();
		}
		return (int)_impl.Value;
	}

	public virtual MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if ((bindingAttr & BindingFlags.IgnoreCase) != BindingFlags.Default)
		{
			return FindMembers(type, bindingAttr, FilterNameIgnoreCase, name);
		}
		return FindMembers(type, bindingAttr, FilterName, name);
	}

	public MethodInfo GetMethod(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		return GetMethodImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, null, null);
	}

	public MethodInfo GetMethod(string name, BindingFlags bindingAttr)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		return GetMethodImpl(name, bindingAttr, null, CallingConventions.Any, null, null);
	}

	public MethodInfo GetMethod(string name, Type[] types)
	{
		return GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, types, null);
	}

	public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
	{
		return GetMethod(name, bindingAttr, binder, CallingConventions.Any, types, modifiers);
	}

	public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (types == null)
		{
			throw new ArgumentNullException("types");
		}
		for (int i = 0; i < types.Length; i++)
		{
			if (types[i] == null)
			{
				throw new ArgumentNullException("types");
			}
		}
		return GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers);
	}

	protected abstract MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers);

	internal MethodInfo GetMethodImplInternal(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		return GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers);
	}

	internal virtual MethodInfo GetMethod(MethodInfo fromNoninstanciated)
	{
		throw new InvalidOperationException("can only be called in generic type");
	}

	internal virtual ConstructorInfo GetConstructor(ConstructorInfo fromNoninstanciated)
	{
		throw new InvalidOperationException("can only be called in generic type");
	}

	internal virtual FieldInfo GetField(FieldInfo fromNoninstanciated)
	{
		throw new InvalidOperationException("can only be called in generic type");
	}

	public MethodInfo[] GetMethods()
	{
		return GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
	}

	public abstract MethodInfo[] GetMethods(BindingFlags bindingAttr);

	public abstract Type[] GetNestedTypes(BindingFlags bindingAttr);

	public abstract PropertyInfo[] GetProperties(BindingFlags bindingAttr);

	public PropertyInfo GetProperty(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		return GetPropertyImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, null, null, null);
	}

	public PropertyInfo GetProperty(string name, BindingFlags bindingAttr)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		return GetPropertyImpl(name, bindingAttr, null, null, null, null);
	}

	public PropertyInfo GetProperty(string name, Type returnType)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		return GetPropertyImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, returnType, null, null);
	}

	public PropertyInfo GetProperty(string name, Type returnType, Type[] types)
	{
		return GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, returnType, types, null);
	}

	public PropertyInfo GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (types == null)
		{
			throw new ArgumentNullException("types");
		}
		foreach (Type type in types)
		{
			if (type == null)
			{
				throw new ArgumentNullException("types");
			}
		}
		return GetPropertyImpl(name, bindingAttr, binder, returnType, types, modifiers);
	}

	protected abstract PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers);

	internal PropertyInfo GetPropertyImplInternal(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
	{
		return GetPropertyImpl(name, bindingAttr, binder, returnType, types, modifiers);
	}

	protected abstract ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers);

	protected abstract TypeAttributes GetAttributeFlagsImpl();

	protected abstract bool HasElementTypeImpl();

	protected abstract bool IsArrayImpl();

	protected abstract bool IsByRefImpl();

	protected abstract bool IsPointerImpl();

	protected abstract bool IsPrimitiveImpl();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool IsArrayImpl(Type type);

	protected virtual bool IsValueTypeImpl()
	{
		if (this == typeof(ValueType) || this == typeof(Enum))
		{
			return false;
		}
		return IsSubclassOf(typeof(ValueType));
	}

	protected virtual bool IsContextfulImpl()
	{
		return typeof(ContextBoundObject).IsAssignableFrom(this);
	}

	protected virtual bool IsMarshalByRefImpl()
	{
		return typeof(MarshalByRefObject).IsAssignableFrom(this);
	}

	[ComVisible(true)]
	public ConstructorInfo GetConstructor(Type[] types)
	{
		return GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any, types, null);
	}

	[ComVisible(true)]
	public ConstructorInfo GetConstructor(BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
	{
		return GetConstructor(bindingAttr, binder, CallingConventions.Any, types, modifiers);
	}

	[ComVisible(true)]
	public ConstructorInfo GetConstructor(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		if (types == null)
		{
			throw new ArgumentNullException("types");
		}
		foreach (Type type in types)
		{
			if (type == null)
			{
				throw new ArgumentNullException("types");
			}
		}
		return GetConstructorImpl(bindingAttr, binder, callConvention, types, modifiers);
	}

	[ComVisible(true)]
	public ConstructorInfo[] GetConstructors()
	{
		return GetConstructors(BindingFlags.Instance | BindingFlags.Public);
	}

	[ComVisible(true)]
	public abstract ConstructorInfo[] GetConstructors(BindingFlags bindingAttr);

	public virtual MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria)
	{
		ArrayList arrayList = new ArrayList();
		if ((memberType & MemberTypes.Method) != 0)
		{
			MethodInfo[] methods = GetMethods(bindingAttr);
			if (filter != null)
			{
				MethodInfo[] array = methods;
				foreach (MemberInfo memberInfo in array)
				{
					if (filter(memberInfo, filterCriteria))
					{
						arrayList.Add(memberInfo);
					}
				}
			}
			else
			{
				arrayList.AddRange(methods);
			}
		}
		if ((memberType & MemberTypes.Constructor) != 0)
		{
			ConstructorInfo[] constructors = GetConstructors(bindingAttr);
			if (filter != null)
			{
				ConstructorInfo[] array2 = constructors;
				foreach (MemberInfo memberInfo2 in array2)
				{
					if (filter(memberInfo2, filterCriteria))
					{
						arrayList.Add(memberInfo2);
					}
				}
			}
			else
			{
				arrayList.AddRange(constructors);
			}
		}
		if ((memberType & MemberTypes.Property) != 0)
		{
			int count = arrayList.Count;
			if (filter != null)
			{
				Type type = this;
				while (arrayList.Count == count && type != null)
				{
					PropertyInfo[] properties = type.GetProperties(bindingAttr);
					PropertyInfo[] array3 = properties;
					foreach (MemberInfo memberInfo3 in array3)
					{
						if (filter(memberInfo3, filterCriteria))
						{
							arrayList.Add(memberInfo3);
						}
					}
					type = type.BaseType;
				}
			}
			else
			{
				PropertyInfo[] properties = GetProperties(bindingAttr);
				arrayList.AddRange(properties);
			}
		}
		if ((memberType & MemberTypes.Event) != 0)
		{
			EventInfo[] events = GetEvents(bindingAttr);
			if (filter != null)
			{
				EventInfo[] array4 = events;
				foreach (MemberInfo memberInfo4 in array4)
				{
					if (filter(memberInfo4, filterCriteria))
					{
						arrayList.Add(memberInfo4);
					}
				}
			}
			else
			{
				arrayList.AddRange(events);
			}
		}
		if ((memberType & MemberTypes.Field) != 0)
		{
			FieldInfo[] fields = GetFields(bindingAttr);
			if (filter != null)
			{
				FieldInfo[] array5 = fields;
				foreach (MemberInfo memberInfo5 in array5)
				{
					if (filter(memberInfo5, filterCriteria))
					{
						arrayList.Add(memberInfo5);
					}
				}
			}
			else
			{
				arrayList.AddRange(fields);
			}
		}
		if ((memberType & MemberTypes.NestedType) != 0)
		{
			Type[] nestedTypes = GetNestedTypes(bindingAttr);
			if (filter != null)
			{
				Type[] array6 = nestedTypes;
				foreach (MemberInfo memberInfo6 in array6)
				{
					if (filter(memberInfo6, filterCriteria))
					{
						arrayList.Add(memberInfo6);
					}
				}
			}
			else
			{
				arrayList.AddRange(nestedTypes);
			}
		}
		MemberInfo[] array7;
		switch (memberType)
		{
		case MemberTypes.Constructor:
			array7 = new ConstructorInfo[arrayList.Count];
			break;
		case MemberTypes.Event:
			array7 = new EventInfo[arrayList.Count];
			break;
		case MemberTypes.Field:
			array7 = new FieldInfo[arrayList.Count];
			break;
		case MemberTypes.Method:
			array7 = new MethodInfo[arrayList.Count];
			break;
		case MemberTypes.TypeInfo:
		case MemberTypes.NestedType:
			array7 = new Type[arrayList.Count];
			break;
		case MemberTypes.Property:
			array7 = new PropertyInfo[arrayList.Count];
			break;
		default:
			array7 = new MemberInfo[arrayList.Count];
			break;
		}
		arrayList.CopyTo(array7);
		return array7;
	}

	public abstract object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters);

	public override string ToString()
	{
		return FullName;
	}

	public virtual Type[] GetGenericArguments()
	{
		throw new NotSupportedException();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern Type GetGenericTypeDefinition_impl();

	public virtual Type GetGenericTypeDefinition()
	{
		throw new NotSupportedException("Derived classes must provide an implementation.");
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Type MakeGenericType(Type gt, Type[] types);

	public virtual Type MakeGenericType(params Type[] typeArguments)
	{
		if (!IsGenericTypeDefinition)
		{
			throw new InvalidOperationException("not a generic type definition");
		}
		if (typeArguments == null)
		{
			throw new ArgumentNullException("typeArguments");
		}
		if (GetGenericArguments().Length != typeArguments.Length)
		{
			throw new ArgumentException($"The type or method has {GetGenericArguments().Length} generic parameter(s) but {typeArguments.Length} generic argument(s) where provided. A generic argument must be provided for each generic parameter.", "typeArguments");
		}
		Type[] array = new Type[typeArguments.Length];
		for (int i = 0; i < typeArguments.Length; i++)
		{
			Type type = typeArguments[i];
			if (type == null)
			{
				throw new ArgumentNullException("typeArguments");
			}
			if (!(type is EnumBuilder) && !(type is TypeBuilder))
			{
				type = type.UnderlyingSystemType;
			}
			if (type == null || !type.IsSystemType)
			{
				throw new ArgumentNullException("typeArguments");
			}
			array[i] = type;
		}
		Type type2 = MakeGenericType(this, array);
		if (type2 == null)
		{
			throw new TypeLoadException();
		}
		return type2;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetGenericParameterPosition();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern Type make_array_type(int rank);

	public virtual Type MakeArrayType()
	{
		return make_array_type(0);
	}

	public virtual Type MakeArrayType(int rank)
	{
		if (rank < 1)
		{
			throw new IndexOutOfRangeException();
		}
		return make_array_type(rank);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern Type make_byref_type();

	public virtual Type MakeByRefType()
	{
		return make_byref_type();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public virtual extern Type MakePointerType();

	internal object[] GetPseudoCustomAttributes()
	{
		int num = 0;
		if ((Attributes & TypeAttributes.Serializable) != TypeAttributes.NotPublic)
		{
			num++;
		}
		if ((Attributes & TypeAttributes.Import) != TypeAttributes.NotPublic)
		{
			num++;
		}
		if (num == 0)
		{
			return null;
		}
		object[] array = new object[num];
		num = 0;
		if ((Attributes & TypeAttributes.Serializable) != TypeAttributes.NotPublic)
		{
			array[num++] = new SerializableAttribute();
		}
		if ((Attributes & TypeAttributes.Import) != TypeAttributes.NotPublic)
		{
			array[num++] = new ComImportAttribute();
		}
		return array;
	}
}
