using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace System.Runtime.Remoting.Channels;

[Serializable]
internal class CrossAppDomainChannel : IChannel, IChannelReceiver, IChannelSender
{
	private static object s_lock = new object();

	public virtual string ChannelName => "MONOCAD";

	public virtual int ChannelPriority => 100;

	public virtual object ChannelData => new CrossAppDomainData(Thread.GetDomainID());

	internal static void RegisterCrossAppDomainChannel()
	{
		lock (s_lock)
		{
			CrossAppDomainChannel chnl = new CrossAppDomainChannel();
			ChannelServices.RegisterChannel(chnl);
		}
	}

	public virtual void StartListening(object data)
	{
	}

	public virtual IMessageSink CreateMessageSink(string url, object data, out string uri)
	{
		uri = null;
		if (data != null && data is CrossAppDomainData crossAppDomainData && crossAppDomainData.ProcessID == RemotingConfiguration.ProcessId)
		{
			return CrossAppDomainSink.GetSink(crossAppDomainData.DomainID);
		}
		if (url != null && url.StartsWith("MONOCAD"))
		{
			throw new NotSupportedException("Can't create a named channel via crossappdomain");
		}
		return null;
	}
}
