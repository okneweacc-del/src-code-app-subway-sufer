using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace System.Security.Policy;

[Serializable]
[ComVisible(true)]
public sealed class Hash : ISerializable, IBuiltInEvidence
{
	private Assembly assembly;

	private byte[] data;

	internal Hash()
	{
	}

	internal Hash(SerializationInfo info, StreamingContext context)
	{
		data = (byte[])info.GetValue("RawData", typeof(byte[]));
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		info.AddValue("RawData", GetData());
	}

	public override string ToString()
	{
		SecurityElement securityElement = new SecurityElement(GetType().FullName);
		securityElement.AddAttribute("version", "1");
		StringBuilder stringBuilder = new StringBuilder();
		byte[] array = GetData();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("X2"));
		}
		securityElement.AddChild(new SecurityElement("RawData", stringBuilder.ToString()));
		return securityElement.ToString();
	}

	private byte[] GetData()
	{
		if (assembly == null && data == null)
		{
			string text = Locale.GetText("No assembly data.");
			throw new SecurityException(text);
		}
		if (data == null)
		{
			FileStream fileStream = new FileStream(assembly.Location, FileMode.Open, FileAccess.Read);
			data = new byte[fileStream.Length];
			fileStream.Read(data, 0, (int)fileStream.Length);
		}
		return data;
	}
}
