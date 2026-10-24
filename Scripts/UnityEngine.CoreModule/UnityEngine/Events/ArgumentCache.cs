using System;
using UnityEngine.Serialization;

namespace UnityEngine.Events;

[Serializable]
internal class ArgumentCache : ISerializationCallbackReceiver
{
	[FormerlySerializedAs("objectArgument")]
	[SerializeField]
	private Object m_ObjectArgument;

	[FormerlySerializedAs("objectArgumentAssemblyTypeName")]
	[SerializeField]
	private string m_ObjectArgumentAssemblyTypeName;

	[SerializeField]
	[FormerlySerializedAs("intArgument")]
	private int m_IntArgument;

	[SerializeField]
	[FormerlySerializedAs("floatArgument")]
	private float m_FloatArgument;

	[SerializeField]
	[FormerlySerializedAs("stringArgument")]
	private string m_StringArgument;

	[SerializeField]
	private bool m_BoolArgument;

	public Object unityObjectArgument => m_ObjectArgument;

	public string unityObjectArgumentAssemblyTypeName => m_ObjectArgumentAssemblyTypeName;

	public int intArgument => m_IntArgument;

	public float floatArgument => m_FloatArgument;

	public string stringArgument => m_StringArgument;

	public bool boolArgument => m_BoolArgument;

	private void TidyAssemblyTypeName()
	{
		if (!string.IsNullOrEmpty(m_ObjectArgumentAssemblyTypeName))
		{
			int num = int.MaxValue;
			int num2 = m_ObjectArgumentAssemblyTypeName.IndexOf(", Version=");
			if (num2 != -1)
			{
				num = Math.Min(num2, num);
			}
			num2 = m_ObjectArgumentAssemblyTypeName.IndexOf(", Culture=");
			if (num2 != -1)
			{
				num = Math.Min(num2, num);
			}
			num2 = m_ObjectArgumentAssemblyTypeName.IndexOf(", PublicKeyToken=");
			if (num2 != -1)
			{
				num = Math.Min(num2, num);
			}
			if (num != int.MaxValue)
			{
				m_ObjectArgumentAssemblyTypeName = m_ObjectArgumentAssemblyTypeName.Substring(0, num);
			}
			num2 = m_ObjectArgumentAssemblyTypeName.IndexOf(", UnityEngine.");
			if (num2 != -1 && m_ObjectArgumentAssemblyTypeName.EndsWith("Module"))
			{
				m_ObjectArgumentAssemblyTypeName = m_ObjectArgumentAssemblyTypeName.Substring(0, num2) + ", UnityEngine";
			}
		}
	}

	public void OnBeforeSerialize()
	{
		TidyAssemblyTypeName();
	}

	public void OnAfterDeserialize()
	{
		TidyAssemblyTypeName();
	}
}
