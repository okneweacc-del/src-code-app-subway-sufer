using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Runtime.Remoting.Messaging;

[Serializable]
[CLSCompliant(false)]
[ComVisible(true)]
public class MethodCall : ISerializable, IInternalMessage, IMessage, IMethodCallMessage, IMethodMessage, ISerializationRootObject
{
	private string _uri;

	private string _typeName;

	private string _methodName;

	private object[] _args;

	private Type[] _methodSignature;

	private MethodBase _methodBase;

	private LogicalCallContext _callContext;

	private Identity _targetIdentity;

	private Type[] _genericArguments;

	protected IDictionary ExternalProperties;

	protected IDictionary InternalProperties;

	string IInternalMessage.Uri
	{
		get
		{
			return Uri;
		}
		set
		{
			Uri = value;
		}
	}

	Identity IInternalMessage.TargetIdentity
	{
		get
		{
			return _targetIdentity;
		}
		set
		{
			_targetIdentity = value;
		}
	}

	public int ArgCount => _args.Length;

	public object[] Args => _args;

	public LogicalCallContext LogicalCallContext
	{
		get
		{
			if (_callContext == null)
			{
				_callContext = new LogicalCallContext();
			}
			return _callContext;
		}
	}

	public MethodBase MethodBase
	{
		get
		{
			if (_methodBase == null)
			{
				ResolveMethod();
			}
			return _methodBase;
		}
	}

	public string MethodName
	{
		get
		{
			if (_methodName == null)
			{
				_methodName = _methodBase.Name;
			}
			return _methodName;
		}
	}

	public object MethodSignature
	{
		get
		{
			if (_methodSignature == null && _methodBase != null)
			{
				ParameterInfo[] parameters = _methodBase.GetParameters();
				_methodSignature = new Type[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					_methodSignature[i] = parameters[i].ParameterType;
				}
			}
			return _methodSignature;
		}
	}

	public virtual IDictionary Properties
	{
		get
		{
			if (ExternalProperties == null)
			{
				InitDictionary();
			}
			return ExternalProperties;
		}
	}

	public string TypeName
	{
		get
		{
			if (_typeName == null)
			{
				_typeName = _methodBase.DeclaringType.AssemblyQualifiedName;
			}
			return _typeName;
		}
	}

	public string Uri
	{
		get
		{
			return _uri;
		}
		set
		{
			_uri = value;
		}
	}

	private Type[] GenericArguments
	{
		get
		{
			if (_genericArguments != null)
			{
				return _genericArguments;
			}
			return _genericArguments = MethodBase.GetGenericArguments();
		}
	}

	public MethodCall(Header[] h1)
	{
		Init();
		if (h1 != null && h1.Length != 0)
		{
			foreach (Header header in h1)
			{
				InitMethodProperty(header.Name, header.Value);
			}
			ResolveMethod();
		}
	}

	internal MethodCall(SerializationInfo info, StreamingContext context)
	{
		Init();
		foreach (SerializationEntry item in info)
		{
			InitMethodProperty(item.Name, item.Value);
		}
	}

	internal MethodCall(CADMethodCallMessage msg)
	{
		_uri = string.Copy(msg.Uri);
		ArrayList arguments = msg.GetArguments();
		_args = msg.GetArgs(arguments);
		_callContext = msg.GetLogicalCallContext(arguments);
		if (_callContext == null)
		{
			_callContext = new LogicalCallContext();
		}
		_methodBase = msg.GetMethod();
		Init();
		if (msg.PropertiesCount > 0)
		{
			CADMessageBase.UnmarshalProperties(Properties, msg.PropertiesCount, arguments);
		}
	}

	internal MethodCall()
	{
	}

