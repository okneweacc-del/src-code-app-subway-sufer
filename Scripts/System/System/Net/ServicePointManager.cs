using System.Collections;
using System.Collections.Specialized;
using System.Net.Security;

namespace System.Net;

public class ServicePointManager
{
	private class SPKey
	{
		private Uri uri;

		private bool use_connect;

		public SPKey(Uri uri, bool use_connect)
		{
			this.uri = uri;
			this.use_connect = use_connect;
		}

		public override int GetHashCode()
		{
			return uri.GetHashCode() + (use_connect ? 1 : 0);
		}

		public override bool Equals(object obj)
		{
			SPKey sPKey = obj as SPKey;
			if (obj == null)
			{
				return false;
			}
			return uri.Equals(sPKey.uri) && sPKey.use_connect == use_connect;
		}
	}

	private static HybridDictionary servicePoints;

	private static ICertificatePolicy policy;

	private static int defaultConnectionLimit;

	private static int maxServicePointIdleTime;

	private static int maxServicePoints;

	private static bool _checkCRL;

	private static SecurityProtocolType _securityProtocol;

	private static bool expectContinue;

	private static bool useNagle;

	private static RemoteCertificateValidationCallback server_cert_cb;

	[Obsolete("Use ServerCertificateValidationCallback instead", false)]
	public static ICertificatePolicy CertificatePolicy => policy;

	[System.MonoTODO("CRL checks not implemented")]
	public static bool CheckCertificateRevocationList => _checkCRL;

	public static SecurityProtocolType SecurityProtocol => _securityProtocol;

	public static RemoteCertificateValidationCallback ServerCertificateValidationCallback => server_cert_cb;

	static ServicePointManager()
	{
		servicePoints = new HybridDictionary();
		policy = new System.Net.DefaultCertificatePolicy();
		defaultConnectionLimit = 2;
		maxServicePointIdleTime = 900000;
		maxServicePoints = 0;
		_checkCRL = false;
		_securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
		expectContinue = true;
	}

	public static ServicePoint FindServicePoint(Uri address, IWebProxy proxy)
	{
		if (address == null)
		{
			throw new ArgumentNullException("address");
		}
		RecycleServicePoints();
		bool usesProxy = false;
		bool flag = false;
		if (proxy != null && !proxy.IsBypassed(address))
		{
			usesProxy = true;
			bool flag2 = address.Scheme == "https";
			address = proxy.GetProxy(address);
			if (address.Scheme != "http" && !flag2)
			{
				throw new NotSupportedException("Proxy scheme not supported.");
			}
			if (flag2 && address.Scheme == "http")
			{
				flag = true;
			}
		}
		address = new Uri(address.Scheme + "://" + address.Authority);
		ServicePoint servicePoint = null;
		lock (servicePoints)
		{
			SPKey key = new SPKey(address, flag);
			if (servicePoints[key] is ServicePoint result)
			{
				return result;
			}
			if (maxServicePoints > 0 && servicePoints.Count >= maxServicePoints)
			{
				throw new InvalidOperationException("maximum number of service points reached");
			}
			string text = address.ToString();
			int connectionLimit = defaultConnectionLimit;
			servicePoint = new ServicePoint(address, connectionLimit, maxServicePointIdleTime);
			servicePoint.Expect100Continue = expectContinue;
			servicePoint.UseNagleAlgorithm = useNagle;
			servicePoint.UsesProxy = usesProxy;
			servicePoint.UseConnect = flag;
			servicePoints.Add(key, servicePoint);
			return servicePoint;
		}
	}

	internal static void RecycleServicePoints()
	{
		ArrayList arrayList = new ArrayList();
		lock (servicePoints)
		{
			IDictionaryEnumerator enumerator = servicePoints.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ServicePoint servicePoint = (ServicePoint)enumerator.Value;
				if (servicePoint.AvailableForRecycling)
				{
					arrayList.Add(enumerator.Key);
				}
			}
			for (int i = 0; i < arrayList.Count; i++)
			{
				servicePoints.Remove(arrayList[i]);
			}
			if (maxServicePoints == 0 || servicePoints.Count <= maxServicePoints)
			{
				return;
			}
			SortedList sortedList = new SortedList(servicePoints.Count);
			enumerator = servicePoints.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ServicePoint servicePoint2 = (ServicePoint)enumerator.Value;
				if (servicePoint2.CurrentConnections == 0)
				{
					while (sortedList.ContainsKey(servicePoint2.IdleSince))
					{
						servicePoint2.IdleSince = servicePoint2.IdleSince.AddMilliseconds(1.0);
					}
					sortedList.Add(servicePoint2.IdleSince, servicePoint2.Address);
				}
			}
			for (int j = 0; j < sortedList.Count; j++)
			{
				if (servicePoints.Count <= maxServicePoints)
				{
					break;
				}
				servicePoints.Remove(sortedList.GetByIndex(j));
			}
		}
	}
}
