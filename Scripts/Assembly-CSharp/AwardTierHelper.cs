using UnityEngine;

public class AwardTierHelper : MonoBehaviour
{
	[SerializeField]
	private GameObject inactiveElementsParent;

	[SerializeField]
	private GameObject activeElement;

	[SerializeField]
	private GameObject inactiveHighlightElement;

	public void TierComplete(bool complete)
	{
		inactiveElementsParent.SetActive(!complete);
		activeElement.SetActive(complete);
		inactiveHighlightElement.SetActive(value: false);
	}

	public void TierNextComplete()
	{
		inactiveElementsParent.SetActive(value: false);
		activeElement.SetActive(value: false);
		inactiveHighlightElement.SetActive(value: true);
	}
}
