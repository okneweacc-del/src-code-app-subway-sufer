using System;
using System.Reflection;
using UnityEngine.Scripting;

namespace UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class GUITargetAttribute : Attribute
{
	internal int displayMask;

	[RequiredByNativeCode]
	private static int GetGUITargetAttrValue(Type klass, string methodName)
	{
		MethodInfo method = klass.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (method != null)
		{
			object[] customAttributes = method.GetCustomAttributes(inherit: true);
			if (customAttributes != null)
			{
				for (int i = 0; i < customAttributes.Length; i++)
				{
					if (customAttributes[i].GetType() == typeof(GUITargetAttribute))
					{
						GUITargetAttribute gUITargetAttribute = customAttributes[i] as GUITargetAttribute;
						return gUITargetAttribute.displayMask;
					}
				}
			}
		}
		return -1;
	}
}
