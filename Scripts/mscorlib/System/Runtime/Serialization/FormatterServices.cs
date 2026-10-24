using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;

namespace System.Runtime.Serialization;

[ComVisible(true)]
public sealed class FormatterServices
{
	public static object[] GetObjectData(object obj, MemberInfo[] members)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		if (members == null)
		{
			throw new ArgumentNullException("members");
		}
		int num = members.Length;
		object[] array = new object[num];
		for (int i = 0; i < num; i++)
		{
			MemberInfo memberInfo = members[i];
			if (memberInfo == null)
			{
				throw new ArgumentNullException($"members[{i}]");
			}
			if (memberInfo.MemberType != MemberTypes.Field)
			{
				throw new SerializationException($"members [{i}] is not a field.");
			}
			FieldInfo fieldInfo = memberInfo as FieldInfo;
			array[i] = fieldInfo.GetValue(obj);
		}
		return array;
	}

	public static MemberInfo[] GetSerializableMembers(Type type, StreamingContext context)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		ArrayList arrayList = new ArrayList();
		for (Type type2 = type; type2 != null; type2 = type2.BaseType)
		{
			if (!type2.IsSerializable)
			{
				string message = $"Type {type2} in assembly {type2.Assembly.FullName} is not marked as serializable.";
				throw new SerializationException(message);
			}
			GetFields(type, type2, arrayList);
		}
		MemberInfo[] array = new MemberInfo[arrayList.Count];
		arrayList.CopyTo(array);
		return array;
	}

	private static void GetFields(Type reflectedType, Type type, ArrayList fields)
	{
		FieldInfo[] fields2 = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		FieldInfo[] array = fields2;
		foreach (FieldInfo fieldInfo in array)
		{
			if (!fieldInfo.IsNotSerialized)
			{
				if (fieldInfo is MonoField monoField && reflectedType != type && !monoField.IsPublic)
				{
					string newName = type.Name + "+" + monoField.Name;
					fields.Add(monoField.Clone(newName));
				}
				else
				{
					fields.Add(fieldInfo);
				}
			}
		}
	}

	public static object GetUninitializedObject(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (type == typeof(string))
		{
			throw new ArgumentException("Uninitialized Strings cannot be created.");
		}
		return ActivationServices.AllocateUninitializedClassInstance(type);
	}

	public static object GetSafeUninitializedObject(Type type)
	{
		return GetUninitializedObject(type);
	}
}
