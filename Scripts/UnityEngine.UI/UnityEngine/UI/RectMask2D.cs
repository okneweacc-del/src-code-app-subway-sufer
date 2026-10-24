using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI;

[DisallowMultipleComponent]
[AddComponentMenu("UI/Rect Mask 2D", 13)]
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class RectMask2D : UIBehaviour, IClipper, ICanvasRaycastFilter
{
	[NonSerialized]
	private readonly RectangularVertexClipper m_VertexClipper = new RectangularVertexClipper();

	[NonSerialized]
	private RectTransform m_RectTransform;

	[NonSerialized]
	private HashSet<IClippable> m_ClipTargets = new HashSet<IClippable>();

	[NonSerialized]
	private bool m_ShouldRecalculateClipRects;

	[NonSerialized]
	private List<RectMask2D> m_Clippers = new List<RectMask2D>();

	[NonSerialized]
	private Rect m_LastClipRectCanvasSpace;

	[NonSerialized]
	private bool m_ForceClip;

	[NonSerialized]
	private Canvas m_Canvas;

	private Vector3[] m_Corners = new Vector3[4];

	private Canvas Canvas
	{
		get
		{
			if (m_Canvas == null)
			{
				List<Canvas> list = ListPool<Canvas>.Get();
				base.gameObject.GetComponentsInParent(includeInactive: false, list);
				if (list.Count > 0)
				{
					m_Canvas = list[list.Count - 1];
				}
				else
				{
					m_Canvas = null;
				}
				ListPool<Canvas>.Release(list);
			}
			return m_Canvas;
		}
	}

	public Rect canvasRect => m_VertexClipper.GetCanvasRect(rectTransform, Canvas);

	public RectTransform rectTransform => m_RectTransform ?? (m_RectTransform = GetComponent<RectTransform>());

	private Rect rootCanvasRect
	{
		get
		{
			rectTransform.GetWorldCorners(m_Corners);
			if (!object.ReferenceEquals(Canvas, null))
			{
				Canvas rootCanvas = Canvas.rootCanvas;
				for (int i = 0; i < 4; i++)
				{
					ref Vector3 reference = ref m_Corners[i];
					reference = rootCanvas.transform.InverseTransformPoint(m_Corners[i]);
				}
			}
			return new Rect(m_Corners[0].x, m_Corners[0].y, m_Corners[2].x - m_Corners[0].x, m_Corners[2].y - m_Corners[0].y);
		}
	}

	protected RectMask2D()
	{
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		m_ShouldRecalculateClipRects = true;
		ClipperRegistry.Register(this);
		MaskUtilities.Notify2DMaskStateChanged(this);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		m_ClipTargets.Clear();
		m_Clippers.Clear();
		ClipperRegistry.Unregister(this);
		MaskUtilities.Notify2DMaskStateChanged(this);
	}

	public virtual bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
	{
		if (!base.isActiveAndEnabled)
		{
			return true;
		}
		return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, sp, eventCamera);
	}

	public virtual void PerformClipping()
	{
		if (object.ReferenceEquals(Canvas, null))
		{
			return;
		}
		if (m_ShouldRecalculateClipRects)
		{
			MaskUtilities.GetRectMasksForClip(this, m_Clippers);
			m_ShouldRecalculateClipRects = false;
		}
		bool validRect = true;
		Rect rect = Clipping.FindCullAndClipWorldRect(m_Clippers, out validRect);
		RenderMode renderMode = Canvas.rootCanvas.renderMode;
		bool flag = (renderMode == RenderMode.ScreenSpaceCamera || renderMode == RenderMode.ScreenSpaceOverlay) && !rect.Overlaps(rootCanvasRect, allowInverse: true);
		bool flag2 = rect != m_LastClipRectCanvasSpace;
		bool forceClip = m_ForceClip;
		foreach (IClippable clipTarget in m_ClipTargets)
		{
			if (flag2 || forceClip)
			{
				clipTarget.SetClipRect(rect, validRect);
			}
			MaskableGraphic maskableGraphic = clipTarget as MaskableGraphic;
			if (!(maskableGraphic != null) || maskableGraphic.canvasRenderer.hasMoved || flag2)
			{
				clipTarget.Cull((!flag) ? rect : Rect.zero, !flag && validRect);
			}
		}
		m_LastClipRectCanvasSpace = rect;
		m_ForceClip = false;
	}

	public void AddClippable(IClippable clippable)
	{
		if (clippable != null)
		{
			m_ShouldRecalculateClipRects = true;
			if (!m_ClipTargets.Contains(clippable))
			{
				m_ClipTargets.Add(clippable);
			}
			m_ForceClip = true;
		}
	}

	public void RemoveClippable(IClippable clippable)
	{
		if (clippable != null)
		{
			m_ShouldRecalculateClipRects = true;
			clippable.SetClipRect(default, validRect: false);
			m_ClipTargets.Remove(clippable);
			m_ForceClip = true;
		}
	}

	protected override void OnTransformParentChanged()
	{
		base.OnTransformParentChanged();
		m_ShouldRecalculateClipRects = true;
	}

	protected override void OnCanvasHierarchyChanged()
	{
		m_Canvas = null;
		base.OnCanvasHierarchyChanged();
		m_ShouldRecalculateClipRects = true;
	}
}
