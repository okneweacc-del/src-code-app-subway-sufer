using System.Runtime.InteropServices;

namespace System.ComponentModel;

[ComVisible(true)]
public interface ISite : IServiceProvider
{
	IContainer Container { get; }

	string Name { get; }
}
