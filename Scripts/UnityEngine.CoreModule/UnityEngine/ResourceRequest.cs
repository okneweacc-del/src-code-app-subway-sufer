using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout((LayoutKind)0)]
[RequiredByNativeCode]
public class ResourceRequest : AsyncOperation
{
	internal string m_Path;

	internal Type m_Type;
}
