using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO;

[Serializable]
[ComVisible(true)]
public sealed class DirectoryInfo : FileSystemInfo
{
	private string current;

	private string parent;

	public override bool Exists
	{
		get
		{
			Refresh(force: false);
			if (stat.Attributes == MonoIO.InvalidFileAttributes)
			{
				return false;
			}
			if ((stat.Attributes & FileAttributes.Directory) == 0)
			{
				return false;
			}
			return true;
		}
	}

	public override string Name => current;

	public DirectoryInfo Parent
	{
		get
		{
			if (parent == null || parent.Length == 0)
			{
				return null;
			}
			return new DirectoryInfo(parent);
		}
	}

	public DirectoryInfo(string path)
		: this(path, simpleOriginalPath: false)
	{
	}

	internal DirectoryInfo(string path, bool simpleOriginalPath)
	{
		CheckPath(path);
		FullPath = Path.GetFullPath(path);
		if (simpleOriginalPath)
		{
			OriginalPath = Path.GetFileName(path);
		}
		else
		{
			OriginalPath = path;
		}
		Initialize();
	}

	private DirectoryInfo(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		Initialize();
	}

	private void Initialize()
	{
		int num = FullPath.Length - 1;
		if (num > 1 && FullPath[num] == Path.DirectorySeparatorChar)
		{
			num--;
		}
		int num2 = FullPath.LastIndexOf(Path.DirectorySeparatorChar, num);
		if (num2 == -1 || (num2 == 0 && num == 0))
		{
			current = FullPath;
			parent = null;
			return;
		}
		current = FullPath.Substring(num2 + 1, num - num2);
		if (num2 == 0 && !Environment.IsRunningOnWindows)
		{
			parent = Path.DirectorySeparatorStr;
		}
		else
		{
			parent = FullPath.Substring(0, num2);
		}
		if (Environment.IsRunningOnWindows && parent.Length == 2 && parent[1] == ':' && char.IsLetter(parent[0]))
		{
			parent += Path.DirectorySeparatorChar;
		}
	}

	public void Create()
	{
		Directory.CreateDirectory(FullPath);
	}

	public FileInfo[] GetFiles()
	{
		return GetFiles("*");
	}

	public FileInfo[] GetFiles(string searchPattern)
	{
		if (searchPattern == null)
		{
			throw new ArgumentNullException("searchPattern");
		}
		string[] files = Directory.GetFiles(FullPath, searchPattern);
		FileInfo[] array = new FileInfo[files.Length];
		int num = 0;
		string[] array2 = files;
		foreach (string fileName in array2)
		{
			array[num++] = new FileInfo(fileName);
		}
		return array;
	}

	public DirectoryInfo[] GetDirectories()
	{
		return GetDirectories("*");
	}

	public DirectoryInfo[] GetDirectories(string searchPattern)
	{
		if (searchPattern == null)
		{
			throw new ArgumentNullException("searchPattern");
		}
		string[] directories = Directory.GetDirectories(FullPath, searchPattern);
		DirectoryInfo[] array = new DirectoryInfo[directories.Length];
		int num = 0;
		string[] array2 = directories;
		foreach (string path in array2)
		{
			array[num++] = new DirectoryInfo(path);
		}
		return array;
	}

	public override void Delete()
	{
		Delete(recursive: false);
	}

	public void Delete(bool recursive)
	{
		Directory.Delete(FullPath, recursive);
	}

	public override string ToString()
	{
		return OriginalPath;
	}
}
