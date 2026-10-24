namespace System.Security.Cryptography.X509Certificates;

public sealed class X509ChainPolicy
{
	private OidCollection apps;

	private OidCollection cert;

	private X509Certificate2Collection store;

	private X509RevocationFlag rflag;

	private X509RevocationMode mode;

	private TimeSpan timeout;

	private X509VerificationFlags vflags;

	private DateTime vtime;

	public X509Certificate2Collection ExtraStore => store;

	public X509RevocationFlag RevocationFlag => rflag;

	public X509RevocationMode RevocationMode
	{
		get
		{
			return mode;
		}
		set
		{
			if (value < X509RevocationMode.NoCheck || value > X509RevocationMode.Offline)
			{
				throw new ArgumentException("RevocationMode");
			}
			mode = value;
		}
	}

	public X509VerificationFlags VerificationFlags => vflags;

	public DateTime VerificationTime => vtime;

	public X509ChainPolicy()
	{
		Reset();
	}

	public void Reset()
	{
		apps = new OidCollection();
		cert = new OidCollection();
		store = new X509Certificate2Collection();
		rflag = X509RevocationFlag.ExcludeRoot;
		mode = X509RevocationMode.Online;
		timeout = TimeSpan.Zero;
		vflags = X509VerificationFlags.NoFlag;
		vtime = DateTime.Now;
	}
}
