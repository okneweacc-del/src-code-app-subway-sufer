using System.Runtime.InteropServices;

namespace System.Security.Permissions;

[Serializable]
[Obsolete("CAS support is not available with Silverlight applications.")]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
[ComVisible(true)]
public abstract class SecurityAttribute : Attribute
{
}
