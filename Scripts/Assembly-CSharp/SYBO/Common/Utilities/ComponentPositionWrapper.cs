using UnityEngine;

namespace SYBO.Common.Utilities;

public struct ComponentPositionWrapper<T>(T component) where T : Component
{
	public T Component = component;

	public Vector3 Position = component.gameObject.transform.position;
}
