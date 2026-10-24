using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace System.Resources;

[ComVisible(true)]
public sealed class ResourceReader : IEnumerable, IDisposable, IResourceReader
{
	private struct ResourceInfo(string resourceName, long valuePosition, int type_index)
	{
		public readonly long ValuePosition = valuePosition;

		public readonly string ResourceName = resourceName;

		public readonly int TypeIndex = type_index;
	}

	private struct ResourceCacheItem(string name, object value)
	{
		public readonly string ResourceName = name;

		public readonly object ResourceValue = value;
	}

	internal sealed class ResourceEnumerator : IEnumerator, IDictionaryEnumerator
	{
		private ResourceReader reader;

		private int index = -1;

		private bool finished;

		public DictionaryEntry Entry
		{
			get
			{
				if (reader.reader == null)
				{
					throw new InvalidOperationException("ResourceReader is closed.");
				}
				if (index < 0)
				{
					throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
				}
				return new DictionaryEntry(Key, Value);
			}
		}

		public object Key
		{
			get
			{
				if (reader.reader == null)
				{
					throw new InvalidOperationException("ResourceReader is closed.");
				}
				if (index < 0)
				{
					throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
				}
				return reader.cache[index].ResourceName;
			}
		}

		public object Value
		{
			get
			{
				if (reader.reader == null)
				{
					throw new InvalidOperationException("ResourceReader is closed.");
				}
				if (index < 0)
				{
					throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
				}
				return reader.cache[index].ResourceValue;
			}
		}

		public object Current => Entry;

		internal ResourceEnumerator(ResourceReader readerToEnumerate)
		{
			reader = readerToEnumerate;
			FillCache();
		}

		public bool MoveNext()
		{
			if (reader.reader == null)
			{
				throw new InvalidOperationException("ResourceReader is closed.");
			}
			if (finished)
			{
				return false;
			}
			if (++index < reader.resourceCount)
			{
				return true;
			}
			finished = true;
			return false;
		}

		public void Reset()
		{
			if (reader.reader == null)
			{
				throw new InvalidOperationException("ResourceReader is closed.");
			}
			index = -1;
			finished = false;
		}

		private void FillCache()
		{
			if (reader.cache != null)
			{
				return;
			}
			lock (reader.cache_lock)
			{
				if (reader.cache == null)
				{
					ResourceCacheItem[] array = new ResourceCacheItem[reader.resourceCount];
					reader.LoadResourceValues(array);
					reader.cache = array;
				}
			}
		}
	}

	private BinaryReader reader;

	private object readerLock = new object();

	private IFormatter formatter;

	internal int resourceCount;

	private int typeCount;

	private string[] typeNames;

	private int[] hashes;

	private ResourceInfo[] infos;

	private int dataSectionOffset;

	private long nameSectionOffset;

	private int resource_ver;

	private ResourceCacheItem[] cache;

	private object cache_lock = new object();

