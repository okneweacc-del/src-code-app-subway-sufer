using System.Runtime.InteropServices;

namespace System.Globalization;

[Serializable]
[Flags]
[ComVisible(true)]
public enum DateTimeStyles
{
	None = 0,
	AllowLeadingWhite = 1,
	AllowTrailingWhite = 2,
	AllowInnerWhite = 4,
	AllowWhiteSpaces = AllowLeadingWhite | AllowTrailingWhite | AllowInnerWhite,
	NoCurrentDateDefault = 8,
	AdjustToUniversal = 0x10,
	AssumeLocal = 0x20,
	AssumeUniversal = 0x40,
	RoundtripKind = 0x80
}
