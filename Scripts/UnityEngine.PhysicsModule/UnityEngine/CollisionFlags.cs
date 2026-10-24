namespace UnityEngine;

public enum CollisionFlags
{
	None = 0,
	Sides = 1,
	Above = 2,
	Below = 4,
	CollidedSides = Sides,
	CollidedAbove = Above,
	CollidedBelow = Below
}
