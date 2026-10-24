using System;

namespace UnityEngine;

public class AndroidJavaObject : IDisposable
{
	private static bool enableDebugPrints = false;

	internal GlobalJavaObjectRef m_jobject;

	internal GlobalJavaObjectRef m_jclass;

	private static AndroidJavaClass s_JavaLangClass;

	protected static AndroidJavaClass JavaLangClass
	{
		get
		{
			if (s_JavaLangClass == null)
			{
				s_JavaLangClass = new AndroidJavaClass(AndroidJNISafe.FindClass("java/lang/Class"));
			}
			return s_JavaLangClass;
		}
	}

	public AndroidJavaObject(string className, params object[] args)
		: this()
	{
		_AndroidJavaObject(className, args);
	}

	internal AndroidJavaObject(IntPtr jobject)
		: this()
	{
		if (jobject == IntPtr.Zero)
		{
			throw new Exception("JNI: Init'd AndroidJavaObject with null ptr!");
		}
		IntPtr objectClass = AndroidJNISafe.GetObjectClass(jobject);
		m_jobject = new GlobalJavaObjectRef(jobject);
		m_jclass = new GlobalJavaObjectRef(objectClass);
		AndroidJNISafe.DeleteLocalRef(objectClass);
	}

	internal AndroidJavaObject()
	{
	}

	public void Dispose()
	{
		_Dispose();
	}

	public void Call(string methodName, params object[] args)
	{
		_Call(methodName, args);
	}

	public void CallStatic(string methodName, params object[] args)
	{
		_CallStatic(methodName, args);
	}

	public FieldType Get<FieldType>(string fieldName)
	{
		return _Get<FieldType>(fieldName);
	}

	public void Set<FieldType>(string fieldName, FieldType val)
	{
		_Set(fieldName, val);
	}

	public FieldType GetStatic<FieldType>(string fieldName)
	{
		return _GetStatic<FieldType>(fieldName);
	}

	public IntPtr GetRawObject()
	{
		return _GetRawObject();
	}

	public IntPtr GetRawClass()
	{
		return _GetRawClass();
	}

	public ReturnType Call<ReturnType>(string methodName, params object[] args)
	{
		return _Call<ReturnType>(methodName, args);
	}

	public ReturnType CallStatic<ReturnType>(string methodName, params object[] args)
	{
		return _CallStatic<ReturnType>(methodName, args);
	}

	protected void DebugPrint(string msg)
	{
		if (enableDebugPrints)
		{
			Debug.Log(msg);
		}
	}

	private void _AndroidJavaObject(string className, params object[] args)
	{
		DebugPrint("Creating AndroidJavaObject from " + className);
		if (args == null)
		{
			args = new object[1];
		}
		using AndroidJavaObject androidJavaObject = FindClass(className);
		m_jclass = new GlobalJavaObjectRef(androidJavaObject.GetRawObject());
		jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
		try
		{
			IntPtr constructorID = AndroidJNIHelper.GetConstructorID(m_jclass, args);
			IntPtr intPtr = AndroidJNISafe.NewObject(m_jclass, constructorID, array);
			m_jobject = new GlobalJavaObjectRef(intPtr);
			AndroidJNISafe.DeleteLocalRef(intPtr);
		}
		finally
		{
			AndroidJNIHelper.DeleteJNIArgArray(args, array);
		}
	}

	~AndroidJavaObject()
	{
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
		m_jobject.Dispose();
		m_jclass.Dispose();
	}

	protected void _Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected void _Call(string methodName, params object[] args)
	{
		if (args == null)
		{
			args = new object[1];
		}
		IntPtr methodID = AndroidJNIHelper.GetMethodID(m_jclass, methodName, args, isStatic: false);
		jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
		try
		{
			AndroidJNISafe.CallVoidMethod(m_jobject, methodID, array);
		}
		finally
		{
			AndroidJNIHelper.DeleteJNIArgArray(args, array);
		}
	}

