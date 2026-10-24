using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Threading;

namespace Mono.Interop;

internal class ComInteropProxy : RealProxy, IRemotingTypeInfo
{
	private __ComObject com_object;

	private int ref_count = 1;

	private string type_name;

	public string TypeName
	{
		get
		{
			return type_name;
		}
		set
		{
			type_name = value;
		}
	}

	private ComInteropProxy(IntPtr pUnk)
		: this(pUnk, typeof(__ComObject))
	{
	}

	internal ComInteropProxy(IntPtr pUnk, Type t)
		: base(t)
	{
		com_object = new __ComObject(pUnk);
		CacheProxy();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddProxy(IntPtr pItf, ComInteropProxy proxy);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern ComInteropProxy FindProxy(IntPtr pItf);

	private void CacheProxy()
	{
		AddProxy(com_object.IUnknown, this);
	}

	internal static ComInteropProxy GetProxy(IntPtr pItf, Type t)
	{
		Guid iid = __ComObject.IID_IUnknown;
		int errorCode = Marshal.QueryInterface(pItf, ref iid, out var ppv);
		Marshal.ThrowExceptionForHR(errorCode);
		ComInteropProxy comInteropProxy = FindProxy(ppv);
		if (comInteropProxy == null)
		{
			Marshal.Release(pItf);
			return new ComInteropProxy(ppv);
		}
		Marshal.Release(pItf);
		Interlocked.Increment(ref comInteropProxy.ref_count);
		return comInteropProxy;
	}

	public override IMessage Invoke(IMessage msg)
	{
		Console.WriteLine("Invoke");
		Console.WriteLine(Environment.StackTrace);
		throw new Exception("The method or operation is not implemented.");
	}

	public bool CanCastTo(Type fromType, object o)
	{
		if (!(o is __ComObject _ComObject))
		{
			throw new NotSupportedException("Only RCWs are currently supported");
		}
		if ((fromType.Attributes & TypeAttributes.Import) == 0)
		{
			return false;
		}
		if (_ComObject.GetInterface(fromType, throwException: false) == IntPtr.Zero)
		{
			return false;
		}
		return true;
	}
}
