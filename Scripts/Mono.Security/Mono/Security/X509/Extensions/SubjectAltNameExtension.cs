using System;

namespace Mono.Security.X509.Extensions;

public class SubjectAltNameExtension : X509Extension
{
	private GeneralNames _names;

	public string[] DNSNames => _names.DNSNames;

	public string[] IPAddresses => _names.IPAddresses;

	public SubjectAltNameExtension(X509Extension extension)
		: base(extension)
	{
	}

	protected override void Decode()
	{
		ASN1 aSN = new ASN1(extnValue.Value);
		if (aSN.Tag != 48)
		{
			throw new ArgumentException("Invalid SubjectAltName extension");
		}
		_names = new GeneralNames(aSN);
	}

	public override string ToString()
	{
		return _names.ToString();
	}
}
