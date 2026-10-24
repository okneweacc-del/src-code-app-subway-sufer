using System.Security.Permissions;

namespace System.Reflection.Emit;

internal struct RefEmitPermissionSet
{
	public SecurityAction action;

	public string pset;
}