	protected ReturnType _Call<ReturnType>(string methodName, params object[] args)
	{
		if (args == null)
		{
			args = new object[1];
		}
		IntPtr methodID = AndroidJNIHelper.GetMethodID<ReturnType>(m_jclass, methodName, args, isStatic: false);
		jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
		try
		{
			if (AndroidReflection.IsPrimitive(typeof(ReturnType)))
			{
				if (typeof(ReturnType) == typeof(int))
				{
					return (ReturnType)(object)AndroidJNISafe.CallIntMethod(m_jobject, methodID, array);
				}
				if (typeof(ReturnType) == typeof(bool))
				{
					return (ReturnType)(object)AndroidJNISafe.CallBooleanMethod(m_jobject, methodID, array);
				}
				if (typeof(ReturnType) == typeof(byte))
				{
					return (ReturnType)(object)AndroidJNISafe.CallByteMethod(m_jobject, methodID, array);
				}
				if (typeof(ReturnType) == typeof(short))
				{
					return (ReturnType)(object)AndroidJNISafe.CallShortMethod(m_jobject, methodID, array);
				}
				if (typeof(ReturnType) == typeof(long))
				{
					return (ReturnType)(object)AndroidJNISafe.CallLongMethod(m_jobject, methodID, array);
				}
				if (typeof(ReturnType) == typeof(float))
				{
					return (ReturnType)(object)AndroidJNISafe.CallFloatMethod(m_jobject, methodID, array);
				}
				if (typeof(ReturnType) == typeof(double))
				{
					return (ReturnType)(object)AndroidJNISafe.CallDoubleMethod(m_jobject, methodID, array);
				}
				if (typeof(ReturnType) == typeof(char))
				{
					return (ReturnType)(object)AndroidJNISafe.CallCharMethod(m_jobject, methodID, array);
				}
				return default;
			}
			if (typeof(ReturnType) == typeof(string))
			{
				return (ReturnType)(object)AndroidJNISafe.CallStringMethod(m_jobject, methodID, array);
			}
			if (typeof(ReturnType) == typeof(AndroidJavaClass))
			{
				IntPtr intPtr = AndroidJNISafe.CallObjectMethod(m_jobject, methodID, array);
				return (!(intPtr == IntPtr.Zero)) ? ((ReturnType)(object)AndroidJavaClassDeleteLocalRef(intPtr)) : default(ReturnType);
			}
			if (typeof(ReturnType) == typeof(AndroidJavaObject))
			{
				IntPtr intPtr2 = AndroidJNISafe.CallObjectMethod(m_jobject, methodID, array);
				return (!(intPtr2 == IntPtr.Zero)) ? ((ReturnType)(object)AndroidJavaObjectDeleteLocalRef(intPtr2)) : default(ReturnType);
			}
			if (AndroidReflection.IsAssignableFrom(typeof(Array), typeof(ReturnType)))
			{
				IntPtr intPtr3 = AndroidJNISafe.CallObjectMethod(m_jobject, methodID, array);
				return (!(intPtr3 == IntPtr.Zero)) ? ((ReturnType)(object)AndroidJNIHelper.ConvertFromJNIArray<ReturnType>(intPtr3)) : default(ReturnType);
			}
			throw new Exception(string.Concat("JNI: Unknown return type '", typeof(ReturnType), "'"));
		}
		finally
		{
			AndroidJNIHelper.DeleteJNIArgArray(args, array);
		}
	}

