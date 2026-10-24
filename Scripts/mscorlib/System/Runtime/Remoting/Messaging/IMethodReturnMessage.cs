using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging;

[ComVisible(true)]
public interface IMethodReturnMessage : IMessage, IMethodMessage
{
	Exception Exception { get; }

	int OutArgCount { get; }

	object[] OutArgs { get; }

	object ReturnValue { get; }
}
