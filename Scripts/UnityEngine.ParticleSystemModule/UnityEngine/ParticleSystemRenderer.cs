using UnityEngine.Bindings;

namespace UnityEngine;

[RequireComponent(typeof(Transform))]
[NativeHeader("Runtime/ParticleSystem/ParticleSystemRenderer.h")]
[NativeHeader("ParticleSystemScriptingClasses.h")]
[RequireComponent(typeof(Transform))]
public sealed class ParticleSystemRenderer : Renderer
{
}
