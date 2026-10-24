using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Lifetime;

[ComVisible(true)]
public interface ILease
{
	TimeSpan CurrentLeaseTime { get; }

	LeaseState CurrentState { get; }

	TimeSpan InitialLeaseTime { set; }

	TimeSpan RenewOnCallTime { get; set; }

	TimeSpan SponsorshipTimeout { set; }

	TimeSpan Renew(TimeSpan renewalTime);
}
