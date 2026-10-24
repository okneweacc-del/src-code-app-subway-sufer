using System;
using UnityEngine;

namespace SYBO.Common.Utility;

public class SubscribableMonobehaviour : MonoBehaviour
{
	public Action OnUpdate;

	public Action OnLateUpdate;

	public Action OnFixedUpdate;

	private void Update()
	{
		if (OnUpdate != null)
		{
			OnUpdate();
		}
	}

	private void LateUpdate()
	{
		if (OnLateUpdate != null)
		{
			OnLateUpdate();
		}
	}

	private void FixedUpdate()
	{
		if (OnFixedUpdate != null)
		{
			OnFixedUpdate();
		}
	}
}
