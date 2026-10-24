using System.Collections;
using System.Diagnostics.SymbolStore;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

[ComDefaultInterface(typeof(_TypeBuilder))]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
public sealed class TypeBuilder : Type, _TypeBuilder
{
	public const int UnspecifiedTypeSize = 0;

	private string tname;

	private string nspace;

	private Type parent;

	private Type nesting_type;

	internal Type[] interfaces;

	internal int num_methods;

	internal MethodBuilder[] methods;

	internal ConstructorBuilder[] ctors;

	internal PropertyBuilder[] properties;

	internal int num_fields;

	internal FieldBuilder[] fields;

	internal EventBuilder[] events;

	private CustomAttributeBuilder[] cattrs;

	internal TypeBuilder[] subtypes;

	internal TypeAttributes attrs;

	private int table_idx;

	private ModuleBuilder pmodule;

	private int class_size;

	private PackingSize packing_size;

	private IntPtr generic_container;

	private GenericTypeParameterBuilder[] generic_params;

	private RefEmitPermissionSet[] permissions;

	private Type created;

	private string fullname;

	private bool createTypeCalled;

	private Type underlying_type;

	public override Assembly Assembly => pmodule.Assembly;

	public override string AssemblyQualifiedName => fullname + ", " + Assembly.FullName;

	public override Type BaseType => parent;

	public override Type DeclaringType => nesting_type;

	public override Type UnderlyingSystemType
	{
		get
		{
			if (is_created)
			{
				return created.UnderlyingSystemType;
			}
			if (IsEnum && !IsCompilerContext)
			{
				if (underlying_type != null)
				{
					return underlying_type;
				}
				throw new InvalidOperationException("Enumeration type is not defined.");
			}
			return this;
		}
	}

	public override string FullName => fullname;

	public override Guid GUID
	{
		get
		{
			check_created();
			return created.GUID;
		}
	}

	public override Module Module => pmodule;

	public override string Name => tname;

	public override string Namespace => nspace;

	public override Type ReflectedType => nesting_type;

	public override RuntimeTypeHandle TypeHandle
	{
		get
		{
			check_created();
			return created.TypeHandle;
		}
	}

	internal bool IsCompilerContext => pmodule.assemblyb.IsCompilerContext;

	internal bool is_created => created != null;

	public override bool ContainsGenericParameters => generic_params != null;

	public override extern bool IsGenericParameter
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public override bool IsGenericTypeDefinition => generic_params != null;

	public override bool IsGenericType => IsGenericTypeDefinition;

	[MonoTODO]
	public override int GenericParameterPosition => 0;

	public override MethodBase DeclaringMethod => null;

	internal TypeBuilder(ModuleBuilder mb, TypeAttributes attr, int table_idx)
	{
		parent = null;
		attrs = attr;
		class_size = 0;
		this.table_idx = table_idx;
		fullname = (tname = ((table_idx != 1) ? ("type_" + table_idx) : "<Module>"));
		nspace = string.Empty;
		pmodule = mb;
		setup_internal_class(this);
	}

	internal TypeBuilder(ModuleBuilder mb, string name, TypeAttributes attr, Type parent, Type[] interfaces, PackingSize packing_size, int type_size, Type nesting_type)
	{
		this.parent = parent;
		attrs = attr;
		class_size = type_size;
		this.packing_size = packing_size;
		this.nesting_type = nesting_type;
		check_name("fullname", name);
		if (parent == null && (attr & TypeAttributes.ClassSemanticsMask) != TypeAttributes.NotPublic && (attr & TypeAttributes.Abstract) == 0)
		{
			throw new InvalidOperationException("Interface must be declared abstract.");
		}
		int num = name.LastIndexOf('.');
		if (num != -1)
		{
			tname = name.Substring(num + 1);
			nspace = name.Substring(0, num);
		}
		else
		{
			tname = name;
			nspace = string.Empty;
		}
		if (interfaces != null)
		{
			this.interfaces = new Type[interfaces.Length];
			Array.Copy(interfaces, this.interfaces, interfaces.Length);
		}
		pmodule = mb;
		if ((attr & TypeAttributes.ClassSemanticsMask) == 0 && parent == null && !IsCompilerContext)
		{
			this.parent = typeof(object);
		}
		table_idx = mb.get_next_table_index(this, 2, inc: true);
		setup_internal_class(this);
		fullname = GetFullName();
	}

