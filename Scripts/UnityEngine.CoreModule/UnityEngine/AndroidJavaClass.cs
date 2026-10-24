using System;

namespace UnityEngine;

public class AndroidJavaClass : AndroidJavaObject
{
	public AndroidJavaClass(string className)
	{
		_AndroidJavaClass(className);
	}

	internal AndroidJavaClass(IntPtr jclass)
	{
		if (jclass == IntPtr.Zero)
		{
			throw new Exception("JNI: Init'd AndroidJavaClass with null ptr!");
		}
		m_jclass = new GlobalJavaObjectRef(jclass);
		m_jobject = new GlobalJavaObjectRef(IntPtr.Zero);
	}

	private void _AndroidJavaClass(string className)
	{
		DebugPrint("Creating AndroidJavaClass from " + className);
		using AndroidJavaObject androidJavaObject = AndroidJavaObject.FindClass(className);
		m_jclass = new GlobalJavaObjectRef(androidJavaObject.GetRawObject());
		m_jobject = new GlobalJavaObjectRef(IntPtr.Zero);
	}
}
