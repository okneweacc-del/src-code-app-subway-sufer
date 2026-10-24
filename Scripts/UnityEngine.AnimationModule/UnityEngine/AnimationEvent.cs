using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[Serializable]
[StructLayout((LayoutKind)0)]
[RequiredByNativeCode]
public sealed class AnimationEvent
{
	internal float m_Time;

	internal string m_FunctionName;

	internal string m_StringParameter;

	internal Object m_ObjectReferenceParameter;

	internal float m_FloatParameter;

	internal int m_IntParameter;

	internal int m_MessageOptions;

	internal AnimationEventSource m_Source;

	internal AnimationState m_StateSender;

	internal AnimatorStateInfo m_AnimatorStateInfo;

	internal AnimatorClipInfo m_AnimatorClipInfo;

	public int intParameter
	{
		set
		{
			m_IntParameter = value;
		}
	}

	public string functionName
	{
		get
		{
			return m_FunctionName;
		}
		set
		{
			m_FunctionName = value;
		}
	}

	public float time
	{
		get
		{
			return m_Time;
		}
		set
		{
			m_Time = value;
		}
	}

	public SendMessageOptions messageOptions
	{
		set
		{
			m_MessageOptions = (int)value;
		}
	}

	public bool isFiredByLegacy => m_Source == AnimationEventSource.Legacy;

	public AnimationState animationState
	{
		get
		{
			if (!isFiredByLegacy)
			{
				Debug.LogError("AnimationEvent was not fired by Animation component, you shouldn't use AnimationEvent.animationState");
			}
			return m_StateSender;
		}
	}

	public AnimationEvent()
	{
		m_Time = 0f;
		m_FunctionName = "";
		m_StringParameter = "";
		m_ObjectReferenceParameter = null;
		m_FloatParameter = 0f;
		m_IntParameter = 0;
		m_MessageOptions = 0;
		m_Source = AnimationEventSource.NoSource;
		m_StateSender = null;
	}
}
