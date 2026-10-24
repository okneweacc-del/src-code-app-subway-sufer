using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;

namespace System.Runtime.Remoting.Messaging;

internal class ClientContextTerminatorSink : IMessageSink
{
	private Context _context;

	public ClientContextTerminatorSink(Context ctx)
	{
		_context = ctx;
	}

	public IMessage SyncProcessMessage(IMessage msg)
	{
		IMessage message = null;
		Context.NotifyGlobalDynamicSinks(start: true, msg, client_site: true, async: false);
		_context.NotifyDynamicSinks(start: true, msg, client_site: true, async: false);
		if (msg is IConstructionCallMessage)
		{
			message = ActivationServices.RemoteActivate((IConstructionCallMessage)msg);
		}
		else
		{
			Identity messageTargetIdentity = RemotingServices.GetMessageTargetIdentity(msg);
			message = messageTargetIdentity.ChannelSink.SyncProcessMessage(msg);
		}
		Context.NotifyGlobalDynamicSinks(start: false, msg, client_site: true, async: false);
		_context.NotifyDynamicSinks(start: false, msg, client_site: true, async: false);
		return message;
	}

	public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
	{
		if (_context.HasDynamicSinks || Context.HasGlobalDynamicSinks)
		{
			Context.NotifyGlobalDynamicSinks(start: true, msg, client_site: true, async: true);
			_context.NotifyDynamicSinks(start: true, msg, client_site: true, async: true);
			if (replySink != null)
			{
				replySink = new ClientContextReplySink(_context, replySink);
			}
		}
		Identity messageTargetIdentity = RemotingServices.GetMessageTargetIdentity(msg);
		IMessageCtrl result = messageTargetIdentity.ChannelSink.AsyncProcessMessage(msg, replySink);
		if (replySink == null && (_context.HasDynamicSinks || Context.HasGlobalDynamicSinks))
		{
			Context.NotifyGlobalDynamicSinks(start: false, msg, client_site: true, async: true);
			_context.NotifyDynamicSinks(start: false, msg, client_site: true, async: true);
		}
		return result;
	}
}
