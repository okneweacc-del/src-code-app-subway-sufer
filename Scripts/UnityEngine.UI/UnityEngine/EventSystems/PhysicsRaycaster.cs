using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Event/Physics Raycaster")]
public class PhysicsRaycaster : BaseRaycaster
{
	protected const int kNoEventMaskSet = -1;

	protected Camera m_EventCamera;

	[SerializeField]
	protected LayerMask m_EventMask = -1;

	[SerializeField]
	protected int m_MaxRayIntersections = 0;

	protected int m_LastMaxRayIntersections = 0;

	private RaycastHit[] m_Hits;

	public override Camera eventCamera
	{
		get
		{
			if (m_EventCamera == null)
			{
				m_EventCamera = GetComponent<Camera>();
			}
			return m_EventCamera ?? Camera.main;
		}
	}

	public virtual int depth => (!(eventCamera != null)) ? 16777215 : ((int)eventCamera.depth);

	public int finalEventMask => (!(eventCamera != null)) ? (-1) : (eventCamera.cullingMask & (int)m_EventMask);

	public LayerMask eventMask
	{
		get
		{
			return m_EventMask;
		}
		set
		{
			m_EventMask = value;
		}
	}

	public int maxRayIntersections
	{
		get
		{
			return m_MaxRayIntersections;
		}
		set
		{
			m_MaxRayIntersections = value;
		}
	}

	protected PhysicsRaycaster()
	{
	}

	protected void ComputeRayAndDistance(PointerEventData eventData, out Ray ray, out float distanceToClipPlane)
	{
		ray = eventCamera.ScreenPointToRay(eventData.position);
		float z = ray.direction.z;
		distanceToClipPlane = ((!Mathf.Approximately(0f, z)) ? Mathf.Abs((eventCamera.farClipPlane - eventCamera.nearClipPlane) / z) : float.PositiveInfinity);
	}

	public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		if (eventCamera == null || !eventCamera.pixelRect.Contains(eventData.position))
		{
			return;
		}
		ComputeRayAndDistance(eventData, out var ray, out var distanceToClipPlane);
		int num = 0;
		if (m_MaxRayIntersections == 0)
		{
			if (ReflectionMethodsCache.Singleton.raycast3DAll == null)
			{
				return;
			}
			m_Hits = ReflectionMethodsCache.Singleton.raycast3DAll(ray, distanceToClipPlane, finalEventMask);
			num = m_Hits.Length;
		}
		else
		{
			if (ReflectionMethodsCache.Singleton.getRaycastNonAlloc == null)
			{
				return;
			}
			if (m_LastMaxRayIntersections != m_MaxRayIntersections)
			{
				m_Hits = new RaycastHit[m_MaxRayIntersections];
				m_LastMaxRayIntersections = m_MaxRayIntersections;
			}
			num = ReflectionMethodsCache.Singleton.getRaycastNonAlloc(ray, m_Hits, distanceToClipPlane, finalEventMask);
		}
		if (num > 1)
		{
			Array.Sort(m_Hits, (RaycastHit r1, RaycastHit r2) => r1.distance.CompareTo(r2.distance));
		}
		if (num != 0)
		{
			int num2 = 0;
			for (int num3 = num; num2 < num3; num2++)
			{
				RaycastResult item = new RaycastResult
				{
					gameObject = m_Hits[num2].collider.gameObject,
					module = this,
					distance = m_Hits[num2].distance,
					worldPosition = m_Hits[num2].point,
					worldNormal = m_Hits[num2].normal,
					screenPosition = eventData.position,
					index = resultAppendList.Count,
					sortingLayer = 0,
					sortingOrder = 0
				};
				resultAppendList.Add(item);
			}
		}
	}
}
