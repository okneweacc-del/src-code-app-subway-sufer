using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout((LayoutKind)0)]
[RequiredByNativeCode]
public class Collision
{
	internal Vector3 m_Impulse;

	internal Vector3 m_RelativeVelocity;

	internal Rigidbody m_Rigidbody;

	internal Collider m_Collider;

	internal ContactPoint[] m_Contacts;
}
