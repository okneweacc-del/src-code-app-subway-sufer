namespace System.Text.RegularExpressions.Syntax;

internal class AnchorInfo
{
	private System.Text.RegularExpressions.Syntax.Expression expr;

	private System.Text.RegularExpressions.Position pos;

	private int offset;

	private string str;

	private int width;

	private bool ignore;

	public int Offset => offset;

	public int Width => width;

	public int Length => (str != null) ? str.Length : 0;

	public bool IsUnknownWidth => width < 0;

	public bool IsComplete => Length == Width;

	public string Substring => str;

	public bool IgnoreCase => ignore;

	public System.Text.RegularExpressions.Position Position => pos;

	public bool IsSubstring => str != null;

	public bool IsPosition => pos != System.Text.RegularExpressions.Position.Any;

	public AnchorInfo(System.Text.RegularExpressions.Syntax.Expression expr, int width)
	{
		this.expr = expr;
		offset = 0;
		this.width = width;
		str = null;
		ignore = false;
		pos = System.Text.RegularExpressions.Position.Any;
	}

	public AnchorInfo(System.Text.RegularExpressions.Syntax.Expression expr, int offset, int width, string str, bool ignore)
	{
		this.expr = expr;
		this.offset = offset;
		this.width = width;
		this.str = ((!ignore) ? str : str.ToLower());
		this.ignore = ignore;
		pos = System.Text.RegularExpressions.Position.Any;
	}

	public AnchorInfo(System.Text.RegularExpressions.Syntax.Expression expr, int offset, int width, System.Text.RegularExpressions.Position pos)
	{
		this.expr = expr;
		this.offset = offset;
		this.width = width;
		this.pos = pos;
		str = null;
		ignore = false;
	}

	public System.Text.RegularExpressions.Interval GetInterval(int start)
	{
		if (!IsSubstring)
		{
			return System.Text.RegularExpressions.Interval.Empty;
		}
		return new System.Text.RegularExpressions.Interval(start + Offset, start + Offset + Length - 1);
	}
}
