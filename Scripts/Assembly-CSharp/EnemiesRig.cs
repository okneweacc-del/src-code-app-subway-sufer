using UnityEngine;

public class EnemiesRig : MonoBehaviour
{
	[SerializeField]
	private Animation _guardAnimation;

	[SerializeField]
	private Animation _dogAnimation;

	public Animation GuardAnimation => _guardAnimation;

	public Animation DogAnimation => _dogAnimation;
}
