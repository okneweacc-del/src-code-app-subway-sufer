using System;
using System.Collections;

namespace Mono.Security.X509;

[Serializable]
internal class X509CertificateCollection : CollectionBase, IEnumerable
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

	IEnumerator IEnumerable.GetEnumerator()
	{
		return base.InnerList.GetEnumerator();
	}

	public int Add(X509Certificate value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		return base.InnerList.Add(value);
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
