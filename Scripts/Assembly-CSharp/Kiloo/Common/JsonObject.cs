using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using Kiloo.Common.Serialization;
using Kiloo.Common.Serialization.Parsing;
using Kiloo.Common.Utility;

namespace Kiloo.Common;

public class JsonObject
{
	[NotNull]
	private readonly IDictionary _json;

	public object this[[NotNull] string key]
	{
		get
		{
			return _json[key];
		}
		set
		{
			object value2 = ((!(value is JsonObject jsonObject)) ? ((value is DateTime) ? ((DateTime)value).ToJSON() : ((!CanSerialize(value)) ? value.ToString() : value)) : jsonObject._json);
			_json[key] = value2;
		}
	}

	public JsonObject([CanBeNull] object json = null)
	{
		if (json is JsonObject jsonObject)
		{
			_json = jsonObject._json;
		}
		else
		{
			_json = ((json != null) ? ((IDictionary)json) : new Dictionary<string, object>());
		}
	}

	public static JsonObject Deserialize(string json)
	{
		return new JsonObject(KilooJson.Deserialize(json));
	}

	public string Serialize()
	{
		KilooSerializer serializer = new KilooSerializer();
		return serializer.ToJson(_json);
	}

	public void Remove([NotNull] string key)
	{
		_json.Remove(key);
	}

	[Pure]
	public T Get<T>([NotNull] string key, IFormatProvider provider = null)
	{
		return ConvertValue<T>(this[key], provider);
	}

	[Pure]
	public T Get<T>([NotNull] string key, T defaultValue, IFormatProvider provider = null)
	{
		return Get(_json, key, defaultValue, provider);
	}

	[Pure]
	public bool TryGet<T>([NotNull] string key, out T result, IFormatProvider provider = null)
	{
		return TryGet<T>(_json, key, out result, provider);
	}

	[Pure]
	public static T Get<T>([CanBeNull] object jsonObject, [NotNull] string key, IFormatProvider provider = null)
	{
		object json = ((jsonObject is IDictionary dictionary) ? dictionary[key] : null);
		return ConvertValue<T>(json, provider);
	}

	[Pure]
	public static T Get<T>([CanBeNull] object jsonObject, [NotNull] string key, T defaultValue, IFormatProvider provider = null)
	{
		if (TryGet<T>(jsonObject, key, out var result, provider))
		{
			return result;
		}
		return defaultValue;
	}

	[Pure]
	public static bool TryGet<T>([CanBeNull] object jsonObject, [NotNull] string key, out T result, [CanBeNull] IFormatProvider provider = null)
	{
		if (jsonObject is IDictionary dictionary && dictionary.Contains(key))
		{
			try
			{
				result = ConvertValue<T>(dictionary[key], provider);
				return true;
			}
			catch (Exception)
			{
			}
		}
		result = default;
		return false;
	}

	[Pure]
	public static T ConvertValue<T>([CanBeNull] object json, IFormatProvider provider = null)
	{
		if (json == null)
		{
			return (T)(object)null;
		}
		if (json is T)
		{
			return (T)json;
		}
		if (typeof(T) == typeof(JsonObject) && json is IDictionary)
		{
			return (T)(object)new JsonObject(json);
		}
		if (typeof(T) == typeof(DateTime) && json is string)
		{
			return (T)(object)DateTime.Parse((string)json, provider ?? CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
		}
		if (json is IConvertible)
		{
			return (T)Convert.ChangeType(json, typeof(T), provider ?? CultureInfo.InvariantCulture);
		}
		if (typeof(T) == typeof(string))
		{
			if (json is IFormattable formattable)
			{
				return (T)(object)formattable.ToString(null, provider ?? CultureInfo.InvariantCulture);
			}
			return (T)(object)json.ToString();
		}
		throw new InvalidCastException();
	}

	[Pure]
	public static bool HasKey([CanBeNull] object jsonObject, [NotNull] string key)
	{
		return jsonObject is IDictionary dictionary && dictionary.Contains(key);
	}

	[Pure]
	public bool HasKey([NotNull] string key)
	{
		return _json.Contains(key);
	}

	[Pure]
	public bool CanSerialize()
	{
		return CanSerialize(_json);
	}

	[Pure]
	public static bool CanSerialize([CanBeNull] object value)
	{
		if (value == null || value is string || value is char || value is bool || value is float || value is int || value is uint || value is long || value is double || value is sbyte || value is byte || value is short || value is ushort || value is ulong || value is decimal)
		{
			return true;
		}
		if (value is IList list)
		{
			foreach (object item in list)
			{
				if (!CanSerialize(item))
				{
					return false;
				}
			}
			return true;
		}
		if (value is IDictionary dictionary)
		{
			foreach (DictionaryEntry item2 in dictionary)
			{
				if (!CanSerialize(item2.Value))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}
}
