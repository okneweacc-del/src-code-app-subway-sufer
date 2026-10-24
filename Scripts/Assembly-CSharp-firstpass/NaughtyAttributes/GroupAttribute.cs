namespace NaughtyAttributes;

public abstract class GroupAttribute : NaughtyAttribute
{
	private string name;

	public string Name => name;

	public GroupAttribute(string name)
	{
		this.name = name;
	}
}
