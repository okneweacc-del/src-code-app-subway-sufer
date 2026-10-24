using SYBO.Components.Logging;

namespace SYBO.Components.Analytics;

public class UnsyncedMTRandomGen : IMTRandomGen, IRandomGen
{
	private MTRandomGen _gen = new MTRandomGen(666u, 0);

	public uint Seed => _gen.Seed;

	public int SeedIncrement => _gen.SeedIncrement;

	public uint NextUint()
	{
		Log.Analytics.LogErrorFormat("Retrieving Random value from unsynced random generator. Client game state diverged");
		return _gen.NextUint();
	}
}