	internal virtual void InitMethodProperty(string key, object value)
	{
		switch (key)
		{
		case "__TypeName":
			_typeName = (string)value;
			break;
		case "__MethodName":
			_methodName = (string)value;
			break;
		case "__MethodSignature":
			_methodSignature = (Type[])value;
			break;
		case "__Args":
			_args = (object[])value;
			break;
		case "__CallContext":
			_callContext = (LogicalCallContext)value;
			break;
		case "__Uri":
			_uri = (string)value;
			break;
		case "__GenericArguments":
			_genericArguments = (Type[])value;
			break;
		default:
			Properties[key] = value;
			break;
		}
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("__TypeName", _typeName);
		info.AddValue("__MethodName", _methodName);
		info.AddValue("__MethodSignature", _methodSignature);
		info.AddValue("__Args", _args);
		info.AddValue("__CallContext", _callContext);
		info.AddValue("__Uri", _uri);
		info.AddValue("__GenericArguments", _genericArguments);
		if (InternalProperties == null)
		{
			return;
		}
		foreach (DictionaryEntry internalProperty in InternalProperties)
		{
			info.AddValue((string)internalProperty.Key, internalProperty.Value);
		}
	}

	internal virtual void InitDictionary()
	{
		InternalProperties = ((MethodDictionary)(ExternalProperties = new MethodCallDictionary(this))).GetInternalProperties();
	}

	public object GetArg(int argNum)
	{
		return _args[argNum];
	}

	public virtual void Init()
	{
	}

	public void ResolveMethod()
	{
		if (_uri != null)
		{
			Type serverTypeForUri = RemotingServices.GetServerTypeForUri(_uri);
			if (serverTypeForUri == null)
			{
				string text = ((_typeName == null) ? string.Empty : (" (" + _typeName + ")"));
				throw new RemotingException("Requested service not found" + text + ". No receiver for uri " + _uri);
			}
			Type type = CastTo(_typeName, serverTypeForUri);
			if (type == null)
			{
				throw new RemotingException("Cannot cast from client type '" + _typeName + "' to server type '" + serverTypeForUri.FullName + "'");
			}
			_methodBase = RemotingServices.GetMethodBaseFromName(type, _methodName, _methodSignature);
			if (_methodBase == null)
			{
				throw new RemotingException("Method " + _methodName + " not found in " + type);
			}
			if (type != serverTypeForUri && type.IsInterface && !serverTypeForUri.IsInterface)
			{
				_methodBase = RemotingServices.GetVirtualMethod(serverTypeForUri, _methodBase);
				if (_methodBase == null)
				{
					throw new RemotingException("Method " + _methodName + " not found in " + serverTypeForUri);
				}
			}
		}
		else
		{
			_methodBase = RemotingServices.GetMethodBaseFromMethodMessage(this);
			if (_methodBase == null)
			{
				throw new RemotingException("Method " + _methodName + " not found in " + TypeName);
			}
		}
		if (_methodBase.IsGenericMethod && _methodBase.ContainsGenericParameters)
		{
			if (GenericArguments == null)
			{
				throw new RemotingException("The remoting infrastructure does not support open generic methods.");
			}
			_methodBase = ((MethodInfo)_methodBase).MakeGenericMethod(GenericArguments);
		}
	}

	private Type CastTo(string clientType, Type serverType)
	{
		clientType = GetTypeNameFromAssemblyQualifiedName(clientType);
		if (clientType == serverType.FullName)
		{
			return serverType;
		}
		for (Type baseType = serverType.BaseType; baseType != null; baseType = baseType.BaseType)
		{
			if (clientType == baseType.FullName)
			{
				return baseType;
			}
		}
		Type[] interfaces = serverType.GetInterfaces();
		Type[] array = interfaces;
		foreach (Type type in array)
		{
			if (clientType == type.FullName)
			{
				return type;
			}
		}
		return null;
	}

	private static string GetTypeNameFromAssemblyQualifiedName(string aqname)
	{
		int num = aqname.IndexOf("]]");
		int num2 = aqname.IndexOf(',', (num != -1) ? (num + 2) : 0);
		if (num2 != -1)
		{
			aqname = aqname.Substring(0, num2).Trim();
		}
		return aqname;
	}
}
