using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Networking;

[StructLayout((LayoutKind)0)]
[NativeHeader("UnityWebRequestScriptingClasses.h")]
[UsedByNativeCode]
[NativeHeader("Modules/UnityWebRequest/Public/UnityWebRequestAsyncOperation.h")]
public class UnityWebRequestAsyncOperation : AsyncOperation
{
	internal UnityWebRequest webRequest { set; }
}
