using System.Text;

namespace Mono.Security.X509;

public sealed class X501
{
	private static byte[] countryName = new byte[3] { 85, 4, 6 };

	private static byte[] organizationName = new byte[3] { 85, 4, 10 };

	private static byte[] organizationalUnitName = new byte[3] { 85, 4, 11 };

	private static byte[] commonName = new byte[3] { 85, 4, 3 };

	private static byte[] localityName = new byte[3] { 85, 4, 7 };

	private static byte[] stateOrProvinceName = new byte[3] { 85, 4, 8 };

	private static byte[] streetAddress = new byte[3] { 85, 4, 9 };

	private static byte[] domainComponent = new byte[10] { 9, 146, 38, 137, 147, 242, 44, 100, 1, 25 };

	private static byte[] userid = new byte[10] { 9, 146, 38, 137, 147, 242, 44, 100, 1, 1 };

	private static byte[] email = new byte[9] { 42, 134, 72, 134, 247, 13, 1, 9, 1 };

	private static byte[] dnQualifier = new byte[3] { 85, 4, 46 };

	private static byte[] title = new byte[3] { 85, 4, 12 };

	private static byte[] surname = new byte[3] { 85, 4, 4 };

	private static byte[] givenName = new byte[3] { 85, 4, 42 };

	private static byte[] initial = new byte[3] { 85, 4, 43 };

	public static string ToString(ASN1 seq)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < seq.Count; i++)
		{
			ASN1 entry = seq[i];
			AppendEntry(stringBuilder, entry, quotes: true);
			if (i < seq.Count - 1)
			{
				stringBuilder.Append(", ");
			}
		}
		return stringBuilder.ToString();
	}

	public static string ToString(ASN1 seq, bool reversed, string separator, bool quotes)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (reversed)
		{
			for (int num = seq.Count - 1; num >= 0; num--)
			{
				ASN1 entry = seq[num];
				AppendEntry(stringBuilder, entry, quotes);
				if (num > 0)
				{
					stringBuilder.Append(separator);
				}
			}
		}
		else
		{
			for (int i = 0; i < seq.Count; i++)
			{
				ASN1 entry2 = seq[i];
				AppendEntry(stringBuilder, entry2, quotes);
				if (i < seq.Count - 1)
				{
					stringBuilder.Append(separator);
				}
			}
		}
		return stringBuilder.ToString();
	}

	private static void AppendEntry(StringBuilder sb, ASN1 entry, bool quotes)
	{
		for (int i = 0; i < entry.Count; i++)
		{
			ASN1 aSN = entry[i];
			ASN1 aSN2 = aSN[1];
			if (aSN2 == null)
			{
				continue;
			}
			ASN1 aSN3 = aSN[0];
			if (aSN3 == null)
			{
				continue;
			}
			if (aSN3.CompareValue(countryName))
			{
				sb.Append("C=");
			}
			else if (aSN3.CompareValue(organizationName))
			{
				sb.Append("O=");
			}
			else if (aSN3.CompareValue(organizationalUnitName))
			{
				sb.Append("OU=");
			}
			else if (aSN3.CompareValue(commonName))
			{
				sb.Append("CN=");
			}
			else if (aSN3.CompareValue(localityName))
			{
				sb.Append("L=");
			}
			else if (aSN3.CompareValue(stateOrProvinceName))
			{
				sb.Append("S=");
			}
			else if (aSN3.CompareValue(streetAddress))
			{
				sb.Append("STREET=");
			}
			else if (aSN3.CompareValue(domainComponent))
			{
				sb.Append("DC=");
			}
			else if (aSN3.CompareValue(userid))
			{
				sb.Append("UID=");
			}
			else if (aSN3.CompareValue(email))
			{
				sb.Append("E=");
			}
			else if (aSN3.CompareValue(dnQualifier))
			{
				sb.Append("dnQualifier=");
			}
			else if (aSN3.CompareValue(title))
			{
				sb.Append("T=");
			}
			else if (aSN3.CompareValue(surname))
			{
				sb.Append("SN=");
			}
			else if (aSN3.CompareValue(givenName))
			{
				sb.Append("G=");
			}
			else if (aSN3.CompareValue(initial))
			{
				sb.Append("I=");
			}
			else
			{
				sb.Append("OID.");
				sb.Append(ASN1Convert.ToOid(aSN3));
				sb.Append("=");
			}
			string text = null;
			if (aSN2.Tag == 30)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int j = 1; j < aSN2.Value.Length; j += 2)
				{
					stringBuilder.Append((char)aSN2.Value[j]);
				}
				text = stringBuilder.ToString();
			}
			else
			{
				text = ((aSN2.Tag != 20) ? Encoding.UTF8.GetString(aSN2.Value) : Encoding.UTF7.GetString(aSN2.Value));
				char[] anyOf = new char[7] { ',', '+', '"', '\\', '<', '>', ';' };
				if (quotes && (text.IndexOfAny(anyOf, 0, text.Length) > 0 || text.StartsWith(" ") || text.EndsWith(" ")))
				{
					text = "\"" + text + "\"";
				}
			}
			sb.Append(text);
			if (i < entry.Count - 1)
			{
				sb.Append(", ");
			}
		}
	}
}
