using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout((LayoutKind)0)]
[RequiredByNativeCode]
public sealed class WaitForSeconds : YieldInstruction
{
	internal float m_Seconds;

	public WaitForSeconds(float seconds)
	{
		m_Seconds = seconds;
	}
}
