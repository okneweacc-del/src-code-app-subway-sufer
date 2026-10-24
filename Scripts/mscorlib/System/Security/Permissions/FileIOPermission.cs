using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Security.Permissions;

[Serializable]
[ComVisible(true)]
public sealed class FileIOPermission : CodeAccessPermission, IBuiltInPermission, IUnrestrictedPermission
{
	private static char[] BadPathNameCharacters;

	private static char[] BadFileNameCharacters;

	private bool m_Unrestricted;

	private FileIOPermissionAccess m_AllFilesAccess;

	private FileIOPermissionAccess m_AllLocalFilesAccess;

	private ArrayList readList;

	private ArrayList writeList;

	private ArrayList appendList;

	private ArrayList pathList;

	public FileIOPermissionAccess AllFiles => m_AllFilesAccess;

	public FileIOPermissionAccess AllLocalFiles => m_AllLocalFilesAccess;

	public FileIOPermission(PermissionState state)
	{
		if (CodeAccessPermission.CheckPermissionState(state, allowUnrestricted: true) == PermissionState.Unrestricted)
		{
			m_Unrestricted = true;
			m_AllFilesAccess = FileIOPermissionAccess.AllAccess;
			m_AllLocalFilesAccess = FileIOPermissionAccess.AllAccess;
		}
		CreateLists();
	}

