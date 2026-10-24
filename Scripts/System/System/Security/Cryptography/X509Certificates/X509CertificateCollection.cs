using System.Collections;

namespace System.Security.Cryptography.X509Certificates;

[Serializable]
public class X509CertificateCollection : CollectionBase
{
	public class X509CertificateEnumerator : IEnumerator
	{
		private IEnumerator enumerator;

		object IEnumerator.Current => enumerator.Current;

		public X509Certificate Current => (X509Certificate)enumerator.Current;

		public X509CertificateEnumerator(X509CertificateCollection mappings)
		{
			enumerator = ((IEnumerable)mappings).GetEnumerator();
		}

		bool IEnumerator.MoveNext()
		{
			return enumerator.MoveNext();
		}

		void IEnumerator.Reset()
		{
			enumerator.Reset();
		}

		public bool MoveNext()
		{
			return enumerator.MoveNext();
		}

		public void Reset()
		{
			enumerator.Reset();
		}
	}

	public X509Certificate this[int index] => (X509Certificate)base.InnerList[index];

	public X509CertificateCollection()
	{
	}

	public X509CertificateCollection(X509Certificate[] value)
	{
		AddRange(value);
	}

	public void AddRange(X509Certificate[] value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		for (int i = 0; i < value.Length; i++)
		{
			base.InnerList.Add(value[i]);
		}
	}

	public new X509CertificateEnumerator GetEnumerator()
	{
		return new X509CertificateEnumerator(this);
	}

	public override int GetHashCode()
	{
		return base.InnerList.GetHashCode();
	}
}
