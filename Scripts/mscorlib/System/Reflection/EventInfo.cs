using System.Runtime.InteropServices;

namespace System.Reflection;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(_EventInfo))]
public abstract class EventInfo : MemberInfo, _EventInfo
{
	private delegate void AddEventAdapter(object _this, Delegate dele);

	private AddEventAdapter cached_add_event;

	public abstract EventAttributes Attributes { get; }

	public Type EventHandlerType
	{
		get
		{
			MethodInfo addMethod = GetAddMethod(nonPublic: true);
			ParameterInfo[] parameters = addMethod.GetParameters();
			if (parameters.Length > 0)
			{
				return parameters[0].ParameterType;
			}
			return null;
		}
	}

	public override MemberTypes MemberType => MemberTypes.Event;

	public abstract MethodInfo GetAddMethod(bool nonPublic);

	public abstract MethodInfo GetRemoveMethod(bool nonPublic);
}
