using System;
using System.Collections.Generic;
using System.Reflection;
using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Components.Json;

public class JsonClassInfo
{
	public readonly Type type;

	public readonly JsonFieldInfo[] CachedSerializedFields;

	public readonly JsonFieldInfo[] CachedSerializedStaticFields;

	private FieldInfo _keyField;

	private FieldInfo _jsonContextField;

	[ThreadStatic]
	private static Dictionary<Type, JsonClassInfo> _cachedClassInfo = new Dictionary<Type, JsonClassInfo>();

	public bool TypeHasKey => _keyField != null;

	private JsonClassInfo(Type reflectedType)
	{
		type = reflectedType;
		CachedSerializedFields = GetJsonFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		CachedSerializedStaticFields = GetJsonFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
	}

	public string GetKey(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		Type type = obj.GetType();
		if (type != this.type && !type.IsSubclassOf(this.type))
		{
			throw new JsonReflectionException($"Object must be descendant of the type associated with the JsonClassInfo instance. ({this.type})");
		}
		return ((_keyField == null) ? null : _keyField.GetValue(obj))?.ToString();
	}

	public object Construct(JsonContext context)
	{
		if (ReflectionWrapper.IsAbstract(type))
		{
			return null;
		}
		ConstructorInfo constructor = ReflectionWrapper.GetConstructor(type);
		object obj = ((constructor == null) ? ReflectionWrapper.GetUninitializedObject(type) : constructor.Invoke(null));
		if (_jsonContextField != null)
		{
			_jsonContextField.SetValue(obj, context);
		}
		return obj;
	}

	private JsonFieldInfo[] GetJsonFields(BindingFlags fieldFlags)
	{
		FieldInfo[] fieldsForTree = ReflectionWrapper.GetFieldsForTree(type, fieldFlags);
		List<JsonFieldInfo> list = new List<JsonFieldInfo>(fieldsForTree.Length);
		FieldInfo[] array = fieldsForTree;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.FieldType == typeof(JsonContext))
			{
				_jsonContextField = fieldInfo;
			}
			if (ShouldSerializeField(fieldInfo, out var serializeAsKey, out var isKeyField, out var enumAsString, out var serializedName, out var sizeLimit, out var failOnSizeLimit))
			{
				JsonFieldInfo jsonFieldInfo = new JsonFieldInfo();
				jsonFieldInfo.Field = fieldInfo;
				jsonFieldInfo.ValueType = LookupValueType(fieldInfo);
				jsonFieldInfo.SerializeByKey = serializeAsKey;
				jsonFieldInfo.EnumAsString = enumAsString;
				jsonFieldInfo.SerializedName = serializedName;
				jsonFieldInfo.SizeLimit = sizeLimit;
				jsonFieldInfo.FailOnSizeLimit = failOnSizeLimit;
				if (isKeyField)
				{
					_keyField = fieldInfo;
				}
				jsonFieldInfo.SerializeByKey = serializeAsKey;
				list.Add(jsonFieldInfo);
			}
		}
		return list.ToArray();
	}

	public static void DiscardClassInfoCache()
	{
		_cachedClassInfo.Clear();
	}

	public static bool IsClassInfoCached(Type reflectedType)
	{
		return _cachedClassInfo.ContainsKey(reflectedType);
	}

	public static JsonClassInfo GetJsonTypeReflection(Type reflectedType)
	{
		if (!_cachedClassInfo.TryGetValue(reflectedType, out var value))
		{
			value = GetNonCachedTypeReflection(reflectedType);
			_cachedClassInfo.Add(reflectedType, value);
		}
		return value;
	}

	public static JsonClassInfo GetNonCachedTypeReflection(Type reflectedType)
	{
		if (reflectedType == null)
		{
			return null;
		}
		return new JsonClassInfo(reflectedType);
	}

	public static string GetSerializedName(FieldInfo field)
	{
		if (field == null)
		{
			return null;
		}
		if (ReflectionWrapper.GetCustomAttribute(field, typeof(SerializedAsAttribute)) is SerializedAsAttribute serializedAsAttribute)
		{
			return serializedAsAttribute.SerializedAsName;
		}
		return field.Name;
	}

	private static Type LookupValueType(FieldInfo field)
	{
		Type fieldType = field.FieldType;
		Type type;
		if ((type = ReflectionWrapper.GetInterface(fieldType, typeof(IList<>))) != null)
		{
			return ReflectionWrapper.GetGenericArguments(type)[0];
		}
		if ((type = ReflectionWrapper.GetInterface(fieldType, typeof(IDictionary<, >))) != null)
		{
			return ReflectionWrapper.GetGenericArguments(type)[1];
		}
		return fieldType;
	}

	private static bool ShouldSerializeField(FieldInfo field, out bool serializeAsKey, out bool isKeyField, out bool enumAsString, out string serializedName, out int sizeLimit, out bool failOnSizeLimit)
	{
		bool result = field.IsPublic;
		serializeAsKey = false;
		isKeyField = false;
		enumAsString = false;
		serializedName = GetSerializedName(field);
		sizeLimit = -1;
		failOnSizeLimit = false;
		foreach (object customAttribute in ReflectionWrapper.GetCustomAttributes(field, inherit: true))
		{
			if (customAttribute is SerializedSizeLimitAttribute)
			{
				sizeLimit = (customAttribute as SerializedSizeLimitAttribute).Size;
				failOnSizeLimit = (customAttribute as SerializedSizeLimitAttribute).FailOnLimit;
			}
			if (customAttribute is EnumAsStringAttribute)
			{
				enumAsString = true;
			}
			if (customAttribute is SerializeField)
			{
				result = true;
			}
			if (customAttribute is NonSerializedAttribute)
			{
				result = false;
			}
			if (customAttribute is JsonKeyAttribute)
			{
				isKeyField = true;
			}
			if (customAttribute is JsonSerializeKeyOnlyAttribute)
			{
				serializeAsKey = true;
			}
		}
		return result;
	}
}
