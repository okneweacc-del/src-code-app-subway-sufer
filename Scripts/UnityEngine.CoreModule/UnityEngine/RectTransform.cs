using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeClass("UI::RectTransform")]
public sealed class RectTransform : Transform
{
	public delegate void ReapplyDrivenProperties(RectTransform driven);

	public enum Edge
	{
		Left = 0,
		Right = 1,
		Top = 2,
		Bottom = 3
	}

	public enum Axis
	{
		Horizontal = 0,
		Vertical = 1
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static ReapplyDrivenProperties reapplyDrivenProperties__BackingField;

	public Rect rect
	{
		get
		{
			INTERNAL_get_rect(out var value);
			return value;
		}
	}

	public Vector2 anchorMin
	{
		get
		{
			INTERNAL_get_anchorMin(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_anchorMin(ref value);
		}
	}

	public Vector2 anchorMax
	{
		get
		{
			INTERNAL_get_anchorMax(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_anchorMax(ref value);
		}
	}

	public Vector2 anchoredPosition
	{
		get
		{
			INTERNAL_get_anchoredPosition(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_anchoredPosition(ref value);
		}
	}

	public Vector2 sizeDelta
	{
		get
		{
			INTERNAL_get_sizeDelta(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_sizeDelta(ref value);
		}
	}

	public Vector2 pivot
	{
		get
		{
			INTERNAL_get_pivot(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_pivot(ref value);
		}
	}

	public static event ReapplyDrivenProperties reapplyDrivenProperties
	{
		add
		{
			ReapplyDrivenProperties reapplyDrivenProperties = reapplyDrivenProperties__BackingField;
			ReapplyDrivenProperties reapplyDrivenProperties2;
			do
			{
				reapplyDrivenProperties2 = reapplyDrivenProperties;
				reapplyDrivenProperties = Interlocked.CompareExchange(ref reapplyDrivenProperties__BackingField, (ReapplyDrivenProperties)Delegate.Combine(reapplyDrivenProperties2, value), reapplyDrivenProperties);
			}
			while ((object)reapplyDrivenProperties != reapplyDrivenProperties2);
		}
		remove
		{
			ReapplyDrivenProperties reapplyDrivenProperties = reapplyDrivenProperties__BackingField;
			ReapplyDrivenProperties reapplyDrivenProperties2;
			do
			{
				reapplyDrivenProperties2 = reapplyDrivenProperties;
				reapplyDrivenProperties = Interlocked.CompareExchange(ref reapplyDrivenProperties__BackingField, (ReapplyDrivenProperties)Delegate.Remove(reapplyDrivenProperties2, value), reapplyDrivenProperties);
			}
			while ((object)reapplyDrivenProperties != reapplyDrivenProperties2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_rect(out Rect value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_anchorMin(out Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_anchorMin(ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_anchorMax(out Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_anchorMax(ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_anchoredPosition(out Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_anchoredPosition(ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_sizeDelta(out Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_sizeDelta(ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_pivot(out Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_pivot(ref Vector2 value);

	[RequiredByNativeCode]
	internal static void SendReapplyDrivenProperties(RectTransform driven)
	{
		if (reapplyDrivenProperties__BackingField != null)
		{
			reapplyDrivenProperties__BackingField(driven);
		}
	}

	public void GetLocalCorners(Vector3[] fourCornersArray)
	{
		if (fourCornersArray == null || fourCornersArray.Length < 4)
		{
			Debug.LogError("Calling GetLocalCorners with an array that is null or has less than 4 elements.");
			return;
		}
		Rect rect = this.rect;
		float x = rect.x;
		float y = rect.y;
		float xMax = rect.xMax;
		float yMax = rect.yMax;
		ref Vector3 reference = ref fourCornersArray[0];
		reference = new Vector3(x, y, 0f);
		ref Vector3 reference2 = ref fourCornersArray[1];
		reference2 = new Vector3(x, yMax, 0f);
		ref Vector3 reference3 = ref fourCornersArray[2];
		reference3 = new Vector3(xMax, yMax, 0f);
		ref Vector3 reference4 = ref fourCornersArray[3];
		reference4 = new Vector3(xMax, y, 0f);
	}

	public void GetWorldCorners(Vector3[] fourCornersArray)
	{
		if (fourCornersArray == null || fourCornersArray.Length < 4)
		{
			Debug.LogError("Calling GetWorldCorners with an array that is null or has less than 4 elements.");
			return;
		}
		GetLocalCorners(fourCornersArray);
		Matrix4x4 matrix4x = base.transform.localToWorldMatrix;
		for (int i = 0; i < 4; i++)
		{
			ref Vector3 reference = ref fourCornersArray[i];
			reference = matrix4x.MultiplyPoint(fourCornersArray[i]);
		}
	}

	public void SetInsetAndSizeFromParentEdge(Edge edge, float inset, float size)
	{
		int index = ((edge == Edge.Top || edge == Edge.Bottom) ? 1 : 0);
		bool flag = edge == Edge.Top || edge == Edge.Right;
		float value = (flag ? 1 : 0);
		Vector2 vector = anchorMin;
		vector[index] = value;
		anchorMin = vector;
		vector = anchorMax;
		vector[index] = value;
		anchorMax = vector;
		Vector2 vector2 = sizeDelta;
		vector2[index] = size;
		sizeDelta = vector2;
		Vector2 vector3 = anchoredPosition;
		vector3[index] = ((!flag) ? (inset + size * pivot[index]) : (0f - inset - size * (1f - pivot[index])));
		anchoredPosition = vector3;
	}

	public void SetSizeWithCurrentAnchors(Axis axis, float size)
	{
		Vector2 vector = sizeDelta;
		vector[(int)axis] = size - GetParentSize()[(int)axis] * (anchorMax[(int)axis] - anchorMin[(int)axis]);
		sizeDelta = vector;
	}

	private Vector2 GetParentSize()
	{
		RectTransform rectTransform = base.parent as RectTransform;
		if (!rectTransform)
		{
			return Vector2.zero;
		}
		return rectTransform.rect.size;
	}
}