	protected override TypeAttributes GetAttributeFlagsImpl()
	{
		return attrs;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void setup_internal_class(TypeBuilder tb);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void create_internal_class(TypeBuilder tb);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void create_generic_class();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern EventInfo get_event_info(EventBuilder eb);

	private string GetFullName()
	{
		if (nesting_type != null)
		{
			return nesting_type.FullName + "+" + tname;
		}
		if (nspace != null && nspace.Length > 0)
		{
			return nspace + "." + tname;
		}
		return tname;
	}

	protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		check_created();
		if (created == typeof(object))
		{
			if (ctors == null)
			{
				return null;
			}
			ConstructorBuilder constructorBuilder = null;
			int num = 0;
			ConstructorBuilder[] array = ctors;
			foreach (ConstructorBuilder constructorBuilder2 in array)
			{
				if (callConvention == CallingConventions.Any || constructorBuilder2.CallingConvention == callConvention)
				{
					constructorBuilder = constructorBuilder2;
					num++;
				}
			}
			if (num == 0)
			{
				return null;
			}
			if (types == null)
			{
				if (num > 1)
				{
					throw new AmbiguousMatchException();
				}
				return constructorBuilder;
			}
			MethodBase[] array2 = new MethodBase[num];
			if (num == 1)
			{
				array2[0] = constructorBuilder;
			}
			else
			{
				num = 0;
				ConstructorBuilder[] array3 = ctors;
				foreach (ConstructorInfo constructorInfo in array3)
				{
					if (callConvention == CallingConventions.Any || constructorInfo.CallingConvention == callConvention)
					{
						array2[num++] = constructorInfo;
					}
				}
			}
			if (binder == null)
			{
				binder = Binder.DefaultBinder;
			}
			return (ConstructorInfo)binder.SelectMethod(bindingAttr, array2, types, modifiers);
		}
		return created.GetConstructor(bindingAttr, binder, callConvention, types, modifiers);
	}

	public override bool IsDefined(Type attributeType, bool inherit)
	{
		if (!is_created && !IsCompilerContext)
		{
			throw new NotSupportedException();
		}
		return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
	}

