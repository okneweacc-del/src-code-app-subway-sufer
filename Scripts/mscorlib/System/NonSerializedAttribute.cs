using System.Runtime.InteropServices;

namespace System;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class NonSerializedAttribute : Attribute
{
}
