using System.Collections;
using System.Runtime.Serialization;
using System.Text.RegularExpressions.Syntax;

namespace System.Text.RegularExpressions;

[Serializable]
public class Regex : ISerializable
{
	private static System.Text.RegularExpressions.FactoryCache cache = new System.Text.RegularExpressions.FactoryCache(15);

	private System.Text.RegularExpressions.IMachineFactory machineFactory;

	private IDictionary mapping;

	private int group_count;

	private int gap;

	private string[] group_names;

	private int[] group_numbers;

	protected internal string pattern;

	protected internal RegexOptions roptions;

	public RegexOptions Options => roptions;

	public bool RightToLeft => (roptions & RegexOptions.RightToLeft) != 0;

	internal int GroupCount => group_count;

	internal int Gap => gap;

	private int[] GroupNumbers
	{
		get
		{
			if (group_numbers == null)
			{
				group_numbers = new int[1 + group_count];
				for (int i = 0; i < gap; i++)
				{
					group_numbers[i] = i;
				}
				for (int j = gap; j <= group_count; j++)
				{
					group_numbers[j] = int.Parse(group_names[j]);
				}
				return group_numbers;
			}
			return group_numbers;
		}
	}

	protected Regex()
	{
	}

	public Regex(string pattern)
		: this(pattern, RegexOptions.None)
	{
	}

	public Regex(string pattern, RegexOptions options)
	{
		if (pattern == null)
		{
			throw new ArgumentNullException("pattern");
		}
		validate_options(options);
		this.pattern = pattern;
		roptions = options;
		Init();
	}

	protected Regex(SerializationInfo info, StreamingContext context)
		: this(info.GetString("pattern"), (RegexOptions)(int)info.GetValue("options", typeof(RegexOptions)))
	{
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("pattern", ToString(), typeof(string));
		info.AddValue("options", Options, typeof(RegexOptions));
	}

	public static bool IsMatch(string input, string pattern)
	{
		return IsMatch(input, pattern, RegexOptions.None);
	}

	public static bool IsMatch(string input, string pattern, RegexOptions options)
	{
		Regex regex = new Regex(pattern, options);
		return regex.IsMatch(input);
	}

	public static string Replace(string input, string pattern, string replacement)
	{
		return Replace(input, pattern, replacement, RegexOptions.None);
	}

	public static string Replace(string input, string pattern, string replacement, RegexOptions options)
	{
		Regex regex = new Regex(pattern, options);
		return regex.Replace(input, replacement);
	}

	private static void validate_options(RegexOptions options)
	{
		if ((options & ~(RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.RightToLeft | RegexOptions.ECMAScript | RegexOptions.CultureInvariant)) != RegexOptions.None)
		{
			throw new ArgumentOutOfRangeException("options");
		}
		if ((options & RegexOptions.ECMAScript) != RegexOptions.None && (options & ~(RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ECMAScript)) != RegexOptions.None)
		{
			throw new ArgumentOutOfRangeException("options");
		}
	}

	private void Init()
	{
		machineFactory = cache.Lookup(pattern, roptions);
		if (machineFactory == null)
		{
			InitNewRegex();
			return;
		}
		group_count = machineFactory.GroupCount;
		gap = machineFactory.Gap;
		mapping = machineFactory.Mapping;
		group_names = machineFactory.NamesMapping;
	}

	private void InitNewRegex()
	{
		machineFactory = CreateMachineFactory(pattern, roptions);
		cache.Add(pattern, roptions, machineFactory);
		group_count = machineFactory.GroupCount;
		gap = machineFactory.Gap;
		mapping = machineFactory.Mapping;
		group_names = machineFactory.NamesMapping;
	}

	private static System.Text.RegularExpressions.IMachineFactory CreateMachineFactory(string pattern, RegexOptions options)
	{
		System.Text.RegularExpressions.Syntax.Parser parser = new System.Text.RegularExpressions.Syntax.Parser();
		System.Text.RegularExpressions.Syntax.RegularExpression regularExpression = parser.ParseRegularExpression(pattern, options);
		System.Text.RegularExpressions.ICompiler compiler = new System.Text.RegularExpressions.PatternCompiler();
		regularExpression.Compile(compiler, (options & RegexOptions.RightToLeft) != 0);
		System.Text.RegularExpressions.IMachineFactory machineFactory = compiler.GetMachineFactory();
		Hashtable hashtable = new Hashtable();
		machineFactory.Gap = parser.GetMapping(hashtable);
		machineFactory.Mapping = hashtable;
		machineFactory.NamesMapping = GetGroupNamesArray(machineFactory.GroupCount, machineFactory.Mapping);
		return machineFactory;
	}

	public int GroupNumberFromName(string name)
	{
		if (!mapping.Contains(name))
		{
			return -1;
		}
		int num = (int)mapping[name];
		if (num >= gap)
		{
			num = int.Parse(name);
		}
		return num;
	}

	internal int GetGroupIndex(int number)
	{
		if (number < gap)
		{
			return number;
		}
		if (gap > group_count)
		{
			return -1;
		}
		return Array.BinarySearch(GroupNumbers, gap, group_count - gap + 1, number);
	}

	private int default_startat(string input)
	{
		return (RightToLeft && input != null) ? input.Length : 0;
	}

	public bool IsMatch(string input)
	{
		return IsMatch(input, default_startat(input));
	}

	public bool IsMatch(string input, int startat)
	{
		return Match(input, startat).Success;
	}

	public Match Match(string input)
	{
		return Match(input, default_startat(input));
	}

	public Match Match(string input, int startat)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		if (startat < 0 || startat > input.Length)
		{
			throw new ArgumentOutOfRangeException("startat");
		}
		return CreateMachine().Scan(this, input, startat, input.Length);
	}

	public MatchCollection Matches(string input)
	{
		return Matches(input, default_startat(input));
	}

	public MatchCollection Matches(string input, int startat)
	{
		Match start = Match(input, startat);
		return new MatchCollection(start);
	}

	public string Replace(string input, string replacement)
	{
		return Replace(input, replacement, int.MaxValue, default_startat(input));
	}

	public string Replace(string input, string replacement, int count, int startat)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		if (replacement == null)
		{
			throw new ArgumentNullException("replacement");
		}
		if (count < -1)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		if (startat < 0 || startat > input.Length)
		{
			throw new ArgumentOutOfRangeException("startat");
		}
		return CreateMachine().Replace(this, input, replacement, count, startat);
	}

	public override string ToString()
	{
		return pattern;
	}

	private System.Text.RegularExpressions.IMachine CreateMachine()
	{
		return machineFactory.NewInstance();
	}

	private static string[] GetGroupNamesArray(int groupCount, IDictionary mapping)
	{
		string[] array = new string[groupCount + 1];
		IDictionaryEnumerator enumerator = mapping.GetEnumerator();
		while (enumerator.MoveNext())
		{
			array[(int)enumerator.Value] = (string)enumerator.Key;
		}
		return array;
	}
}
