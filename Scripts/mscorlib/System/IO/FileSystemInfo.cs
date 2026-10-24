using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO;

[Serializable]
[ComVisible(true)]
public abstract class FileSystemInfo : MarshalByRefObject, ISerializable
{
	protected string FullPath;

	protected string OriginalPath;

	internal MonoIOStat stat;

	internal bool valid;

	public abstract bool Exists { get; }

	public abstract string Name { get; }

	public virtual string FullName => FullPath;

	public DateTime LastWriteTime
	{
		get
		{
			Refresh(force: false);
			return DateTime.FromFileTime(stat.LastWriteTime);
		}
		set
		{
			long last_write_time = value.ToFileTime();
			if (!MonoIO.SetFileTime(FullName, -1L, -1L, last_write_time, out var error))
			{
				throw MonoIO.GetException(FullName, error);
			}
			Refresh(force: true);
		}
	}

	[ComVisible(false)]
	public DateTime LastWriteTimeUtc
	{
		get
		{
			Refresh(force: false);
			return LastWriteTime.ToUniversalTime();
		}
		set
		{
			LastWriteTime = value.ToLocalTime();
		}
	}

	protected FileSystemInfo()
	{
		valid = false;
		FullPath = null;
	}

	protected FileSystemInfo(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		FullPath = info.GetString("FullPath");
		OriginalPath = info.GetString("OriginalPath");
	}

	[ComVisible(false)]
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("OriginalPath", OriginalPath, typeof(string));
		info.AddValue("FullPath", FullPath, typeof(string));
	}

	public abstract void Delete();

	internal void Refresh(bool force)
	{
		if (!valid || force)
		{
			MonoIO.GetFileStat(FullName, out stat, out var _);
			valid = true;
			InternalRefresh();
		}
	}

	internal virtual void InternalRefresh()
	{
	}

	internal void CheckPath(string path)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		if (path.Length == 0)
		{
			throw new ArgumentException("An empty file name is not valid.");
		}
		if (path.IndexOfAny(Path.InvalidPathChars) != -1)
		{
			throw new ArgumentException("Illegal characters in path.");
		}
	}
}
