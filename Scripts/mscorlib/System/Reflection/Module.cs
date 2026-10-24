using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection;

[Serializable]
[ComVisible(true)]
[ComDefaultInterface(typeof(_Module))]
[ClassInterface(ClassInterfaceType.None)]
public class Module : ISerializable, ICustomAttributeProvider, _Module
{
	private const BindingFlags defaultBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

	public static readonly TypeFilter FilterTypeName;

	public static readonly TypeFilter FilterTypeNameIgnoreCase;

	private IntPtr _impl;

	internal Assembly assembly;

	internal string fqname;

	internal string name;

	internal string scopename;

	internal bool is_resource;

	internal int token;

	public Assembly Assembly => assembly;

	public virtual string FullyQualifiedName => fqname;

	public string Name => name;

	public string ScopeName => scopename;

	internal Module()
	{
	}

	static Module()
	{
		FilterTypeName = filter_by_type_name;
		FilterTypeNameIgnoreCase = filter_by_type_name_ignore_case;
	}

	public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		UnitySerializationHolder.GetModuleData(this, info, context);
	}

	[ComVisible(true)]
	public virtual Type GetType(string className)
	{
		return GetType(className, throwOnError: false, ignoreCase: false);
	}

	[ComVisible(true)]
	public virtual Type GetType(string className, bool ignoreCase)
	{
		return GetType(className, throwOnError: false, ignoreCase);
	}

	[ComVisible(true)]
	public virtual Type GetType(string className, bool throwOnError, bool ignoreCase)
	{
		if (className == null)
		{
			throw new ArgumentNullException("className");
		}
		if (className == string.Empty)
		{
			throw new ArgumentException("Type name can't be empty");
		}
		return assembly.InternalGetType(this, className, throwOnError, ignoreCase);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern Type[] InternalGetTypes();

	public virtual Type[] GetTypes()
	{
		return InternalGetTypes();
	}

	public virtual bool IsDefined(Type attributeType, bool inherit)
	{
		return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
	}

	public bool IsResource()
	{
		return is_resource;
	}

	public override string ToString()
	{
		return name;
	}

	internal virtual Guid GetModuleVersionId()
	{
		return new Guid(GetGuidInternal());
	}

	private static bool filter_by_type_name(Type m, object filterCriteria)
	{
		string text = (string)filterCriteria;
		if (text.EndsWith("*"))
		{
			return m.Name.StartsWith(text.Substring(0, text.Length - 1));
		}
		return m.Name == text;
	}

	private static bool filter_by_type_name_ignore_case(Type m, object filterCriteria)
	{
		string text = (string)filterCriteria;
		if (text.EndsWith("*"))
		{
			return m.Name.ToLower().StartsWith(text.Substring(0, text.Length - 1).ToLower());
		}
		return string.Compare(m.Name, text, ignoreCase: true) == 0;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern IntPtr GetHINSTANCE();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern string GetGuidInternal();
}
