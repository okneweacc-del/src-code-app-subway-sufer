using System;

namespace UnityEngine;

internal class AudioExtensionDefinition
{
	private string assemblyName;

	private string extensionNamespace;

	private string extensionTypeName;

	private Type extensionType;

	public Type GetExtensionType()
	{
		if (extensionType == null)
		{
			extensionType = Type.GetType(extensionNamespace + "." + extensionTypeName + ", " + assemblyName);
		}
		return extensionType;
	}
}
