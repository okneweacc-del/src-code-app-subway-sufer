namespace System.Globalization;

[Flags]
internal enum DateTimeFormatFlags
{
	Unused = 0,
	But = 1,
	Serialized = 2,
	By = But | Serialized,
	Microsoft = 4
}
