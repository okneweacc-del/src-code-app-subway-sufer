using System;
using UnityEngine;

namespace Kiloo.Common.UI;

public class Container
{
	public Action onRecycleContainer;

	public Action onPositionContainer;

	public long rowId;

	public GameObject model;

	public Vector2 coordinates = Vector2.zero;

	private readonly Transform _modelTransform;

	public Bounds bounds;

	public int dataIndex;

	public int modelType;

	public float height;

	public float width;

	public bool isMeasured;

	private bool _parentSet;

	private bool _scaleSet;

	public object tag;

	public float top;

	public float bottom;

	public float right;

	public float left;

	public Transform modelTransform => _modelTransform;

	public bool hasTransientState { get; set; }

	public Container(GameObject model)
	{
		this.model = UnityEngine.Object.Instantiate(model);
		GameObject gameObject = this.model;
		if (gameObject != null)
		{
			_modelTransform = gameObject.transform;
		}
	}

	public virtual void Measure(bool includeInactive = true)
	{
		if (!isMeasured)
		{
			bounds = NGUIMath.CalculateRelativeWidgetBounds(model.transform, includeInactive);
			width = bounds.size.x;
			height = bounds.size.y;
			isMeasured = true;
		}
	}

	public void SetPosition()
	{
		_modelTransform.localPosition = coordinates;
		if (onPositionContainer != null)
		{
			onPositionContainer();
		}
	}

	public void SetParent(Transform parent)
	{
		if (!_parentSet)
		{
			model.transform.parent = parent;
			model.transform.localRotation = Quaternion.identity;
			_parentSet = true;
		}
	}

	public void SetScale(Vector3 scale)
	{
		if (!_scaleSet)
		{
			_scaleSet = true;
			model.transform.localScale = scale;
		}
	}

	public void SetModelType(int mType)
	{
		modelType = mType;
	}

	public override string ToString()
	{
		return string.Format(string.Concat("[Container - #Top: ", top, " #Bottom: ", bottom, "Left: ", left, " Right:", right, " #height: ", height, " #width:", width, " #dataIndex: ", dataIndex, "# modelType:", modelType, "# Bounds:", bounds, "# 2DCoordinates:", coordinates));
	}
}
