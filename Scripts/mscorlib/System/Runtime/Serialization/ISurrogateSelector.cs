using System.Runtime.InteropServices;

namespace System.Runtime.Serialization;

[ComVisible(true)]
public interface ISurrogateSelector
{
	ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector);
}
