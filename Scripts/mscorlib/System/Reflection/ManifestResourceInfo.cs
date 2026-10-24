using System.Runtime.InteropServices;

namespace System.Reflection;

[ComVisible(true)]
public class ManifestResourceInfo
{
	private Assembly _assembly;

	private string _filename;

	private ResourceLocation _location;

	public virtual string FileName => _filename;

	public virtual Assembly ReferencedAssembly => _assembly;

	public virtual ResourceLocation ResourceLocation => _location;

	internal ManifestResourceInfo()
	{
	}
}
