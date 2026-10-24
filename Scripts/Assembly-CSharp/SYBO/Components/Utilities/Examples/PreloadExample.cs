using UnityEngine;
using UnityEngine.UI;

namespace SYBO.Components.Utilities.Examples;

public class PreloadExample : MonoBehaviour
{
	public Text text;

	private void Start()
	{
		StartBalancing();
	}

	private void Progress(float progress)
	{
		text.text = $"Progress: {progress * 100f:0.00}%";
	}

	public void StartBalancing()
	{
		PoolManager.BeginBalancing(Progress);
	}
}
