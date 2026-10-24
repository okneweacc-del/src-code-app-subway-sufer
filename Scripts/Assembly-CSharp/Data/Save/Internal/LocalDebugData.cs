namespace Data.Save.Internal;

public class LocalDebugData : DiskData
{
	public int overridePerformanceLevel = -1;

	public bool disableAdSystem;

	public override void Obfuscate()
	{
	}

	public override void Deobfuscate()
	{
	}
}
