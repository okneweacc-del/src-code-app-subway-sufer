using System;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace UnityEngine.UI;

public abstract class MaskableGraphic : Graphic, IClippable, IMaskable, IMaterialModifier
{
	[Serializable]
	public class CullStateChangedEvent : UnityEvent<bool>
	{
	}

	[NonSerialized]
	protected bool m_ShouldRecalculateStencil = true;

	[NonSerialized]
	protected Material m_MaskMaterial;

	[NonSerialized]
	private RectMask2D m_ParentMask;

	[NonSerialized]
	private bool m_Maskable = true;

	[NonSerialized]
	[Obsolete("Not used anymore.", true)]
	protected bool m_IncludeForMasking = false;

	[SerializeField]
	private CullStateChangedEvent m_OnCullStateChanged = new CullStateChangedEvent();

	[NonSerialized]
	[Obsolete("Not used anymore", true)]
	protected bool m_ShouldRecalculate = true;

	[NonSerialized]
	protected int m_StencilValue;

	private readonly Vector3[] m_Corners = new Vector3[4];

	public CullStateChangedEvent onCullStateChanged
	{
		get
		{
			return m_OnCullStateChanged;
		}
		set
		{
			m_OnCullStateChanged = value;
		}
	}

	public bool maskable
	{
		get
		{
			return m_Maskable;
		}
		set
		{
			if (value != m_Maskable)
			{
				m_Maskable = value;
				m_ShouldRecalculateStencil = true;
				SetMaterialDirty();
			}
		}
	}

	private Rect rootCanvasRect
	{
		get
		{
			base.rectTransform.GetWorldCorners(m_Corners);
			if ((bool)base.canvas)
			{
				Matrix4x4 worldToLocalMatrix = base.canvas.rootCanvas.transform.worldToLocalMatrix;
				for (int i = 0; i < 4; i++)
				{
					ref Vector3 reference = ref m_Corners[i];
					reference = worldToLocalMatrix.MultiplyPoint(m_Corners[i]);
				}
			}
			return new Rect(m_Corners[0].x, m_Corners[0].y, m_Corners[2].x - m_Corners[0].x, m_Corners[2].y - m_Corners[0].y);
		}
	}

	GameObject IClippable.gameObject => base.gameObject;

	public virtual Material GetModifiedMaterial(Material baseMaterial)
	{
		Material material = baseMaterial;
		if (m_ShouldRecalculateStencil)
		{
			Transform stopAfter = MaskUtilities.FindRootSortOverrideCanvas(base.transform);
			m_StencilValue = (maskable ? MaskUtilities.GetStencilDepth(base.transform, stopAfter) : 0);
			m_ShouldRecalculateStencil = false;
		}
		Mask component = GetComponent<Mask>();
		if (m_StencilValue > 0 && (component == null || !component.IsActive()))
		{
			Material maskMaterial = StencilMaterial.Add(material, (1 << m_StencilValue) - 1, StencilOp.Keep, CompareFunction.Equal, ColorWriteMask.All, (1 << m_StencilValue) - 1, 0);
			StencilMaterial.Remove(m_MaskMaterial);
			m_MaskMaterial = maskMaterial;
			material = m_MaskMaterial;
		}
		return material;
	}

	public virtual void Cull(Rect clipRect, bool validRect)
	{
		bool cull = !validRect || !clipRect.Overlaps(rootCanvasRect, allowInverse: true);
		UpdateCull(cull);
	}

	private void UpdateCull(bool cull)
	{
		if (base.canvasRenderer.cull != cull)
		{
			base.canvasRenderer.cull = cull;
			UISystemProfilerApi.AddMarker("MaskableGraphic.cullingChanged", this);
			m_OnCullStateChanged.Invoke(cull);
			OnCullingChanged();
		}
	}

	public virtual void SetClipRect(Rect clipRect, bool validRect)
	{
		if (validRect)
		{
			base.canvasRenderer.EnableRectClipping(clipRect);
		}
		else
		{
			base.canvasRenderer.DisableRectClipping();
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		m_ShouldRecalculateStencil = true;
		UpdateClipParent();
		SetMaterialDirty();
		if (GetComponent<Mask>() != null)
		{
			MaskUtilities.NotifyStencilStateChanged(this);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		m_ShouldRecalculateStencil = true;
		SetMaterialDirty();
		UpdateClipParent();
		StencilMaterial.Remove(m_MaskMaterial);
		m_MaskMaterial = null;
		if (GetComponent<Mask>() != null)
		{
			MaskUtilities.NotifyStencilStateChanged(this);
		}
	}

	protected override void OnTransformParentChanged()
	{
		base.OnTransformParentChanged();
		if (base.isActiveAndEnabled)
		{
			m_ShouldRecalculateStencil = true;
			UpdateClipParent();
			SetMaterialDirty();
		}
	}

	[Obsolete("Not used anymore.", true)]
	public virtual void ParentMaskStateChanged()
	{
	}

	protected override void OnCanvasHierarchyChanged()
	{
		base.OnCanvasHierarchyChanged();
		if (base.isActiveAndEnabled)
		{
			m_ShouldRecalculateStencil = true;
			UpdateClipParent();
			SetMaterialDirty();
		}
	}

	private void UpdateClipParent()
	{
		RectMask2D rectMask2D = ((!maskable || !IsActive()) ? null : MaskUtilities.GetRectMaskForClippable(this));
		if (m_ParentMask != null && (rectMask2D != m_ParentMask || !rectMask2D.IsActive()))
		{
			m_ParentMask.RemoveClippable(this);
			UpdateCull(cull: false);
		}
		if (rectMask2D != null && rectMask2D.IsActive())
		{
			rectMask2D.AddClippable(this);
		}
		m_ParentMask = rectMask2D;
	}

	public virtual void RecalculateClipping()
	{
		UpdateClipParent();
	}

	public virtual void RecalculateMasking()
	{
		m_ShouldRecalculateStencil = true;
		SetMaterialDirty();
	}
}
