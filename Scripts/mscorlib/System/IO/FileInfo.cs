using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO;

[Serializable]
[ComVisible(true)]
public sealed class FileInfo : FileSystemInfo
{
	private bool exists;

	public override bool Exists
	{
		get
		{
			Refresh(force: false);
			if (stat.Attributes == MonoIO.InvalidFileAttributes)
			{
				return false;
			}
			if ((stat.Attributes & FileAttributes.Directory) != 0)
			{
				return false;
			}
			return exists;
		}
	}

	public override string Name => Path.GetFileName(FullPath);

	public long Length
	{
		get
		{
			if (!Exists)
			{
				throw new FileNotFoundException("Could not find file \"" + OriginalPath + "\".", OriginalPath);
			}
			return stat.Length;
		}
	}

	public FileInfo(string fileName)
	{
		if (fileName == null)
		{
			throw new ArgumentNullException("fileName");
		}
		CheckPath(fileName);
		OriginalPath = fileName;
		FullPath = Path.GetFullPath(fileName);
	}

	private FileInfo(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	internal override void InternalRefresh()
	{
		exists = File.Exists(FullPath);
	}

	public FileStream OpenRead()
	{
		return Open(FileMode.Open, FileAccess.Read, FileShare.Read);
	}

	public FileStream Open(FileMode mode, FileAccess access, FileShare share)
	{
		return new FileStream(FullPath, mode, access, share);
	}

	public override void Delete()
	{
		if (MonoIO.Exists(FullPath, out var error))
		{
			if (MonoIO.ExistsDirectory(FullPath, out error))
			{
				throw new UnauthorizedAccessException("Access to the path \"" + FullPath + "\" is denied.");
			}
			if (!MonoIO.DeleteFile(FullPath, out error))
			{
				throw MonoIO.GetException(OriginalPath, error);
			}
		}
	}

	public FileInfo CopyTo(string destFileName, bool overwrite)
	{
		if (destFileName == null)
		{
			throw new ArgumentNullException("destFileName");
		}
		if (destFileName.Length == 0)
		{
			throw new ArgumentException("An empty file name is not valid.", "destFileName");
		}
		string fullPath = Path.GetFullPath(destFileName);
		if (overwrite && File.Exists(fullPath))
		{
			File.Delete(fullPath);
		}
		File.Copy(FullPath, fullPath);
		return new FileInfo(fullPath);
	}

	public override string ToString()
	{
		return Name;
	}
}
