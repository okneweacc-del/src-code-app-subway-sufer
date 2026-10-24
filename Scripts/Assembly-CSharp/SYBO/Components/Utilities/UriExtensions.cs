using System;
using System.Linq;

namespace SYBO.Components.Utilities;

public static class UriExtensions
{
	public static Uri Append(this Uri uri, params string[] paths)
	{
		return new Uri(paths.Aggregate(uri.AbsoluteUri, (string current, string path) => $"{current.TrimEnd('/')}/{path.TrimStart('/')}"));
	}
}
