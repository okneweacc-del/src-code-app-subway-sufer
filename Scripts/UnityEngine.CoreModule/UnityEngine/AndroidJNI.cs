using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

public sealed class AndroidJNI
{
	[ThreadAndSerializationSafe]
	public static IntPtr FindClass(string name)
	{
		INTERNAL_CALL_FindClass(name, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_FindClass(string name, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr FromReflectedMethod(IntPtr refMethod)
	{
		INTERNAL_CALL_FromReflectedMethod(refMethod, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_FromReflectedMethod(IntPtr refMethod, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr FromReflectedField(IntPtr refField)
	{
		INTERNAL_CALL_FromReflectedField(refField, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_FromReflectedField(IntPtr refField, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr ExceptionOccurred()
	{
		INTERNAL_CALL_ExceptionOccurred(out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ExceptionOccurred(out IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern void ExceptionClear();

	[ThreadAndSerializationSafe]
	public static IntPtr NewGlobalRef(IntPtr obj)
	{
		INTERNAL_CALL_NewGlobalRef(obj, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_NewGlobalRef(IntPtr obj, out IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern void DeleteGlobalRef(IntPtr obj);

	[ThreadAndSerializationSafe]
	public static IntPtr NewLocalRef(IntPtr obj)
	{
		INTERNAL_CALL_NewLocalRef(obj, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_NewLocalRef(IntPtr obj, out IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern void DeleteLocalRef(IntPtr obj);

	[ThreadAndSerializationSafe]
	public static IntPtr NewObject(IntPtr clazz, IntPtr methodID, jvalue[] args)
	{
		INTERNAL_CALL_NewObject(clazz, methodID, args, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_NewObject(IntPtr clazz, IntPtr methodID, jvalue[] args, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr GetObjectClass(IntPtr obj)
	{
		INTERNAL_CALL_GetObjectClass(obj, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetObjectClass(IntPtr obj, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr GetMethodID(IntPtr clazz, string name, string sig)
	{
		INTERNAL_CALL_GetMethodID(clazz, name, sig, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetMethodID(IntPtr clazz, string name, string sig, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr GetFieldID(IntPtr clazz, string name, string sig)
	{
		INTERNAL_CALL_GetFieldID(clazz, name, sig, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetFieldID(IntPtr clazz, string name, string sig, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr GetStaticMethodID(IntPtr clazz, string name, string sig)
	{
		INTERNAL_CALL_GetStaticMethodID(clazz, name, sig, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetStaticMethodID(IntPtr clazz, string name, string sig, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr GetStaticFieldID(IntPtr clazz, string name, string sig)
	{
		INTERNAL_CALL_GetStaticFieldID(clazz, name, sig, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetStaticFieldID(IntPtr clazz, string name, string sig, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr NewStringUTF(string bytes)
	{
		INTERNAL_CALL_NewStringUTF(bytes, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_NewStringUTF(string bytes, out IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern string GetStringUTFChars(IntPtr str);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern string CallStringMethod(IntPtr obj, IntPtr methodID, jvalue[] args);

	[ThreadAndSerializationSafe]
	public static IntPtr CallObjectMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
	{
		INTERNAL_CALL_CallObjectMethod(obj, methodID, args, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_CallObjectMethod(IntPtr obj, IntPtr methodID, jvalue[] args, out IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern int CallIntMethod(IntPtr obj, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern bool CallBooleanMethod(IntPtr obj, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern short CallShortMethod(IntPtr obj, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern byte CallByteMethod(IntPtr obj, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern char CallCharMethod(IntPtr obj, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern float CallFloatMethod(IntPtr obj, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern double CallDoubleMethod(IntPtr obj, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern long CallLongMethod(IntPtr obj, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern void CallVoidMethod(IntPtr obj, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern string GetStringField(IntPtr obj, IntPtr fieldID);

	[ThreadAndSerializationSafe]
	public static IntPtr GetObjectField(IntPtr obj, IntPtr fieldID)
	{
		INTERNAL_CALL_GetObjectField(obj, fieldID, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetObjectField(IntPtr obj, IntPtr fieldID, out IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern bool GetBooleanField(IntPtr obj, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern byte GetByteField(IntPtr obj, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern char GetCharField(IntPtr obj, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern short GetShortField(IntPtr obj, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern int GetIntField(IntPtr obj, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern long GetLongField(IntPtr obj, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern float GetFloatField(IntPtr obj, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern double GetDoubleField(IntPtr obj, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern void SetStringField(IntPtr obj, IntPtr fieldID, string val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern void SetObjectField(IntPtr obj, IntPtr fieldID, IntPtr val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern void SetBooleanField(IntPtr obj, IntPtr fieldID, bool val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern void SetByteField(IntPtr obj, IntPtr fieldID, byte val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern void SetCharField(IntPtr obj, IntPtr fieldID, char val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern void SetShortField(IntPtr obj, IntPtr fieldID, short val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern void SetIntField(IntPtr obj, IntPtr fieldID, int val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern void SetLongField(IntPtr obj, IntPtr fieldID, long val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern void SetFloatField(IntPtr obj, IntPtr fieldID, float val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern void SetDoubleField(IntPtr obj, IntPtr fieldID, double val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern string CallStaticStringMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);

	[ThreadAndSerializationSafe]
	public static IntPtr CallStaticObjectMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
	{
		INTERNAL_CALL_CallStaticObjectMethod(clazz, methodID, args, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_CallStaticObjectMethod(IntPtr clazz, IntPtr methodID, jvalue[] args, out IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern int CallStaticIntMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern bool CallStaticBooleanMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern short CallStaticShortMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern byte CallStaticByteMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern char CallStaticCharMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern float CallStaticFloatMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern double CallStaticDoubleMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern long CallStaticLongMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern void CallStaticVoidMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern string GetStaticStringField(IntPtr clazz, IntPtr fieldID);

	[ThreadAndSerializationSafe]
	public static IntPtr GetStaticObjectField(IntPtr clazz, IntPtr fieldID)
	{
		INTERNAL_CALL_GetStaticObjectField(clazz, fieldID, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetStaticObjectField(IntPtr clazz, IntPtr fieldID, out IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern bool GetStaticBooleanField(IntPtr clazz, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern byte GetStaticByteField(IntPtr clazz, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern char GetStaticCharField(IntPtr clazz, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern short GetStaticShortField(IntPtr clazz, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern int GetStaticIntField(IntPtr clazz, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern long GetStaticLongField(IntPtr clazz, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern float GetStaticFloatField(IntPtr clazz, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern double GetStaticDoubleField(IntPtr clazz, IntPtr fieldID);

	[ThreadAndSerializationSafe]
	public static IntPtr ToBooleanArray(bool[] array)
	{
		INTERNAL_CALL_ToBooleanArray(array, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ToBooleanArray(bool[] array, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr ToByteArray(byte[] array)
	{
		INTERNAL_CALL_ToByteArray(array, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ToByteArray(byte[] array, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr ToCharArray(char[] array)
	{
		INTERNAL_CALL_ToCharArray(array, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ToCharArray(char[] array, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr ToShortArray(short[] array)
	{
		INTERNAL_CALL_ToShortArray(array, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ToShortArray(short[] array, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr ToIntArray(int[] array)
	{
		INTERNAL_CALL_ToIntArray(array, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ToIntArray(int[] array, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr ToLongArray(long[] array)
	{
		INTERNAL_CALL_ToLongArray(array, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ToLongArray(long[] array, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr ToFloatArray(float[] array)
	{
		INTERNAL_CALL_ToFloatArray(array, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ToFloatArray(float[] array, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr ToDoubleArray(double[] array)
	{
		INTERNAL_CALL_ToDoubleArray(array, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ToDoubleArray(double[] array, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr ToObjectArray(IntPtr[] array, IntPtr arrayClass)
	{
		INTERNAL_CALL_ToObjectArray(array, arrayClass, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ToObjectArray(IntPtr[] array, IntPtr arrayClass, out IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern bool[] FromBooleanArray(IntPtr array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern byte[] FromByteArray(IntPtr array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern char[] FromCharArray(IntPtr array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern short[] FromShortArray(IntPtr array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern int[] FromIntArray(IntPtr array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern long[] FromLongArray(IntPtr array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern float[] FromFloatArray(IntPtr array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern double[] FromDoubleArray(IntPtr array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public static extern int GetArrayLength(IntPtr array);

	[ThreadAndSerializationSafe]
	public static IntPtr NewObjectArray(int size, IntPtr clazz, IntPtr obj)
	{
		INTERNAL_CALL_NewObjectArray(size, clazz, obj, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_NewObjectArray(int size, IntPtr clazz, IntPtr obj, out IntPtr value);

	[ThreadAndSerializationSafe]
	public static IntPtr GetObjectArrayElement(IntPtr array, int index)
	{
		INTERNAL_CALL_GetObjectArrayElement(array, index, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetObjectArrayElement(IntPtr array, int index, out IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[ThreadAndSerializationSafe]
	public static extern void SetObjectArrayElement(IntPtr array, int index, IntPtr obj);
}
