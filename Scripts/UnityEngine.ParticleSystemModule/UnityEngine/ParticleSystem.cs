using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
[RequireComponent(typeof(Transform))]
public sealed class ParticleSystem : Component
{
	public struct MinMaxCurve
	{
		private ParticleSystemCurveMode m_Mode;

		private float m_CurveMultiplier;

		private AnimationCurve m_CurveMin;

		private AnimationCurve m_CurveMax;

		private float m_ConstantMin;

		private float m_ConstantMax;

		public MinMaxCurve(float constant)
		{
			m_Mode = ParticleSystemCurveMode.Constant;
			m_CurveMultiplier = 0f;
			m_CurveMin = null;
			m_CurveMax = null;
			m_ConstantMin = 0f;
			m_ConstantMax = constant;
		}

		public MinMaxCurve(float min, float max)
		{
			m_Mode = ParticleSystemCurveMode.TwoConstants;
			m_CurveMultiplier = 0f;
			m_CurveMin = null;
			m_CurveMax = null;
			m_ConstantMin = min;
			m_ConstantMax = max;
		}

		public static implicit operator MinMaxCurve(float constant)
		{
			return new MinMaxCurve(constant);
		}
	}

	public struct MinMaxGradient(Color min, Color max)
	{
		private ParticleSystemGradientMode m_Mode = ParticleSystemGradientMode.TwoColors;

		private Gradient m_GradientMin = null;

		private Gradient m_GradientMax = null;

		private Color m_ColorMin = min;

		private Color m_ColorMax = max;
	}

	public struct MainModule
	{
		private ParticleSystem m_ParticleSystem;

		public float duration => GetDuration(m_ParticleSystem);

		public MinMaxCurve startSpeed
		{
			set
			{
				SetStartSpeed(m_ParticleSystem, ref value);
			}
		}

		public float startSizeMultiplier
		{
			set
			{
				SetStartSizeXMultiplier(m_ParticleSystem, value);
			}
		}

		public MinMaxCurve startRotation
		{
			set
			{
				SetStartRotationZ(m_ParticleSystem, ref value);
			}
		}

		public MinMaxGradient startColor
		{
			set
			{
				SetStartColor(m_ParticleSystem, ref value);
			}
		}

		public float simulationSpeed
		{
			set
			{
				SetSimulationSpeed(m_ParticleSystem, value);
			}
		}

		public bool playOnAwake
		{
			set
			{
				SetPlayOnAwake(m_ParticleSystem, value);
			}
		}

		internal MainModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern float GetDuration(ParticleSystem system);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartSpeed(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartSizeXMultiplier(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartRotationZ(ParticleSystem system, ref MinMaxCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetStartColor(ParticleSystem system, ref MinMaxGradient gradient);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetSimulationSpeed(ParticleSystem system, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetPlayOnAwake(ParticleSystem system, bool value);
	}

	public struct EmissionModule
	{
		private ParticleSystem m_ParticleSystem;

		public bool enabled
		{
			set
			{
				SetEnabled(m_ParticleSystem, value);
			}
		}

		public MinMaxCurve rateOverTime
		{
			set
			{
				SetRateOverTime(m_ParticleSystem, ref value);
			}
		}

		internal EmissionModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetEnabled(ParticleSystem system, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		private static extern void SetRateOverTime(ParticleSystem system, ref MinMaxCurve curve);
	}

	public MainModule main => new MainModule(this);

	public EmissionModule emission => new EmissionModule(this);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Simulate(float t, [DefaultValue("true")] bool withChildren, [DefaultValue("true")] bool restart, [DefaultValue("true")] bool fixedTimeStep);

	[ExcludeFromDocs]
	public void Simulate(float t)
	{
		bool fixedTimeStep = true;
		bool restart = true;
		bool withChildren = true;
		Simulate(t, withChildren, restart, fixedTimeStep);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Play([DefaultValue("true")] bool withChildren);

	[ExcludeFromDocs]
	public void Play()
	{
		bool withChildren = true;
		Play(withChildren);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Stop([DefaultValue("true")] bool withChildren, [DefaultValue("ParticleSystemStopBehavior.StopEmitting")] ParticleSystemStopBehavior stopBehavior);

	[ExcludeFromDocs]
	public void Stop()
	{
		ParticleSystemStopBehavior stopBehavior = ParticleSystemStopBehavior.StopEmitting;
		bool withChildren = true;
		Stop(withChildren, stopBehavior);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Clear([DefaultValue("true")] bool withChildren);

	[ExcludeFromDocs]
	public void Clear()
	{
		bool withChildren = true;
		Clear(withChildren);
	}
}
