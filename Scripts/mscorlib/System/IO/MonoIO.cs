using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.IO;

internal sealed class MonoIO
{
	public static readonly FileAttributes InvalidFileAttributes = (FileAttributes)(-1);

	public static readonly IntPtr InvalidHandle = (IntPtr)(-1L);

	public static extern IntPtr ConsoleOutput
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static extern IntPtr ConsoleInput
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static extern IntPtr ConsoleError
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static extern char VolumeSeparatorChar
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static extern char DirectorySeparatorChar
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static extern char AltDirectorySeparatorChar
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static extern char PathSeparator
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static Exception GetException(MonoIOError error)
	{
		switch (error)
		{
		case MonoIOError.ERROR_ACCESS_DENIED:
			return new UnauthorizedAccessException("Access to the path is denied.");
		case MonoIOError.ERROR_FILE_EXISTS:
		{
			string message = "Cannot create a file that already exist.";
			return new IOException(message, (int)((MonoIOError)(-2147024896) | error));
		}
		default:
			return GetException(string.Empty, error);
		}
	}

	public static Exception GetException(string path, MonoIOError error)
	{
		switch (error)
		{
		case MonoIOError.ERROR_FILE_NOT_FOUND:
		{
			string message = $"Could not find file \"{path}\"";
			return new IsolatedStorageException(message);
		}
		case MonoIOError.ERROR_TOO_MANY_OPEN_FILES:
			return new IOException("Too many open files", (int)((MonoIOError)(-2147024896) | error));
		case MonoIOError.ERROR_PATH_NOT_FOUND:
		{
			string message = $"Could not find a part of the path \"{path}\"";
			return new IsolatedStorageException(message);
		}
		case MonoIOError.ERROR_ACCESS_DENIED:
		{
			string message = $"Access to the path \"{path}\" is denied.";
			return new UnauthorizedAccessException(message);
		}
		case MonoIOError.ERROR_INVALID_HANDLE:
		{
			string message = $"Invalid handle to path \"{path}\"";
			return new IOException(message, (int)((MonoIOError)(-2147024896) | error));
		}
		case MonoIOError.ERROR_INVALID_DRIVE:
		{
			string message = $"Could not find the drive  '{path}'. The drive might not be ready or might not be mapped.";
			return new IOException(message, (int)((MonoIOError)(-2147024896) | error));
		}
		case MonoIOError.ERROR_FILE_EXISTS:
		{
			string message = $"Could not create file \"{path}\". File already exists.";
			return new IOException(message, (int)((MonoIOError)(-2147024896) | error));
		}
		case MonoIOError.ERROR_FILENAME_EXCED_RANGE:
		{
			string message = $"Path is too long. Path: {path}";
			return new PathTooLongException(message);
		}
		case MonoIOError.ERROR_INVALID_PARAMETER:
		{
			string message = $"Invalid parameter";
			return new IOException(message, (int)((MonoIOError)(-2147024896) | error));
		}
		case MonoIOError.ERROR_WRITE_FAULT:
		{
			string message = $"Write fault on path {path}";
			return new IOException(message, (int)((MonoIOError)(-2147024896) | error));
		}
		case MonoIOError.ERROR_SHARING_VIOLATION:
		{
			string message = $"Sharing violation on path {path}";
			return new IOException(message, (int)((MonoIOError)(-2147024896) | error));
		}
		case MonoIOError.ERROR_LOCK_VIOLATION:
		{
			string message = $"Lock violation on path {path}";
			return new IOException(message, (int)((MonoIOError)(-2147024896) | error));
		}
		case MonoIOError.ERROR_HANDLE_DISK_FULL:
		{
			string message = $"Disk full. Path {path}";
			return new IOException(message, (int)((MonoIOError)(-2147024896) | error));
		}
		case MonoIOError.ERROR_DIR_NOT_EMPTY:
		{
			string message = $"Directory {path} is not empty";
			return new IOException(message, (int)((MonoIOError)(-2147024896) | error));
		}
		case MonoIOError.ERROR_ENCRYPTION_FAILED:
			return new IOException("Encryption failed", (int)((MonoIOError)(-2147024896) | error));
		case MonoIOError.ERROR_CANNOT_MAKE:
		{
			string message = $"Path {path} is a directory";
			return new IOException(message, (int)((MonoIOError)(-2147024896) | error));
		}
		case MonoIOError.ERROR_NOT_SAME_DEVICE:
		{
			string message = "Source and destination are not on the same device";
			return new IOException(message, (int)((MonoIOError)(-2147024896) | error));
		}
		default:
		{
			string message = $"Win32 IO returned {error}. Path: {path}";
			return new IOException(message, (int)((MonoIOError)(-2147024896) | error));
		}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool CreateDirectory(string path, out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool RemoveDirectory(string path, out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern string[] GetFileSystemEntries(string path, string path_with_pattern, int attrs, int mask, out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern string GetCurrentDirectory(out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool SetCurrentDirectory(string path, out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool MoveFile(string path, string dest, out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool CopyFile(string path, string dest, bool overwrite, out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool DeleteFile(string path, out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern FileAttributes GetFileAttributes(string path, out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool SetFileAttributes(string path, FileAttributes attrs, out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern MonoFileType GetFileType(IntPtr handle, out MonoIOError error);

	public static bool Exists(string path, out MonoIOError error)
	{
		FileAttributes fileAttributes = GetFileAttributes(path, out error);
		if (fileAttributes == InvalidFileAttributes)
		{
			return false;
		}
		return true;
	}

	public static bool ExistsFile(string path, out MonoIOError error)
	{
		FileAttributes fileAttributes = GetFileAttributes(path, out error);
		if (fileAttributes == InvalidFileAttributes)
		{
			return false;
		}
		if ((fileAttributes & FileAttributes.Directory) != 0)
		{
			return false;
		}
		return true;
	}

	public static bool ExistsDirectory(string path, out MonoIOError error)
	{
		FileAttributes fileAttributes = GetFileAttributes(path, out error);
		if (error == MonoIOError.ERROR_FILE_NOT_FOUND)
		{
			error = MonoIOError.ERROR_PATH_NOT_FOUND;
		}
		if (fileAttributes == InvalidFileAttributes)
		{
			return false;
		}
		if ((fileAttributes & FileAttributes.Directory) == 0)
		{
			return false;
		}
		return true;
	}

	public static bool ExistsSymlink(string path, out MonoIOError error)
	{
		FileAttributes fileAttributes = GetFileAttributes(path, out error);
		if (fileAttributes == InvalidFileAttributes)
		{
			return false;
		}
		if ((fileAttributes & FileAttributes.ReparsePoint) == 0)
		{
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool GetFileStat(string path, out MonoIOStat stat, out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern IntPtr Open(string filename, FileMode mode, FileAccess access, FileShare share, FileOptions options, out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool Close(IntPtr handle, out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern int Read(IntPtr handle, byte[] dest, int dest_offset, int count, out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern int Write(IntPtr handle, [In] byte[] src, int src_offset, int count, out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern long Seek(IntPtr handle, long offset, SeekOrigin origin, out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern long GetLength(IntPtr handle, out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool SetLength(IntPtr handle, long length, out MonoIOError error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool SetFileTime(IntPtr handle, long creation_time, long last_access_time, long last_write_time, out MonoIOError error);

	public static bool SetFileTime(string path, long creation_time, long last_access_time, long last_write_time, out MonoIOError error)
	{
		return SetFileTime(path, 0, creation_time, last_access_time, last_write_time, DateTime.MinValue, out error);
	}

	public static bool SetFileTime(string path, int type, long creation_time, long last_access_time, long last_write_time, DateTime dateTime, out MonoIOError error)
	{
		IntPtr intPtr = Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, FileOptions.None, out error);
		if (intPtr == InvalidHandle)
		{
			return false;
		}
		switch (type)
		{
		case 1:
			creation_time = dateTime.ToFileTime();
			break;
		case 2:
			last_access_time = dateTime.ToFileTime();
			break;
		case 3:
			last_write_time = dateTime.ToFileTime();
			break;
		}
		bool result = SetFileTime(intPtr, creation_time, last_access_time, last_write_time, out error);
		Close(intPtr, out var _);
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool RemapPath(string path, out string newPath);
}
