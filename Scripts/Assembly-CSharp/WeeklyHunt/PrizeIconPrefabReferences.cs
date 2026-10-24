using System;
using UnityEngine;

namespace WeeklyHunt;

public class PrizeIconPrefabReferences : MonoBehaviour
{
	[Serializable]
	private struct PrizeModel
	{
		public WeeklyHuntData.WeeklyHuntPrize rewardType;

		public GameObject iconPrefab;
	}

	[SerializeField]
	private PrizeModel[] modelReferences = new PrizeModel[0];

	public GameObject GetPrefab(WeeklyHuntData.PrizeData prizeData)
	{
		WeeklyHuntData.WeeklyHuntPrize weeklyHuntPrize = WeeklyHuntManager.DeterminePrize(prizeData);
		for (int i = 0; i < modelReferences.Length; i++)
		{
			if (modelReferences[i].rewardType == weeklyHuntPrize)
			{
				GameObject iconPrefab = modelReferences[i].iconPrefab;
				if (!iconPrefab)
				{
					SDebug.LogError($"Prize '{weeklyHuntPrize.ToString()}' doesn't have a model set", base.gameObject);
				}
				return iconPrefab;
			}
		}
		SDebug.LogError($"Unable to find icon prefab for prize '{prizeData.rewardType.ToString()}'", base.gameObject);
		return null;
	}
}
