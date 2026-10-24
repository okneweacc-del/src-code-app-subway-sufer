using System;

namespace UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class CreateAssetMenuAttribute : Attribute
{
	public string menuName { set; }
}
