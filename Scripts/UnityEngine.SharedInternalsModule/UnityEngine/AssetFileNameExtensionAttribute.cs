using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Bindings;

namespace UnityEngine;

[VisibleToOtherModules]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class AssetFileNameExtensionAttribute : Attribute
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly string preferredExtension;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly IEnumerable<string> otherExtensions;

	public AssetFileNameExtensionAttribute(string preferredExtension, params string[] otherExtensions)
	{
		this.preferredExtension = preferredExtension;
		this.otherExtensions = otherExtensions;
	}
}
