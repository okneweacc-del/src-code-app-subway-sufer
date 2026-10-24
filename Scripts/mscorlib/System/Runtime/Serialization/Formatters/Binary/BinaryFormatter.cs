using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Serialization.Formatters.Binary;

[ComVisible(true)]
public sealed class BinaryFormatter : IRemotingFormatter, IFormatter
{
	private FormatterAssemblyStyle assembly_format;

	private SerializationBinder binder;

	private StreamingContext context;

	private ISurrogateSelector surrogate_selector;

	private FormatterTypeStyle type_format = FormatterTypeStyle.TypesAlways;

	private TypeFilterLevel filter_level = TypeFilterLevel.Full;

	public static ISurrogateSelector DefaultSurrogateSelector { get; }

	public FormatterAssemblyStyle AssemblyFormat
	{
		set
		{
			assembly_format = value;
		}
	}

	public SerializationBinder Binder => binder;

	public StreamingContext Context => context;

	public ISurrogateSelector SurrogateSelector
	{
		get
		{
			return surrogate_selector;
		}
		set
		{
			surrogate_selector = value;
		}
	}

	public TypeFilterLevel FilterLevel => filter_level;

	public BinaryFormatter()
	{
		surrogate_selector = DefaultSurrogateSelector;
		context = new StreamingContext(StreamingContextStates.All);
	}

	public BinaryFormatter(ISurrogateSelector selector, StreamingContext context)
	{
		surrogate_selector = selector;
		this.context = context;
	}

	public object Deserialize(Stream serializationStream)
	{
		return NoCheckDeserialize(serializationStream, null);
	}

	public object Deserialize(Stream serializationStream, HeaderHandler handler)
	{
		return NoCheckDeserialize(serializationStream, handler);
	}

	private object NoCheckDeserialize(Stream serializationStream, HeaderHandler handler)
	{
		if (serializationStream == null)
		{
			throw new ArgumentNullException("serializationStream");
		}
		if (serializationStream.CanSeek && serializationStream.Length == 0L)
		{
			throw new SerializationException("serializationStream supports seeking, but its length is 0");
		}
		BinaryReader binaryReader = new BinaryReader(serializationStream);
		ReadBinaryHeader(binaryReader, out var hasHeaders);
		BinaryElement binaryElement = (BinaryElement)binaryReader.Read();
		switch (binaryElement)
		{
		case BinaryElement.MethodCall:
			return MessageFormatter.ReadMethodCall(binaryElement, binaryReader, hasHeaders, handler, this);
		case BinaryElement.MethodResponse:
			return MessageFormatter.ReadMethodResponse(binaryElement, binaryReader, hasHeaders, handler, null, this);
		default:
		{
			ObjectReader objectReader = new ObjectReader(this);
			objectReader.ReadObjectGraph(binaryElement, binaryReader, hasHeaders, out var result, out var headers);
			handler?.Invoke(headers);
			return result;
		}
		}
	}

	public object DeserializeMethodResponse(Stream serializationStream, HeaderHandler handler, IMethodCallMessage methodCallMessage)
	{
		return NoCheckDeserializeMethodResponse(serializationStream, handler, methodCallMessage);
	}

	private object NoCheckDeserializeMethodResponse(Stream serializationStream, HeaderHandler handler, IMethodCallMessage methodCallMessage)
	{
		if (serializationStream == null)
		{
			throw new ArgumentNullException("serializationStream");
		}
		if (serializationStream.CanSeek && serializationStream.Length == 0L)
		{
			throw new SerializationException("serializationStream supports seeking, but its length is 0");
		}
		BinaryReader reader = new BinaryReader(serializationStream);
		ReadBinaryHeader(reader, out var hasHeaders);
		return MessageFormatter.ReadMethodResponse(reader, hasHeaders, handler, methodCallMessage, this);
	}

	public void Serialize(Stream serializationStream, object graph)
	{
		Serialize(serializationStream, graph, null);
	}

	public void Serialize(Stream serializationStream, object graph, Header[] headers)
	{
		if (serializationStream == null)
		{
			throw new ArgumentNullException("serializationStream");
		}
		BinaryWriter binaryWriter = new BinaryWriter(serializationStream);
		WriteBinaryHeader(binaryWriter, headers != null);
		if (graph is IMethodCallMessage)
		{
			MessageFormatter.WriteMethodCall(binaryWriter, graph, headers, surrogate_selector, context, assembly_format, type_format);
		}
		else if (graph is IMethodReturnMessage)
		{
			MessageFormatter.WriteMethodResponse(binaryWriter, graph, headers, surrogate_selector, context, assembly_format, type_format);
		}
		else
		{
			ObjectWriter objectWriter = new ObjectWriter(surrogate_selector, context, assembly_format, type_format);
			objectWriter.WriteObjectGraph(binaryWriter, graph, headers);
		}
		binaryWriter.Flush();
	}

	private void WriteBinaryHeader(BinaryWriter writer, bool hasHeaders)
	{
		writer.Write((byte)0);
		writer.Write(1);
		if (hasHeaders)
		{
			writer.Write(2);
		}
		else
		{
			writer.Write(-1);
		}
		writer.Write(1);
		writer.Write(0);
	}

	private void ReadBinaryHeader(BinaryReader reader, out bool hasHeaders)
	{
		reader.ReadByte();
		reader.ReadInt32();
		int num = reader.ReadInt32();
		hasHeaders = num == 2;
		reader.ReadInt32();
		reader.ReadInt32();
	}
}
