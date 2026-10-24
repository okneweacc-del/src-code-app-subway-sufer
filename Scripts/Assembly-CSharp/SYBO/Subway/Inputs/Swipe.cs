using UnityEngine;

namespace SYBO.Subway.Inputs;

public class Swipe
{
	public Vector3 start;

	public Vector3 end;

	public float startTime;

	public float endTime;

	public override string ToString()
	{
		return $"[Swipe (start:{start}, end:{end}]";
	}
}
