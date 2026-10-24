using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine;

internal static class BeforeRenderHelper
{
	private struct OrderBlock
	{
		internal int order;

		internal UnityAction callback;
	}

	private static List<OrderBlock> s_OrderBlocks = new List<OrderBlock>();

	public static void Invoke()
	{
		lock (s_OrderBlocks)
		{
			for (int i = 0; i < s_OrderBlocks.Count; i++)
			{
				s_OrderBlocks[i].callback?.Invoke();
			}
		}
	}
}
