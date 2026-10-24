using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.U2D;

[Serializable]
[NativeHeader("Runtime/2D/Common/SpriteDataAccess.h")]
[NativeType(CodegenOptions.Custom, "ScriptingSpriteBone")]
[RequiredByNativeCode]
public struct SpriteBone
{
	[NativeName("name")]
	[SerializeField]
	private string m_Name;

	[NativeName("position")]
	[SerializeField]
	private Vector3 m_Position;

	[NativeName("rotation")]
	[SerializeField]
	private Quaternion m_Rotation;

	[SerializeField]
	[NativeName("length")]
	private float m_Length;

	[NativeName("parentId")]
	[SerializeField]
	private int m_ParentId;
}
