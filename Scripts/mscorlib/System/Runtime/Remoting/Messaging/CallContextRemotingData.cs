namespace System.Runtime.Remoting.Messaging;

[Serializable]
internal class CallContextRemotingData : ICloneable
{
	private string _logicalCallID;

	public object Clone()
	{
		CallContextRemotingData callContextRemotingData = new CallContextRemotingData();
		callContextRemotingData._logicalCallID = _logicalCallID;
		return callContextRemotingData;
	}
}