	public FileIOPermission(FileIOPermissionAccess access, string path)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		CreateLists();
		AddPathList(access, path);
	}

	static FileIOPermission()
	{
		BadPathNameCharacters = Path.GetInvalidPathChars();
		BadFileNameCharacters = Path.GetInvalidFileNameChars();
	}

	internal void CreateLists()
	{
		readList = new ArrayList();
		writeList = new ArrayList();
		appendList = new ArrayList();
		pathList = new ArrayList();
	}

	public void AddPathList(FileIOPermissionAccess access, string path)
	{
		if ((FileIOPermissionAccess.AllAccess & access) != access)
		{
			ThrowInvalidFlag(access, context: true);
		}
		ThrowIfInvalidPath(path);
		AddPathInternal(access, path);
	}

	public void AddPathList(FileIOPermissionAccess access, string[] pathList)
	{
		if ((FileIOPermissionAccess.AllAccess & access) != access)
		{
			ThrowInvalidFlag(access, context: true);
		}
		ThrowIfInvalidPath(pathList);
		foreach (string path in pathList)
		{
			AddPathInternal(access, path);
		}
	}

	internal void AddPathInternal(FileIOPermissionAccess access, string path)
	{
		path = Path.InsecureGetFullPath(path);
		if ((access & FileIOPermissionAccess.Read) == FileIOPermissionAccess.Read)
		{
			readList.Add(path);
		}
		if ((access & FileIOPermissionAccess.Write) == FileIOPermissionAccess.Write)
		{
			writeList.Add(path);
		}
		if ((access & FileIOPermissionAccess.Append) == FileIOPermissionAccess.Append)
		{
			appendList.Add(path);
		}
		if ((access & FileIOPermissionAccess.PathDiscovery) == FileIOPermissionAccess.PathDiscovery)
		{
			pathList.Add(path);
		}
	}

	public override void FromXml(SecurityElement esd)
	{
		CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
		if (CodeAccessPermission.IsUnrestricted(esd))
		{
			m_Unrestricted = true;
			return;
		}
		m_Unrestricted = false;
		string text = esd.Attribute("Read");
		if (text != null)
		{
			string[] array = text.Split(';');
			AddPathList(FileIOPermissionAccess.Read, array);
		}
		text = esd.Attribute("Write");
		if (text != null)
		{
			string[] array = text.Split(';');
			AddPathList(FileIOPermissionAccess.Write, array);
		}
		text = esd.Attribute("Append");
		if (text != null)
		{
			string[] array = text.Split(';');
			AddPathList(FileIOPermissionAccess.Append, array);
		}
		text = esd.Attribute("PathDiscovery");
		if (text != null)
		{
			string[] array = text.Split(';');
			AddPathList(FileIOPermissionAccess.PathDiscovery, array);
		}
	}

	public string[] GetPathList(FileIOPermissionAccess access)
	{
		if ((FileIOPermissionAccess.AllAccess & access) != access)
		{
			ThrowInvalidFlag(access, context: true);
		}
		ArrayList arrayList = new ArrayList();
		switch (access)
		{
		case FileIOPermissionAccess.Read:
			arrayList.AddRange(readList);
			break;
		case FileIOPermissionAccess.Write:
			arrayList.AddRange(writeList);
			break;
		case FileIOPermissionAccess.Append:
			arrayList.AddRange(appendList);
			break;
		case FileIOPermissionAccess.PathDiscovery:
			arrayList.AddRange(pathList);
			break;
		default:
			ThrowInvalidFlag(access, context: false);
			break;
		case FileIOPermissionAccess.NoAccess:
			break;
		}
		return (arrayList.Count <= 0) ? null : ((string[])arrayList.ToArray(typeof(string)));
	}

	public override bool IsSubsetOf(IPermission target)
	{
		FileIOPermission fileIOPermission = Cast(target);
		if (fileIOPermission == null)
		{
			return false;
		}
		if (fileIOPermission.IsEmpty())
		{
			return IsEmpty();
		}
		if (IsUnrestricted())
		{
			return fileIOPermission.IsUnrestricted();
		}
		if (fileIOPermission.IsUnrestricted())
		{
			return true;
		}
		if ((m_AllFilesAccess & fileIOPermission.AllFiles) != m_AllFilesAccess)
		{
			return false;
		}
		if ((m_AllLocalFilesAccess & fileIOPermission.AllLocalFiles) != m_AllLocalFilesAccess)
		{
			return false;
		}
		if (!KeyIsSubsetOf(appendList, fileIOPermission.appendList))
		{
			return false;
		}
		if (!KeyIsSubsetOf(readList, fileIOPermission.readList))
		{
			return false;
		}
		if (!KeyIsSubsetOf(writeList, fileIOPermission.writeList))
		{
			return false;
		}
		if (!KeyIsSubsetOf(pathList, fileIOPermission.pathList))
		{
			return false;
		}
		return true;
	}

	public bool IsUnrestricted()
	{
		return m_Unrestricted;
	}

	public override SecurityElement ToXml()
	{
		SecurityElement securityElement = Element(1);
		if (m_Unrestricted)
		{
			securityElement.AddAttribute("Unrestricted", "true");
		}
		else
		{
			string[] array = GetPathList(FileIOPermissionAccess.Append);
			if (array != null && array.Length > 0)
			{
				securityElement.AddAttribute("Append", string.Join(";", array));
			}
			array = GetPathList(FileIOPermissionAccess.Read);
			if (array != null && array.Length > 0)
			{
				securityElement.AddAttribute("Read", string.Join(";", array));
			}
			array = GetPathList(FileIOPermissionAccess.Write);
			if (array != null && array.Length > 0)
			{
				securityElement.AddAttribute("Write", string.Join(";", array));
			}
			array = GetPathList(FileIOPermissionAccess.PathDiscovery);
			if (array != null && array.Length > 0)
			{
				securityElement.AddAttribute("PathDiscovery", string.Join(";", array));
			}
		}
		return securityElement;
	}

	[ComVisible(false)]
	[MonoTODO("(2.0)")]
	public override bool Equals(object obj)
	{
		return false;
	}

	[MonoTODO("(2.0)")]
	[ComVisible(false)]
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	private bool IsEmpty()
	{
		return !m_Unrestricted && appendList.Count == 0 && readList.Count == 0 && writeList.Count == 0 && pathList.Count == 0;
	}

	private static FileIOPermission Cast(IPermission target)
	{
		if (target == null)
		{
			return null;
		}
		FileIOPermission fileIOPermission = target as FileIOPermission;
		if (fileIOPermission == null)
		{
			CodeAccessPermission.ThrowInvalidPermission(target, typeof(FileIOPermission));
		}
		return fileIOPermission;
	}

	internal static void ThrowInvalidFlag(FileIOPermissionAccess access, bool context)
	{
		string text = null;
		text = ((!context) ? Locale.GetText("Invalid flag '{0}' in this context.") : Locale.GetText("Unknown flag '{0}'."));
		throw new ArgumentException(string.Format(text, access), "access");
	}

	internal static void ThrowIfInvalidPath(string path)
	{
		string directoryName = Path.GetDirectoryName(path);
		if (directoryName != null && directoryName.LastIndexOfAny(BadPathNameCharacters) >= 0)
		{
			string message = string.Format(Locale.GetText("Invalid path characters in path: '{0}'"), path);
			throw new ArgumentException(message, "path");
		}
		string fileName = Path.GetFileName(path);
		if (fileName != null && fileName.LastIndexOfAny(BadFileNameCharacters) >= 0)
		{
			string message2 = string.Format(Locale.GetText("Invalid filename characters in path: '{0}'"), path);
			throw new ArgumentException(message2, "path");
		}
		if (!Path.IsPathRooted(path))
		{
			string text = Locale.GetText("Absolute path information is required.");
			throw new ArgumentException(text, "path");
		}
	}

	internal static void ThrowIfInvalidPath(string[] paths)
	{
		foreach (string path in paths)
		{
			ThrowIfInvalidPath(path);
		}
	}

	internal static bool KeyIsSubsetOf(IList local, IList target)
	{
		bool flag = false;
		foreach (string item in local)
		{
			foreach (string item2 in target)
			{
				if (Path.IsPathSubsetOf(item2, item))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}
}
