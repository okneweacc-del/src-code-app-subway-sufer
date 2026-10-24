namespace System.Runtime.Remoting.Messaging;

internal class CADObjRef
{
	private ObjRef objref;

	public int SourceDomain;

	public string TypeName => objref.TypeInfo.TypeName;

	public string URI => objref.URI;

	public CADObjRef(ObjRef o, int sourceDomain)
	{
		objref = o;
		SourceDomain = sourceDomain;
	}
}
