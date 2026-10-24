using System;
using System.Collections;

namespace Mono.Security.X509;

[Serializable]
public class X509CertificateCollection : CollectionBase, IEnumerable
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

	public X509CertificateCollection(X509CertificateCollection value)
	{
		AddRange(value);
	}

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

	public void AddRange(X509CertificateCollection value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		for (int i = 0; i < value.InnerList.Count; i++)
		{
			base.InnerList.Add(value[i]);
		}
	}

	public bool Contains(X509Certificate value)
	{
		return IndexOf(value) != -1;
	}

	public new X509CertificateEnumerator GetEnumerator()
	{
		return new X509CertificateEnumerator(this);
	}

	public override int GetHashCode()
	{
		return base.InnerList.GetHashCode();
	}

	public int IndexOf(X509Certificate value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		byte[] hash = value.Hash;
		for (int i = 0; i < base.InnerList.Count; i++)
		{
			X509Certificate x509Certificate = (X509Certificate)base.InnerList[i];
			if (Compare(x509Certificate.Hash, hash))
			{
				return i;
			}
		}
		return -1;
	}

	public void Remove(X509Certificate value)
	{
		base.InnerList.Remove(value);
	}

	private bool Compare(byte[] array1, byte[] array2)
	{
		if (array1 == null && array2 == null)
		{
			return true;
		}
		if (array1 == null || array2 == null)
		{
			return false;
		}
		if (array1.Length != array2.Length)
		{
			return false;
		}
		for (int i = 0; i < array1.Length; i++)
		{
			if (array1[i] != array2[i])
			{
				return false;
			}
		}
		return true;
	}
}
