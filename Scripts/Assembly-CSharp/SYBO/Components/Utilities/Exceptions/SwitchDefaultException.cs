using System;

namespace SYBO.Components.Utilities.Exceptions;

public class SwitchDefaultException : Exception
{
	public SwitchDefaultException(string missingValue)
		: base($"Add case for '{missingValue}'.")
	{
	}

	public static void Throw(object missingValue)
	{
		throw new SwitchDefaultException(missingValue.ToString());
	}
}
