using System;
using System.Collections;

namespace Mono.Security.X509;

public sealed class X509ExtensionCollection : CollectionBase, IEnumerable
{
	private bool readOnly;

	public X509Extension this[string oid]
	{
		get
		{
			int num = IndexOf(oid);
			if (num == -1)
			{
				return null;
			}
			return (X509Extension)base.InnerList[num];
		}
	}

	public X509ExtensionCollection()
	{
	}

	public X509ExtensionCollection(ASN1 asn1)
		: this()
	{
		readOnly = true;
		if (asn1 != null)
		{
			if (asn1.Tag != 48)
			{
				throw new Exception("Invalid extensions format");
			}
			for (int i = 0; i < asn1.Count; i++)
			{
				X509Extension value = new X509Extension(asn1[i]);
				base.InnerList.Add(value);
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return base.InnerList.GetEnumerator();
	}

	public int IndexOf(string oid)
	{
		if (oid == null)
		{
			throw new ArgumentNullException("oid");
		}
		for (int i = 0; i < base.InnerList.Count; i++)
		{
			X509Extension x509Extension = (X509Extension)base.InnerList[i];
			if (x509Extension.Oid == oid)
			{
				return i;
			}
		}
		return -1;
	}
}
