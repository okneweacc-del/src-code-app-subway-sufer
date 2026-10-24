namespace System.Net;

public class ServicePoint
{
	private Uri uri;

	private int connectionLimit;

	private int maxIdleTime;

	private int currentConnections;

	private DateTime idleSince;

	private bool usesProxy;

	private bool sendContinue = true;

	private bool useConnect;

	private object locker = new object();

	private object hostE = new object();

	private bool useNagle;

	public Uri Address => uri;

	public int CurrentConnections => currentConnections;

	public DateTime IdleSince
	{
		get
		{
			return idleSince;
		}
		internal set
		{
			lock (locker)
			{
				idleSince = value;
			}
		}
	}

	public bool Expect100Continue
	{
		set
		{
			SendContinue = value;
		}
	}

	public bool UseNagleAlgorithm
	{
		set
		{
			useNagle = value;
		}
	}

	internal bool SendContinue
	{
		set
		{
			sendContinue = value;
		}
	}

	internal bool UsesProxy
	{
		set
		{
			usesProxy = value;
		}
	}

	internal bool UseConnect
	{
		set
		{
			useConnect = value;
		}
	}

	internal bool AvailableForRecycling => CurrentConnections == 0 && maxIdleTime != -1 && DateTime.Now >= IdleSince.AddMilliseconds(maxIdleTime);

	internal ServicePoint(Uri uri, int connectionLimit, int maxIdleTime)
	{
		this.uri = uri;
		this.connectionLimit = connectionLimit;
		this.maxIdleTime = maxIdleTime;
		currentConnections = 0;
		idleSince = DateTime.Now;
	}
}
