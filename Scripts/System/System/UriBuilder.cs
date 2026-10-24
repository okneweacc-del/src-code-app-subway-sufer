using System.Text;

namespace System;

public class UriBuilder
{
	private string scheme;

	private string host;

	private int port;

	private string path;

	private string query;

	private string fragment;

	private string username;

	private string password;

	private Uri uri;

	private bool modified;

	public string Query
	{
		set
		{
			if (value == null || value.Length == 0)
			{
				query = string.Empty;
			}
			else
			{
				query = "?" + value;
			}
			modified = true;
		}
	}

	public Uri Uri
	{
		get
		{
			if (!modified)
			{
				return uri;
			}
			uri = new Uri(ToString(), dontEscape: true);
			modified = false;
			return uri;
		}
	}

	public UriBuilder(Uri uri)
	{
		scheme = uri.Scheme;
		host = uri.Host;
		port = uri.Port;
		path = uri.AbsolutePath;
		query = uri.Query;
		fragment = uri.Fragment;
		username = uri.UserInfo;
		int num = username.IndexOf(':');
		if (num != -1)
		{
			password = username.Substring(num + 1);
			username = username.Substring(0, num);
		}
		else
		{
			password = string.Empty;
		}
		modified = true;
	}

	public override bool Equals(object rparam)
	{
		return rparam != null && Uri.Equals(rparam.ToString());
	}

	public override int GetHashCode()
	{
		return Uri.GetHashCode();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(scheme);
		stringBuilder.Append("://");
		if (username != string.Empty)
		{
			stringBuilder.Append(username);
			if (password != string.Empty)
			{
				stringBuilder.Append(":" + password);
			}
			stringBuilder.Append('@');
		}
		stringBuilder.Append(host);
		if (port > 0)
		{
			stringBuilder.Append(":" + port);
		}
		if (path != string.Empty && stringBuilder[stringBuilder.Length - 1] != '/' && path.Length > 0 && path[0] != '/')
		{
			stringBuilder.Append('/');
		}
		stringBuilder.Append(path);
		stringBuilder.Append(query);
		stringBuilder.Append(fragment);
		return stringBuilder.ToString();
	}
}
