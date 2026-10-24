using System.Runtime.InteropServices;

namespace System.Resources;

[ComVisible(true)]
public interface IResourceWriter : IDisposable
{
	void Close();

	void Generate();
}
