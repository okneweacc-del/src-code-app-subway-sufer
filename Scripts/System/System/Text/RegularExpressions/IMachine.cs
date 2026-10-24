namespace System.Text.RegularExpressions;

internal interface IMachine
{
	Match Scan(Regex regex, string text, int start, int end);

	string Replace(Regex regex, string input, string replacement, int count, int startat);
}
