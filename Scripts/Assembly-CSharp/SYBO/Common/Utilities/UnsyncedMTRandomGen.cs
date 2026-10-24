namespace SYBO.Common.Utilities;

public class UnsyncedMTRandomGen : IMTRandomGen, IRandomGen
{
	private MTRandomGen _gen = new MTRandomGen(666u, 0);

	public uint Seed => _gen.Seed;

	public int SeedIncrement => _gen.SeedIncrement;

	public uint NextUint()
	{
		SDebug.LogError("Retrieving Random value from unsynced random generator. Client game state diverged");
		return _gen.NextUint();
	}
}
