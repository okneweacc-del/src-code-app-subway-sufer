namespace System.Security.Cryptography;

public sealed class Oid
{
	internal const string oidRSA = "1.2.840.113549.1.1.1";

	internal const string nameRSA = "RSA";

	internal const string oidPkcs7Data = "1.2.840.113549.1.7.1";

	internal const string namePkcs7Data = "PKCS 7 Data";

	internal const string oidPkcs9ContentType = "1.2.840.113549.1.9.3";

	internal const string namePkcs9ContentType = "Content Type";

	internal const string oidPkcs9MessageDigest = "1.2.840.113549.1.9.4";

	internal const string namePkcs9MessageDigest = "Message Digest";

	internal const string oidPkcs9SigningTime = "1.2.840.113549.1.9.5";

	internal const string namePkcs9SigningTime = "Signing Time";

	internal const string oidMd5 = "1.2.840.113549.2.5";

	internal const string nameMd5 = "md5";

	internal const string oid3Des = "1.2.840.113549.3.7";

	internal const string name3Des = "3des";

	internal const string oidSha1 = "1.3.14.3.2.26";

	internal const string nameSha1 = "sha1";

	internal const string oidSubjectAltName = "2.5.29.17";

	internal const string nameSubjectAltName = "Subject Alternative Name";

	internal const string oidNetscapeCertType = "2.16.840.1.113730.1.1";

	internal const string nameNetscapeCertType = "Netscape Cert Type";

	private string _value;

	private string _name;

	public string FriendlyName
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
			_value = GetValue(_name);
		}
	}

	public string Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
			_name = GetName(_value);
		}
	}

	public Oid()
	{
	}

	public Oid(string oid)
	{
		if (oid == null)
		{
			throw new ArgumentNullException("oid");
		}
		_value = oid;
		_name = GetName(oid);
	}

	public Oid(string value, string friendlyName)
	{
		_value = value;
		_name = friendlyName;
	}

	public Oid(Oid oid)
	{
		if (oid == null)
		{
			throw new ArgumentNullException("oid");
		}
		_value = oid.Value;
		_name = oid.FriendlyName;
	}

	private string GetName(string oid)
	{
		return oid switch
		{
			"1.2.840.113549.1.1.1" => "RSA", 
			"1.2.840.113549.1.7.1" => "PKCS 7 Data", 
			"1.2.840.113549.1.9.3" => "Content Type", 
			"1.2.840.113549.1.9.4" => "Message Digest", 
			"1.2.840.113549.1.9.5" => "Signing Time", 
			"1.2.840.113549.3.7" => "3des", 
			"2.5.29.19" => "Basic Constraints", 
			"2.5.29.15" => "Key Usage", 
			"2.5.29.37" => "Enhanced Key Usage", 
			"2.5.29.14" => "Subject Key Identifier", 
			"2.5.29.17" => "Subject Alternative Name", 
			"2.16.840.1.113730.1.1" => "Netscape Cert Type", 
			"1.2.840.113549.2.5" => "md5", 
			"1.3.14.3.2.26" => "sha1", 
			_ => _name, 
		};
	}

	private string GetValue(string name)
	{
		return name switch
		{
			"RSA" => "1.2.840.113549.1.1.1", 
			"PKCS 7 Data" => "1.2.840.113549.1.7.1", 
			"Content Type" => "1.2.840.113549.1.9.3", 
			"Message Digest" => "1.2.840.113549.1.9.4", 
			"Signing Time" => "1.2.840.113549.1.9.5", 
			"3des" => "1.2.840.113549.3.7", 
			"Basic Constraints" => "2.5.29.19", 
			"Key Usage" => "2.5.29.15", 
			"Enhanced Key Usage" => "2.5.29.37", 
			"Subject Key Identifier" => "2.5.29.14", 
			"Subject Alternative Name" => "2.5.29.17", 
			"Netscape Cert Type" => "2.16.840.1.113730.1.1", 
			"md5" => "1.2.840.113549.2.5", 
			"sha1" => "1.3.14.3.2.26", 
			_ => _value, 
		};
	}
}
