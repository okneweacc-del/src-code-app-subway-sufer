namespace System.IO;

internal class SearchPattern
{
	internal static readonly char[] WildcardChars = new char[2] { '*', '?' };

	internal static readonly char[] InvalidChars = new char[2]
	{
		Path.DirectorySeparatorChar,
		Path.AltDirectorySeparatorChar
	};
}
