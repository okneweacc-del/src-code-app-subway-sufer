using System.Runtime.InteropServices;

namespace System;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
public sealed class SerializableAttribute : Attribute
{
}
