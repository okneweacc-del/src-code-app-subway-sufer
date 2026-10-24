using System.Collections.Generic;

namespace Info;

public class SocialLinkOverrideInfo
{
	public readonly bool enabled = true;

	public readonly string link;

	public readonly Dictionary<string, string> title;

	public readonly Dictionary<string, string> desc;

	public readonly Dictionary<string, string> iosDesc;

	public readonly int reward = -1;
}
