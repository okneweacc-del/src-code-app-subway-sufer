using System;
using System.Collections;
using System.Reflection;

namespace Mono.Security.X509;

[DefaultMember("Item")]
internal sealed class X509ExtensionCollection : CollectionBase, IEnumerable
{
	private bool readOnly;

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
}
