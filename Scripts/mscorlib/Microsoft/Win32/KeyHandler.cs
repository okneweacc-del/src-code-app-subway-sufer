using System;
using System.Collections;
using System.IO;
using System.Security;
using System.Threading;

namespace Microsoft.Win32;

internal class KeyHandler
{
	private static Hashtable key_to_handler = new Hashtable();

	private static Hashtable dir_to_handler = new Hashtable(new CaseInsensitiveHashCodeProvider(), new CaseInsensitiveComparer());

	public string Dir;

	private Hashtable values;

	private string file;

	private bool dirty;

	public bool IsMarkedForDeletion => !dir_to_handler.Contains(Dir);

	private static string UserStore => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ".mono/registry");

	private static string MachineStore
	{
		get
		{
			string environmentVariable = Environment.GetEnvironmentVariable("MONO_REGISTRY_PATH");
			if (environmentVariable != null)
			{
				return environmentVariable;
			}
			environmentVariable = Environment.GetMachineConfigPath();
			int num = environmentVariable.IndexOf("machine.config");
			return Path.Combine(Path.Combine(environmentVariable.Substring(0, num - 1), ".."), "registry");
		}
	}

	private KeyHandler(RegistryKey rkey, string basedir)
	{
		if (!Directory.Exists(basedir))
		{
			try
			{
				Directory.CreateDirectory(basedir);
			}
			catch (UnauthorizedAccessException)
			{
				throw new SecurityException("No access to the given key");
			}
		}
		Dir = basedir;
		file = Path.Combine(Dir, "values.xml");
		Load();
	}

	public void Load()
	{
		values = new Hashtable();
		if (!File.Exists(file))
		{
			return;
		}
		try
		{
			using FileStream stream = File.OpenRead(file);
			StreamReader streamReader = new StreamReader(stream);
			string text = streamReader.ReadToEnd();
			if (text.Length == 0)
			{
				return;
			}
			SecurityElement securityElement = SecurityElement.FromString(text);
			if (!(securityElement.Tag == "values") || securityElement.Children == null)
			{
				return;
			}
			foreach (SecurityElement child in securityElement.Children)
			{
				if (child.Tag == "value")
				{
					LoadKey(child);
				}
			}
		}
		catch (UnauthorizedAccessException)
		{
			values.Clear();
			throw new SecurityException("No access to the given key");
		}
		catch (Exception arg)
		{
			Console.Error.WriteLine("While loading registry key at {0}: {1}", file, arg);
			values.Clear();
		}
	}

	private void LoadKey(SecurityElement se)
	{
		Hashtable attributes = se.Attributes;
		try
		{
			string text = (string)attributes["name"];
			if (text == null)
			{
				return;
			}
			string text2 = (string)attributes["type"];
			if (text2 == null)
			{
				return;
			}
			switch (text2)
			{
			case "int":
				values[text] = int.Parse(se.Text);
				break;
			case "bytearray":
				values[text] = Convert.FromBase64String(se.Text);
				break;
			case "string":
				values[text] = se.Text;
				break;
			case "expand":
				values[text] = new ExpandString(se.Text);
				break;
			case "qword":
				values[text] = long.Parse(se.Text);
				break;
			case "string-array":
			{
				ArrayList arrayList = new ArrayList();
				if (se.Children != null)
				{
					foreach (SecurityElement child in se.Children)
					{
						arrayList.Add(child.Text);
					}
				}
				values[text] = arrayList.ToArray(typeof(string));
				break;
			}
			}
		}
		catch
		{
		}
	}

	public RegistryKey Ensure(RegistryKey rkey, string extra, bool writable)
	{
		lock (typeof(KeyHandler))
		{
			string text = Path.Combine(Dir, extra);
			KeyHandler keyHandler = (KeyHandler)dir_to_handler[text];
			if (keyHandler == null)
			{
				keyHandler = new KeyHandler(rkey, text);
			}
			RegistryKey registryKey = new RegistryKey(keyHandler, CombineName(rkey, extra), writable);
			key_to_handler[registryKey] = keyHandler;
			dir_to_handler[text] = keyHandler;
			return registryKey;
		}
	}

	public RegistryKey Probe(RegistryKey rkey, string extra, bool writable)
	{
		RegistryKey registryKey = null;
		lock (typeof(KeyHandler))
		{
			string text = Path.Combine(Dir, extra);
			KeyHandler keyHandler = (KeyHandler)dir_to_handler[text];
			if (keyHandler != null)
			{
				registryKey = new RegistryKey(keyHandler, CombineName(rkey, extra), writable);
				key_to_handler[registryKey] = keyHandler;
			}
			else if (Directory.Exists(text))
			{
				keyHandler = new KeyHandler(rkey, text);
				registryKey = new RegistryKey(keyHandler, CombineName(rkey, extra), writable);
				dir_to_handler[text] = keyHandler;
				key_to_handler[registryKey] = keyHandler;
			}
			return registryKey;
		}
	}

	private static string CombineName(RegistryKey rkey, string extra)
	{
		if (extra.IndexOf('/') != -1)
		{
			extra = extra.Replace('/', '\\');
		}
		return rkey.Name + "\\" + extra;
	}

	public static KeyHandler Lookup(RegistryKey rkey, bool createNonExisting)
	{
		lock (typeof(KeyHandler))
		{
			KeyHandler keyHandler = (KeyHandler)key_to_handler[rkey];
			if (keyHandler != null)
			{
				return keyHandler;
			}
			if (!rkey.IsRoot || !createNonExisting)
			{
				return null;
			}
			RegistryHive hive = rkey.Hive;
			switch (hive)
			{
			case RegistryHive.CurrentUser:
			{
				string text2 = Path.Combine(UserStore, hive.ToString());
				keyHandler = new KeyHandler(rkey, text2);
				dir_to_handler[text2] = keyHandler;
				break;
			}
			case RegistryHive.ClassesRoot:
			case RegistryHive.LocalMachine:
			case RegistryHive.Users:
			case RegistryHive.PerformanceData:
			case RegistryHive.CurrentConfig:
			case RegistryHive.DynData:
			{
				string text = Path.Combine(MachineStore, hive.ToString());
				keyHandler = new KeyHandler(rkey, text);
				dir_to_handler[text] = keyHandler;
				break;
			}
			default:
				throw new Exception("Unknown RegistryHive");
			}
			key_to_handler[rkey] = keyHandler;
			return keyHandler;
		}
	}

	public static void Drop(RegistryKey rkey)
	{
		lock (typeof(KeyHandler))
		{
			KeyHandler keyHandler = (KeyHandler)key_to_handler[rkey];
			if (keyHandler == null)
			{
				return;
			}
			key_to_handler.Remove(rkey);
			int num = 0;
			foreach (DictionaryEntry item in key_to_handler)
			{
				if (item.Value == keyHandler)
				{
					num++;
				}
			}
			if (num == 0)
			{
				dir_to_handler.Remove(keyHandler.Dir);
			}
		}
	}

	public object GetValue(string name, RegistryValueOptions options)
	{
		if (IsMarkedForDeletion)
		{
			return null;
		}
		if (name == null)
		{
			name = string.Empty;
		}
		object obj = values[name];
		if (!(obj is ExpandString expandString))
		{
			return obj;
		}
		if ((options & RegistryValueOptions.DoNotExpandEnvironmentNames) == 0)
		{
			return expandString.Expand();
		}
		return expandString.ToString();
	}

	public void SetValue(string name, object value)
	{
		AssertNotMarkedForDeletion();
		if (name == null)
		{
			name = string.Empty;
		}
		if (value is int || value is string || value is byte[] || value is string[])
		{
			values[name] = value;
		}
		else
		{
			values[name] = value.ToString();
		}
		SetDirty();
	}

	public string[] GetValueNames()
	{
		AssertNotMarkedForDeletion();
		ICollection keys = values.Keys;
		string[] array = new string[keys.Count];
		keys.CopyTo(array, 0);
		return array;
	}

	private void SetDirty()
	{
		lock (typeof(KeyHandler))
		{
			if (!dirty)
			{
				dirty = true;
				new Timer(DirtyTimeout, null, 3000, -1);
			}
		}
	}

	public void DirtyTimeout(object state)
	{
		Flush();
	}

	public void Flush()
	{
		lock (typeof(KeyHandler))
		{
			if (dirty)
			{
				Save();
				dirty = false;
			}
		}
	}

	public bool ValueExists(string name)
	{
		if (name == null)
		{
			name = string.Empty;
		}
		return values.Contains(name);
	}

	public void RemoveValue(string name)
	{
		AssertNotMarkedForDeletion();
		values.Remove(name);
		SetDirty();
	}

	~KeyHandler()
	{
		Flush();
	}

	private void Save()
	{
		if (IsMarkedForDeletion || (!File.Exists(file) && values.Count == 0))
		{
			return;
		}
		SecurityElement securityElement = new SecurityElement("values");
		foreach (DictionaryEntry value2 in values)
		{
			object value = value2.Value;
			SecurityElement securityElement2 = new SecurityElement("value");
			securityElement2.AddAttribute("name", SecurityElement.Escape((string)value2.Key));
			if (value is string)
			{
				securityElement2.AddAttribute("type", "string");
				securityElement2.Text = SecurityElement.Escape((string)value);
			}
			else if (value is int)
			{
				securityElement2.AddAttribute("type", "int");
				securityElement2.Text = value.ToString();
			}
			else if (value is long)
			{
				securityElement2.AddAttribute("type", "qword");
				securityElement2.Text = value.ToString();
			}
			else if (value is byte[])
			{
				securityElement2.AddAttribute("type", "bytearray");
				securityElement2.Text = Convert.ToBase64String((byte[])value);
			}
			else if (value is ExpandString)
			{
				securityElement2.AddAttribute("type", "expand");
				securityElement2.Text = SecurityElement.Escape(value.ToString());
			}
			else if (value is string[])
			{
				securityElement2.AddAttribute("type", "string-array");
				string[] array = (string[])value;
				foreach (string str in array)
				{
					SecurityElement securityElement3 = new SecurityElement("string");
					securityElement3.Text = SecurityElement.Escape(str);
					securityElement2.AddChild(securityElement3);
				}
			}
			securityElement.AddChild(securityElement2);
		}
		using FileStream stream = File.Create(file);
		StreamWriter streamWriter = new StreamWriter(stream);
		streamWriter.Write(securityElement.ToString());
		streamWriter.Flush();
	}

	private void AssertNotMarkedForDeletion()
	{
		if (IsMarkedForDeletion)
		{
			throw RegistryKey.CreateMarkedForDeletionException();
		}
	}
}
