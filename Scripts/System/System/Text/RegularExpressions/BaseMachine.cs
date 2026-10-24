using System.Collections.Generic;

namespace System.Text.RegularExpressions;

internal abstract class BaseMachine : System.Text.RegularExpressions.IMachine
{
	internal delegate void MatchAppendEvaluator(Match match, StringBuilder sb);

	protected bool needs_groups_or_captures = true;

	public virtual string Replace(Regex regex, string input, string replacement, int count, int startat)
	{
		System.Text.RegularExpressions.ReplacementEvaluator replacementEvaluator = new System.Text.RegularExpressions.ReplacementEvaluator(regex, replacement);
		if (regex.RightToLeft)
		{
			return RTLReplace(regex, input, replacementEvaluator.Evaluate, count, startat);
		}
		return LTRReplace(regex, input, replacementEvaluator.EvaluateAppend, count, startat, replacementEvaluator.NeedsGroupsOrCaptures);
	}

	public virtual Match Scan(Regex regex, string text, int start, int end)
	{
		throw new NotImplementedException("Scan method must be implemented in derived classes");
	}

	internal string LTRReplace(Regex regex, string input, MatchAppendEvaluator evaluator, int count, int startat, bool needs_groups_or_captures)
	{
		this.needs_groups_or_captures = needs_groups_or_captures;
		Match match = Scan(regex, input, startat, input.Length);
		if (!match.Success)
		{
			return input;
		}
		StringBuilder stringBuilder = new StringBuilder(input.Length);
		int num = startat;
		int num2 = count;
		stringBuilder.Append(input, 0, num);
		while (count == -1 || num2-- > 0)
		{
			if (match.Index < num)
			{
				throw new SystemException("how");
			}
			stringBuilder.Append(input, num, match.Index - num);
			evaluator(match, stringBuilder);
			num = match.Index + match.Length;
			match = match.NextMatch();
			if (!match.Success)
			{
				break;
			}
		}
		stringBuilder.Append(input, num, input.Length - num);
		return stringBuilder.ToString();
	}

	internal string RTLReplace(Regex regex, string input, MatchEvaluator evaluator, int count, int startat)
	{
		Match match = Scan(regex, input, startat, input.Length);
		if (!match.Success)
		{
			return input;
		}
		int num = startat;
		int num2 = count;
		List<string> list = new List<string>();
		list.Add(input.Substring(num));
		while (count == -1 || num2-- > 0)
		{
			if (match.Index + match.Length > num)
			{
				throw new SystemException("how");
			}
			list.Add(input.Substring(match.Index + match.Length, num - match.Index - match.Length));
			list.Add(evaluator(match));
			num = match.Index;
			match = match.NextMatch();
			if (!match.Success)
			{
				break;
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(input, 0, num);
		int num3 = list.Count;
		while (num3 > 0)
		{
			stringBuilder.Append(list[--num3]);
		}
		list.Clear();
		return stringBuilder.ToString();
	}
}