	public override object[] GetCustomAttributes(bool inherit)
	{
		check_created();
		return created.GetCustomAttributes(inherit);
	}

	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		check_created();
		return created.GetCustomAttributes(attributeType, inherit);
	}

	[ComVisible(true)]
	public ConstructorBuilder DefineConstructor(MethodAttributes attributes, CallingConventions callingConvention, Type[] parameterTypes)
	{
		return DefineConstructor(attributes, callingConvention, parameterTypes, null, null);
	}

	[ComVisible(true)]
	public ConstructorBuilder DefineConstructor(MethodAttributes attributes, CallingConventions callingConvention, Type[] parameterTypes, Type[][] requiredCustomModifiers, Type[][] optionalCustomModifiers)
	{
		check_not_created();
		ConstructorBuilder constructorBuilder = new ConstructorBuilder(this, attributes, callingConvention, parameterTypes, requiredCustomModifiers, optionalCustomModifiers);
		if (ctors != null)
		{
			ConstructorBuilder[] array = new ConstructorBuilder[ctors.Length + 1];
			Array.Copy(ctors, array, ctors.Length);
			array[ctors.Length] = constructorBuilder;
			ctors = array;
		}
		else
		{
			ctors = new ConstructorBuilder[1];
			ctors[0] = constructorBuilder;
		}
		return constructorBuilder;
	}

	[ComVisible(true)]
	public ConstructorBuilder DefineDefaultConstructor(MethodAttributes attributes)
	{
		Type type = ((parent == null) ? pmodule.assemblyb.corlib_object_type : parent);
		ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
		if (constructor == null)
		{
			throw new NotSupportedException("Parent does not have a default constructor. The default constructor must be explicitly defined.");
		}
		ConstructorBuilder constructorBuilder = DefineConstructor(attributes, CallingConventions.Standard, Type.EmptyTypes);
		ILGenerator iLGenerator = constructorBuilder.GetILGenerator();
		iLGenerator.Emit(OpCodes.Ldarg_0);
		iLGenerator.Emit(OpCodes.Call, constructor);
		iLGenerator.Emit(OpCodes.Ret);
		return constructorBuilder;
	}

	private void append_method(MethodBuilder mb)
	{
		if (methods != null)
		{
			if (methods.Length == num_methods)
			{
				MethodBuilder[] destinationArray = new MethodBuilder[methods.Length * 2];
				Array.Copy(methods, destinationArray, num_methods);
				methods = destinationArray;
			}
		}
		else
		{
			methods = new MethodBuilder[1];
		}
		methods[num_methods] = mb;
		num_methods++;
	}

	public MethodBuilder DefineMethod(string name, MethodAttributes attributes, Type returnType, Type[] parameterTypes)
	{
		return DefineMethod(name, attributes, CallingConventions.Standard, returnType, parameterTypes);
	}

	public MethodBuilder DefineMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
	{
		return DefineMethod(name, attributes, callingConvention, returnType, null, null, parameterTypes, null, null);
	}

	public MethodBuilder DefineMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] returnTypeRequiredCustomModifiers, Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes, Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers)
	{
		check_name("name", name);
		check_not_created();
		if (IsInterface && ((attributes & MethodAttributes.Abstract) == 0 || (attributes & MethodAttributes.Virtual) == 0) && (attributes & MethodAttributes.Static) == 0)
		{
			throw new ArgumentException("Interface method must be abstract and virtual.");
		}
		if (returnType == null)
		{
			returnType = pmodule.assemblyb.corlib_void_type;
		}
		MethodBuilder methodBuilder = new MethodBuilder(this, name, attributes, callingConvention, returnType, returnTypeRequiredCustomModifiers, returnTypeOptionalCustomModifiers, parameterTypes, parameterTypeRequiredCustomModifiers, parameterTypeOptionalCustomModifiers);
		append_method(methodBuilder);
		return methodBuilder;
	}

	public MethodBuilder DefinePInvokeMethod(string name, string dllName, string entryName, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, CallingConvention nativeCallConv, CharSet nativeCharSet)
	{
		return DefinePInvokeMethod(name, dllName, entryName, attributes, callingConvention, returnType, null, null, parameterTypes, null, null, nativeCallConv, nativeCharSet);
	}

	public MethodBuilder DefinePInvokeMethod(string name, string dllName, string entryName, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] returnTypeRequiredCustomModifiers, Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes, Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers, CallingConvention nativeCallConv, CharSet nativeCharSet)
	{
		check_name("name", name);
		check_name("dllName", dllName);
		check_name("entryName", entryName);
		if ((attributes & MethodAttributes.Abstract) != MethodAttributes.PrivateScope)
		{
			throw new ArgumentException("PInvoke methods must be static and native and cannot be abstract.");
		}
		if (IsInterface)
		{
			throw new ArgumentException("PInvoke methods cannot exist on interfaces.");
		}
		check_not_created();
		MethodBuilder methodBuilder = new MethodBuilder(this, name, attributes, callingConvention, returnType, returnTypeRequiredCustomModifiers, returnTypeOptionalCustomModifiers, parameterTypes, parameterTypeRequiredCustomModifiers, parameterTypeOptionalCustomModifiers, dllName, entryName, nativeCallConv, nativeCharSet);
		append_method(methodBuilder);
		return methodBuilder;
	}

	public void DefineMethodOverride(MethodInfo methodInfoBody, MethodInfo methodInfoDeclaration)
	{
		if (methodInfoBody == null)
		{
			throw new ArgumentNullException("methodInfoBody");
		}
		if (methodInfoDeclaration == null)
		{
			throw new ArgumentNullException("methodInfoDeclaration");
		}
		check_not_created();
		if (methodInfoBody.DeclaringType != this)
		{
			throw new ArgumentException("method body must belong to this type");
		}
		if (methodInfoBody is MethodBuilder)
		{
			MethodBuilder methodBuilder = (MethodBuilder)methodInfoBody;
			methodBuilder.set_override(methodInfoDeclaration);
		}
	}

	public FieldBuilder DefineField(string fieldName, Type type, FieldAttributes attributes)
	{
		return DefineField(fieldName, type, null, null, attributes);
	}

	public FieldBuilder DefineField(string fieldName, Type type, Type[] requiredCustomModifiers, Type[] optionalCustomModifiers, FieldAttributes attributes)
	{
		check_name("fieldName", fieldName);
		if (type == typeof(void))
		{
			throw new ArgumentException("Bad field type in defining field.");
		}
		check_not_created();
		FieldBuilder fieldBuilder = new FieldBuilder(this, fieldName, type, attributes, requiredCustomModifiers, optionalCustomModifiers);
		if (fields != null)
		{
			if (fields.Length == num_fields)
			{
				FieldBuilder[] destinationArray = new FieldBuilder[fields.Length * 2];
				Array.Copy(fields, destinationArray, num_fields);
				fields = destinationArray;
			}
			fields[num_fields] = fieldBuilder;
			num_fields++;
		}
		else
		{
			fields = new FieldBuilder[1];
			fields[0] = fieldBuilder;
			num_fields++;
			create_internal_class(this);
		}
		if (IsEnum && !IsCompilerContext && underlying_type == null && (attributes & FieldAttributes.Static) == 0)
		{
			underlying_type = type;
		}
		return fieldBuilder;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern Type create_runtime_class(TypeBuilder tb);

	private bool is_nested_in(Type t)
	{
		while (t != null)
		{
			if (t == this)
			{
				return true;
			}
			t = t.DeclaringType;
		}
		return false;
	}

	private bool has_ctor_method()
	{
		MethodAttributes methodAttributes = MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
		for (int i = 0; i < num_methods; i++)
		{
			MethodBuilder methodBuilder = methods[i];
			if (methodBuilder.Name == ConstructorInfo.ConstructorName && (methodBuilder.Attributes & methodAttributes) == methodAttributes)
			{
				return true;
			}
		}
		return false;
	}

	public Type CreateType()
	{
		if (createTypeCalled)
		{
			return created;
		}
		if (!IsInterface && parent == null && this != pmodule.assemblyb.corlib_object_type && FullName != "<Module>")
		{
			SetParent(pmodule.assemblyb.corlib_object_type);
		}
		create_generic_class();
		if (fields != null)
		{
			FieldBuilder[] array = fields;
			foreach (FieldBuilder fieldBuilder in array)
			{
				if (fieldBuilder == null)
				{
					continue;
				}
				Type fieldType = fieldBuilder.FieldType;
				if (fieldBuilder.IsStatic || !(fieldType is TypeBuilder) || !fieldType.IsValueType || fieldType == this || !is_nested_in(fieldType))
				{
					continue;
				}
				TypeBuilder typeBuilder = (TypeBuilder)fieldType;
				if (!typeBuilder.is_created)
				{
					AppDomain.CurrentDomain.DoTypeResolve(typeBuilder);
					if (typeBuilder.is_created)
					{
					}
				}
			}
		}
		if (parent != null && parent.IsSealed)
		{
			throw new TypeLoadException(string.Concat("Could not load type '", FullName, "' from assembly '", Assembly, "' because the parent type is sealed."));
		}
		if (parent == pmodule.assemblyb.corlib_enum_type && methods != null)
		{
			throw new TypeLoadException(string.Concat("Could not load type '", FullName, "' from assembly '", Assembly, "' because it is an enum with methods."));
		}
		if (methods != null)
		{
			bool flag = !IsAbstract;
			for (int j = 0; j < num_methods; j++)
			{
				MethodBuilder methodBuilder = methods[j];
				if (flag && methodBuilder.IsAbstract)
				{
					throw new InvalidOperationException("Type is concrete but has abstract method " + methodBuilder);
				}
				methodBuilder.check_override();
				methodBuilder.fixup();
			}
		}
		if (!IsInterface && !IsValueType && ctors == null && tname != "<Module>" && ((GetAttributeFlagsImpl() & TypeAttributes.Abstract) | TypeAttributes.Sealed) != (TypeAttributes.Abstract | TypeAttributes.Sealed) && !has_ctor_method())
		{
			DefineDefaultConstructor(MethodAttributes.Public);
		}
		if (ctors != null)
		{
			ConstructorBuilder[] array2 = ctors;
			foreach (ConstructorBuilder constructorBuilder in array2)
			{
				constructorBuilder.fixup();
			}
		}
		createTypeCalled = true;
		created = create_runtime_class(this);
		if (created != null)
		{
			return created;
		}
		return this;
	}

	internal void GenerateDebugInfo(ISymbolWriter symbolWriter)
	{
		symbolWriter.OpenNamespace(Namespace);
		if (methods != null)
		{
			for (int i = 0; i < num_methods; i++)
			{
				MethodBuilder methodBuilder = methods[i];
				methodBuilder.GenerateDebugInfo(symbolWriter);
			}
		}
		if (ctors != null)
		{
			ConstructorBuilder[] array = ctors;
			foreach (ConstructorBuilder constructorBuilder in array)
			{
				constructorBuilder.GenerateDebugInfo(symbolWriter);
			}
		}
		symbolWriter.CloseNamespace();
		if (subtypes != null)
		{
			for (int k = 0; k < subtypes.Length; k++)
			{
				subtypes[k].GenerateDebugInfo(symbolWriter);
			}
		}
	}

	[ComVisible(true)]
	public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
	{
		if (is_created)
		{
			return created.GetConstructors(bindingAttr);
		}
		if (!IsCompilerContext)
		{
			throw new NotSupportedException();
		}
		return GetConstructorsInternal(bindingAttr);
	}

	internal ConstructorInfo[] GetConstructorsInternal(BindingFlags bindingAttr)
	{
		if (ctors == null)
		{
			return new ConstructorInfo[0];
		}
		ArrayList arrayList = new ArrayList();
		ConstructorBuilder[] array = ctors;
		foreach (ConstructorBuilder constructorBuilder in array)
		{
			bool flag = false;
			MethodAttributes attributes = constructorBuilder.Attributes;
			if ((attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public)
			{
				if ((bindingAttr & BindingFlags.Public) != BindingFlags.Default)
				{
					flag = true;
				}
			}
			else if ((bindingAttr & BindingFlags.NonPublic) != BindingFlags.Default)
			{
				flag = true;
			}
			if (!flag)
			{
				continue;
			}
			flag = false;
			if ((attributes & MethodAttributes.Static) != MethodAttributes.PrivateScope)
			{
				if ((bindingAttr & BindingFlags.Static) != BindingFlags.Default)
				{
					flag = true;
				}
			}
			else if ((bindingAttr & BindingFlags.Instance) != BindingFlags.Default)
			{
				flag = true;
			}
			if (flag)
			{
				arrayList.Add(constructorBuilder);
			}
		}
		ConstructorInfo[] array2 = new ConstructorInfo[arrayList.Count];
		arrayList.CopyTo(array2);
		return array2;
	}

	public override Type GetElementType()
	{
		throw new NotSupportedException();
	}

	public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
	{
		check_created();
		return created.GetEvent(name, bindingAttr);
	}

	public override EventInfo[] GetEvents(BindingFlags bindingAttr)
	{
		if (is_created)
		{
			return created.GetEvents(bindingAttr);
		}
		if (!IsCompilerContext)
		{
			throw new NotSupportedException();
		}
		return new EventInfo[0];
	}

	internal EventInfo[] GetEvents_internal(BindingFlags bindingAttr)
	{
		if (events == null)
		{
			return new EventInfo[0];
		}
		ArrayList arrayList = new ArrayList();
		EventBuilder[] array = events;
		foreach (EventBuilder eventBuilder in array)
		{
			if (eventBuilder == null)
			{
				continue;
			}
			EventInfo eventInfo = get_event_info(eventBuilder);
			bool flag = false;
			MethodInfo methodInfo = eventInfo.GetAddMethod(nonPublic: true);
			if (methodInfo == null)
			{
				methodInfo = eventInfo.GetRemoveMethod(nonPublic: true);
			}
			if (methodInfo == null)
			{
				continue;
			}
			MethodAttributes attributes = methodInfo.Attributes;
			if ((attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public)
			{
				if ((bindingAttr & BindingFlags.Public) != BindingFlags.Default)
				{
					flag = true;
				}
			}
			else if ((bindingAttr & BindingFlags.NonPublic) != BindingFlags.Default)
			{
				flag = true;
			}
			if (!flag)
			{
				continue;
			}
			flag = false;
			if ((attributes & MethodAttributes.Static) != MethodAttributes.PrivateScope)
			{
				if ((bindingAttr & BindingFlags.Static) != BindingFlags.Default)
				{
					flag = true;
				}
			}
			else if ((bindingAttr & BindingFlags.Instance) != BindingFlags.Default)
			{
				flag = true;
			}
			if (flag)
			{
				arrayList.Add(eventInfo);
			}
		}
		EventInfo[] array2 = new EventInfo[arrayList.Count];
		arrayList.CopyTo(array2);
		return array2;
	}

	public override FieldInfo GetField(string name, BindingFlags bindingAttr)
	{
		if (created != null)
		{
			return created.GetField(name, bindingAttr);
		}
		if (fields == null)
		{
			return null;
		}
		FieldBuilder[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo == null || fieldInfo.Name != name)
			{
				continue;
			}
			bool flag = false;
			FieldAttributes attributes = fieldInfo.Attributes;
			if ((attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public)
			{
				if ((bindingAttr & BindingFlags.Public) != BindingFlags.Default)
				{
					flag = true;
				}
			}
			else if ((bindingAttr & BindingFlags.NonPublic) != BindingFlags.Default)
			{
				flag = true;
			}
			if (!flag)
			{
				continue;
			}
			flag = false;
			if ((attributes & FieldAttributes.Static) != FieldAttributes.PrivateScope)
			{
				if ((bindingAttr & BindingFlags.Static) != BindingFlags.Default)
				{
					flag = true;
				}
			}
			else if ((bindingAttr & BindingFlags.Instance) != BindingFlags.Default)
			{
				flag = true;
			}
			if (flag)
			{
				return fieldInfo;
			}
		}
		return null;
	}

	public override FieldInfo[] GetFields(BindingFlags bindingAttr)
	{
		if (created != null)
		{
			return created.GetFields(bindingAttr);
		}
		if (fields == null)
		{
			return new FieldInfo[0];
		}
		ArrayList arrayList = new ArrayList();
		FieldBuilder[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo == null)
			{
				continue;
			}
			bool flag = false;
			FieldAttributes attributes = fieldInfo.Attributes;
			if ((attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public)
			{
				if ((bindingAttr & BindingFlags.Public) != BindingFlags.Default)
				{
					flag = true;
				}
			}
			else if ((bindingAttr & BindingFlags.NonPublic) != BindingFlags.Default)
			{
				flag = true;
			}
			if (!flag)
			{
				continue;
			}
			flag = false;
			if ((attributes & FieldAttributes.Static) != FieldAttributes.PrivateScope)
			{
				if ((bindingAttr & BindingFlags.Static) != BindingFlags.Default)
				{
					flag = true;
				}
			}
			else if ((bindingAttr & BindingFlags.Instance) != BindingFlags.Default)
			{
				flag = true;
			}
			if (flag)
			{
				arrayList.Add(fieldInfo);
			}
		}
		FieldInfo[] array2 = new FieldInfo[arrayList.Count];
		arrayList.CopyTo(array2);
		return array2;
	}

	public override Type GetInterface(string name, bool ignoreCase)
	{
		check_created();
		return created.GetInterface(name, ignoreCase);
	}

	public override Type[] GetInterfaces()
	{
		if (is_created)
		{
			return created.GetInterfaces();
		}
		if (interfaces != null)
		{
			Type[] array = new Type[interfaces.Length];
			interfaces.CopyTo(array, 0);
			return array;
		}
		return Type.EmptyTypes;
	}

	public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
	{
		check_created();
		return created.GetMember(name, type, bindingAttr);
	}

	private MethodInfo[] GetMethodsByName(string name, BindingFlags bindingAttr, bool ignoreCase, Type reflected_type)
	{
		MethodInfo[] array2;
		if ((bindingAttr & BindingFlags.DeclaredOnly) == 0 && parent != null)
		{
			MethodInfo[] array = parent.GetMethods(bindingAttr);
			ArrayList arrayList = new ArrayList(array.Length);
			bool flag = (bindingAttr & BindingFlags.FlattenHierarchy) != 0;
			foreach (MethodInfo methodInfo in array)
			{
				MethodAttributes attributes = methodInfo.Attributes;
				if ((!methodInfo.IsStatic || flag) && (attributes & MethodAttributes.MemberAccessMask) switch
				{
					MethodAttributes.Public => (bindingAttr & BindingFlags.Public) != 0, 
					MethodAttributes.Assembly => (bindingAttr & BindingFlags.NonPublic) != 0, 
					MethodAttributes.Private => false, 
					_ => (bindingAttr & BindingFlags.NonPublic) != 0, 
				})
				{
					arrayList.Add(methodInfo);
				}
			}
			if (methods == null)
			{
				array2 = new MethodInfo[arrayList.Count];
				arrayList.CopyTo(array2);
			}
			else
			{
				array2 = new MethodInfo[methods.Length + arrayList.Count];
				arrayList.CopyTo(array2, 0);
				methods.CopyTo(array2, arrayList.Count);
			}
		}
		else
		{
			array2 = methods;
		}
		if (array2 == null)
		{
			return new MethodInfo[0];
		}
		ArrayList arrayList2 = new ArrayList();
		MethodInfo[] array3 = array2;
		foreach (MethodInfo methodInfo2 in array3)
		{
			if (methodInfo2 == null || (name != null && string.Compare(methodInfo2.Name, name, ignoreCase) != 0))
			{
				continue;
			}
			bool flag2 = false;
			MethodAttributes attributes = methodInfo2.Attributes;
			if ((attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public)
			{
				if ((bindingAttr & BindingFlags.Public) != BindingFlags.Default)
				{
					flag2 = true;
				}
			}
			else if ((bindingAttr & BindingFlags.NonPublic) != BindingFlags.Default)
			{
				flag2 = true;
			}
			if (!flag2)
			{
				continue;
			}
			flag2 = false;
			if ((attributes & MethodAttributes.Static) != MethodAttributes.PrivateScope)
			{
				if ((bindingAttr & BindingFlags.Static) != BindingFlags.Default)
				{
					flag2 = true;
				}
			}
			else if ((bindingAttr & BindingFlags.Instance) != BindingFlags.Default)
			{
				flag2 = true;
			}
			if (flag2)
			{
				arrayList2.Add(methodInfo2);
			}
		}
		MethodInfo[] array4 = new MethodInfo[arrayList2.Count];
		arrayList2.CopyTo(array4);
		return array4;
	}

	public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
	{
		return GetMethodsByName(null, bindingAttr, ignoreCase: false, this);
	}

	protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		check_created();
		bool ignoreCase = (bindingAttr & BindingFlags.IgnoreCase) != 0;
		MethodInfo[] methodsByName = GetMethodsByName(name, bindingAttr, ignoreCase, this);
		MethodInfo methodInfo = null;
		int num = ((types != null) ? types.Length : 0);
		int num2 = 0;
		MethodInfo[] array = methodsByName;
		foreach (MethodInfo methodInfo2 in array)
		{
			if (callConvention == CallingConventions.Any || (methodInfo2.CallingConvention & callConvention) == callConvention)
			{
				methodInfo = methodInfo2;
				num2++;
			}
		}
		switch (num2)
		{
		case 0:
			return null;
		case 1:
			if (num == 0)
			{
				return methodInfo;
			}
			break;
		}
		MethodBase[] array2 = new MethodBase[num2];
		if (num2 == 1)
		{
			array2[0] = methodInfo;
		}
		else
		{
			num2 = 0;
			MethodInfo[] array3 = methodsByName;
			foreach (MethodInfo methodInfo3 in array3)
			{
				if (callConvention == CallingConventions.Any || (methodInfo3.CallingConvention & callConvention) == callConvention)
				{
					array2[num2++] = methodInfo3;
				}
			}
		}
		if (types == null)
		{
			return (MethodInfo)Binder.FindMostDerivedMatch(array2);
		}
		if (binder == null)
		{
			binder = Binder.DefaultBinder;
		}
		return (MethodInfo)binder.SelectMethod(bindingAttr, array2, types, modifiers);
	}

	public override Type[] GetNestedTypes(BindingFlags bindingAttr)
	{
		if (!is_created && !IsCompilerContext)
		{
			throw new NotSupportedException();
		}
		ArrayList arrayList = new ArrayList();
		if (subtypes == null)
		{
			return Type.EmptyTypes;
		}
		TypeBuilder[] array = subtypes;
		foreach (TypeBuilder typeBuilder in array)
		{
			bool flag = false;
			if ((typeBuilder.attrs & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPublic)
			{
				if ((bindingAttr & BindingFlags.Public) != BindingFlags.Default)
				{
					flag = true;
				}
			}
			else if ((bindingAttr & BindingFlags.NonPublic) != BindingFlags.Default)
			{
				flag = true;
			}
			if (flag)
			{
				arrayList.Add(typeBuilder);
			}
		}
		Type[] array2 = new Type[arrayList.Count];
		arrayList.CopyTo(array2);
		return array2;
	}

	public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
	{
		if (is_created)
		{
			return created.GetProperties(bindingAttr);
		}
		if (properties == null)
		{
			return new PropertyInfo[0];
		}
		ArrayList arrayList = new ArrayList();
		PropertyBuilder[] array = properties;
		foreach (PropertyInfo propertyInfo in array)
		{
			bool flag = false;
			MethodInfo methodInfo = propertyInfo.GetGetMethod(nonPublic: true);
			if (methodInfo == null)
			{
				methodInfo = propertyInfo.GetSetMethod(nonPublic: true);
			}
			if (methodInfo == null)
			{
				continue;
			}
			MethodAttributes attributes = methodInfo.Attributes;
			if ((attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public)
			{
				if ((bindingAttr & BindingFlags.Public) != BindingFlags.Default)
				{
					flag = true;
				}
			}
			else if ((bindingAttr & BindingFlags.NonPublic) != BindingFlags.Default)
			{
				flag = true;
			}
			if (!flag)
			{
				continue;
			}
			flag = false;
			if ((attributes & MethodAttributes.Static) != MethodAttributes.PrivateScope)
			{
				if ((bindingAttr & BindingFlags.Static) != BindingFlags.Default)
				{
					flag = true;
				}
			}
			else if ((bindingAttr & BindingFlags.Instance) != BindingFlags.Default)
			{
				flag = true;
			}
			if (flag)
			{
				arrayList.Add(propertyInfo);
			}
		}
		PropertyInfo[] array2 = new PropertyInfo[arrayList.Count];
		arrayList.CopyTo(array2);
		return array2;
	}

	protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
	{
		throw not_supported();
	}

	protected override bool HasElementTypeImpl()
	{
		if (!is_created)
		{
			return false;
		}
		return created.HasElementType;
	}

	public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
	{
		check_created();
		return created.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
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
		return (Type.type_is_subtype_of(this, pmodule.assemblyb.corlib_value_type, check_interfaces: false) || Type.type_is_subtype_of(this, typeof(ValueType), check_interfaces: false)) && this != pmodule.assemblyb.corlib_value_type && this != pmodule.assemblyb.corlib_enum_type;
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

	[MonoTODO]
	public override Type MakeGenericType(params Type[] typeArguments)
	{
		return base.MakeGenericType(typeArguments);
	}

	public override Type MakePointerType()
	{
		return new PointerType(this);
	}

	public void SetParent(Type parent)
	{
		check_not_created();
		if (parent == null)
		{
			if ((attrs & TypeAttributes.ClassSemanticsMask) != TypeAttributes.NotPublic)
			{
				if ((attrs & TypeAttributes.Abstract) == 0)
				{
					throw new InvalidOperationException("Interface must be declared abstract.");
				}
				this.parent = null;
			}
			else
			{
				this.parent = typeof(object);
			}
		}
		else
		{
			this.parent = parent;
		}
		setup_internal_class(this);
	}

	internal int get_next_table_index(object obj, int table, bool inc)
	{
		return pmodule.get_next_table_index(obj, table, inc);
	}

	private Exception not_supported()
	{
		return new NotSupportedException("The invoked member is not supported in a dynamic module.");
	}

	private void check_not_created()
	{
		if (is_created)
		{
			throw new InvalidOperationException("Unable to change after type has been created.");
		}
	}

	private void check_created()
	{
		if (!is_created)
		{
			throw not_supported();
		}
	}

	private void check_name(string argName, string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException(argName);
		}
		if (name.Length == 0)
		{
			throw new ArgumentException("Empty name is not legal", argName);
		}
		if (name[0] == '\0')
		{
			throw new ArgumentException("Illegal name", argName);
		}
	}

	public override string ToString()
	{
		return FullName;
	}

	[MonoTODO]
	public override bool IsAssignableFrom(Type c)
	{
		return base.IsAssignableFrom(c);
	}

	[MonoTODO]
	[ComVisible(true)]
	public override bool IsSubclassOf(Type c)
	{
		return base.IsSubclassOf(c);
	}

	[MonoTODO("arrays")]
	internal bool IsAssignableTo(Type c)
	{
		if (c == this)
		{
			return true;
		}
		if (c.IsInterface)
		{
			if (parent != null && is_created && c.IsAssignableFrom(parent))
			{
				return true;
			}
			if (interfaces == null)
			{
				return false;
			}
			Type[] array = interfaces;
			foreach (Type c2 in array)
			{
				if (c.IsAssignableFrom(c2))
				{
					return true;
				}
			}
			if (!is_created)
			{
				return false;
			}
		}
		if (parent == null)
		{
			return c == typeof(object);
		}
		return c.IsAssignableFrom(parent);
	}

	public override Type[] GetGenericArguments()
	{
		if (generic_params == null)
		{
			return null;
		}
		Type[] array = new Type[generic_params.Length];
		generic_params.CopyTo(array, 0);
		return array;
	}

	public override Type GetGenericTypeDefinition()
	{
		if (generic_params == null)
		{
			throw new InvalidOperationException("Type is not generic");
		}
		return this;
	}

	public static ConstructorInfo GetConstructor(Type type, ConstructorInfo constructor)
	{
		if (type == null)
		{
			throw new ArgumentException("Type is not generic", "type");
		}
		ConstructorInfo constructor2 = type.GetConstructor(constructor);
		if (constructor2 == null)
		{
			throw new ArgumentException("constructor not found");
		}
		return constructor2;
	}

	private static bool IsValidGetMethodType(Type type)
	{
		if (type is TypeBuilder || type is MonoGenericClass)
		{
			return true;
		}
		if (type.Module is ModuleBuilder)
		{
			return true;
		}
		if (type.IsGenericParameter)
		{
			return false;
		}
		Type[] genericArguments = type.GetGenericArguments();
		if (genericArguments == null)
		{
			return false;
		}
		for (int i = 0; i < genericArguments.Length; i++)
		{
			if (IsValidGetMethodType(genericArguments[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static MethodInfo GetMethod(Type type, MethodInfo method)
	{
		if (!IsValidGetMethodType(type))
		{
			throw new ArgumentException("type is not TypeBuilder but " + type.GetType(), "type");
		}
		if (!type.IsGenericType)
		{
			throw new ArgumentException("type is not a generic type", "type");
		}
		if (!method.DeclaringType.IsGenericTypeDefinition)
		{
			throw new ArgumentException("method declaring type is not a generic type definition", "method");
		}
		if (method.DeclaringType != type.GetGenericTypeDefinition())
		{
			throw new ArgumentException("method declaring type is not the generic type definition of type", "method");
		}
		MethodInfo method2 = type.GetMethod(method);
		if (method2 == null)
		{
			throw new ArgumentException($"method {method.Name} not found in type {type}");
		}
		return method2;
	}

	public static FieldInfo GetField(Type type, FieldInfo field)
	{
		FieldInfo field2 = type.GetField(field);
		if (field2 == null)
		{
			throw new Exception("field not found");
		}
		return field2;
	}
}
