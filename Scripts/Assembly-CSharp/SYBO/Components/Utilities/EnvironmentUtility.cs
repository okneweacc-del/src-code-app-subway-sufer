using System;
using System.Threading;
using SYBO.Components.Base;

namespace SYBO.Components.Utilities;

public class EnvironmentUtility
{
	public const ActiveTargetPlatform ActiveTargetPlatform = ActiveTargetPlatform.Android;

	public const string ActiveTargetPlatformString = "android";

	public static bool IsMainThread => Thread.CurrentThread.Equals(Thread.CurrentThread);

	public static bool IsRunningUnitTests => BaseEnvironment.IsRunningUnitTests;

	public static string BuildNumber => GetEnvironmentVariable("BUILD_NUMBER");

	public static string Workspace => GetEnvironmentVariable("WORKSPACE");

	public static string Path => GetEnvironmentVariable("PATH");

	public static string GitCommit => GetEnvironmentVariable("GIT_COMMIT");

	public static string GitBranch => GetEnvironmentVariable("GIT_BRANCH");

	public static bool IsBatchMode()
	{
		string commandLine = Environment.CommandLine;
		if (commandLine.Contains("-batchmode"))
		{
			return true;
		}
		return false;
	}

	public static bool HasEnvironmentVariable(string name)
	{
		return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name));
	}

	public static string GetEnvironmentVariable(string name)
	{
		return Environment.GetEnvironmentVariable(name);
	}

	public static void SetEnvironmentVariable(string name, string value)
	{
		Environment.SetEnvironmentVariable(name, value);
	}
}
