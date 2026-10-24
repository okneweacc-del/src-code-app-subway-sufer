using System.Runtime.InteropServices;

namespace System.ComponentModel;

[ComVisible(true)]
public interface IContainer : IDisposable
{
	void Remove(IComponent component);
}
