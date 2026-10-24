using System.Text;
using Mono.Security;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates;

[System.MonoTODO("Some X500DistinguishedNameFlags options aren't supported, like DoNotUsePlusSign, DoNotUseQuotes and ForceUTF8Encoding")]
public sealed class X500DistinguishedName : AsnEncodedData
{
	private string name;

	public X500DistinguishedName(byte[] encodedDistinguishedName)
	{
		if (encodedDistinguishedName == null)
		{
			throw new ArgumentNullException("encodedDistinguishedName");
		}
		base.Oid = new Oid();
		base.RawData = encodedDistinguishedName;
		if (encodedDistinguishedName.Length > 0)
		{
			DecodeRawData();
		}
		else
		{
			name = string.Empty;
		}
	}

	public string Decode(X500DistinguishedNameFlags flag)
	{
		if (flag != X500DistinguishedNameFlags.None && (flag & (X500DistinguishedNameFlags.Reversed | X500DistinguishedNameFlags.UseSemicolons | X500DistinguishedNameFlags.DoNotUsePlusSign | X500DistinguishedNameFlags.DoNotUseQuotes | X500DistinguishedNameFlags.UseCommas | X500DistinguishedNameFlags.UseNewLines | X500DistinguishedNameFlags.UseUTF8Encoding | X500DistinguishedNameFlags.UseT61Encoding | X500DistinguishedNameFlags.ForceUTF8Encoding)) == 0)
		{
			throw new ArgumentException("flag");
		}
		if (base.RawData.Length == 0)
		{
			return string.Empty;
		}
		bool flag2 = (flag & X500DistinguishedNameFlags.Reversed) != 0;
		bool flag3 = (flag & X500DistinguishedNameFlags.DoNotUseQuotes) == 0;
		string separator = GetSeparator(flag);
		ASN1 aSN = new ASN1(base.RawData);
		return X501.ToString(aSN, flag2, separator, flag3);
	}

	public override string Format(bool multiLine)
	{
		if (multiLine)
		{
			string text = Decode(X500DistinguishedNameFlags.UseNewLines);
			if (text.Length > 0)
			{
				return text + Environment.NewLine;
			}
			return text;
		}
		return Decode(X500DistinguishedNameFlags.UseCommas);
	}

	private static string GetSeparator(X500DistinguishedNameFlags flag)
	{
		if ((flag & X500DistinguishedNameFlags.UseSemicolons) != X500DistinguishedNameFlags.None)
		{
			return "; ";
		}
		if ((flag & X500DistinguishedNameFlags.UseCommas) != X500DistinguishedNameFlags.None)
		{
			return ", ";
		}
		if ((flag & X500DistinguishedNameFlags.UseNewLines) != X500DistinguishedNameFlags.None)
		{
			return Environment.NewLine;
		}
		return ", ";
	}

	private void DecodeRawData()
	{
		if (base.RawData == null || base.RawData.Length < 3)
		{
			name = string.Empty;
			return;
		}
		ASN1 aSN = new ASN1(base.RawData);
		name = X501.ToString(aSN, reversed: true, ", ", quotes: true);
	}

	private static string Canonize(string s)
	{
		int i = s.IndexOf('=');
		StringBuilder stringBuilder = new StringBuilder(s.Substring(0, i + 1));
		while (char.IsWhiteSpace(s, ++i))
		{
		}
		s = s.TrimEnd();
		bool flag = false;
		for (; i < s.Length; i++)
		{
			if (flag)
			{
				flag = char.IsWhiteSpace(s, i);
				if (flag)
				{
					continue;
				}
			}
			if (char.IsWhiteSpace(s, i))
			{
				flag = true;
			}
			stringBuilder.Append(char.ToUpperInvariant(s[i]));
		}
		return stringBuilder.ToString();
	}

	internal static bool AreEqual(X500DistinguishedName name1, X500DistinguishedName name2)
	{
		if (name1 == null)
		{
			return name2 == null;
		}
		if (name2 == null)
		{
			return false;
		}
		X500DistinguishedNameFlags flag = X500DistinguishedNameFlags.DoNotUseQuotes | X500DistinguishedNameFlags.UseNewLines;
		string[] separator = new string[1] { Environment.NewLine };
		string[] array = name1.Decode(flag).Split(separator, StringSplitOptions.RemoveEmptyEntries);
		string[] array2 = name2.Decode(flag).Split(separator, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length != array2.Length)
		{
			return false;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (Canonize(array[i]) != Canonize(array2[i]))
			{
				return false;
			}
		}
		return true;
	}
}
