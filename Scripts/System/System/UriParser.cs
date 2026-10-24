using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;

namespace System;

public abstract class UriParser
{
	private static object lock_object = new object();

	private static Hashtable table;

	internal string scheme_name;

	private int default_port;

	private static readonly Regex uri_regex = new Regex("^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\\?([^#]*))?(#(.*))?");

	private static readonly Regex auth_regex = new Regex("^(([^@]+)@)?(.*?)(:([0-9]+))?$");

	internal string SchemeName
	{
		set
		{
			scheme_name = value;
		}
	}

	internal int DefaultPort
	{
		get
		{
			return default_port;
		}
		set
		{
			default_port = value;
		}
	}

	protected internal virtual void InitializeAndValidate(Uri uri, out UriFormatException parsingError)
	{
		if (uri.Scheme != scheme_name && scheme_name != "*")
		{
			parsingError = new UriFormatException("The argument Uri's scheme does not match");
		}
		else
		{
			parsingError = null;
		}
	}

	[System.MonoTODO]
	protected virtual void OnRegister(string schemeName, int defaultPort)
	{
	}

	private static void CreateDefaults()
	{
		if (table != null)
		{
			return;
		}
		Hashtable hashtable = new Hashtable();
		InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeFile, -1);
		InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeFtp, 21);
		InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeGopher, 70);
		InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeHttp, 80);
		InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeHttps, 443);
		InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeMailto, 25);
		InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeNetPipe, -1);
		InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeNetTcp, -1);
		InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeNews, 119);
		InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeNntp, 119);
		InternalRegister(hashtable, new System.DefaultUriParser(), "ldap", 389);
		lock (lock_object)
		{
			if (table == null)
			{
				table = hashtable;
			}
			else
			{
				hashtable = null;
			}
		}
	}

	private static void InternalRegister(Hashtable table, UriParser uriParser, string schemeName, int defaultPort)
	{
		uriParser.SchemeName = schemeName;
		uriParser.DefaultPort = defaultPort;
		if (uriParser is GenericUriParser)
		{
			table.Add(schemeName, uriParser);
		}
		else
		{
			System.DefaultUriParser defaultUriParser = new System.DefaultUriParser();
			defaultUriParser.SchemeName = schemeName;
			defaultUriParser.DefaultPort = defaultPort;
			table.Add(schemeName, defaultUriParser);
		}
		uriParser.OnRegister(schemeName, defaultPort);
	}

	internal static UriParser GetParser(string schemeName)
	{
		if (schemeName == null)
		{
			return null;
		}
		CreateDefaults();
		string key = schemeName.ToLower(CultureInfo.InvariantCulture);
		return (UriParser)table[key];
	}
}
