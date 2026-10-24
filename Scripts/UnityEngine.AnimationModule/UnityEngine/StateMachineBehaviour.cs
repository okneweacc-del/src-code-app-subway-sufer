using UnityEngine.Scripting;

namespace UnityEngine;

[RequiredByNativeCode]
public abstract class StateMachineBehaviour : ScriptableObject
{
	public virtual void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