	protected FieldType _Get<FieldType>(string fieldName)
	{
		IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(m_jclass, fieldName, isStatic: false);
		if (AndroidReflection.IsPrimitive(typeof(FieldType)))
		{
			if (typeof(FieldType) == typeof(int))
			{
				return (FieldType)(object)AndroidJNISafe.GetIntField(m_jobject, fieldID);
			}
			if (typeof(FieldType) == typeof(bool))
			{
				return (FieldType)(object)AndroidJNISafe.GetBooleanField(m_jobject, fieldID);
			}
			if (typeof(FieldType) == typeof(byte))
			{
				return (FieldType)(object)AndroidJNISafe.GetByteField(m_jobject, fieldID);
			}
			if (typeof(FieldType) == typeof(short))
			{
				return (FieldType)(object)AndroidJNISafe.GetShortField(m_jobject, fieldID);
			}
			if (typeof(FieldType) == typeof(long))
			{
				return (FieldType)(object)AndroidJNISafe.GetLongField(m_jobject, fieldID);
			}
			if (typeof(FieldType) == typeof(float))
			{
				return (FieldType)(object)AndroidJNISafe.GetFloatField(m_jobject, fieldID);
			}
			if (typeof(FieldType) == typeof(double))
			{
				return (FieldType)(object)AndroidJNISafe.GetDoubleField(m_jobject, fieldID);
			}
			if (typeof(FieldType) == typeof(char))
			{
				return (FieldType)(object)AndroidJNISafe.GetCharField(m_jobject, fieldID);
			}
			return default;
		}
		if (typeof(FieldType) == typeof(string))
		{
			return (FieldType)(object)AndroidJNISafe.GetStringField(m_jobject, fieldID);
		}
		if (typeof(FieldType) == typeof(AndroidJavaClass))
		{
			IntPtr objectField = AndroidJNISafe.GetObjectField(m_jobject, fieldID);
			return (!(objectField == IntPtr.Zero)) ? ((FieldType)(object)AndroidJavaClassDeleteLocalRef(objectField)) : default(FieldType);
		}
		if (typeof(FieldType) == typeof(AndroidJavaObject))
		{
			IntPtr objectField2 = AndroidJNISafe.GetObjectField(m_jobject, fieldID);
			return (!(objectField2 == IntPtr.Zero)) ? ((FieldType)(object)AndroidJavaObjectDeleteLocalRef(objectField2)) : default(FieldType);
		}
		if (AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType)))
		{
			IntPtr objectField3 = AndroidJNISafe.GetObjectField(m_jobject, fieldID);
			return (!(objectField3 == IntPtr.Zero)) ? ((FieldType)(object)AndroidJNIHelper.ConvertFromJNIArray<FieldType>(objectField3)) : default(FieldType);
		}
		throw new Exception(string.Concat("JNI: Unknown field type '", typeof(FieldType), "'"));
	}

	protected void _Set<FieldType>(string fieldName, FieldType val)
	{
		IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(m_jclass, fieldName, isStatic: false);
		if (AndroidReflection.IsPrimitive(typeof(FieldType)))
		{
			if (typeof(FieldType) == typeof(int))
			{
				AndroidJNISafe.SetIntField(m_jobject, fieldID, (int)(object)val);
			}
			else if (typeof(FieldType) == typeof(bool))
			{
				AndroidJNISafe.SetBooleanField(m_jobject, fieldID, (bool)(object)val);
			}
			else if (typeof(FieldType) == typeof(byte))
			{
				AndroidJNISafe.SetByteField(m_jobject, fieldID, (byte)(object)val);
			}
			else if (typeof(FieldType) == typeof(short))
			{
				AndroidJNISafe.SetShortField(m_jobject, fieldID, (short)(object)val);
			}
			else if (typeof(FieldType) == typeof(long))
			{
				AndroidJNISafe.SetLongField(m_jobject, fieldID, (long)(object)val);
			}
			else if (typeof(FieldType) == typeof(float))
			{
				AndroidJNISafe.SetFloatField(m_jobject, fieldID, (float)(object)val);
			}
			else if (typeof(FieldType) == typeof(double))
			{
				AndroidJNISafe.SetDoubleField(m_jobject, fieldID, (double)(object)val);
			}
			else if (typeof(FieldType) == typeof(char))
			{
				AndroidJNISafe.SetCharField(m_jobject, fieldID, (char)(object)val);
			}
		}
		else if (typeof(FieldType) == typeof(string))
		{
			AndroidJNISafe.SetStringField(m_jobject, fieldID, (string)(object)val);
		}
		else if (typeof(FieldType) == typeof(AndroidJavaClass))
		{
			AndroidJNISafe.SetObjectField(m_jobject, fieldID, ((AndroidJavaClass)(object)val).m_jclass);
		}
		else if (typeof(FieldType) == typeof(AndroidJavaObject))
		{
			AndroidJNISafe.SetObjectField(m_jobject, fieldID, ((AndroidJavaObject)(object)val).m_jobject);
		}
		else
		{
			if (!AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType)))
			{
				throw new Exception(string.Concat("JNI: Unknown field type '", typeof(FieldType), "'"));
			}
			IntPtr val2 = AndroidJNIHelper.ConvertToJNIArray((Array)(object)val);
			AndroidJNISafe.SetObjectField(m_jclass, fieldID, val2);
		}
	}

	protected void _CallStatic(string methodName, params object[] args)
	{
		if (args == null)
		{
			args = new object[1];
		}
		IntPtr methodID = AndroidJNIHelper.GetMethodID(m_jclass, methodName, args, isStatic: true);
		jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
		try
		{
			AndroidJNISafe.CallStaticVoidMethod(m_jclass, methodID, array);
		}
		finally
		{
			AndroidJNIHelper.DeleteJNIArgArray(args, array);
		}
	}

	protected ReturnType _CallStatic<ReturnType>(string methodName, params object[] args)
	{
		if (args == null)
		{
			args = new object[1];
		}
		IntPtr methodID = AndroidJNIHelper.GetMethodID<ReturnType>(m_jclass, methodName, args, isStatic: true);
		jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
		try
		{
			if (AndroidReflection.IsPrimitive(typeof(ReturnType)))
			{
				if (typeof(ReturnType) == typeof(int))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStaticIntMethod(m_jclass, methodID, array);
				}
				if (typeof(ReturnType) == typeof(bool))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStaticBooleanMethod(m_jclass, methodID, array);
				}
				if (typeof(ReturnType) == typeof(byte))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStaticByteMethod(m_jclass, methodID, array);
				}
				if (typeof(ReturnType) == typeof(short))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStaticShortMethod(m_jclass, methodID, array);
				}
				if (typeof(ReturnType) == typeof(long))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStaticLongMethod(m_jclass, methodID, array);
				}
				if (typeof(ReturnType) == typeof(float))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStaticFloatMethod(m_jclass, methodID, array);
				}
				if (typeof(ReturnType) == typeof(double))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStaticDoubleMethod(m_jclass, methodID, array);
				}
				if (typeof(ReturnType) == typeof(char))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStaticCharMethod(m_jclass, methodID, array);
				}
				return default;
			}
			if (typeof(ReturnType) == typeof(string))
			{
				return (ReturnType)(object)AndroidJNISafe.CallStaticStringMethod(m_jclass, methodID, array);
			}
			if (typeof(ReturnType) == typeof(AndroidJavaClass))
			{
				IntPtr intPtr = AndroidJNISafe.CallStaticObjectMethod(m_jclass, methodID, array);
				return (!(intPtr == IntPtr.Zero)) ? ((ReturnType)(object)AndroidJavaClassDeleteLocalRef(intPtr)) : default(ReturnType);
			}
			if (typeof(ReturnType) == typeof(AndroidJavaObject))
			{
				IntPtr intPtr2 = AndroidJNISafe.CallStaticObjectMethod(m_jclass, methodID, array);
				return (!(intPtr2 == IntPtr.Zero)) ? ((ReturnType)(object)AndroidJavaObjectDeleteLocalRef(intPtr2)) : default(ReturnType);
			}
			if (AndroidReflection.IsAssignableFrom(typeof(Array), typeof(ReturnType)))
			{
				IntPtr intPtr3 = AndroidJNISafe.CallStaticObjectMethod(m_jclass, methodID, array);
				return (!(intPtr3 == IntPtr.Zero)) ? ((ReturnType)(object)AndroidJNIHelper.ConvertFromJNIArray<ReturnType>(intPtr3)) : default(ReturnType);
			}
			throw new Exception(string.Concat("JNI: Unknown return type '", typeof(ReturnType), "'"));
		}
		finally
		{
			AndroidJNIHelper.DeleteJNIArgArray(args, array);
		}
	}

	protected FieldType _GetStatic<FieldType>(string fieldName)
	{
		IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(m_jclass, fieldName, isStatic: true);
		if (AndroidReflection.IsPrimitive(typeof(FieldType)))
		{
			if (typeof(FieldType) == typeof(int))
			{
				return (FieldType)(object)AndroidJNISafe.GetStaticIntField(m_jclass, fieldID);
			}
			if (typeof(FieldType) == typeof(bool))
			{
				return (FieldType)(object)AndroidJNISafe.GetStaticBooleanField(m_jclass, fieldID);
			}
			if (typeof(FieldType) == typeof(byte))
			{
				return (FieldType)(object)AndroidJNISafe.GetStaticByteField(m_jclass, fieldID);
			}
			if (typeof(FieldType) == typeof(short))
			{
				return (FieldType)(object)AndroidJNISafe.GetStaticShortField(m_jclass, fieldID);
			}
			if (typeof(FieldType) == typeof(long))
			{
				return (FieldType)(object)AndroidJNISafe.GetStaticLongField(m_jclass, fieldID);
			}
			if (typeof(FieldType) == typeof(float))
			{
				return (FieldType)(object)AndroidJNISafe.GetStaticFloatField(m_jclass, fieldID);
			}
			if (typeof(FieldType) == typeof(double))
			{
				return (FieldType)(object)AndroidJNISafe.GetStaticDoubleField(m_jclass, fieldID);
			}
			if (typeof(FieldType) == typeof(char))
			{
				return (FieldType)(object)AndroidJNISafe.GetStaticCharField(m_jclass, fieldID);
			}
			return default;
		}
		if (typeof(FieldType) == typeof(string))
		{
			return (FieldType)(object)AndroidJNISafe.GetStaticStringField(m_jclass, fieldID);
		}
		if (typeof(FieldType) == typeof(AndroidJavaClass))
		{
			IntPtr staticObjectField = AndroidJNISafe.GetStaticObjectField(m_jclass, fieldID);
			return (!(staticObjectField == IntPtr.Zero)) ? ((FieldType)(object)AndroidJavaClassDeleteLocalRef(staticObjectField)) : default(FieldType);
		}
		if (typeof(FieldType) == typeof(AndroidJavaObject))
		{
			IntPtr staticObjectField2 = AndroidJNISafe.GetStaticObjectField(m_jclass, fieldID);
			return (!(staticObjectField2 == IntPtr.Zero)) ? ((FieldType)(object)AndroidJavaObjectDeleteLocalRef(staticObjectField2)) : default(FieldType);
		}
		if (AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType)))
		{
			IntPtr staticObjectField3 = AndroidJNISafe.GetStaticObjectField(m_jclass, fieldID);
			return (!(staticObjectField3 == IntPtr.Zero)) ? ((FieldType)(object)AndroidJNIHelper.ConvertFromJNIArray<FieldType>(staticObjectField3)) : default(FieldType);
		}
		throw new Exception(string.Concat("JNI: Unknown field type '", typeof(FieldType), "'"));
	}

	internal static AndroidJavaObject AndroidJavaObjectDeleteLocalRef(IntPtr jobject)
	{
		try
		{
			return new AndroidJavaObject(jobject);
		}
		finally
		{
			AndroidJNISafe.DeleteLocalRef(jobject);
		}
	}

	internal static AndroidJavaClass AndroidJavaClassDeleteLocalRef(IntPtr jclass)
	{
		try
		{
			return new AndroidJavaClass(jclass);
		}
		finally
		{
			AndroidJNISafe.DeleteLocalRef(jclass);
		}
	}

	protected IntPtr _GetRawObject()
	{
		return m_jobject;
	}

	protected IntPtr _GetRawClass()
	{
		return m_jclass;
	}

	protected static AndroidJavaObject FindClass(string name)
	{
		return JavaLangClass.CallStatic<AndroidJavaObject>("forName", new object[1] { name.Replace('/', '.') });
	}
}
