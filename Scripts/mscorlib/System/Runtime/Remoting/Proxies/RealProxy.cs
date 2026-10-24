using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;

namespace System.Runtime.Remoting.Proxies;

[ComVisible(true)]
public abstract class RealProxy
{
	private Type class_to_proxy;

	internal Context _targetContext;

	private MarshalByRefObject _server;

	private int _targetDomainId = -1;

	internal string _targetUri;

	internal Identity _objectIdentity;

	private object _objTP;

	private object _stubData;

	internal Identity ObjectIdentity
	{
		get
		{
			return _objectIdentity;
		}
		set
		{
			_objectIdentity = value;
		}
	}

	protected RealProxy(Type classToProxy)
		: this(classToProxy, IntPtr.Zero, null)
	{
	}

	internal RealProxy(Type classToProxy, ClientIdentity identity)
		: this(classToProxy, IntPtr.Zero, null)
	{
		_objectIdentity = identity;
	}

	protected RealProxy(Type classToProxy, IntPtr stub, object stubData)
	{
		if (!classToProxy.IsMarshalByRef && !classToProxy.IsInterface)
		{
			throw new ArgumentException("object must be MarshalByRef");
		}
		class_to_proxy = classToProxy;
		if (stub != IntPtr.Zero)
		{
			throw new NotSupportedException("stub is not used in Mono");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Type InternalGetProxyType(object transparentProxy);

	public Type GetProxiedType()
	{
		if (_objTP == null)
		{
			if (class_to_proxy.IsInterface)
			{
				return typeof(MarshalByRefObject);
			}
			return class_to_proxy;
		}
		return InternalGetProxyType(_objTP);
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		object transparentProxy = GetTransparentProxy();
		RemotingServices.GetObjectData(transparentProxy, info, context);
	}

	public abstract IMessage Invoke(IMessage msg);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal virtual extern object InternalGetTransparentProxy(string className);

	public virtual object GetTransparentProxy()
	{
		if (_objTP == null)
		{
			string text;
			if (this is IRemotingTypeInfo remotingTypeInfo)
			{
				text = remotingTypeInfo.TypeName;
				if (text == null || text == typeof(MarshalByRefObject).AssemblyQualifiedName)
				{
					text = class_to_proxy.AssemblyQualifiedName;
				}
			}
			else
			{
				text = class_to_proxy.AssemblyQualifiedName;
			}
			_objTP = InternalGetTransparentProxy(text);
		}
		return _objTP;
	}

	protected void AttachServer(MarshalByRefObject s)
	{
		_server = s;
	}

	internal void SetTargetDomain(int domainId)
	{
		_targetDomainId = domainId;
	}
}
