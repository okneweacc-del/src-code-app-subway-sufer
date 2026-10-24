namespace SYBO.Components.Base;

public class BaseEnvironment
{
	private static bool _isRunningUnitTests;

	public static bool IsRunningUnitTests => _isRunningUnitTests;
}
