namespace System.Text.RegularExpressions;

internal struct Mark
{
	public int Start;

	public int End;

	public int Previous;

	public bool IsDefined => Start >= 0 && End >= 0;

	public int Index => (Start >= End) ? End : Start;

	public int Length => (Start >= End) ? (Start - End) : (End - Start);
}
