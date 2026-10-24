using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;

namespace System;

[Serializable]
[ComVisible(true)]
public abstract class MarshalByRefObject
{
	[NonSerialized]
	private ServerIdentity _identity;

	internal ServerIdentity ObjectIdentity
	{
		get
		{
			return _identity;
		}
		set
		{
			_identity = value;
		}
	}

	public virtual ObjRef CreateObjRef(Type requestedType)
	{
		if (_identity == null)
		{
			throw new RemotingException(Locale.GetText("No remoting information was found for the object."));
		}
		return _identity.CreateObjRef(requestedType);
	}

	public object GetLifetimeService()
	{
		if (_identity == null)
		{
			return null;
		}
		return _identity.Lease;
	}

	public virtual object InitializeLifetimeService()
	{
		if (_identity != null && _identity.Lease != null)
		{
			return _identity.Lease;
		}
		return new Lease();
	}
}
