using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[AddComponentMenu("Event/Graphic Raycaster")]
public class GraphicRaycaster : BaseRaycaster
{
	public enum BlockingObjects
	{
		None = 0,
		TwoD = 1,
		ThreeD = 2,
		All = 3
	}

	protected const int kNoEventMaskSet = -1;

	[SerializeField]
	[FormerlySerializedAs("ignoreReversedGraphics")]
	private bool m_IgnoreReversedGraphics = true;

	[FormerlySerializedAs("blockingObjects")]
	[SerializeField]
	private BlockingObjects m_BlockingObjects = BlockingObjects.None;

	[SerializeField]
	protected LayerMask m_BlockingMask = -1;

	private Canvas m_Canvas;

	[NonSerialized]
	private List<Graphic> m_RaycastResults = new List<Graphic>();

	[NonSerialized]
	private static readonly List<Graphic> s_SortedGraphics = new List<Graphic>();

	public override int sortOrderPriority
	{
		get
		{
			if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				return canvas.sortingOrder;
			}
			return base.sortOrderPriority;
		}
	}

	public override int renderOrderPriority
	{
		get
		{
			if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				return canvas.rootCanvas.renderOrder;
			}
			return base.renderOrderPriority;
		}
	}

	public bool ignoreReversedGraphics
	{
		get
		{
			return m_IgnoreReversedGraphics;
		}
		set
		{
			m_IgnoreReversedGraphics = value;
		}
	}

	public BlockingObjects blockingObjects
	{
		get
		{
			return m_BlockingObjects;
		}
		set
		{
			m_BlockingObjects = value;
		}
	}

	private Canvas canvas
	{
		get
		{
			if (m_Canvas != null)
			{
				return m_Canvas;
			}
			m_Canvas = GetComponent<Canvas>();
			return m_Canvas;
		}
	}

	public override Camera eventCamera
	{
		get
		{
			if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null))
			{
				return null;
			}
			return (!(canvas.worldCamera != null)) ? Camera.main : canvas.worldCamera;
		}
	}

	protected GraphicRaycaster()
	{
	}

	public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		if (canvas == null)
		{
			return;
		}
		IList<Graphic> graphicsForCanvas = GraphicRegistry.GetGraphicsForCanvas(canvas);
		if (graphicsForCanvas == null || graphicsForCanvas.Count == 0)
		{
			return;
		}
		Camera camera = eventCamera;
		int num = ((canvas.renderMode != RenderMode.ScreenSpaceOverlay && !(camera == null)) ? camera.targetDisplay : canvas.targetDisplay);
		Vector3 vector = Display.RelativeMouseAt(eventData.position);
		if (vector != Vector3.zero)
		{
			int num2 = (int)vector.z;
			if (num2 != num)
			{
				return;
			}
		}
		else
		{
			vector = eventData.position;
		}
		Vector2 vector2;
		if (camera == null)
		{
			float num3 = Screen.width;
			float num4 = Screen.height;
			if (num > 0 && num < Display.displays.Length)
			{
				num3 = Display.displays[num].systemWidth;
				num4 = Display.displays[num].systemHeight;
			}
			vector2 = new Vector2(vector.x / num3, vector.y / num4);
		}
		else
		{
			vector2 = camera.ScreenToViewportPoint(vector);
		}
		if (vector2.x < 0f || vector2.x > 1f || vector2.y < 0f || vector2.y > 1f)
		{
			return;
		}
		float num5 = float.MaxValue;
		Ray r = default;
		if (camera != null)
		{
			r = camera.ScreenPointToRay(vector);
		}
		if (canvas.renderMode != RenderMode.ScreenSpaceOverlay && blockingObjects != BlockingObjects.None)
		{
			float f = 100f;
			if (camera != null)
			{
				float z = r.direction.z;
				f = ((!Mathf.Approximately(0f, z)) ? Mathf.Abs((camera.farClipPlane - camera.nearClipPlane) / z) : float.PositiveInfinity);
			}
			if ((blockingObjects == BlockingObjects.ThreeD || blockingObjects == BlockingObjects.All) && ReflectionMethodsCache.Singleton.raycast3D != null)
			{
				RaycastHit[] array = ReflectionMethodsCache.Singleton.raycast3DAll(r, f, m_BlockingMask);
				if (array.Length > 0)
				{
					num5 = array[0].distance;
				}
			}
			if ((blockingObjects == BlockingObjects.TwoD || blockingObjects == BlockingObjects.All) && ReflectionMethodsCache.Singleton.raycast2D != null)
			{
				RaycastHit2D[] array2 = ReflectionMethodsCache.Singleton.getRayIntersectionAll(r, f, m_BlockingMask);
				if (array2.Length > 0)
				{
					num5 = array2[0].distance;
				}
			}
		}
		m_RaycastResults.Clear();
		Raycast(canvas, camera, vector, graphicsForCanvas, m_RaycastResults);
		int count = m_RaycastResults.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = m_RaycastResults[i].gameObject;
			bool flag = true;
			if (ignoreReversedGraphics)
			{
				if (camera == null)
				{
					Vector3 rhs = gameObject.transform.rotation * Vector3.forward;
					flag = Vector3.Dot(Vector3.forward, rhs) > 0f;
				}
				else
				{
					Vector3 lhs = camera.transform.rotation * Vector3.forward;
					Vector3 rhs2 = gameObject.transform.rotation * Vector3.forward;
					flag = Vector3.Dot(lhs, rhs2) > 0f;
				}
			}
			if (!flag)
			{
				continue;
			}
			float num6 = 0f;
			if (camera == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				num6 = 0f;
			}
			else
			{
				Transform transform = gameObject.transform;
				Vector3 forward = transform.forward;
				num6 = Vector3.Dot(forward, transform.position - r.origin) / Vector3.Dot(forward, r.direction);
				if (num6 < 0f)
				{
					continue;
				}
			}
			if (!(num6 >= num5))
			{
				RaycastResult item = new RaycastResult
				{
					gameObject = gameObject,
					module = this,
					distance = num6,
					screenPosition = vector,
					index = resultAppendList.Count,
					depth = m_RaycastResults[i].depth,
					sortingLayer = canvas.sortingLayerID,
					sortingOrder = canvas.sortingOrder
				};
				resultAppendList.Add(item);
			}
		}
	}

	private static void Raycast(Canvas canvas, Camera eventCamera, Vector2 pointerPosition, IList<Graphic> foundGraphics, List<Graphic> results)
	{
		int count = foundGraphics.Count;
		for (int i = 0; i < count; i++)
		{
			Graphic graphic = foundGraphics[i];
			if (graphic.depth != -1 && graphic.raycastTarget && !graphic.canvasRenderer.cull && RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, pointerPosition, eventCamera) && (!(eventCamera != null) || !(eventCamera.WorldToScreenPoint(graphic.rectTransform.position).z > eventCamera.farClipPlane)) && graphic.Raycast(pointerPosition, eventCamera))
			{
				s_SortedGraphics.Add(graphic);
			}
		}
		s_SortedGraphics.Sort((Graphic g1, Graphic g2) => g2.depth.CompareTo(g1.depth));
		count = s_SortedGraphics.Count;
		for (int num = 0; num < count; num++)
		{
			results.Add(s_SortedGraphics[num]);
		}
		s_SortedGraphics.Clear();
	}
}
