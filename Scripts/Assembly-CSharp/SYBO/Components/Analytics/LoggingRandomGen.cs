namespace SYBO.Components.Analytics;

public class LoggingRandomGen : IMTRandomGen, IRandomGen
{
	private IMTRandomGen _gen;

	public uint Seed => _gen.Seed;

	public int SeedIncrement => _gen.SeedIncrement;

	public LoggingRandomGen(IMTRandomGen gen)
	{
		_gen = gen;
	}

	public uint NextUint()
	{
		return _gen.NextUint();
	}
}