	public ResourceReader(Stream stream)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		if (!stream.CanRead)
		{
			throw new ArgumentException("Stream was not readable.");
		}
		reader = new BinaryReader(stream, Encoding.UTF8);
		formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File | StreamingContextStates.Persistence));
		ReadHeaders();
	}

	public ResourceReader(string fileName)
	{
		reader = new BinaryReader(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read));
		formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File | StreamingContextStates.Persistence));
		ReadHeaders();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IResourceReader)this).GetEnumerator();
	}

	void IDisposable.Dispose()
	{
		Dispose(disposing: true);
	}

	private void ReadHeaders()
	{
		try
		{
			int num = reader.ReadInt32();
			if (num != ResourceManager.MagicNumber)
			{
				throw new ArgumentException($"Stream is not a valid .resources file, magic=0x{num:x}");
			}
			int num2 = reader.ReadInt32();
			int num3 = reader.ReadInt32();
			if (num2 > ResourceManager.HeaderVersionNumber)
			{
				reader.BaseStream.Seek(num3, SeekOrigin.Current);
			}
			else
			{
				string text = reader.ReadString();
				if (!text.StartsWith("System.Resources.ResourceReader"))
				{
					throw new NotSupportedException("This .resources file requires reader class " + text);
				}
				string text2 = reader.ReadString();
				if (!text2.StartsWith(typeof(ResourceSet).FullName) && !text2.StartsWith("System.Resources.RuntimeResourceSet"))
				{
					throw new NotSupportedException("This .resources file requires set class " + text2);
				}
			}
			resource_ver = reader.ReadInt32();
			if (resource_ver != 1 && resource_ver != 2)
			{
				throw new NotSupportedException("This .resources file requires unsupported set class version: " + resource_ver);
			}
			resourceCount = reader.ReadInt32();
			typeCount = reader.ReadInt32();
			typeNames = new string[typeCount];
			for (int i = 0; i < typeCount; i++)
			{
				typeNames[i] = reader.ReadString();
			}
			int num4 = (int)(reader.BaseStream.Position & 7);
			int num5 = 0;
			if (num4 != 0)
			{
				num5 = 8 - num4;
			}
			for (int j = 0; j < num5; j++)
			{
				byte b = reader.ReadByte();
				if (b != "PAD"[j % 3])
				{
					throw new ArgumentException("Malformed .resources file (padding values incorrect)");
				}
			}
			hashes = new int[resourceCount];
			for (int k = 0; k < resourceCount; k++)
			{
				hashes[k] = reader.ReadInt32();
			}
			long[] array = new long[resourceCount];
			for (int l = 0; l < resourceCount; l++)
			{
				array[l] = reader.ReadInt32();
			}
			dataSectionOffset = reader.ReadInt32();
			nameSectionOffset = reader.BaseStream.Position;
			long position = reader.BaseStream.Position;
			infos = new ResourceInfo[resourceCount];
			for (int m = 0; m < resourceCount; m++)
			{
				CreateResourceInfo(array[m], ref infos[m]);
			}
			reader.BaseStream.Seek(position, SeekOrigin.Begin);
			array = null;
		}
		catch (EndOfStreamException innerException)
		{
			throw new ArgumentException("Stream is not a valid .resources file!  It was possibly truncated.", innerException);
		}
	}

	private void CreateResourceInfo(long position, ref ResourceInfo info)
	{
		long offset = position + nameSectionOffset;
		reader.BaseStream.Seek(offset, SeekOrigin.Begin);
		int num = Read7BitEncodedInt();
		byte[] array = new byte[num];
		reader.Read(array, 0, num);
		string resourceName = Encoding.Unicode.GetString(array);
		long offset2 = reader.ReadInt32() + dataSectionOffset;
		reader.BaseStream.Seek(offset2, SeekOrigin.Begin);
		int type_index = Read7BitEncodedInt();
		info = new ResourceInfo(resourceName, reader.BaseStream.Position, type_index);
	}

	private int Read7BitEncodedInt()
	{
		int num = 0;
		int num2 = 0;
		byte b;
		do
		{
			b = reader.ReadByte();
			num |= (b & 0x7F) << num2;
			num2 += 7;
		}
		while ((b & 0x80) == 128);
		return num;
	}

	private object ReadValueVer2(int type_index)
	{
		switch ((PredefinedResourceType)type_index)
		{
		case PredefinedResourceType.Null:
			return null;
		case PredefinedResourceType.String:
			return reader.ReadString();
		case PredefinedResourceType.Bool:
			return reader.ReadBoolean();
		case PredefinedResourceType.Char:
			return (char)reader.ReadUInt16();
		case PredefinedResourceType.Byte:
			return reader.ReadByte();
		case PredefinedResourceType.SByte:
			return reader.ReadSByte();
		case PredefinedResourceType.Int16:
			return reader.ReadInt16();
		case PredefinedResourceType.UInt16:
			return reader.ReadUInt16();
		case PredefinedResourceType.Int32:
			return reader.ReadInt32();
		case PredefinedResourceType.UInt32:
			return reader.ReadUInt32();
		case PredefinedResourceType.Int64:
			return reader.ReadInt64();
		case PredefinedResourceType.UInt64:
			return reader.ReadUInt64();
		case PredefinedResourceType.Single:
			return reader.ReadSingle();
		case PredefinedResourceType.Double:
			return reader.ReadDouble();
		case PredefinedResourceType.Decimal:
			return reader.ReadDecimal();
		case PredefinedResourceType.DateTime:
			return new DateTime(reader.ReadInt64());
		case PredefinedResourceType.TimeSpan:
			return new TimeSpan(reader.ReadInt64());
		case PredefinedResourceType.ByteArray:
			return reader.ReadBytes(reader.ReadInt32());
		case PredefinedResourceType.Stream:
		{
			byte[] array = new byte[reader.ReadUInt32()];
			reader.Read(array, 0, array.Length);
			return new MemoryStream(array);
		}
		default:
			type_index -= 64;
			return ReadNonPredefinedValue(Type.GetType(typeNames[type_index], throwOnError: true));
		}
	}

	private object ReadValueVer1(Type type)
	{
		if (type == typeof(string))
		{
			return reader.ReadString();
		}
		if (type == typeof(int))
		{
			return reader.ReadInt32();
		}
		if (type == typeof(byte))
		{
			return reader.ReadByte();
		}
		if (type == typeof(double))
		{
			return reader.ReadDouble();
		}
		if (type == typeof(short))
		{
			return reader.ReadInt16();
		}
		if (type == typeof(long))
		{
			return reader.ReadInt64();
		}
		if (type == typeof(sbyte))
		{
			return reader.ReadSByte();
		}
		if (type == typeof(float))
		{
			return reader.ReadSingle();
		}
		if (type == typeof(TimeSpan))
		{
			return new TimeSpan(reader.ReadInt64());
		}
		if (type == typeof(ushort))
		{
			return reader.ReadUInt16();
		}
		if (type == typeof(uint))
		{
			return reader.ReadUInt32();
		}
		if (type == typeof(ulong))
		{
			return reader.ReadUInt64();
		}
		if (type == typeof(decimal))
		{
			return reader.ReadDecimal();
		}
		if (type == typeof(DateTime))
		{
			return new DateTime(reader.ReadInt64());
		}
		return ReadNonPredefinedValue(type);
	}

	private object ReadNonPredefinedValue(Type exp_type)
	{
		object obj = formatter.Deserialize(reader.BaseStream);
		if (obj.GetType() != exp_type)
		{
			throw new InvalidOperationException("Deserialized object is wrong type");
		}
		return obj;
	}

	private void LoadResourceValues(ResourceCacheItem[] store)
	{
		lock (readerLock)
		{
			for (int i = 0; i < resourceCount; i++)
			{
				ResourceInfo resourceInfo = infos[i];
				if (resourceInfo.TypeIndex == -1)
				{
					ref ResourceCacheItem reference = ref store[i];
					reference = new ResourceCacheItem(resourceInfo.ResourceName, null);
					continue;
				}
				reader.BaseStream.Seek(resourceInfo.ValuePosition, SeekOrigin.Begin);
				object value = ((resource_ver != 2) ? ReadValueVer1(Type.GetType(typeNames[resourceInfo.TypeIndex], throwOnError: true)) : ReadValueVer2(resourceInfo.TypeIndex));
				ref ResourceCacheItem reference2 = ref store[i];
				reference2 = new ResourceCacheItem(resourceInfo.ResourceName, value);
			}
		}
	}

	public void Close()
	{
		Dispose(disposing: true);
	}

	public IDictionaryEnumerator GetEnumerator()
	{
		if (reader == null)
		{
			throw new InvalidOperationException("ResourceReader is closed.");
		}
		return new ResourceEnumerator(this);
	}

	private void Dispose(bool disposing)
	{
		if (disposing && reader != null)
		{
			reader.Close();
		}
		reader = null;
		hashes = null;
		infos = null;
		typeNames = null;
		cache = null;
	}
}
