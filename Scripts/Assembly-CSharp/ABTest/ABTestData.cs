using System.Collections.Generic;

namespace ABTest;

public class ABTestData
{
	public class ABTestDataItem
	{
		public bool forceDisable;

		public string identifier;

		public string defaultPayload;

		public TestRange testRange;

		public string debugTitle;

		public List<string> debugPayload;
	}

	public const string ABTEST_SLOWER_REVIVE_TIMER = "slowrevivetimer";

	public static readonly List<ABTestDataItem> testData = new List<ABTestDataItem>
	{
		new ABTestDataItem
		{
			identifier = "slowrevivetimer",
			defaultPayload = "4",
			testRange = TestRange.Kiloo
		}
	};
}
